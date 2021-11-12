using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.src {
    public abstract class Card {
        public Guid id { get; private set; }
        public string name { get; private set; }
        public double damage { get; private set; }
        public ElementType elementType { get; private set; }

        public Card(Guid id, string name, double damage) {
            this.id = id;
            this.name = name;
            this.damage = damage;
            this.elementType = ElementType.normal;
            
            if (damage < 0) {
                throw new ArgumentException("Damage has to be positive.");
            }

            if (name.Length >= 7) {
                if (name.Substring(0, 4).ToLower() == "fire") {
                    this.elementType = ElementType.fire;
                } else if (name.Substring(0, 5).ToLower() == "water") {
                    this.elementType = ElementType.water;
                } else if (name.Substring(0, 7).ToLower() == "regular") {
                    this.elementType = ElementType.normal;
                }
            }
        }

        public override string ToString() {
            return $"id:{this.id},name:{this.name},damage:{this.damage},elementType:{this.elementType}";
        }
    }

    public class MonsterCard : Card {
        public MonsterType monsterType { get; private set; }

        public MonsterCard(Guid id, string name, double damage) : base(id, name, damage) {
            int idx = 0;
            if (elementType == ElementType.fire) {
                idx = 4;
            } else if (elementType == ElementType.water) {
                idx = 5;
            }

            monsterType = (MonsterType)System.Enum.Parse(typeof(MonsterType), name.Substring(idx).ToLower());
        }

        public override string ToString() {
            return base.ToString() + $",monsterType:{this.monsterType}";
        }
    }

    public class SpellCard : Card {
        public SpellCard(Guid id, string name, double damage) : base(id, name, damage) {
            if (this.elementType == ElementType.normal && name.Substring(0, 7).ToLower() != "regular") {
                throw new ArgumentException("Spell card has to begin with 'Regular', 'Fire' or 'Water'.");
            }

            if (name.Substring(name.Length-5).ToLower() != "spell") {
                throw new ArgumentException("Spell card has to end with 'Spell'.");
            }
        }
    }
}