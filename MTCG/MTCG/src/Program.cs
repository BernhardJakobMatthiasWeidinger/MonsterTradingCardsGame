using System;
using MTCG.src;

namespace MTCG {
    class Program {
        static void Main(string[] args) {
            MonsterCard m1 = new MonsterCard(new Guid(), "Goblin", 10.0);
            SpellCard s1 = new SpellCard(new Guid(), "RegularSpell", 10.0);
            SpellCard s2 = new SpellCard(new Guid(), "WaterSpell", 10.0);

            Console.WriteLine(m1.elementType + " " + m1.monsterType);
            Console.WriteLine(s1.elementType);
            Console.WriteLine(s2.elementType);
            Console.Read();
        }
    }
}
