using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public class Trade {
        public Guid Id { get; private set; }
        public Card CardToTrade { get; private set; }
        public User U1 { get; private set; }
        public CardType CardType { get; private set; }
        public ElementType ElementType { get; private set; }
        public double MinimumDamage { get; private set; }

        public Trade(Guid id, Card cardToTrade, User user, CardType cardType, 
            ElementType elementType, double minimumDamage) {
            this.Id = id;
            this.U1 = user;
            this.CardType = cardType;
            this.ElementType = elementType;
            this.MinimumDamage = minimumDamage;

            if (user.Deck.Contains(cardToTrade)) {
                throw new ArgumentException("Cannot trade card if it's in the deck.");
            } else if (!user.Stack.Contains(cardToTrade)) {
                throw new ArgumentException("Cannot trade card if it's not in the stack.");
            } else {
                this.CardToTrade = cardToTrade;
            }
        }

        public void TradeCard(User u2, Card cardForTrade) {
            if (U1 == u2) {
                throw new ArgumentException("Cannot trade card with yourself.");
            } else if (u2.Deck.Contains(cardForTrade)) {
                throw new ArgumentException("Cannot trade card if it's in the deck.");
            } else if (!u2.Stack.Contains(cardForTrade)) {
                throw new ArgumentException("Cannot trade card if it's not in the stack.");
            } else if (cardForTrade.GetType().Name == "MonsterCard" && CardType == CardType.spell ||
                cardForTrade.GetType().Name == "SpellCard" && CardType == CardType.monster) {
                throw new ArgumentException("Cannot trade card, wrong card type was provided.");
            } else if (this.ElementType != cardForTrade.ElementType) {
                throw new ArgumentException("Cannot trade card, wrong element type was provided.");
            } else if (this.MinimumDamage > cardForTrade.Damage) {
                throw new ArgumentException("Cannot trade card, damage of card is too small.");
            }

            //trade provided card
            u2.Stack.Remove(cardForTrade);
            U1.Stack.Add(cardForTrade);

            //trade card of Trade
            U1.Stack.Remove(CardToTrade);
            u2.Stack.Add(CardToTrade);
        }
    }
}