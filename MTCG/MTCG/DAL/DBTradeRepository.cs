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

        public void CreateTrade(Guid id, Card cardToTrade, User provider, CardType cardType, double minimumDamage) {
            lock (this) {
                if (trades.Any(t => t.Id == id || t.CardToTrade == cardToTrade)) {
                    throw new ArgumentException("Trade already exists");
                } else {
                    Trade trade = new Trade(id, cardToTrade, provider, cardType, minimumDamage);
                    DBConnection.InsertTrade(trade);
                    trades.Add(trade);
                }
            }
        }

        public void TradeCard(User trader, Guid tradeId, Card card) {
            lock (this) {
                Trade trade = trades.FirstOrDefault(t => t.Id == tradeId);
                if (trade == null) {
                    throw new ArgumentException("Trade not found");
                } else if (trade.Provider == trader) {
                    throw new InvalidOperationException("Cannot trade card with yourself");
                }

                trade.TradeCard(trader, card);
                trades.Remove(trade);

                DBConnection.UpdateCard(trade.CardToTrade.Id, false, trader.Id);
                DBConnection.UpdateCard(card.Id, false, trade.Provider.Id);
                DBConnection.DeleteTrade(trade);
            }
        }

        public void DeleteTrade(User provider, Guid tradeId) {
            lock (this) {
                Trade trade = trades.FirstOrDefault(t => t.Id == tradeId);
                if (trade == null) {
                    throw new ArgumentException("Trade not found");
                }
                else if (trade.Provider != provider) {
                    throw new InvalidOperationException("Cannot delete trade");
                }
                trades.Remove(trade);
                DBConnection.DeleteTrade(trade);
            }
        }
    }
}
