﻿using System;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;

using MTCG.GameLogic;

namespace MTCG.Test.GameLogic {
    public class TestPackage {
        private MonsterCard m1;
        private MonsterCard m2;
        private MonsterCard m3;
        private SpellCard s1;
        private SpellCard s2;
        private SpellCard s3;

        [SetUp]
        public void Init() {
            m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);

            s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 12.0);
            s2 = new SpellCard(Guid.NewGuid(), "RegularSpell", 23.0);
            s3 = new SpellCard(Guid.NewGuid(), "RegularSpell", 3.0);
        }

        [Test]
        public void testConstructor_throwsNoException() {
            //arrange
            //act
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });

            //assert
            Assert.AreEqual(5, p1.Cards.Count);
        }

        [Test]
        public void testConstructor_throwsExceptionTooFewCards() {
            //arrange
            //act & assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Package(new List<Card> { m1, m2, m3, s1 }); });
            Assert.That(ex1.Message, Is.EqualTo("A package should be provided with 5 cards, cards given: 4"));
        }

        [Test]
        public void testConstructor_throwsExceptionTooManyCards() {
            //arrange
            //act & assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Package(new List<Card> { m1, m2, m3, s1, s2, s3 }); });
            Assert.That(ex1.Message, Is.EqualTo("A package should be provided with 5 cards, cards given: 6"));
        }


        [Test]
        public void testAcquirePackage() {
            //arrange
            User u1 = new User("maxi", "musterpassword1");
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });

            //act
            p1.AquirePackage(u1);

            //assert
            Assert.AreEqual(5, u1.Stack.Count);
            Assert.AreEqual(15, u1.Coins);
        }

        [Test]
        public void testAcquirePackage_throwsException() {
            //arrange
            User u1 = new User("maxi", "musterpassword1");
            u1.Coins = 3;

            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });

            //act & assert
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { p1.AquirePackage(u1); });
            Assert.That(ex.Message, Is.EqualTo($"User maxi has an insufficent amount of coins (3), coins needed: 5"));
        }
    }
}
