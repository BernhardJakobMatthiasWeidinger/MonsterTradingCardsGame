using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Models {
    public interface IRule {
        bool CheckRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2);
    }

    public class ElementRule : IRule {
        public ElementType Element1 { get; private set; }
        public ElementType Element2 { get; private set; }
        public double Multiplier1 { get; private set; }
        public double Multiplier2 { get; private set; }

        public ElementRule(ElementType element1, ElementType element2, double multiplier1, double multiplier2) {
            if (multiplier1 < 0 || multiplier2 < 0) {
                throw new ArgumentException("Multipliers have to be positive.");
            }
            this.Element1 = element1;
            this.Element2 = element2;
            this.Multiplier1 = multiplier1;
            this.Multiplier2 = multiplier2;
        }

        public bool CheckRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2) {
            if (card1.ElementType == Element1 && card2.ElementType == Element2) {
                calcDamage1 *= Multiplier1;
                calcDamage2 *= Multiplier2;
                return true;
            } else if (card1.ElementType == Element2 && card2.ElementType == Element1) {
                calcDamage1 *= Multiplier2;
                calcDamage2 *= Multiplier1;
                return true;
            }
            return false;
        }
    }

    public class SpecialRule : IRule {
        public string CardName1 { get; private set; }
        public string CardName2 { get; private set; }
        public double? Damage1 { get; private set; }
        public double? Damage2 { get; private set; }

        public SpecialRule(string cardName1, string cardName2, double? damage1, double? damage2) {
            if (damage1.HasValue) {
                if (damage1.Value < 0) 
                    throw new ArgumentException("Damages of rule have to be positive.");
            }
            if (damage2.HasValue) {
                if (damage2.Value < 0)
                    throw new ArgumentException("Damages of rule have to be positive.");
            }
            this.CardName1 = cardName1.ToLower();
            this.CardName2 = cardName2.ToLower();
            this.Damage1 = damage1;
            this.Damage2 = damage2;
        }

        public bool CheckRule(Card card1, Card card2, ref double calcDamage1, ref double calcDamage2) {
            if (card1.Name.ToLower().Contains(CardName1) && card2.Name.ToLower().Contains(CardName2)) {
                calcDamage1 = Damage1 != null ? Damage1.Value : calcDamage1;
                calcDamage2 = Damage2 != null ? Damage2.Value : calcDamage2;
                return true;
            } else if (card1.Name.ToLower().Contains(CardName2) && card2.Name.ToLower().Contains(CardName1)) {
                calcDamage1 = Damage2 != null ? Damage2.Value : calcDamage1;
                calcDamage2 = Damage1 != null ? Damage1.Value : calcDamage2;
                return true;
            }
            return false;
        }
    }
}
