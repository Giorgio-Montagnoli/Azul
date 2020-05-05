using Azul.Helpers;

namespace Azul.Models
{
    public class WallTile : Tile
    {
        private bool _placed;
        public bool Placed
        {
            get
            {
                return _placed;
            }
        }

        public WallTile(TileType type) : base(type) { }

        public void HasBeenPlaced()
        {
            _placed = true;
        }
    }
}