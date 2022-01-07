using MTCG.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Models {
    public class Package {
        public Guid Id { get; private set; }
        public List<Card> Cards { get; private set; }

        public Package(Guid id, List<Card> cards) {
            if (cards.Count != 5) {
                throw new InconsistentNumberException();
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
                throw new InconsistentNumberException();
            }
        }
    }
}