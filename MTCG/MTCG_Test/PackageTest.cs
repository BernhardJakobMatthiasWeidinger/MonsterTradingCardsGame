using System;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;

using MTCG.src;

namespace MTCG_Test {
    public class PackageTest {
        [Test]
        public void CheckConstructor() {
            //arrange
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "Goblin", 25.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 12.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "RegularSpell", 23.0);
            SpellCard s3 = new SpellCard(Guid.NewGuid(), "RegularSpell", 3.0);
            SpellCard s4 = new SpellCard(Guid.NewGuid(), "FireSpell", 3.0);
            SpellCard s5 = new SpellCard(Guid.NewGuid(), "WaterSpell", 3.0);
            SpellCard s6 = new SpellCard(Guid.NewGuid(), "WaterSpell", 3.0);
            SpellCard s7 = new SpellCard(Guid.NewGuid(), "WaterSpell", 3.0);
            SpellCard s8 = new SpellCard(Guid.NewGuid(), "WaterSpell", 3.0);
            SpellCard s9 = new SpellCard(Guid.NewGuid(), "WaterSpell", 3.0);

            //act
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Package(new List<Card> { m4, m5, m6, s3, s4, s5 }); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { new Package(new List<Card> { s6, s7, s8, s9 }); });

            //assert
            Assert.AreEqual(p1.cards.Count, 5);
            Assert.That(ex1.Message, Is.EqualTo("A package should be provided with 5 cards, cards given: 6"));
            Assert.That(ex2.Message, Is.EqualTo("A package should be provided with 5 cards, cards given: 4"));
        }

        [Test]
        public void checkAcquirePackage() {
            //arrange
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);

            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m8 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m9 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m10 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);

            User u1 = new User("maxi", "musterpassword1");
            User u2 = new User("mini", "musterpassword1");
            u2.coins = 3;

            Package p1 = new Package(new List<Card> { m1, m2, m3, m4, m5 });
            Package p2 = new Package(new List<Card> { m6, m7, m8, m9, m10 });

            //act
            p1.aquirePackage(u1);
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { p2.aquirePackage(u2); });

            //assert
            Assert.AreEqual(u1.stack.Count, 5);
            Assert.That(ex.Message, Is.EqualTo($"User mini has an insufficent amount of coins (3), coins needed: 5"));
        }
    }
}
