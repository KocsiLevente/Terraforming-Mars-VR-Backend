using ClientTest.Models;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClientTest
{
    class Program
    {
        static bool isInMultiplayerLobby = true;
        static bool isJoined = false;
        static bool isInGameRoom = false;
        static bool isInGame = false;
        static bool isDataChanged = false;
        
        static string userOuterId;
        static string userName;
        static string lobbyChoice;
        static List<TerraformingMarsUser> users = new List<TerraformingMarsUser>();
        static List<ChatMessage> messages = new List<ChatMessage>();
        static List<GameRoom> gameRooms = new List<GameRoom>();

        static async Task SetupMultiplayerLobbyOnConsole(ClientWebSocket client, CancellationTokenSource token)
        {
            Console.Clear();

            Console.Write("Available Users: ");
            users.ForEach(u => Console.Write(u.Name + ", "));
            Console.Write("\nAvailable GameRooms: ");
            gameRooms.ForEach(g => Console.Write(g.Id + ", "));
            Console.Write("\nChat:\n");
            messages.ForEach(m => Console.Write(m.TimeSent + " : " + m.Name + " : " + m.Message + "\n"));
            Console.Write("\nLobby actions:\n - 1. Send chat message \"hello\" in lobby\n - 2. Create a GameRoom\n - 3. Join a GameRoom (enter id after action)\n\n");
            Console.Write("Enter your lobby action:\n\n");

            lobbyChoice = Console.ReadLine();

            try
            {
                switch (int.Parse(lobbyChoice))
                {
                    case 1:
                        SendChatMessage sendChatMessage = new SendChatMessage(userOuterId, "hello");
                        TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.SendChatMessage, JsonSerializer.Serialize(sendChatMessage));
                        string messageToSend = JsonSerializer.Serialize(terraformingMarsMessage);

                        //We send the message to the backend.
                        ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
                        await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, token.Token);
                        break;
                    case 2:
                        CreateGameRoom createGameRoom = new CreateGameRoom(userOuterId);
                        terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.CreateGameRoom, JsonSerializer.Serialize(createGameRoom));
                        messageToSend = JsonSerializer.Serialize(terraformingMarsMessage);

                        //We send the message to the backend.
                        byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
                        await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, token.Token);
                        break;
                    case 3:
                        string id = Console.ReadLine();

                        JoinGameRoom joinGameRoom = new JoinGameRoom(userOuterId, int.Parse(id));
                        terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.JoinGameRoom, JsonSerializer.Serialize(joinGameRoom));
                        messageToSend = JsonSerializer.Serialize(terraformingMarsMessage);

                        //We send the message to the backend.
                        byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
                        await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, token.Token);
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        static async Task Main(string[] args)
        {
            //Display test information.
            Console.WriteLine("Press enter to start testing communication with the server.");
            Console.ReadLine();

            using(ClientWebSocket client = new ClientWebSocket())
            {
                //Configure connection.
                Uri serviceUri = new Uri("ws://localhost:5000/terraforming_mars/game");
                var token = new CancellationTokenSource();
                token.CancelAfter(TimeSpan.FromSeconds(120));

                try
                {
                    //Connect.
                    await client.ConnectAsync(serviceUri, token.Token);

                    new Thread(async () =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        while (client.State == WebSocketState.Open)
                        {
                            var responseBuffer = new byte[4048 * 4];
                            var offset = 0;
                            var packet = 4048;

                            //We receive the result from the backend.
                            ArraySegment<byte> byteToReceive = new ArraySegment<byte>(responseBuffer, offset, packet);
                            WebSocketReceiveResult respone = await client.ReceiveAsync(byteToReceive, token.Token);
                            var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, respone.Count);

                            TerraformingMarsMessage messageToHandle = (TerraformingMarsMessage)JsonSerializer.Deserialize(responseMessage, typeof(TerraformingMarsMessage));

                            switch (messageToHandle.Type)
                            {
                                case CommunicationType.JoinMultiplayerLobbyResult:
                                    //Reading message.
                                    JoinMultiplayerLobbyResult joinLobbyResult = (JoinMultiplayerLobbyResult)JsonSerializer.Deserialize(messageToHandle.Data, typeof(JoinMultiplayerLobbyResult));

                                    //We joined the lobby and received the results.
                                    userOuterId = joinLobbyResult.OuterId;
                                    List<TerraformingMarsUser> updatedUser = (List<TerraformingMarsUser>)JsonSerializer.Deserialize(joinLobbyResult.AvailableUserData, typeof(List<TerraformingMarsUser>));
                                    List<ChatMessage> updatedChat = (List<ChatMessage>)JsonSerializer.Deserialize(joinLobbyResult.AvailableChatData, typeof(List<ChatMessage>));
                                    List<GameRoom> updatedGameRoom = (List<GameRoom>)JsonSerializer.Deserialize(joinLobbyResult.AvailableGameRoomData, typeof(List<GameRoom>));
                                    if (users.Count != updatedUser.Count || messages.Count != updatedChat.Count || gameRooms.Count != updatedGameRoom.Count)
                                    {
                                        isDataChanged = true;
                                    }
                                    users.Clear();
                                    messages.Clear();
                                    gameRooms.Clear();
                                    updatedUser.ForEach(users.Add);
                                    updatedChat.ForEach(messages.Add);
                                    updatedGameRoom.ForEach(gameRooms.Add);
                                    break;
                                default:
                                    break;
                            }

                            if (isDataChanged)
                            {
                                if (isInMultiplayerLobby)
                                {
                                    if (isJoined)
                                    {
                                        await SetupMultiplayerLobbyOnConsole(client, token);
                                    }
                                }

                                isDataChanged = false;
                            }
                        }
                    }).Start();

                    while (client.State == WebSocketState.Open)
                    {
                        if (isInMultiplayerLobby)
                        {
                            if (!isJoined)
                            {
                                Console.Write("Enter your user name: ");
                                userName = Console.ReadLine();

                                Console.Write("\nEnter your user identifier (enter if you are a new user): ");
                                string userId = Console.ReadLine();

                                JoinMultiplayerLobby joinMessage = new JoinMultiplayerLobby(userName, userId);
                                TerraformingMarsMessage terraformingMarsMessage = new TerraformingMarsMessage(CommunicationType.JoinMultiplayerLobby, JsonSerializer.Serialize(joinMessage));
                                string messageToSend = JsonSerializer.Serialize(terraformingMarsMessage);

                                //We send the message to the backend.
                                ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
                                await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, token.Token);
                                isJoined = true;
                            }
                        }
                    }
                }
                catch (WebSocketException wex)
                {
                    Console.WriteLine(wex.Message);
                }
            }
        }
    }
}
