using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public interface IRule {
        Tuple<double, double> checkRule(Card card1, Card card2, double calcDamage1, double calcDamage2);
    }

    public class ElementRule : IRule {
        private ElementType element1;
        private ElementType element2;
        private double multiplier1;
        private double multiplier2;

        public ElementRule(ElementType element1, ElementType element2, double multiplier1, double multiplier2) {
            this.element1 = element1;
            this.element2 = element2;
            this.multiplier1 = multiplier1;
            this.multiplier2 = multiplier2;
        }

        public Tuple<double, double> checkRule(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            if (card1.elementType == element1 && card2.elementType == element2) {
                return new Tuple<double, double>(calcDamage1 * multiplier1, calcDamage2 * multiplier2);
            } else if (card1.elementType == element2 && card2.elementType == element1) {
                return new Tuple<double, double>(calcDamage1 * multiplier2, calcDamage2 * multiplier1);
            }
            return null;
        }
    }

    public class SpecialRule : IRule {
        private string monsterName1;
        private string monsterName2;
        private double multiplier1;
        private double multiplier2;

        public SpecialRule(string monsterName1, string monsterName2, double multiplier1, double multiplier2) {
            this.monsterName1 = monsterName1;
            this.monsterName2 = monsterName2;
            this.multiplier1 = multiplier1;
            this.multiplier2 = multiplier2;
        }

        public Tuple<double, double> checkRule(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            if (card1.name.Contains(monsterName1) && card1.name.Contains(monsterName2)) {
                return new Tuple<double, double>(calcDamage1 * multiplier1, calcDamage2 * multiplier2);
            } else if (card1.name.Contains(monsterName2) && card1.name.Contains(monsterName1)) {
                return new Tuple<double, double>(calcDamage1 * multiplier2, calcDamage2 * multiplier1);
            }
            return null;
        }
    }
}
