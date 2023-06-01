using System;
using System.Collections.Generic;

namespace TerraformingMarsBackend.Models
{
    public class Hexagon
    {
        public int Id { get; set; }
        public Dictionary<HexagonDirections, int> Neighbours { get; set; } = new Dictionary<HexagonDirections, int>();
        public Building BuildingModel { get; set; }

        public Hexagon(int id, Building buildingModel = null)
        {
            Id = id;
            BuildingModel = buildingModel;

            //Default empty Neighbours;
            Neighbours.Add(HexagonDirections.Left, -1);
            Neighbours.Add(HexagonDirections.Right, -1);
            Neighbours.Add(HexagonDirections.UpLeft, -1);
            Neighbours.Add(HexagonDirections.UpRight, -1);
            Neighbours.Add(HexagonDirections.DownLeft, -1);
            Neighbours.Add(HexagonDirections.DownRight, -1);
        }

        //Setting the neighbour of a Hexagon.
        public void SetNeighbour(HexagonDirections dir, Hexagon hexagon)
        {
            if (hexagon != null)
            {
                Neighbours[dir] = hexagon.Id;
            }
        }
    }

    //Enum for directions of a Hexagon.
    public enum HexagonDirections
    {
        Left = 0, Right = 1, UpLeft = 2, UpRight = 3, DownLeft = 4, DownRight = 5
    }
}
