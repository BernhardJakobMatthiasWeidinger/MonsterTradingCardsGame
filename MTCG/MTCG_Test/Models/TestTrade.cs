using System;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;

using MTCG.Models;
using MTCG.Exceptions;

namespace MTCG.Test.Models {
    public class TestTrade {
        private User u1;
        private User u2;
        private MonsterCard m1;
        private MonsterCard m2;
        private MonsterCard m3;
        private MonsterCard m4;
        private MonsterCard m5;
        private MonsterCard m6;
        private MonsterCard m7;
        private MonsterCard m8;
        private MonsterCard m9;

        [OneTimeSetUp]
        public void Init() {
            m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            m3 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            m4 = new MonsterCard(Guid.NewGuid(), "WaterKnight", 25.0);
            m5 = new MonsterCard(Guid.NewGuid(), "Ork", 25.0);
            m6 = new MonsterCard(Guid.NewGuid(), "WaterOrk", 25.0);
            m7 = new MonsterCard(Guid.NewGuid(), "Kraken", 25.0);
            m8 = new MonsterCard(Guid.NewGuid(), "FireOrk", 25.0);
            m9 = new MonsterCard(Guid.NewGuid(), "WaterKraken", 10.0);

            u1 = new User(Guid.NewGuid(), "maxi", "maxiPW");
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.Deck.AddRange(new List<Card> { m1, m2, m3, m4 });

            u2 = new User(Guid.NewGuid(), "mini", "miniPW");
            u2.Stack.AddRange(new List<Card> { m6, m7, m8, m9 });
            u2.Deck.AddRange(new List<Card> { m6, m7, m8, m9 });
        }
        [Test]
        public void testConstructor() {
            //arrange
            //act
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, 20.0);

            //assert
            Assert.AreEqual(m5, t1.CardToTrade);
            Assert.AreEqual(u1, t1.Provider);
            Assert.AreEqual(CardType.monster, t1.CardType);
            Assert.AreEqual(20.0, t1.MinimumDamage);
        }

        [Test]
        public void testConstructor_throwsExceptionCardNotInStack() {
            //arrange
            //act & assert
            Assert.Throws<NotInDeckOrStackException>(delegate { new Trade(Guid.NewGuid(), m6, u1, CardType.monster, 20.0); });
        }

        [Test]
        public void testConstructor_throwsExceptionCardInDeck() {
            //arrange
            //act & assert
            Assert.Throws<InDeckException>(delegate { new Trade(Guid.NewGuid(), m1, u1, CardType.monster, 20.0); });
        }

        [Test]
        public void testTradeCard() {
            //arrange
            MonsterCard m_1 = new MonsterCard(Guid.NewGuid(), "WaterWizard", 30.0);
            MonsterCard m_2 = new MonsterCard(Guid.NewGuid(), "WaterWizard", 30.0);

            u1.Stack.Add(m_1);
            u2.Stack.Add(m_2);

            Trade t1 = new Trade(Guid.NewGuid(), m_1, u1, CardType.monster, 20.0);

            //act
            t1.TradeCard(u2, m_2);

            //assert
            Assert.Contains(m_2, u1.Stack);
            Assert.Contains(m_1, u2.Stack);
        }

        [Test]
        public void testTradeCard_throwsExceptionSameUser() {
            //arrange
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, 20.0);

            //act & assert
            Assert.Throws<InvalidOperationException>(delegate { t1.TradeCard(u1, m1); });
        }

        [Test]
        public void testTradeCard_throwsExceptionInDeck() {
            //arrange
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, 20.0);

            //act & assert
            Assert.Throws<InDeckException>(delegate { t1.TradeCard(u2, m6); });
        }

        [Test]
        public void testTradeCard_throwsExceptionNotInStack() {
            //arrange
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, 20.0);

            //act & assert
            Assert.Throws<NotInDeckOrStackException>(delegate { t1.TradeCard(u2, m1); });
        }

        [Test]
        public void testTradeCard_throwsExceptionWrongCardType() {
            //arrange
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, 20.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "FireSpell", 30.0);
            u2.Stack.Add(s1);

            //act & assert
            Assert.Throws<InvalidCardTypeException>(delegate { t1.TradeCard(u2, s1); });
        }

        [Test]
        public void testTradeCard_throwsExceptionTooSmallDamage() {
            //arrange
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, 20.0);
            MonsterCard m = new MonsterCard(Guid.NewGuid(), "WaterWizard", 10.0);
            u2.Stack.Add(m);

            //act & assert
            Assert.Throws<InconsistentNumberException>(delegate { t1.TradeCard(u2, m); });
        }
    }
}
