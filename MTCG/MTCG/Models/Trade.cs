using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Models {
    public class Trade {
        public Guid Id { get; private set; }
        public Card CardToTrade { get; private set; }
        [JsonIgnore]
        public User Provider { get; private set; }
        public CardType CardType { get; private set; }
        public ElementType? ElementType { get; private set; }
        public double MinimumDamage { get; private set; }

        public Trade(Guid id, Card cardToTrade, User user, CardType cardType, 
            ElementType? elementType, double minimumDamage) {
            this.Id = id;
            this.Provider = user;
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

        public override string ToString() {
            return $"TradeId:{Id},CardToTrade:{CardToTrade},Type:{CardType},ElementType:{ElementType},MinimumDamage:{MinimumDamage}"; ;
        }

        public void TradeCard(User u2, Card cardForTrade) {
            if (Provider == u2) {
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
            Provider.Stack.Add(cardForTrade);

            //trade card of Trade
            Provider.Stack.Remove(CardToTrade);
            u2.Stack.Add(CardToTrade);
        }

        public string TradeToString(bool json) {
            if (json) {
                return JsonConvert.SerializeObject(this);
            } else {
                return ToString();
            }
        }
    }
}