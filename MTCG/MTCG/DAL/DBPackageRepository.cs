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
    public class DBPackageRepository {
        private readonly List<Package> packages = new List<Package>();

        public DBPackageRepository() {
            packages = DBConnection.SelectAllPackages();
        }

        public Package CreatePackage(string username, string payload) {
            if (username != "admin") {
                return null;
            }

            List<Card> packageCards = new List<Card>();
            JArray jsonCards = JsonConvert.DeserializeObject<JArray>(payload);
            Package package;

            lock (this) {
                foreach (JObject card in jsonCards) {
                    Guid id = Guid.Parse(card["Id"].ToString());
                    string name = card["Name"].ToString();
                    double damage = Double.Parse(card["Damage"].ToString());

                    if (name.ToLower().Contains("spell")) {
                        packageCards.Add(new SpellCard(id, name, damage));
                    } else {
                        packageCards.Add(new MonsterCard(id, name, damage));
                    }
                }
                package = new Package(Guid.NewGuid(), packageCards);
                packages.Add(package);
            }

            return package;
        }

        public bool AcquirePackage(User user) {
            lock (this) {
                if (packages.Count > 0) {
                    Package package = packages[0];

                    try {
                        List<Card> cs = package.Cards;
                        package.AquirePackage(user);
                        cs.ForEach(c => DBConnection.UpdateCard(c.Id, false, user.Id));
                    } catch (Exception) {
                        return false;
                    }

                    DBConnection.DeletePackage(package);
                    DBConnection.UpdateUser(user);
                    packages.Remove(package);
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
