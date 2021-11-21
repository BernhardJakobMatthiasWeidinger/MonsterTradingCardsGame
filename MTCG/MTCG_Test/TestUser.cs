using MTCG.src;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.Test {
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
            ArgumentException ex1 = Assert.Throws<ArgumentException>(delegate { new User("test,User", "testUserPassword"); });
            ArgumentException ex2 = Assert.Throws<ArgumentException>(delegate { new User("test;User", "testUserPassword"); });
            ArgumentException ex3 = Assert.Throws<ArgumentException>(delegate { new User("test/User", "testUserPassword"); });
            ArgumentException ex4 = Assert.Throws<ArgumentException>(delegate { new User("test\\User", "testUserPassword"); });
            ArgumentException ex5 = Assert.Throws<ArgumentException>(delegate { new User("test\'User", "testUserPassword"); });
            ArgumentException ex6 = Assert.Throws<ArgumentException>(delegate { new User("test\"User", "testUserPassword"); });

            ArgumentException ex7 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test,User;Password"); });
            ArgumentException ex8 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test;User/Password"); });
            ArgumentException ex9 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test/User\\Password"); });
            ArgumentException ex10 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test\\User\'Password"); });
            ArgumentException ex11 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test\'User\"Password"); });
            ArgumentException ex12 = Assert.Throws<ArgumentException>(delegate { new User("testUser", "test\"User,Password"); });

            //assert
            Assert.That(ex1.Message, Is.EqualTo("Username is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex2.Message, Is.EqualTo("Username is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex3.Message, Is.EqualTo("Username is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex4.Message, Is.EqualTo("Username is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex5.Message, Is.EqualTo("Username is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex6.Message, Is.EqualTo("Username is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex7.Message, Is.EqualTo("Password is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex8.Message, Is.EqualTo("Password is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex9.Message, Is.EqualTo("Password is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex10.Message, Is.EqualTo("Password is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex11.Message, Is.EqualTo("Password is not allowed to contain following characters: , ; / \\ \' \""));
            Assert.That(ex12.Message, Is.EqualTo("Password is not allowed to contain following characters: , ; / \\ \' \""));
        }

        [Test]
        public void testToString() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testSetUserData() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testGetCardFromDeck() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testAddCards() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testConfigureDeck() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testConfigureDeckAfterBattle() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testAddFriend() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testGetUserData() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testGetUserStats() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testDeckToString() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }

        [Test]
        public void testStackToString() {
            //arrange
            //act
            User u1 = new User("testUser", "testUserPassword");
            User u2 = new User("maxi", "supersecretpassword1");

            //assert
            Assert.AreEqual(u1.ToString(), "");
        }
    }
}
