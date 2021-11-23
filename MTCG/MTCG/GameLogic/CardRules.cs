using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public class CardRules {
        public static Tuple<double, double> compareUniqueRule(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            Tuple<double, double> res = new Tuple<double, double>(calcDamage1, calcDamage2);

            if (card1.name.ToLower().Contains("goblin") && card2.name.ToLower().Contains("dragon") ||
                card1.name.ToLower().Contains("ork") && card2.name.ToLower().Contains("wizard") ||
                card1.name.ToLower().Contains("spell") && card2.name.ToLower().Contains("kraken") ||
                card1.name.ToLower().Contains("dragon") && card2.name.ToLower().Contains("fireelf")) {
                res = new Tuple<double, double>(0, calcDamage2);
            } else if (card1.name.ToLower().Contains("wizard") && card2.name.ToLower().Contains("ork") ||
                card1.name.ToLower().Contains("kraken") && card2.name.ToLower().Contains("spell") ||
                card1.name.ToLower().Contains("fireelf") && card2.name.ToLower().Contains("dragon") ||
                card1.name.ToLower().Contains("dragon") && card2.name.ToLower().Contains("goblin")) {
                res = new Tuple<double, double>(calcDamage1, 0);
            } else if (card1.name.ToLower().Contains("knight") && card2.name.ToLower().Contains("waterspell")) {
                res = new Tuple<double, double>(0, 9999);
            } else if (card1.name.ToLower().Contains("waterspell") && card2.name.ToLower().Contains("knight")) {
                res = new Tuple<double, double>(9999, 0);
            } 

            return res;
        }

        public static Tuple<double, double> compareElementType(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            Tuple<double, double> res = new Tuple<double, double>(calcDamage1, calcDamage2);

            if (card1.GetType().Name == "SpellCard" || card2.GetType().Name == "SpellCard") {
                //if card1 has disadvantage type
                if (card1.elementType == ElementType.fire && card2.elementType == ElementType.water ||
                    card1.elementType == ElementType.water && card2.elementType == ElementType.normal ||
                    card1.elementType == ElementType.normal && card2.elementType == ElementType.fire) {
                    res = new Tuple<double, double>(calcDamage1 / 2, calcDamage2 * 2);
                    //if card2 has disadvantage type
                } else if (card1.elementType == ElementType.fire && card2.elementType == ElementType.normal ||
                    card1.elementType == ElementType.water && card2.elementType == ElementType.fire ||
                    card1.elementType == ElementType.normal && card2.elementType == ElementType.water) {
                    res = new Tuple<double, double>(calcDamage1 * 2, calcDamage2 / 2);
                } 
            }

            return res;
        }

        public static string compareAllRules(string user1, string user2, Card card1, Card card2, ref double calc1, ref double calc2) {
            string res = $"{user1}: {card1.name} ({card1.damage} Damage) vs {user2}: {card2.name} ({card2.damage} Damage)";

            Tuple<double, double> calcValues = CardRules.compareUniqueRule(card1, card2, calc1, calc2);
            if (calcValues.Item1 != calc1 || calcValues.Item2 != calc2) {
                res += $" => {calc1} VS {calc2} -> {calcValues.Item1} VS {calcValues.Item2}";
                calc1 = calcValues.Item1;
                calc2 = calcValues.Item2;
            } else {
                calcValues = CardRules.compareElementType(card1, card2, calc1, calc2);
                if (calcValues.Item1 != calc1 || calcValues.Item2 != calc2) {
                    res += $" => {calc1} VS {calc2} -> {calcValues.Item1} VS {calcValues.Item2}";
                    calc1 = calcValues.Item1;
                    calc2 = calcValues.Item2;
                }
            }

            return res;
        }
    }
}