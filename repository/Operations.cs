using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.Extensions.Logging;
using PokerSlackAvatar.Models;
using System.Net.Http.Headers;

namespace PokerSlackAvatar.Repository {
    public class Operations : IOperations {
        private ISecrets _secrets;
        private readonly ILogger<Operations> _logger;
        static readonly HttpClient client = new HttpClient();

        public Operations(ISecrets secrets, ILogger<Operations> logger) {
            _secrets = secrets;
            _logger = logger;
        }

        public class PokerUser {
            public string SlackID { get; set;}
            public string AvatarHash {get; set;}
            public string UserName {get; set;}
        }
        private SqlConnectionStringBuilder GetSqlConnectionStringBuilder() {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //builder.DataSource = _secrets.DBDataSource();
            builder.ConnectionString = "Server=localhost\\sqlexpress;Database=Poker;Trusted_Connection=True;MultipleActiveResultSets=true";
            //builder.UserID = _secrets.DBUserID();
            //builder.Password = _secrets.DBPassword();     
            //builder.InitialCatalog = _secrets.DBName();
            return builder;
        }
        private void UpdateUserAvatarHash(string userID, string hash) {
            var builder = GetSqlConnectionStringBuilder();
            try {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    _logger.LogInformation("\nQuerying Poker Database");
                    connection.Open();
                    string sql = $"UPDATE [user] SET AvatarHash = '{hash}' WHERE SlackID = '{userID}'";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _logger.LogError(e.ToString());
            }
        }
        public List<PokerUser> GetPokerUserSlackIDs() {
            var builder = GetSqlConnectionStringBuilder();
            List<PokerUser> slackIDs = new List<PokerUser>();    
            try { 
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    _logger.LogInformation("\nQuerying Poker Database");
                    connection.Open();       
                    string sql = "SELECT SlackID, AvatarHash, UserName FROM [User]";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try {
                                   
                                    PokerUser p = new PokerUser() {
                                    SlackID = reader.GetString(0),
                                    AvatarHash = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    UserName = reader.GetString(2) };
                                    slackIDs.Add(p);
                                }
                                catch (Exception e) {
                                    _logger.LogError(e.Message);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _logger.LogError(e.ToString());
            }            
            return slackIDs;
        }
        public async Task GetSlackAvatars(List<PokerUser> users) {
            if(!Directory.Exists(_secrets.AvatarDir()))
            {
                Directory.CreateDirectory(_secrets.AvatarDir());
            }
            var size = new MagickGeometry(32, 32);
            size.IgnoreAspectRatio = true;
            int counter = 0;
            foreach (var user in users)
            {
                string uid = user.SlackID;
                var request = new HttpRequestMessage {
                    RequestUri = new Uri("https://slack.com/api/users.profile.get?user=" + uid),
                    Method = HttpMethod.Get
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secrets.Token()); 
                counter++;
                //string uri = "https://slack.com/api/users.profile.get?token=" + _secrets.UserToken() + "&user=" + uid;
                try
                {
                    var responseMessage = await client.SendAsync(request);
                    var responseBody = await responseMessage.Content.ReadAsStringAsync();
                    //string responseBody = await client.GetStringAsync(uri);
                    SlackUser u = SlackUser.FromJson(responseBody);
                    if(u.Profile == null) {
                        continue;
                    }
                    if(!u.Profile.AvatarHash.Equals(user.AvatarHash) || !File.Exists(_secrets.AvatarDir() + user.SlackID + ".png"))
                    {
                        MagickImage i = new MagickImage(await client.GetStreamAsync(u.Profile.Image48));
                        i.Resize(size);
                        if(File.Exists(_secrets.AvatarDir() + user.SlackID + ".png"))
                        {
                            File.Delete(_secrets.AvatarDir() + user.SlackID + ".png");
                        }
                        _logger.LogInformation(_secrets.AvatarDir() + user.SlackID + ".png");
                        i.Write(_secrets.AvatarDir() + user.SlackID + ".png");
                        SetAvatarPath(user.UserName, _secrets.AvatarDir() + user.SlackID + ".png");
                        UpdateUserAvatarHash(user.SlackID, u.Profile.AvatarHash);
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("\nException Caught!");
                    _logger.LogError("Message :{0} ", e.Message);
                }
            }
        }
        private void SetAvatarPath(string name, string path)
        {
            var mavenClient = new MavenClient<AccountsEdit>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "AccountsEdit");
            dict.Add("Player", name);
            dict.Add("AvatarFile", path);
            dict.Add("Avatar", "0");
            mavenClient.Post(client, dict);
        }
    }
}