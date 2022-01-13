using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.DAL {
    public interface IUserRepository {
        User GetUserByAuthToken(string authToken);
        User GetUserByCredentials(string username, string password);
        User GetUserByUsername(string username);
        User GetUserById(Guid id);
        bool InsertUser(User user);
        string GetScoreboard(bool json);

        Card GetCardById(Guid id);
        void AssignCardsAtStart();
        List<Card> GetStack(Guid userId);
        List<Card> GetDeck(Guid userId);
        void ConfigureDeck(User user, List<Guid> cardIds);

        void AssignFriendsAtStart();
        string GetFriends(User user, bool isJson);
        void AddFriend(User user1, string other);
        void DeleteFriend(User user1, string other);
    }
}
