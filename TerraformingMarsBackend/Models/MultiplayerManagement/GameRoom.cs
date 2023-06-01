using System;
using System.Collections.Generic;
using TerraformingMarsBackend.Service;

namespace TerraformingMarsBackend.Models
{
    public class GameRoom
    {
        public int Id { get; set; }
        public Guid LeaderUserId { get; set; }

        public int GameId { get; set; }
        public Game Game
        {
            get => GameDataService.GetGameById(GameId);
        }

        public List<TerraformingMarsUser> JoinedUsers
        {
            get => GameDataService.GetUsersByGameRoomId(Id);
        }

        public List<ChatMessage> ChatMessages
        {
            get => GameDataService.GetChatMessagesForGameRoom(Id);
        }

        public string ToJsonString()
        {
            return "{ \"Id\":" + Id + "}";
        }
    }
}
