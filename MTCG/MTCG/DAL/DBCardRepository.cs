using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public class DBCardRepository {
        private readonly List<Tuple<Card, bool, Guid>> cards = new List<Tuple<Card, bool, Guid>>();
        private readonly List<Card> packages = new List<Card>();

        public DBCardRepository() {
            cards = DBConnection.SelectAllCards();
        }

        public List<Card> GetStack(Guid userId) {
            List<Card> deck = new List<Card>();
            foreach (Tuple<Card, bool, Guid> tuple in cards.FindAll(t => t.Item3 == userId)) {
                deck.Add(tuple.Item1);
            }

            return deck;
        }

        public List<Card> GetDeck(Guid userId) {
            List<Card> deck = new List<Card>();
            foreach (Tuple<Card, bool, Guid> tuple in cards.FindAll(t => t.Item3 == userId && t.Item2 == true)) {
                deck.Add(tuple.Item1);
            }

            return deck;
        }
    }
}
