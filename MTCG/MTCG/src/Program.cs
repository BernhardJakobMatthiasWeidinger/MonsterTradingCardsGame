using System;
using System.Collections.Generic;
using MTCG.src;

namespace MTCG {
    class Program {
        static void Main(string[] args) {
            //Create cards
            Console.WriteLine("Create Cards:");
            try {
                MonsterCard m0 = new MonsterCard(Guid.NewGuid(), "Unicorn", 10.0);
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "Ork", 10.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 34.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireKraken", 20.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 14.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "WaterSpell", 20.0);
            SpellCard s3 = new SpellCard(Guid.NewGuid(), "FireSpell", 30.0);
            SpellCard s4 = new SpellCard(Guid.NewGuid(), "WaterSpell", 25.0);
            SpellCard s5 = new SpellCard(Guid.NewGuid(), "RegularSpell", 40.0);

            //Create packages
            Console.WriteLine("\nCreate Packages:");
            try {
                Package p0 = new Package(new List<Card> { m1, m2, s1, s2 });
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });
            Package p2 = new Package(new List<Card> { m4, m5, s3, s4, s5 });

            //Create user1
            Console.WriteLine("\nCreate User 1 and add cards to stack and deck:");
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

            //Create user2
            User u2 = new User("miniiii", "supersecretpassword2");
            p2.aquirePackage(u2);
            u2.configureDeck(new List<Guid> { m4.id, m5.id, s3.id, s4.id });

            //Create Trade
            Console.WriteLine("\nCreate Trade:");
            try {
                Trade tfail1 = new Trade(Guid.NewGuid(), s4, u1, CardType.spell, ElementType.fire, 15.0);
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
            try {
                Trade tfail2 = new Trade(Guid.NewGuid(), s1, u1, CardType.spell, ElementType.fire, 15.0);
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }

            Trade t1 = new Trade(Guid.NewGuid(), s2, u1, CardType.spell, ElementType.normal, 15.0);

            //Trade with user2
            Console.WriteLine("\nTrade with user2:");
            try {
                t1.TradeCard(u1, s5);
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
            try {
                t1.TradeCard(u2, s4);
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Before Trade:");
            Console.WriteLine("User1 has card s5 in stack: " + u1.stack.Contains(s5));
            Console.WriteLine("User2 has card s2 in stack: " + u2.stack.Contains(s2));
            t1.TradeCard(u2, s5);
            Console.WriteLine("After Trade:");
            Console.WriteLine("User1 has card s5 in stack: " + u1.stack.Contains(s5));
            Console.WriteLine("User2 has card s2 in stack: " + u2.stack.Contains(s2));

            Console.WriteLine("\nBattle:");
            Battle b1 = new Battle(Guid.NewGuid(), u1);
            Console.WriteLine(b1.play(u2));

            Console.WriteLine(u1.deck.Count);
            Console.WriteLine(u2.deck.Count);

            Console.WriteLine(u1.stackToString(true));
            Console.WriteLine(u1.deckToString(true));
            Console.WriteLine(u1.getUserStats(true));
            Console.WriteLine(u1.getUserData(true));
            Console.WriteLine();
            Console.WriteLine(u1.stackToString(false));
            Console.WriteLine(u1.deckToString(false));
            Console.WriteLine(u1.getUserStats(false));
            Console.WriteLine(u1.getUserData(false));
            Console.Read();
        }
    }
}
