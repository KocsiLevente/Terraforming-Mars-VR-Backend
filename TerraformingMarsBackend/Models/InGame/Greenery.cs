using System;

namespace TerraformingMarsBackend.Models
{
    public class Greenery : Building
    {
        public Greenery(int id, Hexagon hexagon, Buildings type) : base(id, hexagon, type)
        {

        }

        //Returning the endscore value of this object.
        public override int GetScore()
        {
            return 2;
        }
    }
}
