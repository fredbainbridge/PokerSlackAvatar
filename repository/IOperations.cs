using System.Collections.Generic;
using System.Threading.Tasks;
using static PokerSlackAvatar.Repository.Operations;

namespace PokerSlackAvatar.Repository {
    public interface IOperations {
        List<PokerUser> GetPokerUserSlackIDs();
        Task GetSlackAvatars(List<PokerUser> users);
    }
}