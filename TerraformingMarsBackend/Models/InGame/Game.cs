using System;
using System.Collections.Generic;
using TerraformingMarsBackend.Service;

namespace TerraformingMarsBackend.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int Difficulty { get; set; }
        public int Generation { get; set; }
        public int TimeRemaining { get; set; }
        public bool IsGameEnded { get; set; }

        public int OxygenLevel { get; set; }
        public int TemperatureLevel { get; set; }
        public int OceanLevel { get; set; }

        public DateTime LastHostedTime { get; set; }

        public GameRoom GameRoom
        {
            get => GameDataService.GetGameRoomByGameId(Id);
        }

        public List<Building> Buildings
        {
            get => GameDataService.GetBuildingsByGameId(Id);
        }

        public List<GameEvent> Events
        {
            get => GameDataService.GetGameEventsByGameId(Id);
        }

        [NonSerialized]
        public List<Hexagon> GameBoard = new List<Hexagon>();
    }
}
