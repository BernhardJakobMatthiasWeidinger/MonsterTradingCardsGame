using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MTCG.Exceptions;

namespace MTCG.Test.Models {
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
            u1 = new User(Guid.NewGuid(), "maxi", "testUserPassword");
            u2 = new User(Guid.NewGuid(), "mini", "testUserPassword");

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
            Assert.AreEqual("maxi", u1.Username);
            Assert.AreEqual("testUserPassword", u1.Password);
            Assert.AreEqual(null, u1.Name);
            Assert.AreEqual("Your bio could stand here!", u1.Bio);
            Assert.AreEqual(20, u1.Coins);
            Assert.AreEqual(0, u1.GamesPlayed);
            Assert.AreEqual(0, u1.GamesWon);
            Assert.AreEqual(0, u1.GamesLost);
            Assert.AreEqual(100, u1.Elo);
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
            Assert.Throws<ArgumentException>(delegate { new User(Guid.NewGuid(), username, "testUserPassword"); });
        }

        [Test]
        [TestCase("test;User/Password")]
        [TestCase("test/User\\Password")]
        [TestCase("test\\User\'Password")]
        [TestCase("test\'User\"Password")]
        [TestCase("test\"User;Password")]
        public void testConstructor_throwExceptionInvalidPassword(string password) {
            //arrange
            //act & assert
            Assert.Throws<ArgumentException>(delegate { new User(Guid.NewGuid(), "testUser", password); });
        }

        [Test]
        [TestCase("testUser", "Max Mustermann", "20yo, Austria, Gemini xxxxDDDD", ":O")]
        [TestCase("mininii", "Mini Mustermann", "Ich war schon immer sehr interessiert an Monster Trading Games.", ":3")]
        public void testToString(string username, string name, string bio, string image) {
            //arrange
            u1 = new User(Guid.NewGuid(), username, "testUserPassword");
            u1.SetUserData(name, bio, image);

            //act
            string userData = u1.ToString();

            //assert
            Assert.AreEqual($"UserId:{u1.Id, -35} Username:{u1.Username, -20} Name:{u1.Name, -30} Bio:{u1.Bio, -30} Image:{u1.Image, -5}" +
                    $"Coins:20  GamesPlayed:0   GamesWon:0   GamesLost:0   Elo:100  Friends:[]", u1.ToString());
        }

        [Test]
        [TestCase("Max Mustermann", "20yo, Austria, Gemini xxxxDDDD", ":O")]
        [TestCase("Mini Mustermann", "Ich war schon immer sehr interessiert an Monster Trading Games.", ":3")]
        public void testSetUserData(string name, string bio, string image) {
            //arrange
            u1 = new User(Guid.NewGuid(), "testUser", "testUserPassword");

            //act
            u1.SetUserData(name, bio, image);

            //assert
            Assert.AreEqual(name, u1.Name);
            Assert.AreEqual(bio, u1.Bio);
            Assert.AreEqual(image, u1.Image);
        }

        [Test]
        public void testGetCardFromDeck() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id });

            //act
            Card card1 = u1.GetCardFromDeck();
            Card card2 = u2.GetCardFromDeck();

            //assert
            Assert.AreEqual(true, u1.Deck.Contains(card1));
            Assert.AreEqual(null, card2);
        }

        [Test]
        public void testConfigureDeck() {
            //arrange
            int before = u1.Deck.Count;
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act
            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id });

            //assert
            Assert.AreEqual(4, u1.Deck.Count);
            Assert.AreEqual(0, before);
            Assert.AreEqual(true, u1.Deck.Contains(m1));
            Assert.AreEqual(false, u1.Deck.Contains(m5));
        }

        [Test]
        public void testConfigureDeck_throwsExceptionTooManyCards() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            Assert.Throws<InconsistentNumberException>(delegate { u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id, m5.Id }); });
        }

        [Test]
        public void testConfigureDeck_throwsExceptionTooFewCards() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            Assert.Throws<InconsistentNumberException>(delegate { u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id }); });
        }

        [Test]
        public void testConfigureDeck_throwsExceptionInvalidCard() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            Assert.Throws<NotInDeckOrStackException>(delegate { u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m5.Id }); });
        }

        [Test]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void testConfigureDeckAfterBattle(int countCards) {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.Deck.AddRange(u1.Stack.GetRange(0, countCards));

            //act
            u1.ConfigureDeckAfterBattle();

            //assert
            Assert.AreEqual(5, u1.Stack.Count);
            Assert.AreEqual(4, u1.Deck.Count);
            Assert.AreEqual(new List<Card> { m1, m2, m3, m4 }, u1.Deck);
        }

        [Test]
        public void testAddFriend() {
            //arrange
            User u3 = new User(Guid.NewGuid(), "otto", "supersecretpassword1");

            //act
            u1.AddFriend(u2);
            u2.AddFriend(u3);

            //assert
            Assert.AreEqual(1, u1.Friends.Count);
            Assert.AreEqual(2, u2.Friends.Count);
            Assert.AreEqual(1, u3.Friends.Count);

            Assert.AreEqual(new List<Guid> { u2.Id }, u1.Friends);
            Assert.AreEqual(new List<Guid> { u1.Id, u3.Id }, u2.Friends);
            Assert.AreEqual(new List<Guid> { u2.Id }, u3.Friends);
        }

        [Test]
        public void testAddFriend_throwsExceptionAlreadyBefriended() {
            //arrange
            //act
            u1.AddFriend(u2);

            //assert
            FriendException ex1 = Assert.Throws<FriendException>(delegate { u2.AddFriend(u1); });
            Assert.That(ex1.Message, Is.EqualTo($"User maxi is already your friend!"));
        }

        [Test]
        public void testAddFriend_throwsExceptionSameUser() {
            //arrange
            //act & assert
            FriendException ex1 = Assert.Throws<FriendException>(delegate { u1.AddFriend(u1); });
            Assert.That(ex1.Message, Is.EqualTo($"You cannot befriend yourself!"));
        }

        [Test]
        public void testRemoveFriend() {
            //arrange
            User u3 = new User(Guid.NewGuid(), "otto", "supersecretpassword1");
            u1.AddFriend(u2);
            u2.AddFriend(u3);

            //act
            u2.RemoveFriend(u1);

            //assert
            Assert.AreEqual(new List<Guid> { }, u1.Friends);
            Assert.AreEqual(new List<Guid> { u3.Id }, u2.Friends);
            Assert.AreEqual(new List<Guid> { u2.Id }, u3.Friends);
        }

        [Test]
        public void testRemoveFriend_throwsExceptionNotBefriended() {
            //arrange
            User u3 = new User(Guid.NewGuid(), "otto", "supersecretpassword1");

            //act & assert
            FriendException ex1 = Assert.Throws<FriendException>(delegate { u1.RemoveFriend(u3); });
            Assert.That(ex1.Message, Is.EqualTo($"User otto is not your friend!"));
        }

        [Test]
        public void testRemoveFriend_throwsExceptionSameUser() {
            //arrange
            //act & assert
            FriendException ex1 = Assert.Throws<FriendException>(delegate { u1.RemoveFriend(u1); });
            Assert.That(ex1.Message, Is.EqualTo($"You cannot unfriend yourself!"));
        }

        [Test]
        public void testGetUserData_plain() {
            //arrange
            //act
            string dataPlain = u1.GetUserData(false);

            //assert
            Assert.AreEqual(dataPlain, u1.ToString());
        }

        [Test]
        [TestCase("maxi", "Username")]
        [TestCase("", "Name")]
        [TestCase("Your bio could stand here!", "Bio")]
        [TestCase("", "Image")]
        [TestCase("20", "Coins")]
        [TestCase("0", "GamesPlayed")]
        [TestCase("0", "GamesWon")]
        [TestCase("0", "GamesLost")]
        [TestCase("100", "Elo")]
        public void testGetUserData_json(string value, string key) {
            //arrange
            //act
            string dataJson = u1.GetUserData(true);
            JObject newU1 = (JObject)JsonConvert.DeserializeObject(dataJson);

            //assert
            Assert.AreEqual(value, newU1.GetValue(key).ToString());
        }

        [Test]
        public void testGetUserStats_plain() {
            //arrange
            //act
            string dataPlain = u1.GetUserStats(false);

            //assert
            Assert.AreEqual($"username:{u1.Username, -20} gamesPlayed:{u1.GamesPlayed, -3} gamesWon:{u1.GamesWon, -3} gamesLost:{u1.GamesWon, -3} elo:{u1.Elo, -4}", dataPlain);
        }

        [Test]
        [TestCase(null, "password")]
        [TestCase("maxi", "username")]
        [TestCase("0", "gamesPlayed")]
        [TestCase("0", "gamesWon")]
        [TestCase("0", "gamesLost")]
        [TestCase("100", "elo")]
        public void testGetUserStats_json(string value, string key) {
            //arrange
            //act
            string dataJson = u1.GetUserStats(true);
            JObject newU1 = (JObject)JsonConvert.DeserializeObject(dataJson);

            //assert
            Assert.AreEqual(value, newU1.GetValue(key)?.ToString());
        }

        [Test]
        public void testDeckToString_plain() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id });

            //act
            string dataPlain = u1.DeckToString(false);

            string dataPlainRes = "";
            foreach (Card card in u1.Deck) {
                dataPlainRes += card.ToString() + "\n";
            }
            
            //assert
            Assert.AreEqual(dataPlain, dataPlainRes);
        }

        [Test]
        public void testDeckToString_json() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id });

            //act
            string dataJson = u1.DeckToString(true);

            JArray u1Deck = (JArray)((JObject)JsonConvert.DeserializeObject(dataJson)).GetValue("Deck");
            JObject m1Json = (JObject)JsonConvert.DeserializeObject(u1Deck[0].ToString());

            //assert
            Assert.AreEqual(m1.Id.ToString(), m1Json.GetValue("Id").ToString());
            Assert.AreEqual(m1.Name, m1Json.GetValue("Name").ToString());
            Assert.AreEqual(m1.Damage.ToString(), m1Json.GetValue("Damage").ToString());
        }

        [Test]
        public void testStackToString_plain() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2 });

            //act
            string dataPlain = u1.StackToString(false);

            string dataPlainRes = "";
            foreach (Card card in u1.Stack) {
                dataPlainRes += card.ToString() + "\n";
            }

            //assert
            Assert.AreEqual(dataPlain, dataPlainRes);
        }

        [Test]
        public void testStackToString_json() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2 });

            //act
            string dataJson = u1.StackToString(true);

            JArray u1Stack = (JArray)((JObject)JsonConvert.DeserializeObject(dataJson)).GetValue("Stack");
            JObject m1Json = (JObject)JsonConvert.DeserializeObject(u1Stack[0].ToString());

            //assert
            Assert.AreEqual(m1.Id.ToString(), m1Json.GetValue("Id").ToString());
            Assert.AreEqual(m1.Name, m1Json.GetValue("Name").ToString());
            Assert.AreEqual(m1.Damage.ToString(), m1Json.GetValue("Damage").ToString());
        }
    }
}