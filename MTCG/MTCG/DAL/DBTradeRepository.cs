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

        public string GetTrades(bool json) {
            if (json) {
                JArray array = new JArray();
                foreach (Trade t in trades) {
                    array.Add(new JObject(t.TradeToString(json)));
                }
                return JsonConvert.SerializeObject(array);
            } else {
                string tradesString = "";
                foreach (Trade t in trades) {
                    tradesString += t.TradeToString(json) + "\n";
                }
                return tradesString;
            }
        }

        public void AddTrade(Trade t) {
            trades.Add(t);
        }
    }
}
