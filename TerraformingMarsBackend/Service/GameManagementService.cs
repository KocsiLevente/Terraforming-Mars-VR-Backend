using System;
using System.Collections.Generic;
using TerraformingMarsBackend.Models;
using System.Net.WebSockets;
using System.Threading;

namespace TerraformingMarsBackend.Service
{
    public static class GameManagementService
    {
        public static List<Game> GamesToManage { get; set; } = new List<Game>();
        public static List<GameRoom> GameRoomsToManage { get; set; } = new List<GameRoom>();
        private static bool IsInitFinished { get; set; } = false;

        public static void InitGames()
        {
            GamesToManage = GameDataService.GetUnfinishedGames();
            GameRoomsToManage = GameDataService.GetAvailableGameRooms();

            IsInitFinished = true;
        }

        public static async void Update()
        {
            if (!IsInitFinished)
            {
                InitGames();
            }
            else
            {
                foreach (Game game in GamesToManage)
                {
                    if (game.GameRoom.JoinedUsers.Count > 0 && !game.IsGameEnded)
                    {
                        if (DateTime.Now.Subtract(game.LastHostedTime) > new TimeSpan(0, 0, 1))
                        {
                            game.TimeRemaining--;
                            game.LastHostedTime = DateTime.Now;
                            if (game.TimeRemaining == 0)
                            {
                                game.Generation++;
                                if (game.Generation == 14)
                                {
                                    game.IsGameEnded = true;
                                    foreach (Hexagon h in game.GameBoard)
                                    {
                                        if (h.BuildingModel != null)
                                        {
                                            foreach (TerraformingMarsUser user in game.GameRoom.JoinedUsers)
                                            {
                                                if (user.Player != null && user.OuterId == h.BuildingModel.UserId)
                                                {
                                                    user.Player.Score += h.BuildingModel.GetScore();
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    game.TimeRemaining = 60;
                                    foreach (TerraformingMarsUser user in game.GameRoom.JoinedUsers)
                                    {
                                        user.Player.Bank.Add(user.Player.Incomes);
                                        GameDatabaseService.UpdatePlayerById(user, game.Id);
                                    }
                                }
                            }
                            foreach (KeyValuePair<int, WebSocket> ws in Startup.ConnectedWebSockets)
                            {
                                if (ws.Key == game.Id)
                                {
                                    await Startup.SendGetGameStateResultMessage(ws.Value, game);
                                }
                            }
                            GameDatabaseService.UpdateGameById(game);
                        }
                    }
                }

                List<KeyValuePair<int, WebSocket>> toManage = new List<KeyValuePair<int, WebSocket>>();
                Startup.ConnectedWebSockets.ForEach(toManage.Add);
                foreach (KeyValuePair<int, WebSocket> ws in toManage)
                {
                    TerraformingMarsUser user = GameDataService.GetTerraformingMarsUserById(ws.Key);
                    if (user != null)
                    {
                        if (user.GameRoom == null)
                        {
                            await Startup.SendJoinMultiplayerLobbyResultMessage(
                                ws.Value, user.OuterId.ToString(), MultiplayerLobby.OnlineUsers, MultiplayerLobby.ChatMessages, MultiplayerLobby.AvailableGameRooms
                                );
                        }
                        else
                        {
                            await Startup.SendJoinMultiplayerLobbyResultMessage(
                                ws.Value, user.OuterId.ToString(), MultiplayerLobby.OnlineUsers, GameDataService.GetChatMessagesForGameRoom(user.GameRoomId), new List<GameRoom>()
                                );
                        }
                    }
                }
            }
        }
    }
}
