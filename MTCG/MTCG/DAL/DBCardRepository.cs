using MTCG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public class DBCardRepository {
        private readonly List<Tuple<Card, bool, Guid?>> cards = new List<Tuple<Card, bool, Guid?>>();
        private readonly List<Package> packages = new List<Package>();

        public DBCardRepository() {
            cards = DBConnection.SelectAllCards();
            SelectAllPackages();
        }

        private void SelectAllPackages() {
            List<List<Guid>> ps = DBConnection.SelectAllPackages();

            foreach (List<Guid> package in ps) {
                List<Card> cs = new List<Card>();
                for (int i = 1; i <= 5; ++i) {
                    cs.Add(cards.Find(t => t.Item1.Id == package[i]).Item1);
                }

                packages.Add(new Package(package[0], cs));
            }
        }

        public Package CreatePackage(string username, string payload) {
            if (username != "admin") {
                return null;
            }

            List<Card> packageCards = new List<Card>();
            JArray jsonCards = JsonConvert.DeserializeObject<JArray>(payload);

            foreach (JObject card in jsonCards) {
                Guid id = Guid.Parse(card["Id"].ToString());
                string name = card["Name"].ToString();
                double damage = Double.Parse(card["Damage"].ToString());

                if (cards.Any(t => t.Item1.Id == id)) {
                    throw new ArgumentException();
                }

                if (name.ToLower().Contains("spell")) {
                    packageCards.Add(new SpellCard(id, name, damage));
                }
                else {
                    packageCards.Add(new MonsterCard(id, name, damage));
                }
            }

            Package package = new Package(Guid.NewGuid(), packageCards);
            packages.Add(package);

            foreach (Card c in packageCards) {
                cards.Add(new Tuple<Card, bool, Guid?>(c, false, null));
            }

            return package;
        }

        public bool AcquirePackage(User user) {
            if (packages.Count > 0) {
                Package package = packages[0];

                try {
                    package.AquirePackage(user);
                } catch (ArgumentException) {
                    return false;
                }

                DBConnection.DeletePackage(package);
                packages.Remove(package);
                return true;
            } else {
                return false;
            }
        }

        public List<Card> GetStack(Guid userId) {
            List<Card> deck = new List<Card>();
            foreach (Tuple<Card, bool, Guid?> tuple in cards.FindAll(t => t.Item3 == userId)) {
                deck.Add(tuple.Item1);
            }

            return deck;
        }

        public List<Card> GetDeck(Guid userId) {
            List<Card> deck = new List<Card>();
            foreach (Tuple<Card, bool, Guid?> tuple in cards.FindAll(t => t.Item3 == userId && t.Item2 == true)) {
                deck.Add(tuple.Item1);
            }

            return deck;
        }

        public void AssignCardsAtStart(User user) {
            foreach (var tuple in cards) {
                if (tuple.Item3 == user.Id) {
                    user.Stack.Add(tuple.Item1);

                    if (tuple.Item2 == true) {
                        user.Deck.Add(tuple.Item1);
                    }
                }
            }
        }

        public void ConfigureDeck(User user, List<Guid> cardIds) {
            foreach (Guid id in cardIds) {
                if (cards.Any(t => t.Item1.Id == id)) {
                    if (!cards.Any(t => t.Item3 == user.Id || t.Item3 == null)) {
                        throw new ArgumentException("Card not available.");
                    }
                } else {
                    throw new ArgumentException("Card not found.");
                }
            }

            cardIds.ForEach(c => DBConnection.UpdateCard(c, true, user.Id));
            user.ConfigureDeck(cardIds);
        }

        public Card GetCardById(Guid id) {
            return cards.FirstOrDefault(t => t.Item1.Id == id).Item1;
        }
    }
}
