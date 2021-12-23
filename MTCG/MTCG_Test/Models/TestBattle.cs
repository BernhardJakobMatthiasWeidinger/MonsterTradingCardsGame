using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Test.Models {
    public class TestBattle {
        private User u1;
        private MonsterCard m1;
        private MonsterCard m2;
        private MonsterCard m3;
        private MonsterCard m4;

        [SetUp]
        public void Init() {
            u1 = new User("testUser", "testUserPassword");
            m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 999.0);
            m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 999.0);
            m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 999.0);
            m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 999.0);
        }

        [Test]
        public void testConstructor() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4});
            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id });

            //act
            Battle b1 = new Battle(Guid.NewGuid(), u1);

            //assert
            Assert.AreEqual(u1, b1.User1);
            Assert.AreEqual(null, b1.User2);
        }

        [Test]
        public void testConstructor_throwsException() {
            //arrange
            //act & assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Battle(Guid.NewGuid(), u1); });
            Assert.That(ex1.Message, Is.EqualTo("A deck should consist of 4 cards, cards in deck: 0"));
        }

        [Test]
        public void testPlay() {
            //arrange
            User u2 = new User("maxi", "supersecretpassword1");
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m8 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);

            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });
            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id });
            u2.Stack.AddRange(new List<Card> { m5, m6, m7, m8 });
            u2.ConfigureDeck(new List<Guid> { m5.Id, m6.Id, m7.Id, m8.Id });

            //act
            Battle b1 = new Battle(Guid.NewGuid(), u1);
            b1.Play(u2);

            //assert
            Assert.GreaterOrEqual(u1.Stack.Count, 5);
            Assert.AreEqual(4, u1.Deck.Count);
            Assert.AreEqual(1, u1.GamesPlayed);
            Assert.AreEqual(1, u1.GamesWon);
            Assert.AreEqual(0, u1.GamesLost);
            Assert.AreEqual(103, u1.Elo);

            Assert.LessOrEqual(u2.Stack.Count, 3);
            Assert.AreEqual(0, u2.Deck.Count);
            Assert.AreEqual(1, u2.GamesPlayed);
            Assert.AreEqual(0, u2.GamesWon);
            Assert.AreEqual(1, u2.GamesLost);
            Assert.AreEqual(95, u2.Elo);
        }
    }
}
