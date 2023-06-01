using System;

namespace TerraformingMarsBackend.Models
{
    public class PowerPlant : Building
    {
        public PowerPlant(int id, Hexagon hexagon, Buildings type) : base(id, hexagon, type)
        {

        }

        //Returning the endscore value of this object.
        public override int GetScore()
        {
            return 1;
        }
    }
}
