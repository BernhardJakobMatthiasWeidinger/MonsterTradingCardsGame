using MTCG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTCG.DAL {
    public class DBUserRepository : IUserRepository {
        private readonly List<User> users = new List<User>();

        public DBUserRepository() {
            users = DBConnection.SelectAllUsers();
        }
        public User GetUserByAuthToken(string authToken) {
            return users.FirstOrDefault(user => user.Username + "-mtcgToken" == authToken);
        }

        public User GetUserByCredentials(string username, string password) {
            return users.FirstOrDefault(user => user.Username == username && user.Password == password);
        }

        public bool InsertUser(User user) {
            if (GetUserByUsername(user.Username) == null) {
                users.Add(user);
                DBConnection.InsertUser(user);
                return true;
            }
            return false;
        }

        public bool UpdateUser(string username, string name, string bio, string image) {
            User u1 = GetUserByUsername(username);
            if (u1 != null) {
                users.Find(u => u == u1).SetUserData(name, bio, image);
                DBConnection.UpdateUser(GetUserByUsername(username));
                return true;
            }
            return false;
        }

        public string GetScoreboard(bool json) {
            users.Sort((x, y) => x.Elo - y.Elo);
            if (json) {
                JArray jArray = new JArray();
                foreach (User user in users.Where(u => u.Username != "admin")) {
                    jArray.Add(user.GetUserStats(json));
                }
                return JsonConvert.SerializeObject(jArray);
            } else {
                string scoreboard = "";
                foreach (User user in users.Where(u => u.Username != "admin")) {
                    scoreboard +=  user.GetUserStats(json) + "\n";
                }
                return scoreboard;
            }
        }

        private User GetUserByUsername(string username) {
            return users.FirstOrDefault(u => u.Username == username);
        }

        public User GetUserById(Guid id) {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAllUsers() {
            return users;
        }
    }
}
