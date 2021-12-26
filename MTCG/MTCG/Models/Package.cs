using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Models {
    public class Package {
        public Guid Id { get; private set; }
        public List<Card> Cards { get; private set; }

        public Package(Guid id, List<Card> cards) {
            if (cards.Count != 5) {
                throw new ArgumentException($"A package should be provided with 5 cards, " +
                    $"cards given: {cards.Count}");
            } else {
                this.Id = id;
                this.Cards = cards;
            }
        }

        public void AquirePackage(User user) {
            if (user.Coins >= 5) {
                user.Coins -= 5;
                user.Stack.AddRange(Cards);
            } else {
                throw new ArgumentException($"User {user.Username} has an insufficent amount of coins ({user.Coins}), coins needed: 5");
            }
        }
    }
}