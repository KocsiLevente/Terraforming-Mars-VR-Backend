using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TerraformingMarsBackend.Models;
using TerraformingMarsBackend.Service;
using System.Threading;
using System.Collections.Generic;

namespace TerraformingMarsBackend
{
    public class Startup
    {
        public static List<KeyValuePair<int, WebSocket>> ConnectedWebSockets { get; set; } = new List<KeyValuePair<int, WebSocket>>();

        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    Thread.Sleep(100);
                    GameManagementService.Update();
                }
            }).Start();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
            app.UseWebSockets(wsOptions);
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/terraforming_mars/game")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using(WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            await ReceiveMessage(webSocket);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            });
        }

        public async static Task SendJoinMultiplayerLobbyResultMessage(WebSocket webSocket, string outerId, List<TerraformingMarsUser> users, List<ChatMessage> messages, List<GameRoom> gameRooms)
        {
            //Create JSON list from data.
            string end = "]";
            string userData = "[";
            foreach (TerraformingMarsUser u in users)
            {
                if (users.IndexOf(u) == users.Count - 1)
                {
                    userData += u.ToJsonString();
                }
                else
                {
                    userData += u.ToJsonString() + ",";
                }
            }
            userData += end;
            string chatData = "[";
            foreach (ChatMessage cm in messages)
            {
                if (messages.IndexOf(cm) == messages.Count - 1)
                {
                    chatData += cm.ToJsonString();
                }
                else
                {
                    chatData += cm.ToJsonString() + ",";
                }
            }
            chatData += end;
            string gameRoomData = "[";
            foreach (GameRoom g in gameRooms)
            {
                if (gameRooms.IndexOf(g) == gameRooms.Count - 1)
                {
                    gameRoomData += g.ToJsonString();
                }
                else
                {
                    gameRoomData += g.ToJsonString() + ",";
                }
            }
            gameRoomData += end;

            JoinMultiplayerLobbyResult joinLobbyResult = new JoinMultiplayerLobbyResult(outerId, userData, chatData, gameRoomData);
            TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.JoinMultiplayerLobbyResult, JsonSerializer.Serialize(joinLobbyResult));
            string toSend = JsonSerializer.Serialize(terraformingMarsMessage);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(toSend)), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        public async static Task SendJoinGameRoomResultMessage(WebSocket webSocket, bool isSuccess, bool isLeader)
        {
            JoinGameRoomResult joinGameRoomResult = new JoinGameRoomResult(isSuccess, isLeader);
            TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.JoinGameRoomResult, JsonSerializer.Serialize(joinGameRoomResult));
            string toSend = JsonSerializer.Serialize(terraformingMarsMessage);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(toSend)), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        public async static Task SendInvitePlayerRequestMessage(WebSocket webSocket, int gameRoom, TerraformingMarsUser user)
        {
            InvitePlayerRequest invitePlayerRequest = new InvitePlayerRequest(gameRoom);
            TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.InvitePlayerRequest, JsonSerializer.Serialize(invitePlayerRequest));
            string toSend = JsonSerializer.Serialize(terraformingMarsMessage);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(toSend)), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        public async static Task SendStartGameResultMessage(WebSocket webSocket, Game game)
        {
            StartGameResultMessage startGameResult = new StartGameResultMessage(game.Id);
            TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.StartGameResult, JsonSerializer.Serialize(startGameResult));
            string toSend = JsonSerializer.Serialize(terraformingMarsMessage);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(toSend)), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        public async static Task SendGetGameStateResultMessage(WebSocket webSocket, Game game)
        {
            GetGameStateResultMessage getGameStateResult = new GetGameStateResultMessage(game.Id, game.Difficulty, game.Generation, game.TimeRemaining, game.IsGameEnded, game.OxygenLevel, game.TemperatureLevel, game.OceanLevel);
            TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.GetGameStateResult, JsonSerializer.Serialize(getGameStateResult));
            string toSend = JsonSerializer.Serialize(terraformingMarsMessage);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(toSend)), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        public async static Task SendBuyBuildingResultMessage(WebSocket webSocket, int buildingId)
        {
            BuyBuildingResultMessage buyBuildingResult = new BuyBuildingResultMessage(buildingId);
            TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.BuyBuildingResult, JsonSerializer.Serialize(buyBuildingResult));
            string toSend = JsonSerializer.Serialize(terraformingMarsMessage);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(toSend)), WebSocketMessageType.Text, false, CancellationToken.None);
            }
        }

        public async static Task HandleJoinMultiplayerLobbyMessage(WebSocket webSocket, TerraformingMarsMessage messageToHandle)
        {
            //Reading message.
            JoinMultiplayerLobby joinLobby = (JoinMultiplayerLobby)JsonSerializer.Deserialize(messageToHandle.Data, typeof(JoinMultiplayerLobby));

            //Checking outer id if user already exist.
            TerraformingMarsUser user = null;
            try
            {
                user = GameDataService.GetTerraformingMarsUserByOuterId(Guid.Parse(joinLobby.OuterId));
                if (user == null)
                {
                    //Inserting the user into the Database.
                    user = new TerraformingMarsUser(-1, Guid.NewGuid(), joinLobby.Name, -1, -1);
                    GameDataService.InsertTerraformingMarsUser(user);
                }
                else
                {
                    //Update the user in the Database.
                    GameDataService.UpdateTerraformingMarsUser(new TerraformingMarsUser(user.Id, user.OuterId, user.Name, -1, -1));
                }
            }
            catch
            {
                //Inserting the user into the Database.
                user = new TerraformingMarsUser(-1, Guid.NewGuid(), joinLobby.Name, -1, -1);
                GameDataService.InsertTerraformingMarsUser(user);
            }

            //Adding connection.
            ConnectedWebSockets.Add(new KeyValuePair<int, WebSocket>(user.Id, webSocket));

            //Send the result message.
            await SendJoinMultiplayerLobbyResultMessage(webSocket, user.OuterId.ToString(), MultiplayerLobby.OnlineUsers, MultiplayerLobby.ChatMessages, MultiplayerLobby.AvailableGameRooms);
        }

        public static void HandleSendChatMessage(TerraformingMarsMessage messageToHandle)
        {
            //Reading message.
            SendChatMessage sendChatMessage = (SendChatMessage)JsonSerializer.Deserialize(messageToHandle.Data, typeof(SendChatMessage));

            //Creating chat message from websocket message.
            ChatMessage chatMessage = new ChatMessage();
            try
            {
                chatMessage.UserId = Guid.Parse(sendChatMessage.User);
                chatMessage.UserName = GameDataService.GetTerraformingMarsUserNameByOuterId(chatMessage.UserId);
                chatMessage.Message = sendChatMessage.Message;

                //Inserting chat message into the Database.
                GameDataService.InsertChatMessage(chatMessage);
            }
            catch { }
        }

        public static void HandleCreateGameRoomMessage(TerraformingMarsMessage messageToHandle)
        {
            //Reading message.
            CreateGameRoom createGameRoom = (CreateGameRoom)JsonSerializer.Deserialize(messageToHandle.Data, typeof(CreateGameRoom));

            //Creating game room from websocket message.
            GameRoom gameRoom = new GameRoom();
            try
            {
                gameRoom.LeaderUserId = Guid.Parse(createGameRoom.UserId);
                gameRoom.GameId = -1;

                //Inserting game room into the Database and updating the user.
                TerraformingMarsUser userToUpdate = GameDataService.GetTerraformingMarsUserByOuterId(gameRoom.LeaderUserId);
                userToUpdate.GameRoomId = GameDataService.InsertGameRoom(gameRoom);
                GameDataService.UpdateTerraformingMarsUser(userToUpdate);
            }
            catch { }
        }

        public async static void HandleJoinGameRoomMessage(TerraformingMarsMessage messageToHandle)
        {
            //Reading message.
            JoinGameRoom joinGameRoom = (JoinGameRoom)JsonSerializer.Deserialize(messageToHandle.Data, typeof(JoinGameRoom));

            //Setting up properties.
            try
            {
                TerraformingMarsUser user = GameDataService.GetTerraformingMarsUserByOuterId(Guid.Parse(joinGameRoom.UserId));

                if (!GameDataService.IsGameRoomFull(joinGameRoom.GameRoomId))
                {
                    bool isLeader = false;
                    user.GameRoomId = joinGameRoom.GameRoomId;
                    if (GameDataService.GetGameRoomById(joinGameRoom.GameRoomId).JoinedUsers.Count == 0)
                    {
                        isLeader = true;
                        GameDataService.GetGameRoomById(joinGameRoom.GameRoomId).LeaderUserId = user.OuterId;
                    }

                    //Updating the user in the Database.
                    GameDataService.UpdateTerraformingMarsUser(user);

                    //Sending back the result.
                    await SendJoinGameRoomResultMessage(ConnectedWebSockets.SingleOrDefault(ws => ws.Key == user.Id).Value, true, isLeader);
                }
                else
                {
                    //Sending back the result.
                    await SendJoinGameRoomResultMessage(ConnectedWebSockets.SingleOrDefault(ws => ws.Key == user.Id).Value, false, false);
                }
            }
            catch { }
        }

        public async static Task HandleInvitePlayerMessage(TerraformingMarsMessage messageToHandle)
        {
            //Reading message.
            InvitePlayer invitePlayer = (InvitePlayer)JsonSerializer.Deserialize(messageToHandle.Data, typeof(InvitePlayer));

            //Searching for invited user.
            TerraformingMarsUser user = GameDataService.GetTerraformingMarsUserByOuterId(Guid.Parse(invitePlayer.User));

            //Send the request message to the given player.
            await SendInvitePlayerRequestMessage(ConnectedWebSockets.SingleOrDefault(ws => ws.Key == user.Id).Value, invitePlayer.GameRoom, user);
        }

        public async static Task HandleLeaveGameRoomMessage(TerraformingMarsMessage messageToHandle)
        {
            //Reading message.
            LeaveGameRoom leaveGameRoom = (LeaveGameRoom)JsonSerializer.Deserialize(messageToHandle.Data, typeof(LeaveGameRoom));

            //Searching for the user and room.
            TerraformingMarsUser user = GameDataService.GetTerraformingMarsUserByOuterId(Guid.Parse(leaveGameRoom.UserId));
            user.GameRoomId = -1;
            GameRoom room = GameDataService.GetGameRoomById(leaveGameRoom.GameRoomId);
            if (room.LeaderUserId == user.OuterId)
            {
                var users = GameDataService.GetUsersByGameRoomId(leaveGameRoom.GameRoomId);
                if (users != null && users.Count > 0)
                {
                    room.LeaderUserId = users[0].OuterId;
                    //Send message to the new leader.
                    await SendJoinGameRoomResultMessage(ConnectedWebSockets.SingleOrDefault(ws => ws.Key == user.Id).Value, true, true);
                }
            }

            //Updating the user in the Database.
            GameDataService.UpdateTerraformingMarsUser(user);
        }

        public async static Task HandleStartGameMessage(WebSocket webSocket, TerraformingMarsMessage messageToHandle)
        {
            //Insert a game into the Database.
            StartGameMessage startGame = (StartGameMessage)JsonSerializer.Deserialize(messageToHandle.Data, typeof(StartGameMessage));
            Game insertedGame = GameDatabaseService.InsertGame(startGame.Difficulty);

            //Start to manage the game in the background.
            if (!string.IsNullOrEmpty(startGame.FirstUser))
            {
                TerraformingMarsUser user = GameDataService.GetTerraformingMarsUserByOuterId(Guid.Parse(startGame.FirstUser));
                Resource cost;
                switch (insertedGame.Difficulty)
                {
                    case 1:
                        cost = new Resource(40, 0, 0, 0, 0, 0);
                        break;
                    case 2:
                        cost = new Resource(30, 0, 0, 0, 0, 0);
                        break;
                    case 3:
                        cost = new Resource(25, 0, 0, 0, 0, 0);
                        break;
                    default:
                        cost = new Resource(40, 0, 0, 0, 0, 0);
                        break;
                }
                Player player = new Player($"Player {startGame.FirstUser}", cost);
                user.PlayerId = GameDataService.InsertPlayer(player);
                GameDataService.UpdateTerraformingMarsUser(user);
            }
            for (int i = 0; i < 180; i++)
            {
                insertedGame.GameBoard.Add(new Hexagon(i + 1, null));
            }
            insertedGame.LastHostedTime = DateTime.Now;
            ConnectedWebSockets.Add(new KeyValuePair<int, WebSocket>(insertedGame.Id, webSocket));
            GameManagementService.GamesToManage.Add(insertedGame);

            //Send the result message.
            await SendStartGameResultMessage(webSocket, insertedGame);
        }

        public async static Task ReceiveMessage(WebSocket webSocket)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var buffer = new byte[1024 * 4];
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result != null)
                    {
                        string message = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));

                        //For debugging.
                        Console.WriteLine($"Client: {message}");

                        TerraformingMarsMessage messageToHandle = (TerraformingMarsMessage)JsonSerializer.Deserialize(message, typeof(TerraformingMarsMessage));

                        switch (messageToHandle.Type)
                        {
                            case CommunicationType.JoinMultiplayerLobby:
                                await HandleJoinMultiplayerLobbyMessage(webSocket, messageToHandle);
                                break;
                            case CommunicationType.CreateGameRoom:
                                HandleCreateGameRoomMessage(messageToHandle);
                                break;
                            case CommunicationType.JoinGameRoom:
                                HandleJoinGameRoomMessage(messageToHandle);
                                break;
                            case CommunicationType.SendChatMessage:
                                HandleSendChatMessage(messageToHandle);
                                break;
                            case CommunicationType.InvitePlayer:
                                await HandleInvitePlayerMessage(messageToHandle);
                                break;
                            case CommunicationType.LeaveGameRoom:
                                await HandleLeaveGameRoomMessage(messageToHandle);
                                break;
                            case CommunicationType.StartGame:
                                await HandleStartGameMessage(webSocket, messageToHandle);
                                break;
                            case CommunicationType.GetGameState:
                                //Get the game from the Database.
                                GetGameStateMessage getGameState = (GetGameStateMessage)JsonSerializer.Deserialize(messageToHandle.Data, typeof(GetGameStateMessage));
                                Game game = GameDatabaseService.GetGameById(getGameState.GameId).First();

                                //Send the result message.
                                await SendGetGameStateResultMessage(webSocket, game);
                                break;
                            case CommunicationType.BuyBuilding:
                                //Insert building into the Database.
                                BuyBuildingMessage buyBuilding = (BuyBuildingMessage)JsonSerializer.Deserialize(messageToHandle.Data, typeof(BuyBuildingMessage));

                                //Add it to the corresponding Game.
                                int buildingId = -1;
                                foreach (Game g in GameManagementService.GamesToManage)
                                {
                                    if (g.Id == buyBuilding.GameId)
                                    {
                                        foreach (Hexagon h in g.GameBoard)
                                        {
                                            if (h.Id == buyBuilding.HexagonId)
                                            {
                                                if (h.BuildingModel == null)
                                                {
                                                    buildingId = GameDatabaseService.InsertBuilding(Guid.Parse(buyBuilding.User), buyBuilding.Type, buyBuilding.HexagonId, buyBuilding.GameId);
                                                    switch (buyBuilding.Type)
                                                    {
                                                        case Buildings.PowerPlant:
                                                            h.BuildingModel = new PowerPlant(buildingId, h, buyBuilding.Type);
                                                            break;
                                                        case Buildings.Greenery:
                                                            h.BuildingModel = new Greenery(buildingId, h, buyBuilding.Type);
                                                            break;
                                                        case Buildings.City:
                                                            h.BuildingModel = new City(buildingId, h, buyBuilding.Type);
                                                            break;
                                                        case Buildings.Ocean:
                                                            h.BuildingModel = new Ocean(buildingId, h, buyBuilding.Type);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    foreach (KeyValuePair<int, WebSocket> ws in ConnectedWebSockets)
                                                    {
                                                        if (ws.Key == g.Id)
                                                        {
                                                            await SendGetGameStateResultMessage(ws.Value, g);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //Send the result message.
                                await SendBuyBuildingResultMessage(webSocket, buildingId);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //For debugging.
                    Console.WriteLine($"One Websocket got closed due to exception. {ex.Message}");
                }
            }
        }
    }
}
