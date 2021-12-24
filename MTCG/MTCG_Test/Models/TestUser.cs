﻿using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            Assert.AreEqual("maxi", u1.Username);
            Assert.AreEqual("testUserPassword", u1.Password);
            Assert.AreEqual(null, u1.Name);
            Assert.AreEqual("Hier könnte deine Biografie stehen!", u1.Bio);
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
            u1.SetUserData(name, bio, image);

            //act
            string userData = u1.ToString();

            //assert
            Assert.AreEqual($"id:{u1.Id},username:{u1.Username},name:{u1.Name},bio:{u1.Bio},image:{u1.Image}," +
                    $"coins:20,gamesPlayed:0,gamesWon:0,gamesLost:0,elo:100,friends:[]", u1.ToString());
        }

        [Test]
        [TestCase("Max Mustermann", "20yo, Austria, Gemini xxxxDDDD", ":O")]
        [TestCase("Mini Mustermann", "Ich war schon immer sehr interessiert an Monster Trading Games.", ":3")]
        public void testSetUserData(string name, string bio, string image) {
            //arrange
            u1 = new User("testUser", "testUserPassword");

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
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m4.Id, m5.Id }); });
            Assert.That(ex1.Message, Is.EqualTo($"A deck should be provided with 4 cards, cards given: 5"));
        }

        [Test]
        public void testConfigureDeck_throwsExceptionTooFewCards() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id }); });
            Assert.That(ex2.Message, Is.EqualTo($"A deck should be provided with 4 cards, cards given: 3"));
        }

        [Test]
        public void testConfigureDeck_throwsExceptionInvalidCard() {
            //arrange
            u1.Stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act & assert
            ArgumentException ex3 = Assert.Throws<ArgumentException>(delegate { u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, m5.Id }); });
            Assert.That(ex3.Message, Is.EqualTo($"Card with id {m5.Id} was not found in stack!"));
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
            User u3 = new User("otto", "supersecretpassword1");

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
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u2.AddFriend(u1); });
            Assert.That(ex1.Message, Is.EqualTo($"User maxi is already your friend!"));
        }

        [Test]
        public void testRemoveFriend() {
            //arrange
            User u3 = new User("otto", "supersecretpassword1");
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
            User u3 = new User("otto", "supersecretpassword1");

            //act & assert
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u1.RemoveFriend(u3); });
            Assert.That(ex1.Message, Is.EqualTo($"User otto is not your friend!"));
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
        [TestCase("Hier könnte deine Biografie stehen!", "Bio")]
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
            Assert.AreEqual($"gamesPlayed:{u1.GamesPlayed},gamesWon:{u1.GamesWon},gamesLost:{u1.GamesWon},elo:{u1.Elo}", dataPlain);
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

            int i = 0;
            string dataPlainRes = "";
            foreach (Card card in u1.Deck) {
                dataPlainRes += card.ToString();
                dataPlainRes += i != (u1.Deck.Count - 1) ? ";" : "";
                i++;
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

            int i = 0;
            string dataPlainRes = "";
            foreach (Card card in u1.Stack) {
                dataPlainRes += card.ToString();
                dataPlainRes += i != (u1.Stack.Count - 1) ? ";" : "";
                i++;
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