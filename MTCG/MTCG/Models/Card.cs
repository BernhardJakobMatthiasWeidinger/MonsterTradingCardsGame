using System;
using MTCG.Exceptions;

namespace MTCG.Models {
    public abstract class Card {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public double Damage { get; private set; }
        public ElementType ElementType { get; private set; }

        public Card(Guid id, string name, double damage) {
            this.Id = id;
            this.Name = name;
            this.Damage = damage;
            this.ElementType = ElementType.normal;
            
            if (damage < 0) {
                throw new InconsistentNumberException();
            }

            if (name.Length >= 7) {
                if (name.Substring(0, 4).ToLower() == "fire") {
                    this.ElementType = ElementType.fire;
                } else if (name.Substring(0, 5).ToLower() == "water") {
                    this.ElementType = ElementType.water;
                } else if (name.Substring(0, 7).ToLower() == "regular") {
                    this.ElementType = ElementType.normal;
                }
            }
        }

        public override string ToString() {
            return $"CardId:{this.Id, -35} Name:{this.Name, -20} Damage:{this.Damage, -3} ElementType:{this.ElementType, -7}";
        }
    }

    public class MonsterCard : Card {
        public MonsterType MonsterType { get; private set; }

        public MonsterCard(Guid id, string name, double damage) : base(id, name, damage) {
            int idx = 0;
            if (ElementType == ElementType.fire) {
                idx = 4;
            } else if (ElementType == ElementType.water) {
                idx = 5;
            }

            MonsterType = (MonsterType)System.Enum.Parse(typeof(MonsterType), name.Substring(idx).ToLower());
        }

        public override string ToString() {
            return base.ToString() + $" MonsterType:{this.MonsterType, -10}";
        }
    }

    public class SpellCard : Card {
        public SpellCard(Guid id, string name, double damage) : base(id, name, damage) {
            if (this.ElementType == ElementType.normal && name.Substring(0, 7).ToLower() != "regular" ||
                name.Substring(name.Length - 5).ToLower() != "spell") {
                throw new InvalidCardNameException();
            }
        }
    }
}