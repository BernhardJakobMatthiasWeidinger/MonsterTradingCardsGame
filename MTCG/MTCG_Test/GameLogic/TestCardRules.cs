using MTCG.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Test.GameLogic {
    public class TestCardRules {
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
        [TestCase("Goblin", 10.0, "Dragon", 35.0, 0, 35.0)]
        [TestCase("Ork", 10.0, "Wizard", 35.0, 0, 35.0)]
        [TestCase("RegularSpell", 10.0, "Kraken", 35.0, 0, 35.0)]
        [TestCase("Dragon", 10.0, "FireElf", 35.0, 0, 35.0)]
        [TestCase("FireWizard", 10.0, "WaterOrk", 35.0, 10, 0)]
        [TestCase("WaterKraken", 10.0, "FireSpell", 35.0, 10, 0)]
        [TestCase("FireElf", 10.0, "WaterDragon", 35.0, 10, 0)]
        [TestCase("FireDragon", 10.0, "WaterGoblin", 35.0, 10, 0)]
        [TestCase("Knight", 10.0, "WaterSpell", 35.0, 0, 9999.0)]
        [TestCase("WaterSpell", 10.0, "FireKnight", 35.0, 9999.0, 0)]
        public void testCompareUniqueRule(string name1, double damage1, string name2, double damage2, double expected1, double expected2) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            //act
            Tuple<double, double> calcDamages = CardRules.compareUniqueRule(card1, card2, card1.damage, card2.damage);

            //assert
            Assert.AreEqual(expected1, calcDamages.Item1);
            Assert.AreEqual(expected2, calcDamages.Item2);
        }

        [Test]
        [TestCase("RegularSpell", 10, "RegularSpell", 10, 10, 10)]
        [TestCase("RegularSpell", 10, "WaterSpell", 10, 20, 5)]
        [TestCase("RegularSpell", 10, "FireSpell", 10, 5, 20)]
        [TestCase("WaterSpell", 10, "RegularSpell", 10, 5, 20)]
        [TestCase("WaterSpell", 10, "WaterSpell", 10, 10, 10)]
        [TestCase("WaterSpell", 10, "FireSpell", 10, 20, 5)]
        [TestCase("FireSpell", 10, "RegularSpell", 10, 20, 5)]
        [TestCase("FireSpell", 10, "WaterSpell", 10, 5, 20)]
        [TestCase("FireSpell", 10, "FireSpell", 10, 10, 10)]
        public void testCompareElementType(string name1, double damage1, string name2, double damage2, double expected1, double expected2) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            //act
            Tuple<double, double> calcDamages = CardRules.compareElementType(card1, card2, card1.damage, card2.damage);

            //assert
            Assert.AreEqual(expected1, calcDamages.Item1);
            Assert.AreEqual(expected2, calcDamages.Item2);
        }

        [Test]
        [TestCase("Maxi", "Mini", "RegularSpell", 10, "RegularSpell", 10, 10, 10)]
        [TestCase("Maxi", "Mini", "FireSpell", 10, "FireOrk", 10, 10, 10)]
        [TestCase("Maxi", "Mini", "WaterDragon", 10, "Wizard", 10, 10, 10)]
        public void testCompareAllRules_noCalculation(string user1, string user2, string name1, double damage1, string name2, double damage2, double expected1, double expected2) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            double before1 = card1.damage;
            double before2 = card2.damage;

            //act
            string damageLog = CardRules.compareAllRules(user1, user2, card1, card2, ref damage1, ref damage2);

            //assert
            Assert.AreEqual($"{user1}: {name1} ({before1} Damage) vs {user2}: {name2} ({before2} Damage)", damageLog);
        }

        [Test]
        [TestCase("Maxi", "Mini", "RegularSpell", 10, "FireSpell", 10, 5, 20)]
        [TestCase("Maxi", "Mini", "FireSpell", 10, "WaterOrk", 10, 5, 20)]
        [TestCase("Maxi", "Mini", "FireKraken", 10, "RegularSpell", 10, 10, 0)]
        [TestCase("Maxi", "Mini", "Knight", 10, "WaterSpell", 10, 0, 9999)]
        public void testCompareAllRules_withCalculation(string user1, string user2, string name1, double damage1, string name2, double damage2, double expected1, double expected2) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            double before1 = damage1;
            double before2 = damage2;

            //act
            string damageLog = CardRules.compareAllRules(user1, user2, card1, card2, ref damage1, ref damage2);

            //assert
            Assert.AreEqual($"{user1}: {name1} ({before1} Damage) vs {user2}: {name2} ({before2} Damage) => {before1} VS {before2} -> {expected1} VS {expected2}", damageLog);
            Assert.AreEqual(expected1, damage1);
            Assert.AreEqual(expected2, damage2);
        }
    }
}
