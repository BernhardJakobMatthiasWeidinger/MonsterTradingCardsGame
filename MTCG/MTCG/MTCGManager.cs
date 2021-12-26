using MTCG.DAL;
using MTCG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG {
    public class MTCGManager {
        private readonly DBUserRepository dBUserRepository;
        private readonly DBCardRepository dBCardRepository;
        private readonly DBTradeRepository dBTradeRepository;

        public MTCGManager(DBUserRepository dBUserRepository, DBCardRepository dBCardRepository, DBTradeRepository dBTradeRepository) {
            this.dBUserRepository = dBUserRepository;
            this.dBCardRepository = dBCardRepository;
            this.dBTradeRepository = dBTradeRepository;
            GetTradesFromDB();
        }

        public User LoginUser(string username, string password) {
            return dBUserRepository.GetUserByCredentials(username, password);
        }
        public bool RegisterUser(string username, string password) {
            User user = new User(Guid.NewGuid(), username, password);
            return dBUserRepository.InsertUser(user);
        }

        public Package CreatePackage(string username, string payload) {
            return dBCardRepository.CreatePackage(username, payload);
        }

        public bool AcquirePackage(User user) {
            return dBCardRepository.AcquirePackage(user);
        }

        public List<Card> GetStack(Guid userId) {
            return dBCardRepository.GetStack(userId);
        }

        public List<Card> GetDeck(Guid userId) {
            return dBCardRepository.GetDeck(userId);
        }

        public void ConfigureDeck(User user, List<Guid> cardIds) {
            dBCardRepository.ConfigureDeck(user, cardIds);
        }

        public string GetScoreboard(bool json) {
            return dBUserRepository.GetScoreboard(json);
        }

        private void GetTradesFromDB() {
            List<List<string>> ts = DBConnection.SelectAllTrades();

            foreach (List<string> trade in ts) {
                Trade t = new Trade(Guid.Parse(trade[0]), dBCardRepository.GetCardById(Guid.Parse(trade[1])), 
                    dBUserRepository.GetUserById(Guid.Parse(trade[2])),
                    (CardType)Enum.Parse(typeof(CardType), trade[3]), (ElementType)Enum.Parse(typeof(ElementType), trade[4]),
                    Double.Parse(trade[5]));

                dBTradeRepository.AddTrade(t);
            }
        }

        public string GetTrades(bool json) {
            return dBTradeRepository.GetTrades(json);
        }
    }
}
