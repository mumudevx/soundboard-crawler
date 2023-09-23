using System.Collections.Generic;

namespace Entities
{
    public class Soundboard
    {
        public string Name { get; set; }

        public List<Sound> Sounds { get; set; }

        public string ZipPath { get; set; }

        public string ZipLink { get; set; }
    }
}
