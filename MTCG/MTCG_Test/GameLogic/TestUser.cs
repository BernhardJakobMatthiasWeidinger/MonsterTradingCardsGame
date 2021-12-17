using MTCG.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTCG.Test.GameLogic {
    public class TestUser {
        private User u1;
        private User u2;
        private MonsterCard m1;
        private MonsterCard m2;
        private MonsterCard m3;
        private MonsterCard m4;
        private MonsterCard m5;
        private MonsterCard m6;

        [SetUp]
        public void Init() {
            u1 = new User("maxi", "testUserPassword");
            u2 = new User("mini", "testUserPassword");

            m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 99.0);
            m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 99.0);
            m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 99.0);
            m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 99.0);
            m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            m6 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
        }

        [Test]
        public void testConstructor() {
            //arrange
            //act
            //assert
            Assert.AreEqual("maxi", u1.username);
            Assert.AreEqual("testUserPassword", u1.password);
            Assert.AreEqual(null, u1.name);
            Assert.AreEqual("Hier könnte deine Biografie stehen!", u1.bio);
            Assert.AreEqual(20, u1.coins);
            Assert.AreEqual(0, u1.gamesPlayed);
            Assert.AreEqual(0, u1.gamesWon);
            Assert.AreEqual(0, u1.gamesLost);
            Assert.AreEqual(100, u1.elo);
        }

        [Test]
        [TestCase("test;User")]
        [TestCase("test/User")]
        [TestCase("test\\User")]
        [TestCase("test\'User")]
        [TestCase("test\"User")]
        public void testConstructor_throwExceptionInvalidUsername(string username) {
            //arrange
            //act & assert
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new User(username, "testUserPassword"); });
            Assert.That(ex.Message, Is.EqualTo("Username is not allowed to contain following characters: ; / \\ \' \""));
        }

        [Test]
        [TestCase("test;User/Password")]
        [TestCase("test/User\\Password")]
        [TestCase("test\\User\'Password")]
        [TestCase("test\'User\"Password")]
        [TestCase("test\"User;Password")]
        public void testConstructor_throwExceptionInvalidPassword(string password) {
            //arrange
            //act
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate { new User("testUser", password); });

            //assert
            Assert.That(ex.Message, Is.EqualTo("Password is not allowed to contain following characters: ; / \\ \' \""));
        }

        [Test]
        [TestCase("testUser", "Max Mustermann", "20yo, Austria, Gemini xxxxDDDD", ":O")]
        [TestCase("mininii", "Mini Mustermann", "Ich war schon immer sehr interessiert an Monster Trading Games.", ":3")]
        public void testToString(string username, string name, string bio, string image) {
            //arrange
            u1 = new User(username, "testUserPassword");
            u1.setUserData(name, bio, image);

            //act
            string userData = u1.ToString();

            //assert
            Assert.AreEqual($"id:{u1.id},username:{u1.username},name:{u1.name},bio:{u1.bio},image:{u1.image}," +
                    $"coins:20,gamesPlayed:0,gamesWon:0,gamesLost:0,elo:100,friends:[]", u1.ToString());
        }

        [Test]
        [TestCase("Max Mustermann", "20yo, Austria, Gemini xxxxDDDD", ":O")]
        [TestCase("Mini Mustermann", "Ich war schon immer sehr interessiert an Monster Trading Games.", ":3")]
        public void testSetUserData(string name, string bio, string image) {
            //arrange
            u1 = new User("testUser", "testUserPassword");

            //act
            u1.setUserData(name, bio, image);

            //assert
            Assert.AreEqual(name, u1.name);
            Assert.AreEqual(bio, u1.bio);
            Assert.AreEqual(image, u1.image);
        }

        [Test]
        public void testGetCardFromDeck() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            //act
            Card card1 = u1.getCardFromDeck();
            Card card2 = u2.getCardFromDeck();

            //assert
            Assert.AreEqual(true, u1.deck.Contains(card1));
            Assert.AreEqual(null, card2);
        }

        [Test]
        public void testConfigureDeck() {
            //arrange
            int before = u1.deck.Count;
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            //assert
            Assert.AreEqual(4, u1.deck.Count);
            Assert.AreEqual(0, before);
            Assert.AreEqual(true, u1.deck.Contains(m1));
            Assert.AreEqual(false, u1.deck.Contains(m5));
        }

        [Test]
        public void testConfigureDeck_throwsExceptionTooManyCards() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id, m5.id }); });
            Assert.That(ex1.Message, Is.EqualTo($"A deck should be provided with 4 cards, cards given: 5"));
        }

        [Test]
        public void testConfigureDeck_throwsExceptionTooFewCards() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id }); });
            Assert.That(ex2.Message, Is.EqualTo($"A deck should be provided with 4 cards, cards given: 3"));
        }

        [Test]
        public void testConfigureDeck_throwsExceptionInvalidCard() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            ArgumentException ex3 = Assert.Throws<ArgumentException>(delegate { u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m5.id }); });
            Assert.That(ex3.Message, Is.EqualTo($"Card with id {m5.id} was not found in stack!"));
        }

        [Test]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void testConfigureDeckAfterBattle(int countCards) {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.deck.AddRange(u1.stack.GetRange(0, countCards));

            //act
            u1.configureDeckAfterBattle();

            //assert
            Assert.AreEqual(5, u1.stack.Count);
            Assert.AreEqual(4, u1.deck.Count);
            Assert.AreEqual(new List<Card> { m1, m2, m3, m4 }, u1.deck);
        }

        [Test]
        public void testAddFriend() {
            //arrange
            User u3 = new User("otto", "supersecretpassword1");

            //act
            u1.addFriend(u2);
            u2.addFriend(u3);

            //assert
            Assert.AreEqual(1, u1.friends.Count);
            Assert.AreEqual(2, u2.friends.Count);
            Assert.AreEqual(1, u3.friends.Count);

            Assert.AreEqual(new List<Guid> { u2.id }, u1.friends);
            Assert.AreEqual(new List<Guid> { u1.id, u3.id }, u2.friends);
            Assert.AreEqual(new List<Guid> { u2.id }, u3.friends);
        }

        [Test]
        public void testAddFriend_throwsExceptionAlreadyBefriended() {
            //arrange
            //act
            u1.addFriend(u2);

            //assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u2.addFriend(u1); });
            Assert.That(ex1.Message, Is.EqualTo($"User maxi is already your friend!"));
        }

        [Test]
        public void testRemoveFriend() {
            //arrange
            User u3 = new User("otto", "supersecretpassword1");
            u1.addFriend(u2);
            u2.addFriend(u3);

            //act
            u2.removeFriend(u1);

            //assert
            Assert.AreEqual(new List<Guid> { }, u1.friends);
            Assert.AreEqual(new List<Guid> { u3.id }, u2.friends);
            Assert.AreEqual(new List<Guid> { u2.id }, u3.friends);
        }

        [Test]
        public void testRemoveFriend_throwsExceptionNotBefriended() {
            //arrange
            User u3 = new User("otto", "supersecretpassword1");

            //act & assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u1.removeFriend(u3); });
            Assert.That(ex1.Message, Is.EqualTo($"User otto is not your friend!"));
        }

        [Test]
        public void testGetUserData_plain() {
            //arrange
            //act
            string dataPlain = u1.getUserData(false);

            //assert
            Assert.AreEqual(dataPlain, u1.ToString());
        }

        [Test]
        [TestCase("maxi", "username")]
        [TestCase("", "name")]
        [TestCase("Hier könnte deine Biografie stehen!", "bio")]
        [TestCase("", "image")]
        [TestCase("20", "coins")]
        [TestCase("0", "gamesPlayed")]
        [TestCase("0", "gamesWon")]
        [TestCase("0", "gamesLost")]
        [TestCase("100", "elo")]
        public void testGetUserData_json(string value, string key) {
            //arrange
            //act
            string dataJson = u1.getUserData(true);
            JObject newU1 = (JObject)JsonConvert.DeserializeObject(dataJson);

            //assert
            Assert.AreEqual(value, newU1.GetValue(key).ToString());
        }

        [Test]
        public void testGetUserStats_plain() {
            //arrange
            //act
            string dataPlain = u1.getUserStats(false);

            //assert
            Assert.AreEqual($"gamesPlayed:{u1.gamesPlayed},gamesWon:{u1.gamesWon},gamesLost:{u1.gamesWon},elo:{u1.elo}", dataPlain);
        }

        [Test]
        [TestCase(null, "username")]
        [TestCase("0", "gamesPlayed")]
        [TestCase("0", "gamesWon")]
        [TestCase("0", "gamesLost")]
        [TestCase("100", "elo")]
        public void testGetUserStats_json(string value, string key) {
            //arrange
            //act
            string dataJson = u1.getUserStats(true);
            JObject newU1 = (JObject)JsonConvert.DeserializeObject(dataJson);

            //assert
            Assert.AreEqual(value, newU1.GetValue(key)?.ToString());
        }

        [Test]
        public void testDeckToString_plain() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            //act
            string dataPlain = u1.deckToString(false);

            int i = 0;
            string dataPlainRes = "";
            foreach (Card card in u1.deck) {
                dataPlainRes += card.ToString();
                dataPlainRes += i != (u1.deck.Count - 1) ? ";" : "";
                i++;
            }
            
            //assert
            Assert.AreEqual(dataPlain, dataPlainRes);
        }

        [Test]
        public void testDeckToString_json() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            //act
            string dataJson = u1.deckToString(true);

            JArray u1Deck = (JArray)((JObject)JsonConvert.DeserializeObject(dataJson)).GetValue("deck");
            JObject m1Json = (JObject)JsonConvert.DeserializeObject(u1Deck[0].ToString());

            //assert
            Assert.AreEqual(m1.id.ToString(), m1Json.GetValue("id").ToString());
            Assert.AreEqual(m1.name, m1Json.GetValue("name").ToString());
            Assert.AreEqual(m1.damage.ToString(), m1Json.GetValue("damage").ToString());
        }

        [Test]
        public void testStackToString_plain() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2 });

            //act
            string dataPlain = u1.stackToString(false);

            int i = 0;
            string dataPlainRes = "";
            foreach (Card card in u1.stack) {
                dataPlainRes += card.ToString();
                dataPlainRes += i != (u1.stack.Count - 1) ? ";" : "";
                i++;
            }

            //assert
            Assert.AreEqual(dataPlain, dataPlainRes);
        }

        [Test]
        public void testStackToString_json() {
            //arrange
            u1.stack.AddRange(new List<Card> { m1, m2 });

            //act
            string dataJson = u1.stackToString(true);

            JArray u1Stack = (JArray)((JObject)JsonConvert.DeserializeObject(dataJson)).GetValue("stack");
            JObject m1Json = (JObject)JsonConvert.DeserializeObject(u1Stack[0].ToString());

            //assert
            Assert.AreEqual(m1.id.ToString(), m1Json.GetValue("id").ToString());
            Assert.AreEqual(m1.name, m1Json.GetValue("name").ToString());
            Assert.AreEqual(m1.damage.ToString(), m1Json.GetValue("damage").ToString());
        }
    }
}