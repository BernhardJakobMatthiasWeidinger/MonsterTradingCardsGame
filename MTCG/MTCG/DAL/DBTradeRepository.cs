using MTCG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public class DBTradeRepository {
        private readonly List<Trade> trades = new List<Trade>();

        public string GetTrades(User user, bool json) {
            if (json) {
                JArray array = new JArray();
                foreach (Trade t in trades.Where(t => t.Provider != user)) {
                    array.Add(t.TradeToString(json));
                }
                return JsonConvert.SerializeObject(array);
            } else {
                string tradesString = "";
                foreach (Trade t in trades.Where(t => t.Provider != user)) {
                    tradesString += t.TradeToString(json) + "\n";
                }
                return tradesString;
            }
        }

        public void AddTradeAtServerStart(Trade trade) {
            trades.Add(trade);
        }

        public void CreateTrade(Guid id, Card cardToTrade, User provider, CardType cardType, ElementType? elementType, double minimumDamage) {
            if (trades.Any(t => t.Id == id || t.CardToTrade == cardToTrade)) {
                throw new ArgumentException("Trade already exists");
            } else {
                Trade trade = new Trade(id, cardToTrade, provider, cardType, elementType, minimumDamage);
                DBConnection.InsertTrade(trade);
                trades.Add(trade);
            }
        }
    }
}
