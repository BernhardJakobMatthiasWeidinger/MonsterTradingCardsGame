using MTCG.Models;
using System.Collections.Generic;
using System.Linq;

namespace MTCG.DAL {
    public class DBUserRepository : IUserRepository {
        private readonly List<User> users = new List<User>();
        public User GetUserByAuthToken(string authToken) {
            return users.FirstOrDefault(user => user.Username + "-mtcgToken" == authToken);
        }

        public User GetUserByCredentials(string username, string password) {
            return users.FirstOrDefault(user => user.Username == username && user.Password == password);
        }

        public bool InsertUser(User user) {
            if (GetUserByUsername(user.Username) == null) {
                users.Add(user);
                return true;
            }
            return false;
        }

        public bool UpdateUser(string username, string name, string bio, string image) {
            User u1 = GetUserByUsername(username);
            if (u1 != null) {
                users.Find(u => u == u1).SetUserData(name, bio, image);
                return true;
            }
            return false;
        }

        private User GetUserByUsername(string username) {
            return users.FirstOrDefault(u => u.Username == username);
        }
    }
}
