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

            if (name.Length >= 5) {
                if (name.Substring(0, 4).ToLower() == "fire") {
                    this.elementType = ElementType.fire;
                } else if (name.Substring(0, 5).ToLower() == "water") {
                    this.elementType = ElementType.water;
                } 
            }
        }
    }

    public class MonsterCard : Card {
        public MonsterType monsterType { get; private set; }

        public MonsterCard(Guid id, string name, double damage) : base(id, name, damage) {
            string monsterName = name;
            if (elementType == ElementType.fire) {
                monsterName = name.Substring(4);
            } else if (elementType == ElementType.water) {
                monsterName = name.Substring(5);
            }
            monsterType = (MonsterType)System.Enum.Parse(typeof(MonsterType), monsterName.ToLower());
        }
    }

    public class SpellCard : Card {
        public SpellCard(Guid id, string name, double damage) : base(id, name, damage) { }
    }
}