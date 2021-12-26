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

        public MTCGManager(DBUserRepository dBUserRepository, DBCardRepository dBCardRepository) {
            this.dBUserRepository = dBUserRepository;
            this.dBCardRepository = dBCardRepository;
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

        public List<Card> AcquirePackage(Guid userId) {
            return null; // dBCardRepository.AcquirePackage(username, payload);
        }

        public List<Card> GetStack(Guid userId) {
            return dBCardRepository.GetStack(userId);
        }

        public List<Card> GetDeck(Guid userId) {
            return dBCardRepository.GetDeck(userId);
        }
    }
}
