using MTCG.Models;
using MTCG.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public class DBPackageRepository : IPackageRepository {
        private readonly List<Package> packages = new List<Package>();

        public DBPackageRepository() {
            packages = DBConnection.SelectAllPackages();
        }

        public Package CreatePackage(string username, List<Card> packageCards) {
            if (username != "admin") {
                return null;
            }

            //create cards and add to package
            lock (this) {
                Package package = new Package(Guid.NewGuid(), packageCards);
                DBConnection.InsertPackage(package);
                packages.Add(package);
                return package;
            }
        }

        public bool AcquirePackage(User user) {
            lock (this) {
                if (packages.Count > 0) {
                    Package package = packages[0];

                    try {
                        //assign all cards to user and delete package
                        List<Card> cs = package.Cards;
                        package.AquirePackage(user);
                        cs.ForEach(c => DBConnection.UpdateCard(c.Id, false, user.Id));

                        DBConnection.DeletePackage(package);
                        DBConnection.UpdateUser(user);
                        packages.Remove(package);
                    } catch (Exception) {
                        return false;
                    }

                    return true;
                } else {
                    throw new EntityNotFoundException();
                }
            }
        }

        public bool CheckIfCardExsists(Card card) {
            foreach (Package package in packages) {
                if (package.Cards.Any(c => c.Id == card.Id)) {
                    return true;
                }
            }
            return false;
        }
    }
}
