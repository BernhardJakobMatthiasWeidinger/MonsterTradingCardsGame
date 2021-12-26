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

            foreach (Card card in packageCards) {
                cards.Add(new Tuple<Card, bool, Guid?>(null, false, null));
            }

            Package package = new Package(packageCards);
            packages.Add(package);

            return package;

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

        public Card GetCardById(Guid cardId) {
            return cards.FirstOrDefault(t => t.Item1.Id == cardId).Item1;
        }
    }
}
