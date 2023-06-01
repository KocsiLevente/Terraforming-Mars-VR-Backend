using System;
using TerraformingMarsBackend.Service;

namespace TerraformingMarsBackend.Models
{
    public class GameEvent
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game
        {
            get => GameDataService.GetGameById(GameId);
        }

        public Guid UserId { get; set; }
        public TerraformingMarsUser User
        {
            get => GameDataService.GetTerraformingMarsUserByOuterId(UserId);
        }
    }
}
