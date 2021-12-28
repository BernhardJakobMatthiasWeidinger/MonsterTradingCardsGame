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
            AssignCardsAtStart();
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
            users.Sort((x, y) => y.Elo - x.Elo);
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
        public Card GetCardById(Guid id) {
            Card card = null;
            foreach (User user in users) {
                card = user.Stack.FirstOrDefault(c => c.Id == id);
                if (card != null) {
                    break;
                }
            }
            return card;
        }

        public List<User> GetAllUsers() {
            return users;
        }

        public void AssignCardsAtStart() {
            List<Tuple<Card, bool, Guid?>> cards = DBConnection.SelectAllCards();

            foreach (User user in users) {
                foreach (var card in cards) {
                    if (card.Item3 == user.Id) {
                        user.Stack.Add(card.Item1);

                        if (card.Item2 == true) {
                            user.Deck.Add(card.Item1);
                        }
                    }
                }
            }
        }
        public List<Card> GetStack(Guid userId) {
            return users.FirstOrDefault(u => u.Id == userId).Stack;
        }

        public List<Card> GetDeck(Guid userId) {
            return users.FirstOrDefault(u => u.Id == userId).Deck;
        }

        public void ConfigureDeck(User user, List<Guid> cardIds) {
            if (cardIds.Count != 4) {
                throw new ArgumentException("Insufficent amount of cards");
            }

            foreach (Guid id in cardIds) {
                if (!user.Stack.Any(c => c.Id == id)) {
                    throw new ArgumentException("Card not found in stack");
                }
            }

            user.Deck.ForEach(c => DBConnection.UpdateCard(c.Id, false, user.Id));
            user.ConfigureDeck(cardIds);
            user.Deck.ForEach(c => DBConnection.UpdateCard(c.Id, true, user.Id));
        }
    }
}
