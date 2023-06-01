using System;
using TerraformingMarsBackend.Service;

namespace TerraformingMarsBackend.Models
{
    public class TerraformingMarsUser
    {
        public int Id { get; set; }
        public Guid OuterId { get; set; }
        public string Name { get; set; }

        public int PlayerId { get; set; }
        public Player Player
        {
            get => GameDataService.GetPlayerById(PlayerId);
        }

        public int GameRoomId { get; set; }
        public GameRoom GameRoom
        {
            get => GameDataService.GetGameRoomById(GameRoomId);
        }

        public string ToJsonString()
        {
            return "{ \"OuterId\": \"" + OuterId.ToString() + "\", \"Name\": \"" + Name + "\"}";
        }

        public TerraformingMarsUser(int id, Guid outerId, string name, int playerId, int gameRoomId)
        {
            Id = id;
            OuterId = outerId;
            Name = name;
            PlayerId = playerId;
            GameRoomId = gameRoomId;
        }
    }
}
