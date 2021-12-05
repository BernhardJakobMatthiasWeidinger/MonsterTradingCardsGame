using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public interface IRule {
        bool checkRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2);
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

        public bool checkRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2) {
            if (card1.elementType == element1 && card2.elementType == element2) {
                calcDamage1 *= multiplier1;
                calcDamage2 *= multiplier2;
                return true;
            } else if (card1.elementType == element2 && card2.elementType == element1) {
                calcDamage1 *= multiplier2;
                calcDamage2 *= multiplier1;
                return true;
            }
            return false;
        }
    }

    public class SpecialRule : IRule {
        private string monsterName1;
        private string monsterName2;
        private double? damage1;
        private double? damage2;

        public SpecialRule(string monsterName1, string monsterName2, double? damage1, double? damage2) {
            this.monsterName1 = monsterName1;
            this.monsterName2 = monsterName2;
            this.damage1 = damage1;
            this.damage2 = damage2;
        }

        public bool checkRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2) {
            if (card1.name.ToLower().Contains(monsterName1) && card2.name.ToLower().Contains(monsterName2)) {
                calcDamage1 = damage1 != null ? damage1.Value : calcDamage1;
                calcDamage2 = damage2 != null ? damage2.Value : calcDamage2;
                return true;
            } else if (card1.name.ToLower().Contains(monsterName2) && card2.name.ToLower().Contains(monsterName1)) {
                calcDamage1 = damage2 != null ? damage2.Value : calcDamage1;
                calcDamage2 = damage1 != null ? damage1.Value : calcDamage2;
                return true;
            }
            return false;
        }
    }
}
