using MTCG.DAL;
using MTCG.Models;
using MTCG.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MTCG {
    public class MTCGManager {
        private readonly DBUserRepository dBUserRepository;
        private readonly DBPackageRepository dBCardRepository;
        private readonly DBTradeRepository dBTradeRepository;
        private readonly DBBattleRepository dBBattleRepository;

        public MTCGManager(DBUserRepository dBUserRepository, DBPackageRepository dBCardRepository, 
            DBTradeRepository dBTradeRepository, DBBattleRepository dBBattleRepository) {
            this.dBUserRepository = dBUserRepository;
            this.dBCardRepository = dBCardRepository;
            this.dBTradeRepository = dBTradeRepository;
            this.dBBattleRepository = dBBattleRepository;

            GetTradesFromDB();
        }

        public User LoginUser(string username, string password) {
            return dBUserRepository.GetUserByCredentials(username, ConvertToHash(password));
        }

        public bool RegisterUser(string username, string password) {
            try {
                User user = new User(Guid.NewGuid(), username, ConvertToHash(password));
                return dBUserRepository.InsertUser(user);
            } catch (ArgumentException) {
                return false;
            }
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
            if (dBTradeRepository.CheckIfTradeExist(user, cardIds)) {
                throw new EntityAlreadyExistsException();
            } else {
                dBUserRepository.ConfigureDeck(user, cardIds);
            }
        }

        public string GetScoreboard(bool json) {
            return dBUserRepository.GetScoreboard(json);
        }

        public string GetLogFromBattle(User user, string username) {
            return dBBattleRepository.Battle(user, username);
        }

        private void GetTradesFromDB() {
            List<List<string>> ts = DBConnection.SelectAllTrades();

            foreach (List<string> trade in ts) {
                Card cardToTrade = dBUserRepository.GetCardById(Guid.Parse(trade[4]));
                User provider = dBUserRepository.GetUserById(Guid.Parse(trade[3]));
                CardType cardType = (CardType)Enum.Parse(typeof(CardType), trade[1]);

                Trade t = new Trade(Guid.Parse(trade[0]), cardToTrade, provider, cardType, Double.Parse(trade[2]));

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
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), json["Type"].ToString());
            double minimumDamage = Double.Parse(json["MinimumDamage"].ToString());
            
            dBTradeRepository.CreateTrade(id, cardToTrade, provider, cardType, minimumDamage);
        }

        public void TradeCard(User provider, string tId, string cId) {
            Guid tradeId = Guid.Parse(tId);
            Guid cardId = Guid.Parse(cId);
            dBTradeRepository.TradeCard(provider, tradeId, dBUserRepository.GetCardById(cardId));
        }

        public void DeleteTrade(User provider, string id) {
            Guid tradeId = Guid.Parse(id);
            dBTradeRepository.DeleteTrade(provider, tradeId);
        }

        public string GetFriends(User user, bool json) {
            return dBUserRepository.GetFriends(user, json);
        }

        public void AddFriend(User user, string other) {
            dBUserRepository.AddFriend(user, other);
        }

        public void DeleteFriend(User user, string other) {
            dBUserRepository.DeleteFriend(user, other);
        }

        private string ConvertToHash(string rawData) {
            // Create a SHA256 hash from password
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
