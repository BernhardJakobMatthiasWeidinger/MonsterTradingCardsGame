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

            user1.gamesPlayed++;
            user2.gamesPlayed++;
            if (user1.deck.Count > user2.deck.Count) {
                user1.gamesWon++;
                user1.elo += 3;
                user2.gamesLost++;
                user2.elo -= 5;
            } else if (user1.deck.Count < user2.deck.Count) {
                user2.gamesWon++;
                user2.elo += 3;
                user1.gamesLost++;
                user1.elo -= 5;
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
            
            string res = $"Round {round}: {user1.username}: {card1.name} ({card1.damage} Damage) vs {user2.username}: {card2.name} ({card2.damage} Damage) ";
            if (card1.name.ToLower().Contains("goblin") && card2.name.ToLower().Contains("dragon") ||
                card1.name.ToLower().Contains("ork") && card2.name.ToLower().Contains("wizard") ||
                card1.name.ToLower().Contains("spell") && card2.name.ToLower().Contains("kraken") ||
                card1.name.ToLower().Contains("dragon") && card2.name.ToLower().Contains("fireelf")) {
                calc1 = 0;
                res += $"=> {card1.damage} VS {card2.damage} -> {calc1} VS {calc2} ";
            } else if (card1.name.ToLower().Contains("wizard") && card2.name.ToLower().Contains("ork") ||
                card1.name.ToLower().Contains("kraken") && card2.name.ToLower().Contains("spell") ||
                card1.name.ToLower().Contains("fireelf") && card2.name.ToLower().Contains("dragon") ||
                card1.name.ToLower().Contains("dragon") && card2.name.ToLower().Contains("goblin")) {
                calc2 = 0;
                res += $"=> {card1.damage} VS {card2.damage} -> {calc1} VS {calc2} ";
            } else if (card1.name.ToLower().Contains("knight") && card2.name.ToLower().Contains("waterspell")) {
                calc2 = 9999;
                res += $"=> {card1.damage} VS {card2.damage} -> {calc1} VS {calc2} ";
            } else if (card1.name.ToLower().Contains("waterspell") && card2.name.ToLower().Contains("knight")) {
                calc1 = 9999;
                res += $"=> {card1.damage} VS {card2.damage} -> {calc1} VS {calc2} ";
            } else if (card1.GetType().Name == "SpellCard" || card2.GetType().Name == "SpellCard") {
                //if card1 has disadvantage type
                if (card1.elementType == ElementType.fire && card2.elementType == ElementType.water ||
                    card1.elementType == ElementType.water && card2.elementType == ElementType.normal ||
                    card1.elementType == ElementType.normal && card2.elementType == ElementType.fire) {
                    calc1 /= 2;
                    calc2 *= 2;
                    res += $"=> {card1.damage} VS {card2.damage} -> {calc1} VS {calc2} ";
                } else if (card1.elementType == ElementType.fire && card2.elementType == ElementType.normal ||
                    card1.elementType == ElementType.water && card2.elementType == ElementType.fire ||
                    card1.elementType == ElementType.normal && card2.elementType == ElementType.water) {
                    calc1 *= 2;
                    calc2 /= 2;
                    res += $"=> {card1.damage} VS {card2.damage} -> {calc1} VS {calc2} ";
                }
            } 

            string winner = "Draw";
            if (calc1 > calc2) {
                winner = $"{card1.name} wins";
                user2.deck.Remove(card2);
                user2.stack.Remove(card2);
                user1.stack.Add(card2);
                user1.deck.Add(card2);
            } else if (calc1 < calc2) {
                winner = $"{card2.name} wins";
                user1.deck.Remove(card1);
                user1.stack.Remove(card1);
                user2.stack.Add(card1);
                user2.deck.Add(card1);
            }

            return res + $"=> {winner}";
        }
    }
}
