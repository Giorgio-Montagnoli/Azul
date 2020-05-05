using Azul.Helpers;

namespace Azul.Models
{
    public class WallTile : Tile
    {
        internal TileType _typeAllowed;
        public TileType TypeAllowed
        {
            get
            {
                return _typeAllowed;
            }
        }

        private bool _placed;
        public bool Placed
        {
            get
            {
                return _placed;
            }
        }

        public WallTile(TileType typeAllowed) : base(typeAllowed)
        {
            _typeAllowed = typeAllowed;
        }

        public void HasBeenPlaced()
        {
            _placed = true;
        }
    }
}