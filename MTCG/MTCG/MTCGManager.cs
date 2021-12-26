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
        private readonly DBPackageRepository dBCardRepository;
        private readonly DBTradeRepository dBTradeRepository;

        public MTCGManager(DBUserRepository dBUserRepository, DBPackageRepository dBCardRepository, DBTradeRepository dBTradeRepository) {
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
            return dBUserRepository.GetStack(userId);
        }

        public List<Card> GetDeck(Guid userId) {
            return dBUserRepository.GetDeck(userId);
        }

        public void ConfigureDeck(User user, List<Guid> cardIds) {
            dBUserRepository.ConfigureDeck(user, cardIds);
        }

        public string GetScoreboard(bool json) {
            return dBUserRepository.GetScoreboard(json);
        }

        private void GetTradesFromDB() {
            List<List<string>> ts = DBConnection.SelectAllTrades();

            foreach (List<string> trade in ts) {
                Card cardToTrade = dBUserRepository.GetCardById(Guid.Parse(trade[5]));
                User provider = dBUserRepository.GetUserById(Guid.Parse(trade[4]));
                CardType cardType = (CardType)Enum.Parse(typeof(CardType), trade[1]);
                ElementType? elementType = !String.IsNullOrWhiteSpace(trade[2]) ? (ElementType)Enum.Parse(typeof(ElementType), trade[2]) : null;

                Trade t = new Trade(Guid.Parse(trade[0]), cardToTrade, 
                    provider, cardType, elementType, Double.Parse(trade[3]));

                dBTradeRepository.AddTradeAtServerStart(t);
            }
        }

        public string GetTrades(User user, bool json) {
            return dBTradeRepository.GetTrades(user, json);
        }

        public void CreateTrade(User provider, string payload) {
            JObject json = (JObject)JsonConvert.DeserializeObject(payload);

            Guid id = Guid.Parse(json["Id"].ToString());
            Card cardToTrade = dBUserRepository.GetCardById(Guid.Parse(json["CardToTrade"].ToString()));
            ElementType? elementType = null;
            if (json["elementType"] != null) {
                elementType = (ElementType)Enum.Parse(typeof(ElementType), json["elementType"].ToString());
            }
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), json["Type"].ToString());
            double minimumDamage = Double.Parse(json["MinimumDamage"].ToString());
            
            dBTradeRepository.CreateTrade(id, cardToTrade, provider, cardType, elementType, minimumDamage);
            
        }
    }
}
