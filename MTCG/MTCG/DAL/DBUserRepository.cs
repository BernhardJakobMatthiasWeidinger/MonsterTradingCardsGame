using MTCG.Exceptions;
using MTCG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTCG.DAL {
    public class DBUserRepository {
        private readonly List<User> users = new List<User>();

        public DBUserRepository() {
            //set values at server start from DB
            users = DBConnection.SelectAllUsers();
            AssignCardsAtStart();
            AssignFriendsAtStart();
        }
        public User GetUserByAuthToken(string authToken) {
            lock (this) {
                return users.FirstOrDefault(user => user.Username + "-mtcgToken" == authToken);
            }
        }

        public User GetUserByCredentials(string username, string password) {
            lock (this) {
                return users.FirstOrDefault(user => user.Username == username && user.Password == password);
            }
        }

        public bool InsertUser(User user) {
            if (GetUserByUsername(user.Username) == null) {
                lock (this) {
                    users.Add(user);
                    try {
                        DBConnection.InsertUser(user);
                    } catch (Exception) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public string GetScoreboard(bool json) {
            //get stats of all players sorted by elo
            lock (this) {
                users.Sort((x, y) => y.Elo - x.Elo);
                if (json) {
                    JArray jArray = new JArray();
                    foreach (User user in users.Where(u => u.Username != "admin")) {
                        jArray.Add(user.GetUserStats(json));
                    }
                    return JsonConvert.SerializeObject(jArray);
                }
                else {
                    string scoreboard = "";
                    foreach (User user in users.Where(u => u.Username != "admin")) {
                        scoreboard += user.GetUserStats(json) + "\n";
                    }
                    return scoreboard;
                }
            }
        }

        private User GetUserByUsername(string username) {
            lock (this) {
                return users.FirstOrDefault(u => u.Username == username);
            }
        }

        public User GetUserById(Guid id) {
            lock (this) {
                return users.FirstOrDefault(u => u.Id == id);
            }
        }

        public Card GetCardById(Guid id) {
            Card card = null;
            lock (this) {
                foreach (User user in users) {
                    card = user.Stack.FirstOrDefault(c => c.Id == id);
                    if (card != null) {
                        break;
                    }
                }
            }
            return card;
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

        public void AssignFriendsAtStart() {
            List<Tuple<Guid, Guid>> friendships = DBConnection.SelectAllFriends();

            foreach (Tuple<Guid, Guid> friends in friendships) {
                User user1 = users.FirstOrDefault(u => u.Id == friends.Item1);
                User user2 = users.FirstOrDefault(u => u.Id == friends.Item2);

                user1.AddFriend(user2);
            }
        }

        public List<Card> GetStack(Guid userId) {
            lock (this) {
                return users.FirstOrDefault(u => u.Id == userId).Stack;
            }
        }

        public List<Card> GetDeck(Guid userId) {
            lock (this) {
                return users.FirstOrDefault(u => u.Id == userId).Deck;
            }
        }

        public void ConfigureDeck(User user, List<Guid> cardIds) {
            if (cardIds.Count != 4) {
                throw new InconsistentNumberException();
            }

            foreach (Guid id in cardIds) {
                if (!user.Stack.Any(c => c.Id == id)) {
                    throw new NotInDeckOrStackException("Card not found in stack");
                }
            }

            lock (this) {
                //set inDeck flag of database to false
                user.Deck.ForEach(c => DBConnection.UpdateCard(c.Id, false, user.Id));
                user.ConfigureDeck(cardIds);
                user.Deck.ForEach(c => DBConnection.UpdateCard(c.Id, true, user.Id));
            }
        }

        public string GetFriends(User user, bool isJson) {
            //return all friends of user in json or plain format
            string res = "";

            lock (this) {
                if (isJson) {
                    JArray array = new JArray();
                    foreach (Guid friendId in user.Friends) {
                        array.Add(JsonConvert.SerializeObject(users.FirstOrDefault(u => u.Id == friendId)));
                    }
                    JObject o = new JObject();
                    o["Friends"] = array;
                    res = o.ToString();
                }
                else {
                    foreach (Guid friendId in user.Friends) {
                        res += users.FirstOrDefault(u => u.Id == friendId).ToString() + "\n";
                    }
                }
            }
            return res;
        }

        public void AddFriend(User user1, string other) {
            lock (this) {
                User user2 = users.FirstOrDefault(u => u.Username == other);
                if (user2 == null) {
                    throw new EntityNotFoundException();
                } 

                user1.AddFriend(user2);
                DBConnection.InsertFriend(user1.Id, user2.Id);
            }
        }

        public void DeleteFriend(User user1, string other) {
            lock (this) {
                User user2 = users.FirstOrDefault(u => u.Username == other);
                if (user2 == null) {
                    throw new EntityNotFoundException();
                } 

                user1.RemoveFriend(user2);
                DBConnection.DeleteFriend(user1.Id, user2.Id);
            }
        }
    }
}
