using System;

namespace TerraformingMarsBackend.Models
{
    public class Resource
    {
        public double Credit { get; set; }
        public double Metal { get; set; }
        public double Titan { get; set; }
        public double Plant { get; set; }
        public double Energy { get; set; }
        public double Heat { get; set; }

        public Resource(double credit, double metal, double titan, double plant, double energy, double heat)
        {
            Credit = credit;
            Metal = metal;
            Titan = titan;
            Plant = plant;
            Energy = energy;
            Heat = heat;
        }

        //Adding a Resource object to another.
        public void Add(Resource toAdd)
        {
            Credit += toAdd.Credit;
            Metal += toAdd.Metal;
            Titan += toAdd.Titan;
            Plant += toAdd.Plant;
            Energy += toAdd.Energy;
            Heat += toAdd.Heat;
        }
    }
}
