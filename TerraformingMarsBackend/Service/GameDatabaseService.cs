using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TerraformingMarsBackend.Models;

namespace TerraformingMarsBackend.Service
{
    public static class GameDatabaseService
    {
        //If id = 0, then get all users.
        public static IEnumerable<TerraformingMarsUser> GetTerraformingMarsUsers(int userId = 0)
        {
            List<TerraformingMarsUser> users = new List<TerraformingMarsUser>();

            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("get_users", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    int id = (int)res.GetValue(res.GetOrdinal("Id"));
                    string outerId = (string)res.GetValue(res.GetOrdinal("OuterId"));
                    string userName = (string)res.GetValue(res.GetOrdinal("UserName"));
                    int playerId = (int)res.GetValue(res.GetOrdinal("PlayerId"));
                    int gameRoomId = (int)res.GetValue(res.GetOrdinal("GameRoomId"));

                    TerraformingMarsUser result = new TerraformingMarsUser(id, Guid.Parse(outerId), userName, playerId, gameRoomId);

                    users.Add(result);
                }
                res.Close();
            }
            con.Close();

            return users;
        }

        public static int InsertTerraformingMarsUser(TerraformingMarsUser user)
        {
            int idToReturn = -1;
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("insert_user", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OuterId", SqlDbType.NVarChar).Value = user.OuterId.ToString();
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = user.Name;
                cmd.Parameters.Add("@PlayerId", SqlDbType.Int).Value = user.PlayerId;
                cmd.Parameters.Add("@GameRoomId", SqlDbType.Int).Value = user.GameRoomId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    idToReturn = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return idToReturn;
        }

        public static int UpdateTerraformingMarsUser(TerraformingMarsUser user)
        {
            int idToReturn = -1;
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("update_user", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = user.Id;
                cmd.Parameters.Add("@OuterId", SqlDbType.NVarChar).Value = user.OuterId.ToString();
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = user.Name;
                cmd.Parameters.Add("@PlayerId", SqlDbType.Int).Value = user.PlayerId;
                cmd.Parameters.Add("@GameRoomId", SqlDbType.Int).Value = user.GameRoomId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    idToReturn = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return idToReturn;
        }

        public static IEnumerable<ChatMessage> GetChatMessages(int chatId = 0)
        {
            List<ChatMessage> chats = new List<ChatMessage>();

            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("get_messages", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = chatId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    int id = (int)res.GetValue(res.GetOrdinal("Id"));
                    string userId = (string)res.GetValue(res.GetOrdinal("UserId"));
                    string userName = (string)res.GetValue(res.GetOrdinal("UserName"));
                    int gameRoomId = (int)res.GetValue(res.GetOrdinal("GameRoomId"));
                    int isLobbyMessage = (byte)res.GetValue(res.GetOrdinal("IsLobbyMessage"));
                    string timeSent = (string)res.GetValue(res.GetOrdinal("TimeSent"));
                    string message = (string)res.GetValue(res.GetOrdinal("Message"));

                    ChatMessage result = new ChatMessage();
                    result.Id = id;
                    result.UserId = Guid.Parse(userId);
                    result.UserName = userName;
                    result.GameRoomId = gameRoomId;
                    result.IsLobbyMessage = isLobbyMessage == 1;
                    result.TimeSent = DateTime.Parse(timeSent);
                    result.Message = message;

                    chats.Add(result);
                }
                res.Close();
            }
            con.Close();

            return chats;
        }

        public static int InsertChatMessage(ChatMessage cm)
        {
            int idToReturn = -1;
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("insert_message", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = cm.UserId.ToString();
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = cm.UserName;
                cmd.Parameters.Add("@GameRoomId", SqlDbType.Int).Value = cm.GameRoomId;
                cmd.Parameters.Add("@IsLobbyMessage", SqlDbType.TinyInt).Value = cm.IsLobbyMessage;
                cmd.Parameters.Add("@TimeSent", SqlDbType.NVarChar).Value = cm.TimeSent.ToString("G");
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar).Value = cm.Message;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    idToReturn = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return idToReturn;
        }

        public static IEnumerable<GameRoom> GetGameRooms(int gameRoomId = 0)
        {
            List<GameRoom> rooms = new List<GameRoom>();

            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("get_game_rooms", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = gameRoomId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    int id = (int)res.GetValue(res.GetOrdinal("Id"));
                    string leaderUserId = (string)res.GetValue(res.GetOrdinal("LeaderUserId"));
                    int gameId = (int)res.GetValue(res.GetOrdinal("GameId"));

                    GameRoom result = new GameRoom();
                    result.Id = id;
                    result.LeaderUserId = Guid.Parse(leaderUserId);
                    result.GameId = gameId;

                    rooms.Add(result);
                }
                res.Close();
            }
            con.Close();

            return rooms;
        }

        public static int InsertGameRoom(GameRoom gameRoom)
        {
            int idToReturn = -1;
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("insert_game_rooms", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@LeaderUserId", SqlDbType.NVarChar).Value = gameRoom.LeaderUserId.ToString();
                cmd.Parameters.Add("@GameId", SqlDbType.Int).Value = gameRoom.GameId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    idToReturn = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return idToReturn;
        }

        public static Game InsertGame(int difficulty)
        {
            Game game = new Game();

            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("insert_game", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@difficulty", SqlDbType.Int).Value = difficulty;
                cmd.Parameters.Add("@generation", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@time_remaining", SqlDbType.Int).Value = 60;
                cmd.Parameters.Add("@is_game_ended", SqlDbType.TinyInt).Value = false;
                cmd.Parameters.Add("@oxygen_level", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@temperature_level", SqlDbType.Int).Value = -30;
                cmd.Parameters.Add("@ocean_level", SqlDbType.Int).Value = 0;

                game.Difficulty = difficulty;
                game.Generation = 1;
                game.TimeRemaining = 60;
                game.IsGameEnded = false;
                game.OxygenLevel = 0;
                game.TemperatureLevel = -30;
                game.OceanLevel = 0;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    game.Id = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return game;
        }

        public static int UpdateGameById(Game game)
        {
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("update_game_by_id", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = game.Id;
                cmd.Parameters.Add("@difficulty", SqlDbType.Int).Value = game.Difficulty;
                cmd.Parameters.Add("@generation", SqlDbType.Int).Value = game.Generation;
                cmd.Parameters.Add("@time_remaining", SqlDbType.Int).Value = game.TimeRemaining;
                cmd.Parameters.Add("@is_game_ended", SqlDbType.TinyInt).Value = game.IsGameEnded;
                cmd.Parameters.Add("@oxygen_level", SqlDbType.Int).Value = game.OxygenLevel;
                cmd.Parameters.Add("@temperature_level", SqlDbType.Int).Value = game.TemperatureLevel;
                cmd.Parameters.Add("@ocean_level", SqlDbType.Int).Value = game.OceanLevel;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    game.Id = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return game.Id;
        }

        public static IEnumerable<Game> GetGameById(int gameId)
        {
            List<Game> game = new List<Game>();

            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("get_game_by_id", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = gameId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    int id = (int)res.GetValue(res.GetOrdinal("id"));
                    int difficulty = (int)res.GetValue(res.GetOrdinal("difficulty"));
                    int generation = (int)res.GetValue(res.GetOrdinal("generation"));
                    int time_remaining = (int)res.GetValue(res.GetOrdinal("time_remaining"));
                    byte is_game_ended = (byte)res.GetValue(res.GetOrdinal("is_game_ended"));
                    int oxygen_level = (int)res.GetValue(res.GetOrdinal("oxygen_level"));
                    int temperature_level = (int)res.GetValue(res.GetOrdinal("temperature_level"));
                    int ocean_level = (int)res.GetValue(res.GetOrdinal("ocean_level"));

                    Game result = new Game();

                    result.Id = id;
                    result.Difficulty = difficulty;
                    result.Generation = generation;
                    result.TimeRemaining = time_remaining;
                    result.IsGameEnded = is_game_ended == 1;
                    result.OxygenLevel = oxygen_level;
                    result.TemperatureLevel = temperature_level;
                    result.OceanLevel = ocean_level;

                    game.Add(result);
                }
                res.Close();
            }
            con.Close();

            return game;
        }

        public static int InsertBuilding(Guid userId, Buildings type, int hexagonId, int gameId)
        {
            int buildingId = -1;

            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("insert_building", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar).Value = userId.ToString();
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@hexagon_id", SqlDbType.Int).Value = hexagonId;
                cmd.Parameters.Add("@game_id", SqlDbType.Int).Value = gameId;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    buildingId = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return buildingId;
        }

        public static int InsertPlayer(Player player)
        {
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("insert_player", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@bank_credit", SqlDbType.Int).Value = player.Bank.Credit;
                cmd.Parameters.Add("@bank_metal", SqlDbType.Int).Value = player.Bank.Metal;
                cmd.Parameters.Add("@bank_titan", SqlDbType.Int).Value = player.Bank.Titan;
                cmd.Parameters.Add("@bank_plant", SqlDbType.Int).Value = player.Bank.Plant;
                cmd.Parameters.Add("@bank_energy", SqlDbType.Int).Value = player.Bank.Energy;
                cmd.Parameters.Add("@bank_heat", SqlDbType.Int).Value = player.Bank.Heat;
                cmd.Parameters.Add("@income_credit", SqlDbType.Int).Value = player.Incomes.Credit;
                cmd.Parameters.Add("@income_metal", SqlDbType.Int).Value = player.Incomes.Metal;
                cmd.Parameters.Add("@income_titan", SqlDbType.Int).Value = player.Incomes.Titan;
                cmd.Parameters.Add("@income_plant", SqlDbType.Int).Value = player.Incomes.Plant;
                cmd.Parameters.Add("@income_energy", SqlDbType.Int).Value = player.Incomes.Energy;
                cmd.Parameters.Add("@income_heat", SqlDbType.Int).Value = player.Incomes.Heat;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    player.Id = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return player.Id;
        }

        public static int UpdatePlayerById(Player player)
        {
            SqlConnection con = null;
            SqlDataReader res = null;

            con = new SqlConnection(@"Data Source = localhost; Initial Catalog = TerraformingMars; Integrated Security = SSPI");
            con.Open();
            using (SqlCommand cmd = new SqlCommand("update_player_by_id", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = player.Id;
                cmd.Parameters.Add("@bank_credit", SqlDbType.Int).Value = player.Bank.Credit;
                cmd.Parameters.Add("@bank_metal", SqlDbType.Int).Value = player.Bank.Metal;
                cmd.Parameters.Add("@bank_titan", SqlDbType.Int).Value = player.Bank.Titan;
                cmd.Parameters.Add("@bank_plant", SqlDbType.Int).Value = player.Bank.Plant;
                cmd.Parameters.Add("@bank_energy", SqlDbType.Int).Value = player.Bank.Energy;
                cmd.Parameters.Add("@bank_heat", SqlDbType.Int).Value = player.Bank.Heat;
                cmd.Parameters.Add("@income_credit", SqlDbType.Int).Value = player.Incomes.Credit;
                cmd.Parameters.Add("@income_metal", SqlDbType.Int).Value = player.Incomes.Metal;
                cmd.Parameters.Add("@income_titan", SqlDbType.Int).Value = player.Incomes.Titan;
                cmd.Parameters.Add("@income_plant", SqlDbType.Int).Value = player.Incomes.Plant;
                cmd.Parameters.Add("@income_energy", SqlDbType.Int).Value = player.Incomes.Energy;
                cmd.Parameters.Add("@income_heat", SqlDbType.Int).Value = player.Incomes.Heat;

                res = cmd.ExecuteReader();

                while (res.Read())
                {
                    player.Id = res.GetInt32(0);
                }
                res.Close();
            }
            con.Close();

            return player.Id;
        }
    }
}
