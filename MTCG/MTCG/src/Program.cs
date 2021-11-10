using System;
using System.Collections.Generic;
using MTCG.src;

namespace MTCG {
    class Program {
        static void Main(string[] args) {
            //Initialize cards
            MonsterCard m1 = new MonsterCard(new Guid(), "Ork", 10.0);
            MonsterCard m2 = new MonsterCard(new Guid(), "FireElf", 12.0);
            SpellCard s1 = new SpellCard(new Guid(), "RegularSpell", 14.0);
            SpellCard s2 = new SpellCard(new Guid(), "WaterSpell", 20.0);

            //Initialize user
            User u1 = new User("maxiiii", "supersecretpassword1");
            u1.deck = new List<Card> { m1, m2, s1, s2 };

            //get 100 times a random card from the deck
            for (int i=0; i < 100; ++i) {
                Console.WriteLine(u1.getCardFromDeck().name);
            }
            Console.Read();
        }
    }
}
