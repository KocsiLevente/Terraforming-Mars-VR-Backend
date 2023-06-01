using System;

namespace TerraformingMarsBackend.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int Score { get; set; }

        //Player's resources in the game.
        public Resource Bank { get; set; }

        //Player's incomes in the game.
        public Resource Incomes { get; set; }

        public Player(string name, Resource bank)
        {
            Incomes = new Resource(40, 0, 0, 0, 0, 0);
            Bank = bank;
        }
    }
}
