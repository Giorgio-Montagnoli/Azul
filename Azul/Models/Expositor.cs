using Azul.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Azul.Models
{
    public class Expositor
    {
        internal int _number;

        public int Number
        {
            get
            {
                return _number;
            }
        }

        public List<Tile> Tiles { get; set; } = new List<Tile>();

        public Expositor(int number)
        {
            _number = number;
        }

        public int SelectedByPlayer(TileType typeSelected)
        {
            return Tiles.Count(t => t.Type.Equals(typeSelected));
        }

        public List<Tile> ToCenterPlate(TileType typeSelected)
        {
            var tilesToCenter = Tiles.Where(t => !t.Type.Equals(typeSelected)).ToList();

            Tiles = new List<Tile>();

            return tilesToCenter;
        }
    }
}