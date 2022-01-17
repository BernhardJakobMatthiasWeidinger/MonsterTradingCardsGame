using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Models {
    public class RuleSet {
        private static List<ElementRule> elementRules = new List<ElementRule> {
            new ElementRule(ElementType.fire, ElementType.normal, 2, 0.5),
            new ElementRule(ElementType.normal, ElementType.water, 2, 0.5),
            new ElementRule(ElementType.water, ElementType.fire, 2, 0.5)
        };

        private static List<SpecialRule> specialRules = new List<SpecialRule> {
            new SpecialRule("goblin", "dragon", 0, null),
            new SpecialRule("wizzard", "ork", null, 0),
            new SpecialRule("knight", "waterspell", 0, 9999),
            new SpecialRule("kraken", "spell", null, 0),
            new SpecialRule("fireelf", "dragon", null, 0),
        };

        private static Tuple<double, double> CompareElementType(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            if (card1.GetType().Name == "SpellCard" || card2.GetType().Name == "SpellCard") {
                foreach (ElementRule er in elementRules) {
                    if (er.CheckRule(card1, card2, ref calcDamage1, ref calcDamage2)) {
                        break;
                    }
                }
            }
            return new Tuple<double, double>(calcDamage1, calcDamage2);
        }

        private static Tuple<double, double> CompareSpecialRule(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            foreach (SpecialRule sr in specialRules) {
                if (sr.CheckRule(card1, card2, ref calcDamage1, ref calcDamage2)) {
                    break;
                }
            }
            return new Tuple<double, double>(calcDamage1, calcDamage2);
        }

        public static string CompareAllRules(string user1, string user2, Card card1, Card card2, ref double calc1, ref double calc2) {
            string res = $"{user1}: {card1.Name} ({card1.Damage} Damage) vs {user2}: {card2.Name} ({card2.Damage} Damage)";

            Tuple<double, double> calcValues = RuleSet.CompareSpecialRule(card1, card2, calc1, calc2);
            if (calcValues.Item1 != calc1 || calcValues.Item2 != calc2) {
                res += $" => {calc1} VS {calc2} -> {calcValues.Item1} VS {calcValues.Item2}";
                calc1 = calcValues.Item1;
                calc2 = calcValues.Item2;
            } else {
                calcValues = RuleSet.CompareElementType(card1, card2, calc1, calc2);
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