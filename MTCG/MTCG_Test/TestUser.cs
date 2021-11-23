using MTCG.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTCG.GameLogic.Test {
    public class TestUser {
        [Test]
        public void testConstructor() {
            //arrange
            //act
            User u1 = new User("testUser!", "testUserPassword");

            //assert
            Assert.AreEqual(u1.username, "testUser!");
            Assert.AreEqual(u1.password, "testUserPassword");
            Assert.AreEqual(u1.name, null);
            Assert.AreEqual(u1.bio, "Hier könnte deine Biografie stehen!");
            Assert.AreEqual(u1.coins, 20);
            Assert.AreEqual(u1.gamesPlayed, 0);
            Assert.AreEqual(u1.gamesWon, 0);
            Assert.AreEqual(u1.gamesLost, 0);
            Assert.AreEqual(u1.elo, 100);
        }

        [Test]
        public void testConstructor_throwException() {
            //arrange
            //act
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new User("test;User", "testUserPassword"); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { new User("test/User", "testUserPassword"); });
            ArgumentException ex3 = Assert.Throws<ArgumentException>(delegate { new User("test\\User", "testUserPassword"); });
            ArgumentException ex4 = Assert.Throws<ArgumentException>(delegate { new User("test\'User", "testUserPassword"); });
            ArgumentException ex5 = Assert.Throws<ArgumentException>(delegate { new User("test\"User", "testUserPassword"); });
            
            ArgumentException ex6 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test;User/Password"); });
            ArgumentException ex7 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test/User\\Password"); });
            ArgumentException ex8 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test\\User\'Password"); });
            ArgumentException ex9 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test\'User\"Password"); });
            ArgumentException ex10 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test\"User;Password"); });

            //assert
            Assert.That(ex1.Message, Is.EqualTo("Username is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex2.Message, Is.EqualTo("Username is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex3.Message, Is.EqualTo("Username is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex4.Message, Is.EqualTo("Username is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex5.Message, Is.EqualTo("Username is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex6.Message, Is.EqualTo("Password is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex7.Message, Is.EqualTo("Password is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex8.Message, Is.EqualTo("Password is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex9.Message, Is.EqualTo("Password is not allowed to contain following characters: ; / \\ \' \""));
            Assert.That(ex10.Message, Is.EqualTo("Password is not allowed to contain following characters: ; / \\ \' \""));
        }

        [Test]
        public void testToString() {
            //arrange
            User u1 = new User("testUser1", "testUserPassword");
            User u2 = new User("testUser2", "testUserPassword");
            User u3 = new User("testUser3", "testUserPassword");

            //act
            u2.setUserData("Max Mustermann", "20yo, Austria, Gemini xxxxDDDD", ":O");
            u2.gamesPlayed++;
            u2.gamesWon++;
            u2.elo += 3;
            u2.addFriend(u3);

            //assert
            Assert.AreEqual(u1.ToString(), $"id:{u1.id},username:testUser1,name:,bio:Hier könnte deine Biografie stehen!,image:," +
                    $"coins:20,gamesPlayed:0,gamesWon:0,gamesLost:0,elo:100,friends:[]");
            Assert.AreEqual(u2.ToString(), $"id:{u2.id},username:testUser2,name:Max Mustermann,bio:20yo, Austria, Gemini xxxxDDDD,image::O," +
                    $"coins:20,gamesPlayed:1,gamesWon:1,gamesLost:0,elo:103,friends:[{u3.id}]");
            Assert.AreEqual(u3.ToString(), $"id:{u3.id},username:testUser3,name:,bio:Hier könnte deine Biografie stehen!,image:," +
                    $"coins:20,gamesPlayed:0,gamesWon:0,gamesLost:0,elo:100,friends:[{u2.id}]");
        }

        [Test]
        public void testSetUserData() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");

            //act
            u1.setUserData("Max Muster\"mann", "20yo; Austria/ Gemini xxxxDDDD", "\"O");

            //assert
            Assert.AreEqual(u1.name, "Max Muster\"mann");
            Assert.AreEqual(u1.bio, "20yo; Austria/ Gemini xxxxDDDD");
            Assert.AreEqual(u1.image, "\"O");
        }

        [Test]
        public void testGetCardFromDeck() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("testUser", "testUserPassword");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);

            Package p1 = new Package(new List<Card> { m1, m2, m3, m4, m5 });
            p1.aquirePackage(u1);
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            //act
            Card card1 = u1.getCardFromDeck();
            Card card2 = u2.getCardFromDeck();

            //assert
            Assert.AreEqual(u1.deck.Contains(card1), true);
            Assert.AreEqual(card2, null);
        }

        [Test]
        public void testConfigureDeck() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);

            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });

            //act
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id, m5.id }); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id }); });
            ArgumentException ex3 = Assert.Throws<ArgumentException>(delegate { u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m5.id }); });

            //assert
            Assert.That(ex1.Message, Is.EqualTo($"A deck should be provided with 4 cards, cards given: 5"));
            Assert.That(ex2.Message, Is.EqualTo($"A deck should be provided with 4 cards, cards given: 3"));
            Assert.That(ex3.Message, Is.EqualTo($"Card with id {m5.id} was not found in stack!"));
        }

        [Test]
        public void testConfigureDeckAfterBattle() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 999.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 999.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 999.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 999.0);

            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);
            MonsterCard m6 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m7 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m8 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);

            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4 });
            u2.stack.AddRange(new List<Card> { m5, m6, m7, m8 });

            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });
            u2.configureDeck(new List<Guid> { m5.id, m6.id, m7.id, m8.id });

            Battle b1 = new Battle(Guid.NewGuid(), u1);

            //act
            b1.play(u2);

            //assert
            Assert.AreEqual(u1.stack.Count, 8);
            Assert.AreEqual(u1.deck.Count, 4);
            Assert.AreEqual(u1.deck, new List<Card> { m1, m2, m3, m4});

            Assert.AreEqual(u2.stack.Count, 0);
            Assert.AreEqual(u2.deck.Count, 0);
        }

        [Test]
        public void testAddFriend() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");
            User u3 = new User("mini", "supersecretpassword1");

            //act
            u1.addFriend(u2);
            u2.addFriend(u3);
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { u2.addFriend(u1); });

            //assert
            Assert.AreEqual(u1.friends.Count, 1);
            Assert.AreEqual(u2.friends.Count, 2);
            Assert.AreEqual(u3.friends.Count, 1);

            Assert.AreEqual(u1.friends, new List<Guid> { u2.id });
            Assert.AreEqual(u2.friends, new List<Guid> { u1.id, u3.id });
            Assert.AreEqual(u3.friends, new List<Guid> { u2.id });

            Assert.That(ex1.Message, Is.EqualTo($"User testUser is already your friend!"));
        }

        [Test]
        public void testGetUserData() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");

            //act
            string dataPlain = u1.getUserData(false);
            string dataJson = u1.getUserData(true);

            JObject newU1 = (JObject)JsonConvert.DeserializeObject(dataJson);

            //assert
            Assert.AreEqual(dataPlain, u1.ToString());
            Assert.AreEqual(u1.id.ToString(), newU1.GetValue("id").ToString());
            Assert.AreEqual("testUser", newU1.GetValue("username").ToString());
            Assert.AreEqual("", newU1.GetValue("name").ToString());
            Assert.AreEqual("Hier könnte deine Biografie stehen!", newU1.GetValue("bio").ToString());
            Assert.AreEqual("", newU1.GetValue("image").ToString());
            Assert.AreEqual("20", newU1.GetValue("coins").ToString());
            Assert.AreEqual("0", newU1.GetValue("gamesPlayed").ToString());
            Assert.AreEqual("0", newU1.GetValue("gamesWon").ToString());
            Assert.AreEqual("0", newU1.GetValue("gamesLost").ToString());
            Assert.AreEqual("100", newU1.GetValue("elo").ToString());
        }

        [Test]
        public void testGetUserStats() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");

            //act
            string dataPlain = u1.getUserStats(false);
            string dataJson = u1.getUserStats(true);

            JObject newU1 = (JObject)JsonConvert.DeserializeObject(dataJson);

            //assert
            Assert.AreEqual(dataPlain, $"gamesPlayed:{u1.gamesPlayed},gamesWon:{u1.gamesWon},gamesLost:{u1.gamesWon},elo:{u1.elo}");
            Assert.AreEqual(null, newU1.GetValue("coins"));
            Assert.AreEqual("0", newU1.GetValue("gamesPlayed").ToString());
            Assert.AreEqual("0", newU1.GetValue("gamesWon").ToString());
            Assert.AreEqual("0", newU1.GetValue("gamesLost").ToString());
            Assert.AreEqual("100", newU1.GetValue("elo").ToString());
        }

        [Test]
        public void testDeckToString() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "Dragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterGoblin", 25.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireGoblin", 25.0);

            u1.stack.AddRange(new List<Card> { m1, m2, m3, m4, m5 });
            u1.configureDeck(new List<Guid> { m1.id, m2.id, m3.id, m4.id });

            //act
            string dataPlain = u1.deckToString(false);
            string dataJson = u1.deckToString(true);

            int i = 0;
            string dataPlainRes = "";
            foreach (Card card in u1.deck) {
                dataPlainRes += card.ToString();
                dataPlainRes += i != (u1.deck.Count - 1) ? ";" : "";
                i++;
            }

            JArray u1Deck = (JArray)((JObject)JsonConvert.DeserializeObject(dataJson)).GetValue("deck");
            JObject m1Json = (JObject)JsonConvert.DeserializeObject(u1Deck[0].ToString());
            
            //assert
            Assert.AreEqual(dataPlain, dataPlainRes);
            Assert.AreEqual(m1.id.ToString(), m1Json.GetValue("id").ToString());
            Assert.AreEqual(m1.name, m1Json.GetValue("name").ToString());
            Assert.AreEqual(m1.damage.ToString(), m1Json.GetValue("damage").ToString());
        }

        [Test]
        public void testStackToString() {
            //arrange
            User u1 = new User("testUser", "testUserPassword");

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireDragon", 25.0);

            u1.stack.AddRange(new List<Card> { m1, m2 });

            //act
            string dataPlain = u1.stackToString(false);
            string dataJson = u1.stackToString(true);

            int i = 0;
            string dataPlainRes = "";
            foreach (Card card in u1.stack) {
                dataPlainRes += card.ToString();
                dataPlainRes += i != (u1.stack.Count - 1) ? ";" : "";
                i++;
            }

            JArray u1Stack = (JArray)((JObject)JsonConvert.DeserializeObject(dataJson)).GetValue("stack");
            JObject m1Json = (JObject)JsonConvert.DeserializeObject(u1Stack[0].ToString());

            //assert
            Assert.AreEqual(dataPlain, dataPlainRes);
            Assert.AreEqual(m1.id.ToString(), m1Json.GetValue("id").ToString());
            Assert.AreEqual(m1.name, m1Json.GetValue("name").ToString());
            Assert.AreEqual(m1.damage.ToString(), m1Json.GetValue("damage").ToString());
        }
    }
}