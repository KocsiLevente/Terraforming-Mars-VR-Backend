using System;
using TerraformingMarsBackend.Service;

namespace TerraformingMarsBackend.Models
{
    public class Building
    {
        public int Id { get; set; }
        public Buildings Type { get; set; }

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

        //The Hexagon under the Building.
        public Hexagon Hexagon { get; set; }

        public Building(int id, Hexagon hexagon, Buildings type)
        {
            Id = id;
            Hexagon = hexagon;
            Type = type;
        }

        //Virtual endscore funcion that will be overridden by the extended classes.
        public virtual int GetScore() { return 0; }
    }

    //Enum for storing the type of Building.
    public enum Buildings
    {
        PowerPlant = 0, Greenery = 1, City = 2, Ocean = 3
    }
}
