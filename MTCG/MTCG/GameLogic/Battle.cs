using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public class Battle {
        public Guid id { get; private set; }
        public User user1 { get; private set; }
        public User user2 { get; private set; }

        public Battle(Guid id, User user1) {
            if (user1.deck.Count != 4) {
                throw new ArgumentException($"A deck should consist of 4 cards, " +
                    $"cards in deck: {user1.deck.Count}");
            }
            this.id = id;
            this.user1 = user1;
        }

        public string play(User user2) {
            if (user2.deck.Count != 4) {
                throw new ArgumentException($"A deck should consist of 4 cards, " +
                    $"cards in deck: {user2.deck.Count}");
            }
            this.user2 = user2;

            string res = "";
            for (int i=1; i <= 100; ++i) {
                res += compareCards(i) + "\n";
                if (user1.deck.Count == 0 || user2.deck.Count == 0) {
                    break;
                } 
            }

            if (user1.deck.Count > user2.deck.Count) {
                calculateStatsAfterBattle(user1, user2);
            } else if (user1.deck.Count < user2.deck.Count) {
                calculateStatsAfterBattle(user2, user1);
            } 

            user1.configureDeckAfterBattle();
            user2.configureDeckAfterBattle();

            return res;
        }

        private string compareCards(int round) {
            //Round 1: PlayerA: FireSpell (10 Damage) vs PlayerB: WaterSpell (20 Damage) => 10 VS 20 -> 05 VS 40 => WaterSpell wins
            Card card1 = user1.getCardFromDeck();
            Card card2 = user2.getCardFromDeck();

            double calc1 = card1.damage;
            double calc2 = card2.damage;
            
            string res = $"Round {round}: " + RuleSet.compareAllRules(user1.username, user2.username, card1, card2, ref calc1, ref calc2);

            string winner = "Draw";
            if (calc1 > calc2) {
                winner = $"{card1.name} wins";
                giveCard(user1, user2, card2);
            } else if (calc1 < calc2) {
                winner = $"{card2.name} wins";
                giveCard(user2, user1, card1);
            }

            return res + $" => {winner}";
        }

        private void calculateStatsAfterBattle(User winner, User loser) {
            winner.gamesPlayed++;
            winner.gamesWon++;
            winner.elo += 3;
            loser.gamesPlayed++;
            loser.gamesLost++;
            loser.elo -= 5;
        }
        
        private void giveCard(User winner, User loser, Card cardToGive) {
            loser.deck.Remove(cardToGive);
            loser.stack.Remove(cardToGive);
            winner.stack.Add(cardToGive);
            winner.deck.Add(cardToGive);
        }
    }
}
