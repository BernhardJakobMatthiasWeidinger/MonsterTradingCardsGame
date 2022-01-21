using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.DAL {
    public interface ITradeRepository {
        string GetTrades(User user, bool json);
        void AddTradeAtServerStart(Trade trade);
        void CreateTrade(Guid id, Card cardToTrade, User provider, CardType cardType, double minimumDamage);
        void TradeCard(User trader, Guid tradeId, Card card);
        void DeleteTrade(User provider, Guid tradeId);
        bool CheckIfTradeExist(User user, List<Guid> cardIds);
    }
}
