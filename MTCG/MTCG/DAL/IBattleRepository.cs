using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public interface IBattleRepository {
        string Battle(User user, string user1Name = "");
    }
}
