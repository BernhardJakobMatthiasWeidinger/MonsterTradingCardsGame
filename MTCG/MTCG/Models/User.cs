using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTCG.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWE1HttpServer.Core.Authentication;

namespace MTCG.Models {
    public class User : IIdentity {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        [JsonIgnore]
        public string Password { get; private set; }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Coins { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int Elo { get; set; }
        [JsonIgnore]
        public List<Card> Stack { get; set; }
        [JsonIgnore]
        public List<Card> Deck { get; set; }
        public List<Guid> Friends { get; set; }

        public User(Guid id, string username, string password) {
            this.Id = id;
            if (username.IndexOfAny(new char[] { ';', '/', '\\', '\'', '"' }) != -1 ||
                password.IndexOfAny(new char[] { ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException();
            }
            this.Username = username;
            this.Password = password;

            this.Bio = "Your bio could stand here!";
            this.Coins = 20;
            this.GamesPlayed = 0;
            this.GamesWon = 0;
            this.GamesLost = 0;
            this.Elo = 100;
            this.Stack = new List<Card>();
            this.Deck = new List<Card>();
            this.Friends = new List<Guid>();
        }

        public override string ToString() {
            string res = $"UserId:{Id, -35} Username:{Username, -20} Name:{Name, -30} Bio:{Bio, -30} Image:{Image, -5}" +
                    $"Coins:{Coins, -3} GamesPlayed:{GamesPlayed, -3} GamesWon:{GamesWon, -3} GamesLost:{GamesLost, -3} Elo:{Elo, -4} Friends:[";

            int i = 0;
            foreach (Guid friend in Friends) {
                res += friend.ToString();
                res += i != (Friends.Count - 1) ? ";" : "";
                i++;
            }
            res += "]";
            return res;
        }

        public void SetUserData(string name, string bio, string image) {
            this.Name = name;
            this.Bio = bio;
            this.Image = image;
        }

        public Card GetCardFromDeck() {
            if (Deck.Count > 0) {
                Random rnd = new Random();
                int cardIndex = rnd.Next(0, Deck.Count);

                return Deck[cardIndex];
            } else {
                return null;
            }
        }
        
        public void ConfigureDeck(List<Guid> guids) {
            List<Card> res = new List<Card>();

            if (guids.Count != 4) {
                throw new InconsistentNumberException();
            }

            foreach (Guid guid in guids) {
                Card card = Stack.Find(c => c.Id == guid);
                if (card != null) {
                    res.Add(card);
                } else {
                    throw new NotInDeckOrStackException();
                }
            }

            Deck = res;
        }

        public void AddFriend(User other) {
            if (this.Friends.Contains(other.Id)) {
                throw new FriendException($"User {other.Username} is already your friend!");
            } else if (this == other) {
                throw new FriendException($"You cannot befriend yourself!");
            } else { 
                this.Friends.Add(other.Id);
                other.Friends.Add(this.Id);
            }
        }

        public void RemoveFriend(User other) {
            if (this.Friends.Contains(other.Id)) {
                this.Friends.Remove(other.Id);
                other.Friends.Remove(this.Id);
            } else if (this == other) {
                throw new FriendException($"You cannot unfriend yourself!");
            } else {
                throw new FriendException($"User {other.Username} is not your friend!");
            }
        }

        public string GetUserData(bool isJson) {
            string res = "";
            if (isJson) {
                res = JsonConvert.SerializeObject(this);
            } else {
                res += this.ToString();
            }
            return res;
        }

        public string GetUserStats(bool isJson) {
            string res = "";
            if (isJson) {
                JObject o = new JObject();
                o["username"] = Username;
                o["gamesPlayed"] = GamesPlayed;
                o["gamesWon"] = GamesWon;
                o["gamesLost"] = GamesLost;
                o["elo"] = Elo;
                res = JsonConvert.SerializeObject(o);
            } else {
                res += $"username:{Username, -20} gamesPlayed:{GamesPlayed, -3} gamesWon:{GamesWon, -3} gamesLost:{GamesWon, -3} elo:{Elo, -4}";
            }
            return res;
        }

        public string DeckToString(bool isJson) {
            string res = "";
            if (isJson) {
                JArray array = new JArray();
                foreach (Card card in Deck) {
                    array.Add(JsonConvert.SerializeObject(card));
                }
                JObject o = new JObject();
                o["Deck"] = array;
                res = o.ToString();
            } else {
                foreach (Card card in Deck) {
                    res += card.ToString() + "\n";
                }
            }
            return res;
        }

        public string StackToString(bool isJson) {
            string res = "";
            if (isJson) {
                JArray array = new JArray();
                foreach (Card card in Stack) {
                    array.Add(JsonConvert.SerializeObject(card));
                }
                JObject o = new JObject();
                o["Stack"] = array;
                res = o.ToString();
            } else {
                foreach (Card card in Stack) {
                    res += card.ToString() + "\n";
                }
            }
            return res;
        }
    }
}
