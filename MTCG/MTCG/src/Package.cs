using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.src {
    public class Package {
        public Guid id { get; private set; }
        public List<Card> cards { get; private set; }

        public Package(List<Card> cards) {
            if (cards.Count != 5) {
                throw new ArgumentException($"A package should be provided with 5 cards, " +
                    $"cards given: {cards.Count}");
            } else {
                this.cards = cards;
            }
        }

        public void aquirePackage(User user) {
            if (user.coins >= 5) {
                user.coins -= 5;
                user.stack.AddRange(cards);
            } else {
                throw new ArgumentException($"User {user.username} has an insufficent amount of coins ({user.coins}), coins needed: 5");
            }
        }
    }
}