using System;

namespace ClientTest.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int Difficulty { get; set; }
        public int Generation { get; set; }
        public int TimeRemaining { get; set; }
        public bool IsGameEnded { get; set; }
        public int Score { get; set; }

        public int OxygenLevel { get; set; }
        public int TemperatureLevel { get; set; }
        public int OceanLevel { get; set; }
    }
}
