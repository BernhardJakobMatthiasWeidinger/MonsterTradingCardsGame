﻿using System;
using System.Threading;
using MTCG.Exceptions;

namespace MTCG.Models {
    public class Battle {
        public Guid Id { get; private set; }
        public User User1 { get; private set; }
        public User User2 { get; private set; }

        private string log;
        private AutoResetEvent waitHandle = new AutoResetEvent(false);

        public Battle(Guid id, User user1) {
            if (user1.Deck.Count != 4) {
                throw new InconsistentNumberException();
            }
            this.Id = id;
            this.User1 = user1;
        }

        public string InitializeBattle() {
            waitHandle.WaitOne();
            return log;
        }

        public string Play(User user2) {
            if (user2.Deck.Count != 4) {
                throw new InconsistentNumberException();
            }
            this.User2 = user2;

            string res = "";
            for (int i=1; i <= 100; ++i) {
                res += CompareCards(i) + "\n";
                if (User1.Deck.Count == 0 || User2.Deck.Count == 0) {
                    break;
                } 
            }

            res += $"{User1.Username} Cards: {User1.Deck.Count}, {User2.Username} Cards: {User2.Deck.Count}";
            if (User1.Deck.Count > User2.Deck.Count) {
                 res += $" => Winner: {User1.Username}";
                CalculateStatsAfterBattle(User1, User2);
            } else if (User1.Deck.Count < User2.Deck.Count) {
                CalculateStatsAfterBattle(User2, User1);
                res += $" => Winner: {User2.Username}";
            } else {
                CalculateStatsAfterBattle(User1, User2, true);
                res += $" => Draw";
            }
            log = res += "\n";

            User1.ConfigureDeckAfterBattle();
            User2.ConfigureDeckAfterBattle();
            waitHandle.Set();

            return log;
        }

        private string CompareCards(int round) {
            //Round 1: PlayerA: FireSpell (10 Damage) vs PlayerB: WaterSpell (20 Damage) => 10 VS 20 -> 05 VS 40 => WaterSpell wins
            Card card1 = User1.GetCardFromDeck();
            Card card2 = User2.GetCardFromDeck();

            double calc1 = card1.Damage;
            double calc2 = card2.Damage;
            
            string res = $"Round {round}: " + RuleSet.CompareAllRules(User1.Username, User2.Username, card1, card2, ref calc1, ref calc2);

            string winner = "Draw";
            if (calc1 > calc2) {
                winner = $"{card1.Name} wins";
                GiveCard(User1, User2, card2);
            } else if (calc1 < calc2) {
                winner = $"{card2.Name} wins";
                GiveCard(User2, User1, card1);
            }

            return res + $" => {winner}";
        }

        private void CalculateStatsAfterBattle(User winner, User loser, bool draw=false) {
            winner.GamesPlayed++;
            loser.GamesPlayed++;
            if (!draw) {
                winner.GamesWon++;
                winner.Elo += 3;
                loser.GamesLost++;
                loser.Elo -= 5;
            }
        }
        
        private void GiveCard(User winner, User loser, Card cardToGive) {
            loser.Deck.Remove(cardToGive);
            loser.Stack.Remove(cardToGive);
            winner.Stack.Add(cardToGive);
            winner.Deck.Add(cardToGive);
        }
    }
}
