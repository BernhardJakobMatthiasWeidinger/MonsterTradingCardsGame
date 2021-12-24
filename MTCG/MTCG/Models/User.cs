﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTCG.Models {
    public class User {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        [JsonIgnore]
        public string Password { get; private set; }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Coins { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int Elo { get; set; }
        [JsonIgnore]
        public List<Card> Stack { get; set; }
        [JsonIgnore]
        public List<Card> Deck { get; set; }
        public List<Guid> Friends { get; set; }

        public User(string username, string password) {
            this.Id = Guid.NewGuid();
            if (username.IndexOfAny(new char[] { ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Username is not allowed to contain following characters: ; / \\ \' \"");
            }
            if (password.IndexOfAny(new char[] { ';', '/', '\\', '\'', '"' }) != -1) {
                throw new ArgumentException("Password is not allowed to contain following characters: ; / \\ \' \"");
            }
            this.Username = username;
            this.Password = password;

            this.Bio = "Hier könnte deine Biografie stehen!";
            this.Coins = 20;
            this.GamesPlayed = 0;
            this.GamesWon = 0;
            this.GamesLost = 0;
            this.Elo = 100;
            this.Stack = new List<Card>();
            this.Deck = new List<Card>();
            this.Friends = new List<Guid>();
        }

        public override string ToString() {
            string res = $"id:{Id},username:{Username},name:{Name},bio:{Bio},image:{Image}," +
                    $"coins:{Coins},gamesPlayed:{GamesPlayed},gamesWon:{GamesWon},gamesLost:{GamesLost},elo:{Elo},friends:[";

            int i = 0;
            foreach (Guid friend in Friends) {
                res += friend.ToString();
                res += i != (Friends.Count - 1) ? ";" : "";
                i++;
            }
            res += "]";
            return res;
        }

        public void SetUserData(string name, string bio, string image) {
            this.Name = name;
            this.Bio = bio;
            this.Image = image;
        }

        public Card GetCardFromDeck() {
            if (Deck.Count > 0) {
                Random rnd = new Random();
                int cardIndex = rnd.Next(0, Deck.Count);

                return Deck[cardIndex];
            } else {
                return null;
            }
        }
        
        public void ConfigureDeck(List<Guid> guids) {
            List<Card> res = new List<Card>();

            if (guids.Count != 4) {
                throw new ArgumentException($"A deck should be provided with 4 cards, " +
                    $"cards given: {guids.Count}");
            }

            foreach (Guid guid in guids) {
                Card card = Stack.Find(c => c.Id == guid);
                if (card != null) {
                    res.Add(card);
                } else {
                    throw new ArgumentException($"Card with id {guid} was not found in stack!");
                }
            }

            Deck = res;
        }

        public void ConfigureDeckAfterBattle() {
            if (Deck.Count > 4) {
                //deck should only consist of the 4 strongest cards
                List<Card> sortedList = Deck.OrderByDescending(c => c.Damage).ToList().Take(4).ToList();
                Deck = sortedList;
            } else if (Deck.Count < 4) {
                //add strongest remaining cards from stack to deck
                List<Card> sortedList = Stack.OrderByDescending(c => c.Damage).ToList().Take(4).ToList();
                List<Card> notInDeck = sortedList.Except(Deck).ToList();
                Deck.AddRange(notInDeck);
            }
        }

        public void AddFriend(User other) {
            if (this.Friends.Contains(other.Id)) {
                throw new ArgumentException($"User {other.Username} is already your friend!");
            } else {
                this.Friends.Add(other.Id);
                other.Friends.Add(this.Id);
            }
        }

        public void RemoveFriend(User other) {
            if (this.Friends.Contains(other.Id)) {
                this.Friends.Remove(other.Id);
                other.Friends.Remove(this.Id);
            } else {
                throw new ArgumentException($"User {other.Username} is not your friend!");
            }
        }

        public string GetUserData(bool isJson) {
            string res = "";
            if (isJson) {
                res = JsonConvert.SerializeObject(this);
            } else {
                res += this.ToString();
            }
            return res;
        }

        public string GetUserStats(bool isJson) {
            string res = "";
            if (isJson) {
                JObject o = new JObject();
                o["gamesPlayed"] = GamesPlayed;
                o["gamesWon"] = GamesWon;
                o["gamesLost"] = GamesLost;
                o["elo"] = Elo;
                res = o.ToString();
            } else {
                res += $"gamesPlayed:{GamesPlayed},gamesWon:{GamesWon},gamesLost:{GamesWon},elo:{Elo}";
            }
            return res;
        }

        public string DeckToString(bool isJson) {
            string res = "";
            if (isJson) {
                JArray array = new JArray();
                foreach (Card card in Deck) {
                    array.Add(JsonConvert.SerializeObject(card));
                }
                JObject o = new JObject();
                o["Deck"] = array;
                res = o.ToString();
            } else {
                int i = 0;
                foreach (Card card in Deck) {
                    res += card.ToString();
                    res += i != (Deck.Count - 1) ? ";" : "";
                    i++;
                }
            }
            return res;
        }

        public string StackToString(bool isJson) {
            string res = "";
            if (isJson) {
                JArray array = new JArray();
                foreach (Card card in Stack) {
                    array.Add(JsonConvert.SerializeObject(card));
                }
                JObject o = new JObject();
                o["Stack"] = array;
                res = o.ToString();
            } else {
                int i = 0;
                foreach (Card card in Stack) {
                    res += card.ToString();
                    res += i != (Stack.Count - 1) ? ";" : "";
                    i++;
                }
            }
            return res;
        }
    }
}