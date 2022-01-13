using MTCG.Exceptions;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL {
    public class DBBattleRepository : IBattleRepository {
        private readonly List<Battle> battles = new List<Battle>();

        public string Battle(User user, string user1Name = "") {
            string log = "";

            if (battles.Any(b => b.User1 == user || b.User2 == user)) {
                throw new EntityAlreadyExistsException();
            }


            Battle battle;
            if (user1Name == "") {
                bool joinBattle = false;
                lock (this) {
                    //Only players with elo difference of 10 or smaller can battle each other
                    battle = battles.FirstOrDefault(b => Math.Abs(b.User1.Elo - user.Elo) <= 10);
                    joinBattle = (battle != null) ? true : false;
                    if (!joinBattle) {
                        battle = new Battle(Guid.NewGuid(), user);
                        battles.Add(battle);
                    } 
                }

                //join or initiate random battle
                if (joinBattle) {
                    log = battle.Play(user);
                } else {
                    log = battle.InitializeBattle();
                }
            } else {
                //join specific battle of friend with username
                lock (this) {
                    battle = battles.FirstOrDefault(b => b.User1.Username == user1Name);
                    if (battle == null) {
                        throw new EntityNotFoundException("No battle with specified user found.");
                    } else if (!battle.User1.Friends.Contains(user.Id)) {
                        throw new FriendException();
                    } 
                    log = battle.Play(user);
                }
            }

            //update database
            lock (this) {
                DBConnection.UpdateUser(user);
                if (battles.Contains(battle)) {
                    battles.Remove(battle);
                }
            }

            return log;
        }
    }
}
