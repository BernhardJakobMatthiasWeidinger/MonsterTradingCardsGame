﻿using MTCG.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG {
    public class DBConnection {
        private static NpgsqlConnection connection;

        public static bool Connect() {
            try {
                connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=abc123;Database=mtcg");
                connection.Open();
            } catch (Exception) {
                return false;
            }
            return true;
        }

        public static List<User> SelectAllUsers() {
            var sql = "select userId, username, password, name, bio, image, coins, gamesPlayed, gamesWon, gamesLost, elo" +
            " from users;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<User> users = new List<User>();
            // Output rows
            while (dr.Read()) {
                User user = new User(Guid.Parse(dr[0].ToString()), dr[1].ToString(), dr[2].ToString());
                user.Name = dr[3].ToString();
                user.Bio = dr[4].ToString();
                user.Image = dr[5].ToString();
                user.Coins = Int32.Parse(dr[6].ToString());
                user.GamesPlayed = Int32.Parse(dr[7].ToString());
                user.GamesWon = Int32.Parse(dr[8].ToString());
                user.GamesLost = Int32.Parse(dr[9].ToString());
                user.Elo = Int32.Parse(dr[10].ToString());

                users.Add(user);
            }
            dr.DisposeAsync();
            return users;
        }

        public static void InsertUser(User user) {
            var sql = "insert into users (userId, username, password, name, bio, image, coins, gamesPlayed, gamesWon, gamesLost, elo)" + 
           " values(@userId, @username, @password, @name, @bio, @image, @coins, @gamesPlayed, @gamesWon, @gamesLost, @elo);";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("userId", user.Id);
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Parameters.AddWithValue("name", "");
            cmd.Parameters.AddWithValue("bio", user.Bio);
            cmd.Parameters.AddWithValue("image", "");
            cmd.Parameters.AddWithValue("coins", user.Coins);
            cmd.Parameters.AddWithValue("gamesPlayed", user.GamesPlayed);
            cmd.Parameters.AddWithValue("gamesWon", user.GamesWon);
            cmd.Parameters.AddWithValue("gamesLost", user.GamesLost);
            cmd.Parameters.AddWithValue("elo", user.Elo);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateUser(User user) {
            var sql = "UPDATE users SET " +
                "name = @name, " +
                "bio = @bio, " +
                "image = @image, " +
                "coins = @coins, " +
                "gamesPlayed = @gamesPlayed, " +
                "gamesWon = @gamesWon, " +
                "gamesLost = @gamesLost, " +
                "elo = @elo " +
                "WHERE userid = @userid; ";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("userid", user.Id);
            cmd.Parameters.AddWithValue("name", user.Name);
            cmd.Parameters.AddWithValue("bio", user.Bio);
            cmd.Parameters.AddWithValue("image", user.Image);
            cmd.Parameters.AddWithValue("coins", user.Coins);
            cmd.Parameters.AddWithValue("gamesPlayed", user.GamesPlayed);
            cmd.Parameters.AddWithValue("gamesWon", user.GamesWon);
            cmd.Parameters.AddWithValue("gamesLost", user.GamesLost);
            cmd.Parameters.AddWithValue("elo", user.Elo);
            cmd.ExecuteNonQuery();
        }

        public static List<Tuple<Card, bool, Guid?>> SelectAllCards() {
            var sql = "select cardid, name, damage, ismonster, indeck, userid" +
            " from cards;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = cmd.ExecuteReader();

            //Card, isInDeck, UserId
            List<Tuple<Card, bool, Guid?>> cards = new List<Tuple<Card, bool, Guid?>>();
            // Output rows
            while (dr.Read()) {
                Card card;
                if (Boolean.Parse(dr[3].ToString()) == false) {
                    card = new SpellCard(Guid.Parse(dr[0].ToString()), dr[1].ToString(), Double.Parse(dr[2].ToString()));
                } else {
                    card = new MonsterCard(Guid.Parse(dr[0].ToString()), dr[1].ToString(), Double.Parse(dr[2].ToString()));
                }
                cards.Add(new Tuple<Card, bool, Guid?>(card, Boolean.Parse(dr[4].ToString()), 
                    !String.IsNullOrWhiteSpace(dr[5].ToString()) ? Guid.Parse(dr[5].ToString()) : null));
            }
            dr.DisposeAsync();
            return cards;
        }

        public static void InsertCard(Card card) {
            var sql = "insert into cards (cardid, name, damage, ismonster, indeck, userid)" +
           " values(@cardid, @name, @damage, @ismonster, @indeck, @userid);";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("cardid", card.Id);
            cmd.Parameters.AddWithValue("name", card.Name);
            cmd.Parameters.AddWithValue("damage", card.Damage);
            cmd.Parameters.AddWithValue("ismonster", card.GetType().Name == "MonsterCard" ? true : false);
            cmd.Parameters.AddWithValue("indeck", false);
            cmd.Parameters.AddWithValue("userid", DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCard(Guid cardId, bool inDeck, Guid userId) {
            var sql = "UPDATE cards SET " +
                "indeck = @indeck, " +
                "userid = @userid " +
                "WHERE cardid = @cardid; ";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("cardid", cardId.ToString());
            cmd.Parameters.AddWithValue("indeck", inDeck);
            cmd.Parameters.AddWithValue("userid", userId);
            cmd.ExecuteNonQuery();
        }

        public static List<List<Guid>> SelectAllPackages() {
            var sql = "select packageid, cardid1, cardid2, cardid3, cardid4, cardid5" +
            " from packages;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            //Card, isInDeck, UserId
            List<List<Guid>> packages = new List<List<Guid>>();
            // Output rows
            while (dr.Read()) {
                List<Guid> package = new List<Guid> { Guid.Parse(dr[0].ToString()), Guid.Parse(dr[1].ToString()), Guid.Parse(dr[2].ToString()),
                Guid.Parse(dr[3].ToString()), Guid.Parse(dr[4].ToString()), Guid.Parse(dr[5].ToString())};
                packages.Add(package);
            }
            dr.DisposeAsync();
            return packages;
        }
        public static void InsertPackage(Package package) {
            foreach (Card card in package.Cards) {
                InsertCard(card);
            }

            var sql = "insert into packages (packageid, cardid1, cardid2, cardid3, cardid4, cardid5)" +
           " values(@packageid, @cardid1, @cardid2, @cardid3, @cardid4, @cardid5);";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("packageid", package.Id);
            cmd.Parameters.AddWithValue("cardid1", package.Cards[0].Id);
            cmd.Parameters.AddWithValue("cardid2", package.Cards[1].Id);
            cmd.Parameters.AddWithValue("cardid3", package.Cards[2].Id);
            cmd.Parameters.AddWithValue("cardid4", package.Cards[3].Id);
            cmd.Parameters.AddWithValue("cardid5", package.Cards[4].Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeletePackage(Package package) {
            var sql = "delete from packages where packageid = @packageid;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("packageid", package.Id.ToString());
            cmd.ExecuteNonQuery();
        }
    }
}
