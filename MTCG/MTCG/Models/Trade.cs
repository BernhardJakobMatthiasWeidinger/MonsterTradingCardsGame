using MTCG.Exceptions;
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
        public double MinimumDamage { get; private set; }

        public Trade(Guid id, Card cardToTrade, User user, CardType cardType, double minimumDamage) {
            this.Id = id;
            this.Provider = user;
            this.CardType = cardType;
            this.MinimumDamage = minimumDamage;

            if (user.Deck.Contains(cardToTrade)) {
                throw new InDeckException();
            } else if (!user.Stack.Contains(cardToTrade)) {
                throw new NotInDeckOrStackException();
            } else {
                this.CardToTrade = cardToTrade;
            }
        }

        public override string ToString() {
            return $"TradeId: {Id, -35} CardToTrade: {CardToTrade, -75} Type: {CardType, -8} MinimumDamage: {MinimumDamage, -3}"; ;
        }

        public void TradeCard(User u2, Card cardForTrade) {
            if (Provider == u2) {
                throw new InvalidOperationException();
            } else if (u2.Deck.Contains(cardForTrade)) {
                throw new InDeckException();
            } else if (!u2.Stack.Contains(cardForTrade)) {
                throw new NotInDeckOrStackException();
            } else if (cardForTrade.GetType().Name == "MonsterCard" && CardType == CardType.spell ||
                cardForTrade.GetType().Name == "SpellCard" && CardType == CardType.monster) {
                throw new InvalidCardTypeException();
            } else if (this.MinimumDamage > cardForTrade.Damage) {
                throw new InconsistentNumberException();
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