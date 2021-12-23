using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public interface IUserRepository {
        User GetUserByCredentials(string username, string password);
        User GetUserByAuthToken(string authToken);
        bool InsertUser(User user);
        bool UpdateUser(string username, string name, string bio, string image);
    }
}
