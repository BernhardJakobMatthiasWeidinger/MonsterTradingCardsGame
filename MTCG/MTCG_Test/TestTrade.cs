using System;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;

using MTCG.src;

namespace MTCG.Test {
    public class TestTrade {
        [Test]
        public void testConstructor() {
            //arrange
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterKnight", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "Ork", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterOrk", 25.0);

            User u1 = new User("maxi", "maxiPW");
            u1.stack.AddRange(new List<Card>{ m1, m2, m3, m4, m5});
            u1.deck.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act
            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, ElementType.water, 20.0);
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new Trade(Guid.NewGuid(), m1, u1, CardType.monster, ElementType.water, 20.0); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { new Trade(Guid.NewGuid(), m6, u1, CardType.monster, ElementType.water, 20.0); });

            //assert
            Assert.AreEqual(t1.cardToTrade, m5);
            Assert.AreEqual(t1.user, u1);
            Assert.AreEqual(t1.cardType, CardType.monster);
            Assert.AreEqual(t1.elementType, ElementType.water);
            Assert.AreEqual(t1.minimumDamage, 20.0);
            Assert.That(ex1.Message, Is.EqualTo("Cannot trade card if it's in the deck."));
            Assert.That(ex2.Message, Is.EqualTo("Cannot trade card if it's not in the stack."));
        }

        [Test]
        public void testTradeCard() {
            //arrange
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterKnight", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "Ork", 25.0);

            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterOrk", 25.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "Kraken", 25.0);
            MonsterCard m8 = new MonsterCard(Guid.NewGuid(), "FireOrk", 25.0);
            MonsterCard m9 = new MonsterCard(Guid.NewGuid(), "WaterKraken", 10.0);

            SpellCard m10 = new SpellCard(Guid.NewGuid(), "FireSpell", 30.0);
            MonsterCard m11 = new MonsterCard(Guid.NewGuid(), "FireWizard", 30.0);
            MonsterCard m12 = new MonsterCard(Guid.NewGuid(), "WaterWizard", 10.0);
            MonsterCard m13 = new MonsterCard(Guid.NewGuid(), "WaterWizard", 30.0);

            User u1 = new User("maxi", "maxiPW");
            User u2 = new User("mini", "miniPW");

            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.deck.AddRange(new List<Card> { m1, m2, m3, m4 });
            u2.stack.AddRange(new List<Card> { m6, m7, m8, m9, m10, m11, m12, m13 });
            u2.deck.AddRange(new List<Card> { m6, m7, m8, m9 });

            Trade t1 = new Trade(Guid.NewGuid(), m5, u1, CardType.monster, ElementType.water, 20.0);

            //act
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { t1.TradeCard(u1, m1); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { t1.TradeCard(u2, m6); });
            ArgumentException ex3 = Assert.Throws<ArgumentException>(delegate { t1.TradeCard(u2, m1); });
            ArgumentException ex4 = Assert.Throws<ArgumentException>(delegate { t1.TradeCard(u2, m10); });
            ArgumentException ex5 = Assert.Throws<ArgumentException>(delegate { t1.TradeCard(u2, m11); });
            ArgumentException ex6 = Assert.Throws<ArgumentException>(delegate { t1.TradeCard(u2, m12); });
            t1.TradeCard(u2, m13);

            //assert
            Assert.AreEqual(t1.cardToTrade, m5);
            Assert.AreEqual(t1.user, u1);
            Assert.AreEqual(t1.cardType, CardType.monster);
            Assert.AreEqual(t1.elementType, ElementType.water);
            Assert.AreEqual(t1.minimumDamage, 20.0);

            Assert.That(ex1.Message, Is.EqualTo("Cannot trade card with yourself."));
            Assert.That(ex2.Message, Is.EqualTo("Cannot trade card if it's in the deck."));
            Assert.That(ex3.Message, Is.EqualTo("Cannot trade card if it's not in the stack."));
            Assert.That(ex4.Message, Is.EqualTo("Cannot trade card, wrong card type was provided."));
            Assert.That(ex5.Message, Is.EqualTo("Cannot trade card, wrong element type was provided."));
            Assert.That(ex6.Message, Is.EqualTo("Cannot trade card, damage of card is too small."));

            Assert.Contains(m13, u1.stack);
            Assert.Contains(m5, u2.stack);
        }
    }
}
