using MTCG.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Test.GameLogic {
    public class TestBattle {
        [Test]
        public void testConstructor() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 999.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 999.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 999.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 999.0);

            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);

            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4});
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            u2.stack.AddRange(new List<Card> { m5, m6, m7 });
            u2.deck.AddRange(new List<Card> { m5, m6, m7 });

            //act
            Guid b1Id = Guid.NewGuid();
            Battle b1 = new Battle(b1Id, u1);
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Battle(Guid.NewGuid(), u2); });

            //assert
            Assert.AreEqual(b1.user1, u1);
            Assert.AreEqual(b1.user2, null);
            Assert.AreEqual(b1.id, b1Id);
            Assert.That(ex1.Message, Is.EqualTo("A deck should consist of 4 cards, cards in deck: 3"));
        }

        [Test]
        public void testPlay() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 999.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 999.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 999.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 999.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m8 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);

            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });
            u2.stack.AddRange(new List<Card> { m5, m6, m7, m8 });
            u2.configureDeck(new List<Guid> { m5.id, m6.id, m7.id, m8.id });

            //act
            Battle b1 = new Battle(Guid.NewGuid(), u1);
            b1.play(u2);

            //assert
            Assert.GreaterOrEqual(u1.stack.Count, 5);
            Assert.AreEqual(u1.deck.Count, 4);
            Assert.AreEqual(u1.gamesPlayed, 1);
            Assert.AreEqual(u1.gamesWon, 1);
            Assert.AreEqual(u1.gamesLost, 0);
            Assert.AreEqual(u1.elo, 103);

            Assert.LessOrEqual(u2.stack.Count, 3);
            Assert.AreEqual(u2.deck.Count, 0);
            Assert.AreEqual(u2.gamesPlayed, 1);
            Assert.AreEqual(u2.gamesWon, 0);
            Assert.AreEqual(u2.gamesLost, 1);
            Assert.AreEqual(u2.elo, 95);
        }
    }
}
