using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public interface IRule {
        bool checkRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2);
    }

    public class ElementRule : IRule {
        public ElementType element1 { get; private set; }
        public ElementType element2 { get; private set; }
        public double multiplier1 { get; private set; }
        public double multiplier2 { get; private set; }

        public ElementRule(ElementType element1, ElementType element2, double multiplier1, double multiplier2) {
            if (multiplier1 < 0 || multiplier2 < 0) {
                throw new ArgumentException("Multipliers have to be positive.");
            }
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
        public string cardName1 { get; private set; }
        public string cardName2 { get; private set; }
        public double? damage1 { get; private set; }
        public double? damage2 { get; private set; }

        public SpecialRule(string cardName1, string cardName2, double? damage1, double? damage2) {
            if (damage1.HasValue) {
                if (damage1.Value < 0) 
                    throw new ArgumentException("Damages of rule have to be positive.");
            }
            if (damage2.HasValue) {
                if (damage2.Value < 0)
                    throw new ArgumentException("Damages of rule have to be positive.");
            }
            this.cardName1 = cardName1.ToLower();
            this.cardName2 = cardName2.ToLower();
            this.damage1 = damage1;
            this.damage2 = damage2;
        }

        public bool checkRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2) {
            if (card1.name.ToLower().Contains(cardName1) && card2.name.ToLower().Contains(cardName2)) {
                calcDamage1 = damage1 != null ? damage1.Value : calcDamage1;
                calcDamage2 = damage2 != null ? damage2.Value : calcDamage2;
                return true;
            } else if (card1.name.ToLower().Contains(cardName2) && card2.name.ToLower().Contains(cardName1)) {
                calcDamage1 = damage2 != null ? damage2.Value : calcDamage1;
                calcDamage2 = damage1 != null ? damage1.Value : calcDamage2;
                return true;
            }
            return false;
        }
    }
}
