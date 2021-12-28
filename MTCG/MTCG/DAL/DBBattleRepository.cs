using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public class DBBattleRepository {
        private readonly List<Battle> battles = new List<Battle>();

        public string Battle(User user, string user1Id = "") {
            string log = "";

            if (battles.Any(b => b.User1 == user || b.User2 == user)) {
                throw new ArgumentException("User is already in a battle");
            }

            //Only players with elo difference of 10 can battle each other

            Battle battle;
            if (user1Id == "") {
                battle = battles.FirstOrDefault(b => Math.Abs(b.User1.Elo - user.Elo) <= 10);
                if (battle != null) {
                    log = battle.Play(user);
                } else {
                    battle = new Battle(Guid.NewGuid(), user);
                    battles.Add(battle);
                    log = battle.InitializeBattle();
                }
            } else {
                battle = battles.FirstOrDefault(b => b.User1.Id == Guid.Parse(user1Id));
                if (battle == null) {
                    throw new InvalidCastException("No battle with specified user found.");
                }
                log = battle.Play(user);
            }

            lock (this) {
                if (battles.Contains(battle)) {
                    battles.Remove(battle);
                }
            }

            return log;
        }
    }
}
