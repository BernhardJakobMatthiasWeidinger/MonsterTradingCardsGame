using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTCG.src {
    public class User {
        public Guid id { get; private set; }
        public string username { get; private set; }
        [JsonIgnore]
        public string password { get; private set; }

        public string name { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
        public int coins { get; set; }
        public int gamesPlayed { get; set; }
        public int gamesWon { get; set; }
        public int gamesLost { get; set; }
        public int elo { get; set; }
        [JsonIgnore]
        public List<Card> stack { get; set; }
        [JsonIgnore]
        public List<Card> deck { get; set; }
        public List<User> friends { get; set; }

        public User(string username, string password) {
            this.id = Guid.NewGuid();
            if (username.IndexOfAny(new char[] { ',', ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Username is not allowed to contain following characters: , ; / \\ \' \"");
            }
            if (password.IndexOfAny(new char[] { ',', ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Password is not allowed to contain following characters: , ; / \\ \' \"");
            }
            this.username = username;
            this.password = password;

            this.bio = "Hier könnte deine Biografie stehen!";
            this.coins = 20;
            this.gamesPlayed = 0;
            this.gamesWon = 0;
            this.gamesLost = 0;
            this.elo = 100;
            this.stack = new List<Card>();
            this.deck = new List<Card>();
            this.friends = new List<User>();
        }

        public override string ToString() {
            string res = $"id:{id},username:{username},name:{name},bio:{bio},image:{image}," +
                    $"coins:{coins},gamesPlayed:{gamesPlayed},gamesWon:{gamesWon},gamesLost:{gamesLost},elo:{elo},friends:[";

            int i = 0;
            foreach (User friend in friends) {
                res += friend.ToString();
                res += i != (friends.Count - 1) ? ";" : "";
                i++;
            }
            res += "]";
            return res;
        }

        public void setUserData(string name, string bio, string image) {
            if (name.IndexOfAny(new char[] { ',', ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Name is not allowed to contain following characters: , ; / \\ \' \"");
            }
            if (bio.IndexOfAny(new char[] { ',', ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Bio is not allowed to contain following characters: , ; / \\ \' \"");
            }
            if (image.IndexOfAny(new char[] { ',', ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Image is not allowed to contain following characters: , ; / \\ \' \"");
            }

            this.name = name;
            this.bio = bio;
            this.image = image;
        }

        public Card getCardFromDeck() {
            if (deck.Count > 0) {
                Random rnd = new Random();
                int cardIndex = rnd.Next(0, deck.Count);

                return deck[cardIndex];
            } else {
                return null;
            }
        }

        public void addCards(List<Card> newCards, bool toStack, bool toDeck) {
            if (toStack) {
                List<Guid> ids = stack.Select(c1 => c1.id).Intersect(newCards.Select(c2 => c2.id)).ToList();
                List<Card> intersectingStackCards = stack.Where(x => ids.Contains(x.id)).ToList();

                if (intersectingStackCards.Count() != 0) {
                    throw new ArgumentException($"Cards with ids {String.Join(", ", ids.ToArray())} are already in the stack!");
                }
                stack.AddRange(newCards);
            }

            if (toDeck) {
                List<Guid> ids = deck.Select(c1 => c1.id).Intersect(newCards.Select(c2 => c2.id)).ToList();
                List<Card> intersectingDeckCards = deck.Where(x => ids.Contains(x.id)).ToList();

                if (intersectingDeckCards.Count() != 0) {
                    throw new ArgumentException($"Cards with ids {String.Join(", ", ids.ToArray())} are already in the deck!");
                }
                deck.AddRange(newCards);
            }
        }

        public void configureDeck(List<Guid> guids) {
            List<Card> res = new List<Card>();

            if (guids.Count != 4) {
                throw new ArgumentException($"A deck should be provided with 4 cards, " +
                    $"cards given: {guids.Count}");
            }

            foreach (Guid guid in guids) {
                Card card = stack.Find(c => c.id == guid);
                if (card != null) {
                    res.Add(card);
                } else {
                    throw new ArgumentException($"Card with id {guid} was not found in stack!");
                }
            }

            deck = res;
        }

        public void configureDeckAfterBattle() {
            if (deck.Count > 4) {
                //deck should only consist of the 4 strongest cards
                List<Card> sortedList = deck.OrderByDescending(c => c.damage).ToList().Take(4).ToList();
                deck = sortedList;
            } else if (deck.Count < 4) {
                //add strongest remaining cards from stack to deck
                List<Card> sortedList = stack.OrderByDescending(c => c.damage).ToList().Take(4 - deck.Count()).ToList();
                addCards(sortedList, false, true);
            }
        }

        public void addFriend(User other) {
            if (this.friends.Contains(other)) {
                throw new ArgumentException($"User {other.username} is already your friend!");
            } else {
                this.friends.Add(other);
            }
        }

        public string getUserData(bool isJson) {
            string res = "";
            if (isJson) {
                res = JsonConvert.SerializeObject(this);
            } else {
                res += this.ToString();
            }
            return res;
        }

        public string getUserStats(bool isJson) {
            string res = "";
            if (isJson) {
                JObject o = new JObject();
                o["gamesPlayed"] = gamesPlayed;
                o["gamesWon"] = gamesWon;
                o["gamesLost"] = gamesLost;
                o["elo"] = elo;
                res = o.ToString();
            } else {
                res += $"gamesPlayed:{gamesPlayed},gamesWon:{gamesWon},gamesLost:{gamesWon},elo:{elo}";
            }
            return res;
        }

        public string deckToString(bool isJson) {
            string res = "";
            if (isJson) {
                JArray array = new JArray();
                foreach (Card card in deck) {
                    array.Add(JsonConvert.SerializeObject(card));
                }
                JObject o = new JObject();
                o["deck"] = array;
                res = o.ToString();
            } else {
                int i = 0;
                foreach (Card card in deck) {
                    res += card.ToString();
                    res += i != (deck.Count - 1) ? ";" : "";
                    i++;
                }
            }
            return res;
        }

        public string stackToString(bool isJson) {
            string res = "";
            if (isJson) {
                JArray array = new JArray();
                foreach (Card card in stack) {
                    array.Add(JsonConvert.SerializeObject(card));
                }
                JObject o = new JObject();
                o["stack"] = array;
                res = o.ToString();
            } else {
                int i = 0;
                foreach (Card card in stack) {
                    res += card.ToString();
                    res += i != (stack.Count - 1) ? ";" : "";
                    i++;
                }
            }
            return res;
        }
    }
}
