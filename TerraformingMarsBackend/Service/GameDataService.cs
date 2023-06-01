using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using TerraformingMarsBackend.Models;

namespace TerraformingMarsBackend.Service
{
    public static class GameDataService
    {
        private static List<TerraformingMarsUser> Users { get; set; } = GameDatabaseService.GetTerraformingMarsUsers(0).ToList();
        private static List<GameRoom> GameRooms { get; set; } = GameDatabaseService.GetGameRooms(0).ToList();
        private static List<ChatMessage> ChatMessages { get; set; } = GameDatabaseService.GetChatMessages(0).ToList();
        private static List<Game> Games { get; set; } = new List<Game>();
        private static List<Player> Players { get; set; } = new List<Player>();
        private static List<Building> Buildings { get; set; } = new List<Building>();
        private static List<GameEvent> GameEvents { get; set; } = new List<GameEvent>();

        //TerraformingMarsUser data handling.
        public static TerraformingMarsUser GetTerraformingMarsUserById(int userId)
        {
            return Users.SingleOrDefault(u => u.Id == userId);
        }

        public static TerraformingMarsUser GetTerraformingMarsUserByOuterId(Guid outerId)
        {
            return Users.SingleOrDefault(u => u.OuterId == outerId);
        }

        public static List<TerraformingMarsUser> GetAvailableUsers()
        {
            return Users.Where(u => u.GameRoomId == -1 && Startup.ConnectedWebSockets.Count(ws => ws.Key == u.Id) == 1).ToList();
        }

        public static List<TerraformingMarsUser> GetUsersByGameRoomId(int gameRoomId)
        {
            return Users.Where(u => u.GameRoomId == gameRoomId).ToList();
        }

        public static string GetTerraformingMarsUserNameByOuterId(Guid outerId)
        {
            TerraformingMarsUser user = Users.SingleOrDefault(u => u.OuterId == outerId);
            if (user != null)
            {
                return user.Name;
            }
            return "USER NOT FOUND";
        }

        public static bool InsertTerraformingMarsUser(TerraformingMarsUser user)
        {
            user.Id = GameDatabaseService.InsertTerraformingMarsUser(user);
            if (user.Id > 0)
            {
                Users.Add(user);
                return true;
            }
            return false;
        }

        public static bool UpdateTerraformingMarsUser(TerraformingMarsUser user)
        {
            user.Id = GameDatabaseService.UpdateTerraformingMarsUser(user);
            if (user.Id > 0)
            {
                TerraformingMarsUser userToFind = Users.SingleOrDefault(u => u.Id == user.Id);
                int idx = Users.IndexOf(userToFind);
                Users[idx] = user;
                return true;
            }
            return false;
        }

        //GameRoom data handling.
        public static GameRoom GetGameRoomById(int gameRoomId)
        {
            return GameRooms.SingleOrDefault(g => g.Id == gameRoomId);
        }

        public static GameRoom GetGameRoomByGameId(int gameId)
        {
            return GameRooms.SingleOrDefault(g => g.GameId == gameId);
        }

        public static List<GameRoom> GetAvailableGameRooms()
        {
            return GameRooms.Where(g => g.GameId == -1).ToList();
        }

        public static bool IsGameRoomFull(int gameRoomId)
        {
            return Users.Count(u => u.GameRoomId == gameRoomId) < 5;
        }

        public static int InsertGameRoom(GameRoom gameRoom)
        {
            gameRoom.Id = GameDatabaseService.InsertGameRoom(gameRoom);
            if (gameRoom.Id > 0)
            {
                GameRooms.Add(gameRoom);
                return gameRoom.Id;
            }
            return -1;
        }

        //ChatMessage data handling.
        public static ChatMessage GetChatMessageById(int messageId)
        {
            return ChatMessages.SingleOrDefault(m => m.Id == messageId);
        }

        public static List<ChatMessage> GetLobbyChatMessages()
        {
            return ChatMessages.Where(m => m.IsLobbyMessage).ToList();
        }

        public static List<ChatMessage> GetChatMessagesForGameRoom(int gameRoomId)
        {
            return ChatMessages.Where(m => m.GameRoomId == gameRoomId).ToList();
        }

        public static bool InsertChatMessage(ChatMessage cm)
        {
            TerraformingMarsUser user = GetTerraformingMarsUserByOuterId(cm.UserId);
            if (user != null)
            {
                cm.IsLobbyMessage = user.GameRoom == null;
                cm.GameRoomId = user.GameRoomId;
                cm.TimeSent = DateTime.Now;
                cm.Id = GameDatabaseService.InsertChatMessage(cm);
                if (cm.Id > 0) {
                    ChatMessages.Add(cm);
                    return true;
                }
            }
            return false;
        }

        //Game data handling.
        public static Game GetGameById(int gameId)
        {
            return Games.SingleOrDefault(g => g.Id == gameId);
        }

        public static List<Game> GetUnfinishedGames()
        {
            return Games.Where(g => !g.IsGameEnded).ToList();
        }

        //Player data handling.
        public static Player GetPlayerById(int playerId)
        {
            return Players.SingleOrDefault(p => p.Id == playerId);
        }

        //Building data handling.
        public static Building GetBuildingById(int buildingId)
        {
            return Buildings.SingleOrDefault(b => b.Id == buildingId);
        }

        public static List<Building> GetBuildingsByGameId(int gameId)
        {
            return Buildings.Where(b => b.GameId == gameId).ToList();
        }

        public static List<Building> GetBuildingsByUserAndGameId(Guid userId, int gameId)
        {
            return Buildings.Where(b => b.UserId == userId && b.GameId == gameId).ToList();
        }

        //GameEvents data handling.
        public static GameEvent GetGameEventById(int gameEventId)
        {
            return GameEvents.SingleOrDefault(e => e.Id == gameEventId);
        }

        public static List<GameEvent> GetGameEventsByGameId(int gameId)
        {
            return GameEvents.Where(e => e.GameId == gameId).ToList();
        }

        public static List<GameEvent> GetGameEventsByUserAndGameId(Guid userId, int gameId)
        {
            return GameEvents.Where(e => e.UserId == userId && e.GameId == gameId).ToList();
        }
    }
}
