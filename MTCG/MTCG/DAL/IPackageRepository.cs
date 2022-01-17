using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.DAL {
    public interface IPackageRepository {
        Package CreatePackage(string username, List<Card> packageCards);
        bool AcquirePackage(User user);
    }
}
