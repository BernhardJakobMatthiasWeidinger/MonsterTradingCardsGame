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

        public static void InsertUser(User user) {
            var sql = "insert into users (userId, username, password, name, bio, image, coins, gamesPlayed, gamesWon, gamesLost, elo)" + 
           " values(@userId, @username, @password, @name, @bio, @image, @coins, @gamesPlayed, @gamesWon, @gamesLost, @elo);";

            using var cmd = new NpgsqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("userId", user.Id);
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Parameters.AddWithValue("name", "");
            cmd.Parameters.AddWithValue("bio", "");
            cmd.Parameters.AddWithValue("image", "");
            cmd.Parameters.AddWithValue("coins", user.Coins);
            cmd.Parameters.AddWithValue("gamesPlayed", user.GamesPlayed);
            cmd.Parameters.AddWithValue("gamesWon", user.GamesWon);
            cmd.Parameters.AddWithValue("gamesLost", user.GamesLost);
            cmd.Parameters.AddWithValue("elo", user.Elo);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateUser(User oldUser, User newUser) {
            var sql = "SELECT version()";

            using var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
        }
    }
}
