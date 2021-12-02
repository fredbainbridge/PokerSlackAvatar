using System.Collections.Generic;
namespace PokerSlackAvatar.Repository {
    public interface ISecrets{
        string Password();
        string Token();
        string UserToken();
        string AvatarDir();
        string EventHubConnectionString();
        string EventHubName();
        string APIURI();
        string DBDataSource();
        string DBUserID();
        string DBPassword();
        string DBName();
    }
}