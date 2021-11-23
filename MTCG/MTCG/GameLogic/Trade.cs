using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public class Trade {
        public Guid id { get; private set; }
        public Card cardToTrade { get; private set; }
        public User user { get; private set; }
        public CardType cardType { get; private set; }
        public ElementType elementType { get; private set; }
        public double minimumDamage { get; private set; }

        public Trade(Guid id, Card cardToTrade, User user, CardType cardType, 
            ElementType elementType, double minimumDamage) {
            this.id = id;
            this.user = user;
            this.cardType = cardType;
            this.elementType = elementType;
            this.minimumDamage = minimumDamage;

            if (user.deck.Contains(cardToTrade)) {
                throw new ArgumentException("Cannot trade card if it's in the deck.");
            } else if (!user.stack.Contains(cardToTrade)) {
                throw new ArgumentException("Cannot trade card if it's not in the stack.");
            } else {
                this.cardToTrade = cardToTrade;
            }
        }

        public void TradeCard(User u2, Card cardForTrade) {
            if (user == u2) {
                throw new ArgumentException("Cannot trade card with yourself.");
            } else if (u2.deck.Contains(cardForTrade)) {
                throw new ArgumentException("Cannot trade card if it's in the deck.");
            } else if (!u2.stack.Contains(cardForTrade)) {
                throw new ArgumentException("Cannot trade card if it's not in the stack.");
            } else if (cardForTrade.GetType().Name == "MonsterCard" && cardType == CardType.spell ||
                cardForTrade.GetType().Name == "SpellCard" && cardType == CardType.monster) {
                throw new ArgumentException("Cannot trade card, wrong card type was provided.");
            } else if (this.elementType != cardForTrade.elementType) {
                throw new ArgumentException("Cannot trade card, wrong element type was provided.");
            } else if (this.minimumDamage > cardForTrade.damage) {
                throw new ArgumentException("Cannot trade card, damage of card is too small.");
            }

            //trade provided card
            u2.stack.Remove(cardForTrade);
            user.stack.Add(cardForTrade);

            //trade card of Trade
            user.stack.Remove(cardToTrade);
            u2.stack.Add(cardToTrade);
        }
    }
}