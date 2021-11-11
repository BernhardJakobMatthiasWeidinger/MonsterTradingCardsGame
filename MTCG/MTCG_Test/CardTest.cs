using System;
using NUnit;
using NUnit.Framework;
using MTCG.src;

namespace MTCG_Test {
    public class CardTest {
        [Test]
        public void CheckSpellName() {
            //arrange
            //act
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 0.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "FireSpell", 12.0);
            SpellCard s3 = new SpellCard(Guid.NewGuid(), "WaterSpell", 15.0);
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new SpellCard(Guid.NewGuid(), "FireSomething", 12.0); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { new SpellCard(Guid.NewGuid(), "SomethingSpell", 12.0); });

            //assert
            Assert.AreEqual(s1.name, "RegularSpell");
            Assert.AreEqual(s2.name, "FireSpell");
            Assert.AreEqual(s3.name, "WaterSpell");
            Assert.That(ex1.Message, Is.EqualTo("Spell card has to end with 'Spell'."));
            Assert.That(ex2.Message, Is.EqualTo("Spell card has to begin with 'Regular', 'Fire' or 'Water'."));
        }

        [Test]
        public void CheckDamage() {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 12.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "RegularSpell", 0.0);
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new SpellCard(Guid.NewGuid(), "FireSpell", -12.0);  });

            //assert
            Assert.AreEqual(m1.damage, 25.0);
            Assert.AreEqual(s1.damage, 12.0);
            Assert.AreEqual(s2.damage, 0.0);
            Assert.That(ex.Message, Is.EqualTo("Damage has to be positive."));
        }

        [Test]
        public void CheckElementType() {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterKnight", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "Kraken", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "Ork", 10.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireWizard", 25.0);

            //assert
            Assert.AreEqual(m1.elementType, ElementType.water);
            Assert.AreEqual(m2.elementType, ElementType.fire);
            Assert.AreEqual(m3.elementType, ElementType.fire);
            Assert.AreEqual(m4.elementType, ElementType.water);
            Assert.AreEqual(m5.elementType, ElementType.normal);
            Assert.AreEqual(m6.elementType, ElementType.normal);
            Assert.AreEqual(m7.elementType, ElementType.fire);
        }

        [Test]
        public void CheckMonsterType() {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterKnight", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "Kraken", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "Ork", 10.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireWizard", 25.0);
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new MonsterCard(Guid.NewGuid(), "FireUnicorn", 10.0); });

            //assert
            Assert.AreEqual(m1.monsterType, MonsterType.dragon);
            Assert.AreEqual(m2.monsterType, MonsterType.elf);
            Assert.AreEqual(m3.monsterType, MonsterType.goblin);
            Assert.AreEqual(m4.monsterType, MonsterType.knight);
            Assert.AreEqual(m5.monsterType, MonsterType.kraken);
            Assert.AreEqual(m6.monsterType, MonsterType.ork);
            Assert.AreEqual(m7.monsterType, MonsterType.wizard);
            Assert.That(ex.Message, Is.EqualTo("Requested value 'unicorn' was not found."));
        }
    }
}