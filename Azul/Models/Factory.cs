using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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