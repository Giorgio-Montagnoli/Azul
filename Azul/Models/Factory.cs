using System.Collections.Generic;

namespace Azul.Models
{
    public class Factory
    {
        public List<Tile> Tiles { get; set; }

        public Factory()
        {
            Tiles = new List<Tile>();
        }
    }
}