using MTCG.DAL;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG {
    public class MTCGManager {
        private readonly DBUserRepository dBUserRepository;

        public MTCGManager(DBUserRepository dBUserRepository) {
            this.dBUserRepository = dBUserRepository;
        }

        public User LoginUser(string username, string password) {
            return dBUserRepository.GetUserByCredentials(username, password);
        }
        public bool RegisterUser(string username, string password) {
            User user = new User(Guid.NewGuid(), username, password);
            return dBUserRepository.InsertUser(user);
        }
    }
}
