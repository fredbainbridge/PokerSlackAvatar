using System;

namespace PokerSlackAvatar.Repository {
    class Secret {
        public string Password { get; set; }
        public string Token { get; set; }
        public string UserToken { get; set; }
        public string APIURI { get; set; }
        public string AvatarDir { get; set; }
        public string EventHubConnectionString {get; set;}
        public string EventHubName {get; set;}
        public string ConnectionString {get; set;}
        public string DBDataSource {get; set;}
        public string DBUserID {get; set;}
        public string DBPassword {get; set;}
        public string DBName {get; set;}
    }
    public class Secrets : ISecrets {
        private Secret _secret;
        public Secrets() {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Secret secret = new Secret();
            secret.AvatarDir = Environment.GetEnvironmentVariable("AVATAR_DIR");
            secret.APIURI = Environment.GetEnvironmentVariable("API_URI");
            secret.Password = Environment.GetEnvironmentVariable("PASSWORD");
            secret.Token = Environment.GetEnvironmentVariable("TOKEN");
            secret.UserToken = Environment.GetEnvironmentVariable("USER_TOKEN");
            secret.EventHubConnectionString = Environment.GetEnvironmentVariable("EVENTHUB_CONNECTION_STRING");
            secret.EventHubName = Environment.GetEnvironmentVariable("EVENTHUB_NAME");
            secret.DBDataSource = Environment.GetEnvironmentVariable("DB_DATA_SOURCE");
            secret.DBUserID = Environment.GetEnvironmentVariable("DB_USER_ID");
            secret.DBPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            secret.DBName = Environment.GetEnvironmentVariable("DB_NAME");

            _secret = secret;
        }
        public string Password(){
            return _secret.Password;
        }
        
        public string Token()
        {
            return _secret.Token;
        }
        public string UserToken()
        {
            return _secret.UserToken;
        }
        public string APIURI()
        {
            return _secret.APIURI;
        }
        public string AvatarDir()
        {
            return _secret.AvatarDir;
        }

        public string EventHubConnectionString() {
            return _secret.EventHubConnectionString;
        }

        public string EventHubName() {
            return _secret.EventHubName;
        }
        public string DBDataSource() {
            return _secret.DBDataSource;
        }
        public string DBUserID() {
            return _secret.DBUserID;
        }
        public string DBPassword() {
            return _secret.DBPassword;
        }
        public string DBName() {
            return _secret.DBName;
        }
    }
}