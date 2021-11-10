using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.src {
    public class User {
        public Guid id { get; private set; }
        public string username { get; private set; }
        public string password { get; private set; }

        public string name { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
        public int coins { get; set; }
        public int gamesPlayed { get; set; }
        public int gamesWon { get; set; }
        public int elo { get; set; }
        public List<Card> stack { get; set; }
        public List<Card> deck { get; set; }
        public List<User> friends { get; set; }

        public User(string username, string password) {
            this.username = username;
            this.password = password;

            this.bio = "Hier könnte ihre Biografie stehen!";
            this.coins = 20;
            this.gamesPlayed = 0;
            this.gamesWon = 0;
            this.elo = 100;
            this.stack = new List<Card>();
            this.deck = new List<Card>();
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

        public string getUserData(bool isJson) {
            return "";
        }

        public string getUserStats(bool isJson) {
            return "";
        }
    }
}
