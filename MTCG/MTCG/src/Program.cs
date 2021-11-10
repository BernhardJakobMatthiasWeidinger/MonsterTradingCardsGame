using System;
using System.Collections.Generic;
using MTCG.src;

namespace MTCG {
    class Program {
        static void Main(string[] args) {
            //Initialize cards
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "Ork", 10.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 14.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "WaterSpell", 20.0);
            SpellCard s3 = new SpellCard(Guid.NewGuid(), "FireSpell", 30.0);

            //Initialize packages
            try {
                Package p0 = new Package(new List<Card> { m1, m2, s1, s2 });
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });

            //Initialize user
            User u1 = new User("maxiiii", "supersecretpassword1");
            p1.aquirePackage(u1);
            try {
                u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, s1.id, s2.id });
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
            try {
                u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, s3.id });
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }

            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, s1.id });

            //get 100 times a random card from the deck
            for (int i=0; i < 100; ++i) {
                Console.WriteLine(u1.getCardFromDeck().name);
            }
            Console.Read();
        }
    }
}
