using System;
using NUnit;
using NUnit.Framework;
using MTCG.src;
using System.Collections.Generic;

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

            //act
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Package(new List<Card> { m4, m5, m6, s3, s4, s5 }); });

            //assert
            Assert.AreEqual(p1.cards.Count, 5);
            Assert.That(ex1.Message, Is.EqualTo("A package should be provided with 5 cards, cards given: 6"));
        }

        [Test]
        public void checkAcquirePackage() {
            //arrange
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);

            User u1 = new User("maxi", "musterpassword1");
            Package p1 = new Package(new List<Card> { m1, m2, m3, m4, m5 });

            //act
            p1.aquirePackage(u1);

            //assert
            Assert.AreEqual(u1.stack.Count, 5);

        }
    }
}
