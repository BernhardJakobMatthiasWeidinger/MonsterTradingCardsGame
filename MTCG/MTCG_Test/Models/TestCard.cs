using System;
using NUnit;
using NUnit.Framework;

using MTCG.Models;
using MTCG.Exceptions;

namespace MTCG.Test.Models {
    public class TestCard {
        [Test]
        [TestCase("RegularSpell")]
        [TestCase("FireSpell")]
        [TestCase("WaterSpell")]
        public void testConstructor_nameNoException(string name) {
            //arrange
            //act
            SpellCard s1 = new SpellCard(Guid.NewGuid(), name, 15.0);

            //assert
            Assert.AreEqual(name, s1.Name);
        }

        [Test]
        public void testConstructor_nameThrowsExceptionWrongPrefix() {
            //arrange
            //act & assert
            Assert.Throws<InvalidCardNameException>(delegate { new SpellCard(Guid.NewGuid(), "SomethingSpell", 12.0); });
        }

        [Test]
        public void testConstructor_nameThrowsExceptionWrongPostfix() {
            //arrange
            //act & assert
            Assert.Throws<InvalidCardNameException>(delegate { new SpellCard(Guid.NewGuid(), "FireSomething", 12.0); });
        }

        [Test]
        public void testConstructor_damage() {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 12.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "RegularSpell", 0.0);
            Assert.Throws<InconsistentNumberException>(delegate { new SpellCard(Guid.NewGuid(), "FireSpell", -12.0);  });

            //assert
            Assert.AreEqual(m1.Damage, 25.0);
            Assert.AreEqual(s1.Damage, 12.0);
            Assert.AreEqual(s2.Damage, 0.0);
        }

        [Test]
        [TestCase("RegularSpell", ElementType.normal)]
        [TestCase("FireSpell", ElementType.fire)]
        [TestCase("WaterSpell", ElementType.water)]
        public void testConstructor_elementTypeSpell(string name, ElementType elementType) {
            //arrange
            //act
            SpellCard m1 = new SpellCard(Guid.NewGuid(), name, 25.0);

            //assert
            Assert.AreEqual(elementType, m1.ElementType);
        }

        [Test]
        [TestCase("Ork", ElementType.normal)]
        [TestCase("FireOrk", ElementType.fire)]
        [TestCase("WaterOrk", ElementType.water)]
        public void testConstructor_elementTypeMonster(string name, ElementType elementType) {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), name, 25.0);

            //assert
            Assert.AreEqual(elementType, m1.ElementType);
        }

        [Test]
        [TestCase("FireTroll", MonsterType.troll)]
        [TestCase("WaterDragon", MonsterType.dragon)]
        [TestCase("FireElf", MonsterType.elf)]
        [TestCase("FireGoblin", MonsterType.goblin)]
        [TestCase("WaterKnight", MonsterType.knight)]
        [TestCase("Kraken", MonsterType.kraken)]
        [TestCase("Ork", MonsterType.ork)]
        [TestCase("FireWizzard", MonsterType.wizzard)]
        public void testConstructor_monsterType(string name, MonsterType monsterType) {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), name, 25.0);

            //assert
            Assert.AreEqual(monsterType, m1.MonsterType);
        }

        [Test]
        public void testConstructor_monsterTypeThrowsException() {
            //arrange
            //act & assert
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new MonsterCard(Guid.NewGuid(), "FireUnicorn", 10.0); });
            Assert.That(ex.Message, Is.EqualTo("Requested value 'unicorn' was not found."));
        }

        [Test]
        [TestCase("WaterDragon", 25.0)]
        [TestCase("Elf", 12.0)]
        public void testToString_monster(string name, double damage) {
            //arrange
            //act
            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), name, damage);

            //assert
            Assert.AreEqual($"CardId:{m1.Id, -35} Name:{name, -20} Damage:{damage, -3} ElementType:{m1.ElementType, -7} MonsterType:{m1.MonsterType,-10}", m1.ToString());
        }

        [Test]
        [TestCase("WaterSpell", 25.0)]
        [TestCase("FireSpell", 12.0)]
        public void testToString_spell(string name, double damage) {
            //arrange
            //act
            SpellCard s1 = new SpellCard(Guid.NewGuid(), name, damage);

            //assert
            Assert.AreEqual($"CardId:{s1.Id, -35} Name:{name, -20} Damage:{damage, -3} ElementType:{s1.ElementType, -7}", s1.ToString());
        }
    }
}