using MTCG.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Test.GameLogic {
    public class TestRule {
        private static List<ElementRule> elementRules = new List<ElementRule> {
            new ElementRule(ElementType.fire, ElementType.normal, 2, 0.5),
            new ElementRule(ElementType.normal, ElementType.water, 2, 0.5),
            new ElementRule(ElementType.water, ElementType.fire, 2, 0.5)
        };

        private static List<SpecialRule> specialRules = new List<SpecialRule> {
            new SpecialRule("goblin", "dragon", 0, null),
            new SpecialRule("wizard", "ork", null, 0),
            new SpecialRule("knight", "waterspell", 0, 9999),
            new SpecialRule("kraken", "spell", null, 0),
            new SpecialRule("fireelf", "dragon", null, 0),
        };

        private Card setUpCard(string name, double damage) {
            Card card;
            if (name.ToLower().Contains("spell")) {
                card = new SpellCard(Guid.NewGuid(), name, damage);
            } else {
                card = new MonsterCard(Guid.NewGuid(), name, damage);
            }
            return card;
        }

        [Test]
        [TestCase("Ork", "Elf", 0, 0)]
        [TestCase("Ork", "Elf", 0, null)]
        [TestCase("Ork", "Elf", 0, 10)]
        [TestCase("Ork", "Elf", null, 0)]
        [TestCase("Ork", "Elf", null, null)]
        [TestCase("Ork", "Elf", null, 10)]
        [TestCase("Ork", "Elf", 10, 0)]
        [TestCase("Ork", "Elf", 10, null)]
        [TestCase("Ork", "Elf", 10, 10)]
        public void testConstructor_SpecialRuleNoException(string name1, string name2, double? damage1, double? damage2) {
            //arrange
            //act
            SpecialRule sr = new SpecialRule(name1, name2, damage1, damage2);

            //assert
            Assert.AreEqual(name1.ToLower(), sr.cardName1);
            Assert.AreEqual(name2.ToLower(), sr.cardName2);
            Assert.AreEqual(damage1, sr.damage1);
            Assert.AreEqual(damage2, sr.damage2);
        }

        [Test]
        [TestCase("Ork", "Elf", 0, -10)]
        [TestCase("Ork", "Elf", -10, 0)]
        public void testConstructor_SpecialRuleThrowsExceptionNegativDamage(string name1, string name2, double? damage1, double? damage2) {
            //arrange
            //act & assert
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new SpecialRule(name1, name2, damage1, damage2); });
            Assert.That(ex.Message, Is.EqualTo("Damages of rule have to be positive."));
        }

        [Test]
        [TestCase(ElementType.fire, ElementType.normal, 0, 0)]
        [TestCase(ElementType.fire, ElementType.normal, 0, 2)]
        [TestCase(ElementType.fire, ElementType.normal, 2, 0)]
        [TestCase(ElementType.fire, ElementType.normal, 2, 2)]
        public void testConstructor_ElementRuleNoException(ElementType element1, ElementType element2, double multiplier1, double multiplier2) {
            //arrange
            //act
            ElementRule er = new ElementRule(element1, element2, multiplier1, multiplier2);

            //assert
            Assert.AreEqual(element1, er.element1);
            Assert.AreEqual(element2, er.element2);
            Assert.AreEqual(multiplier1, er.multiplier1);
            Assert.AreEqual(multiplier2, er.multiplier2);
        }

        [Test]
        [TestCase(ElementType.fire, ElementType.normal, 0, -2)]
        [TestCase(ElementType.fire, ElementType.normal, -1, 0)]
        public void testConstructor_ElementRuleThrowsExceptionNegativDamage(ElementType element1, ElementType element2, double multiplier1, double multiplier2) {
            //arrange
            //act & assert
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new ElementRule(element1, element2, multiplier1, multiplier2); });
            Assert.That(ex.Message, Is.EqualTo("Multipliers have to be positive."));
        }

        [Test]
        [TestCase("Goblin", 10.0, "Dragon", 35.0, 0, 35.0, 0)]
        [TestCase("Ork", 10.0, "Wizard", 35.0, 0, 35.0, 1)]
        [TestCase("RegularSpell", 10.0, "Kraken", 35.0, 0, 35.0, 3)]
        [TestCase("Dragon", 10.0, "FireElf", 35.0, 0, 35.0, 4)]
        [TestCase("FireWizard", 10.0, "WaterOrk", 35.0, 10, 0, 1)]
        [TestCase("WaterKraken", 10.0, "FireSpell", 35.0, 10, 0, 3)]
        [TestCase("FireElf", 10.0, "WaterDragon", 35.0, 10, 0, 4)]
        [TestCase("FireDragon", 10.0, "WaterGoblin", 35.0, 10, 0, 0)]
        [TestCase("Knight", 10.0, "WaterSpell", 35.0, 0, 9999.0, 2)]
        [TestCase("WaterSpell", 10.0, "FireKnight", 35.0, 9999.0, 0, 2)]
        public void testSpecialRule_checkRule(string name1, double damage1, string name2, double damage2, double expected1, double expected2, int idx) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            //act
            specialRules[idx].checkRule(card1, card2, ref damage1, ref damage2);

            //assert
            Assert.AreEqual(expected1, damage1);
            Assert.AreEqual(expected2, damage2);
        }

        [Test]
        [TestCase("RegularSpell", 10, "RegularSpell", 10, 10, 10, 0)]
        [TestCase("RegularSpell", 10, "WaterSpell", 10, 20, 5, 1)]
        [TestCase("RegularSpell", 10, "FireSpell", 10, 5, 20, 0)]
        [TestCase("WaterSpell", 10, "RegularSpell", 10, 5, 20, 1)]
        [TestCase("WaterSpell", 10, "WaterSpell", 10, 10, 10, 1)]
        [TestCase("WaterSpell", 10, "FireSpell", 10, 20, 5, 2)]
        [TestCase("FireSpell", 10, "RegularSpell", 10, 20, 5, 0)]
        [TestCase("FireSpell", 10, "WaterSpell", 10, 5, 20, 2)]
        [TestCase("FireSpell", 10, "FireSpell", 10, 10, 10, 2)]
        public void testElementRule_checkRule(string name1, double damage1, string name2, double damage2, double expected1, double expected2, int idx) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            //act
            elementRules[idx].checkRule(card1, card2, ref damage1, ref damage2);

            //assert
            Assert.AreEqual(expected1, damage1);
            Assert.AreEqual(expected2, damage2);
        }
    }
}
