using MTCG.Models;
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
                "WHERE username = @username; ";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("name", user.Name ?? "");
            cmd.Parameters.AddWithValue("bio", user.Bio ?? "");
            cmd.Parameters.AddWithValue("image", user.Image ?? "");
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
                Card card = GetCard(Guid.Parse(dr[0].ToString()), dr[1].ToString(), Double.Parse(dr[2].ToString()), Boolean.Parse(dr[3].ToString()));
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

        public static List<Package> SelectAllPackages() {
            var sql = "select packageid, " +
                "c1.cardid, c1.name, c1.damage, c1.ismonster, " +
                "c2.cardid, c2.name, c2.damage, c2.ismonster, " +
                "c3.cardid, c3.name, c3.damage, c3.ismonster, " +
                "c4.cardid, c4.name, c4.damage, c4.ismonster, " +
                "c5.cardid, c5.name, c5.damage, c5.ismonster " +
                "from packages p " +
                "join cards c1 on p.cardid1 = c1.cardid " +
                "join cards c2 on p.cardid2 = c2.cardid " +
                "join cards c3 on p.cardid3 = c3.cardid " +
                "join cards c4 on p.cardid4 = c4.cardid " +
                "join cards c5 on p.cardid5 = c5.cardid; ";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            //Card, isInDeck, UserId
            List<Package> packages = new List<Package>();
            // Output rows
            while (dr.Read()) {
                List<Card> cards = new List<Card>();
                for (int i=1; i<=20; i+=4) {
                    Card card = GetCard(Guid.Parse(dr[i].ToString()), dr[i+1].ToString(), Double.Parse(dr[i+2].ToString()), Boolean.Parse(dr[i+3].ToString()));
                    cards.Add(card);
                }

                Package package = new Package(Guid.Parse(dr[0].ToString()), cards);
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
           " values (@packageid, @cardid1, @cardid2, @cardid3, @cardid4, @cardid5);";

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

        public static List<List<string>> SelectAllTrades() {
            var sql = "select tradeid, cardtype, minimumdamage, userid, cardid" +
            " from trades;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<List<string>> trades = new List<List<string>>();

            while (dr.Read()) {
                List<string> trade = new List<string>();
                for (int i=0; i <=4; ++i) {
                    trade.Add(dr[i].ToString());
                }
                trades.Add(trade);
            }
            dr.DisposeAsync();

            return trades;
        }

        public static void InsertTrade(Trade trade) {
            var sql = "insert into trades (tradeid, cardtype, minimumdamage, userid, cardid)" +
           " values (@tradeid, @cardtype, @minimumdamage, @userid, @cardid);";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("tradeid", trade.Id);
            cmd.Parameters.AddWithValue("cardtype", trade.CardType.ToString());
            cmd.Parameters.AddWithValue("minimumdamage", trade.MinimumDamage);
            cmd.Parameters.AddWithValue("userid", trade.Provider.Id);
            cmd.Parameters.AddWithValue("cardid", trade.CardToTrade.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteTrade(Trade trade) {
            var sql = "delete from trades where tradeid = @tradeid";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("tradeid", trade.Id.ToString());
            cmd.ExecuteNonQuery();
        }

        private static Card GetCard(Guid id, string name, double damage, bool isMonster) {
            Card card;
            if (isMonster) {
                card = new MonsterCard(id, name, damage);
            } else {
                card = new SpellCard(id, name, damage);
            }
            return card;
        }

        public static List<Tuple<Guid, Guid>> SelectAllFriends() {
            var sql = "select userid1, userid2" +
            " from friends;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            List<Tuple<Guid, Guid>> friends = new List<Tuple<Guid, Guid>>();

            while (dr.Read()) {
                friends.Add(new Tuple<Guid, Guid>(Guid.Parse(dr[0].ToString()), Guid.Parse(dr[1].ToString())));
            }
            dr.DisposeAsync();
            return friends;
        }

        public static void InsertFriend(Guid userid1, Guid userid2) {
            var sql = "insert into friends (userid1, userid2) values (@userid1, @userid2);";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("userid1", userid1);
            cmd.Parameters.AddWithValue("userid2", userid2);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteFriend(Guid userid1, Guid userid2) {
            var sql = "delete from friends where userid1 = @userid1 and userid2 = @userid2";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("userid1", userid1.ToString());
            cmd.Parameters.AddWithValue("userid2", userid2.ToString());

            if (cmd.ExecuteNonQuery() == 0) {
                sql = "delete from friends where userid1 = @userid2 and userid2 = @userid1";

                cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("userid1", userid1.ToString());
                cmd.Parameters.AddWithValue("userid2", userid2.ToString());
                cmd.ExecuteNonQuery();
            }
        }
    }
}