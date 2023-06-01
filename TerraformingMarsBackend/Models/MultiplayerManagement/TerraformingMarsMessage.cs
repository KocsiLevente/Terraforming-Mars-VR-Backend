using System;

namespace TerraformingMarsBackend.Models
{
    public class TerraformingMarsMessage
    {
        public CommunicationType Type { get; set; }
        public string Data { get; set; }

        public TerraformingMarsMessage(CommunicationType type, string data)
        {
            Type = type;
            Data = data;
        }
    }

    public abstract class MessageData
    {
        public MessageData() { }
    }

    //Client sends, Server receives.
    public class JoinMultiplayerLobby : MessageData
    {
        public string Name { get; set; }
        public string OuterId { get; set; }

        public JoinMultiplayerLobby(string name, string outerId)
        {
            Name = name;
            OuterId = outerId;
        }
    }

    //Server sends, Client receives.
    public class JoinMultiplayerLobbyResult : MessageData
    {
        public string OuterId { get; set; }
        public string AvailableUserData { get; set; }
        public string AvailableChatData { get; set; }
        public string AvailableGameRoomData { get; set; }

        public JoinMultiplayerLobbyResult(string outerId, string availableUserData, string availableChatData, string availableGameRoomData)
        {
            OuterId = outerId;
            AvailableUserData = availableUserData;
            AvailableChatData = availableChatData;
            AvailableGameRoomData = availableGameRoomData;
        }
    }

    //Client sends, Server receives.
    public class CreateGameRoom : MessageData
    {
        public string UserId { get; set; }

        public CreateGameRoom(string userId)
        {
            UserId = userId;
        }
    }

    //Client sends, Server receives.
    public class JoinGameRoom : MessageData
    {
        public string UserId { get; set; }
        public int GameRoomId { get; set; }

        public JoinGameRoom(string userId, int gameRoomId)
        {
            UserId = userId;
            GameRoomId = gameRoomId;
        }
    }

    //Client sends, Server receives.
    public class SendChatMessage : MessageData
    {
        public string User { get; set; }
        public string Message { get; set; }

        public SendChatMessage(string user, string message)
        {
            User = user;
            Message = message;
        }
    }

    //Client sends, Server receives.
    public class InvitePlayer : MessageData
    {
        public string User { get; set; }
        public string UserToInvite { get; set; }
        public int GameRoom { get; set; }

        public InvitePlayer(string user, string userToInvite, int gameRoom)
        {
            User = user;
            UserToInvite = userToInvite;
            GameRoom = gameRoom;
        }
    }

    //Server sends, Client receives.
    public class InvitePlayerRequest : MessageData
    {
        public int GameRoom { get; set; }

        public InvitePlayerRequest(int gameRoom)
        {
            GameRoom = gameRoom;
        }
    }

    //Server sends, Client receives.
    public class InvitePlayerResult : MessageData
    {
        public bool Result { get; set; }

        public InvitePlayerResult(bool result)
        {
            Result = result;
        }
    }

    //Client sends, Server receives.
    public class StartGameMessage : MessageData
    {
        public string FirstUser { get; set; }
        public string SecondUser { get; set; }
        public int Difficulty { get; set; } = 1;

        public StartGameMessage(string firstUser, string secondUser, int difficulty = 1)
        {
            FirstUser = firstUser;
            SecondUser = secondUser;
            Difficulty = difficulty;
        }
    }

    //Server sends, Client receives.
    public class StartGameResultMessage : MessageData
    {
        public int GameId { get; set; }

        public StartGameResultMessage(int gameId)
        {
            GameId = gameId;
        }
    }

    //Client sends, Server receives.
    public class GetGameStateMessage : MessageData
    {
        public string User { get; set; }
        public int GameId { get; set; }

        public GetGameStateMessage(string user, int gameId)
        {
            User = user;
            GameId = gameId;
        }
    }

    //Server sends, Client receives.
    public class GetGameStateResultMessage : MessageData
    {
        public int Id { get; set; }
        public int Difficulty { get; set; }
        public int Generation { get; set; }
        public int TimeRemaining { get; set; }
        public bool IsGameEnded { get; set; }

        public int OxygenLevel { get; set; }
        public int TemperatureLevel { get; set; }
        public int OceanLevel { get; set; }

        public GetGameStateResultMessage(int id, int difficulty, int generation, int timeRemaining, bool isGameEnded, int oxygenLevel, int temperatureLevel, int oceanLevel)
        {
            Id = id;
            Difficulty = difficulty;
            Generation = generation;
            TimeRemaining = timeRemaining;
            IsGameEnded = isGameEnded;
            OxygenLevel = oxygenLevel;
            TemperatureLevel = temperatureLevel;
            OceanLevel = oceanLevel;
        }
    }

    //Client sends, Server receives.
    public class BuyBuildingMessage : MessageData
    {
        public string User { get; set; }
        public Buildings Type { get; set; }
        public int HexagonId { get; set; }
        public int GameId { get; set; }

        public BuyBuildingMessage(string user, Buildings type, int hexagonId, int gameId)
        {
            User = user;
            Type = type;
            HexagonId = hexagonId;
            GameId = gameId;
        }
    }

    //Server sends, Client receives.
    public class BuyBuildingResultMessage : MessageData
    {
        public int BuildingId { get; set; }

        public BuyBuildingResultMessage(int buildingId)
        {
            BuildingId = buildingId;
        }
    }

    //Enum for storing the type of Message between cliend and server.
    public enum CommunicationType
    {
        JoinMultiplayerLobby, JoinMultiplayerLobbyResult,
        SendChatMessage, CreateGameRoom, JoinGameRoom, LeaveGameRoom,
        InvitePlayer, InvitePlayerRequest, InvitePlayerResult, KickPlayer,
        StartGame, StartGameResult,
        GetGameState, GetGameStateResult,
        BuyBuilding, BuyBuildingResult,
    }
}
