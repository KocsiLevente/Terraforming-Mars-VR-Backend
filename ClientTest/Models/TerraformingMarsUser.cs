using System;

namespace ClientTest.Models
{
    public class TerraformingMarsUser
    {
        public string OuterId { get; set; }
        public string Name { get; set; }

        public TerraformingMarsUser(string outerId, string name)
        {
            OuterId = outerId;
            Name = name;
        }
    }
}
