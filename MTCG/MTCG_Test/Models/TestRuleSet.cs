using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Test.Models {
    public class TestRuleSet {
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
        [TestCase("Maxi", "Mini", "RegularSpell", 10, "RegularSpell", 10, 10, 10)]
        [TestCase("Maxi", "Mini", "FireSpell", 10, "FireOrk", 10, 10, 10)]
        [TestCase("Maxi", "Mini", "WaterDragon", 10, "Wizzard", 10, 10, 10)]
        public void testCompareAllRules_noCalculation(string user1, string user2, string name1, double damage1, string name2, double damage2, double expected1, double expected2) {
            //arrange
            Card card1 = setUpCard(name1, damage1);
            Card card2 = setUpCard(name2, damage2);

            double before1 = card1.Damage;
            double before2 = card2.Damage;

            //act
            string damageLog = RuleSet.CompareAllRules(user1, user2, card1, card2, ref damage1, ref damage2);

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
            string damageLog = RuleSet.CompareAllRules(user1, user2, card1, card2, ref damage1, ref damage2);

            //assert
            Assert.AreEqual($"{user1}: {name1} ({before1} Damage) vs {user2}: {name2} ({before2} Damage) => {before1} VS {before2} -> {expected1} VS {expected2}", damageLog);
            Assert.AreEqual(expected1, damage1);
            Assert.AreEqual(expected2, damage2);
        }
    }
}
