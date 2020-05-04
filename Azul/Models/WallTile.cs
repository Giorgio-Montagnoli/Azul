using Azul.Helpers;

namespace Azul.Models
{
    public class WallTile
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

        public WallTile(TileType typeAllowed)
        {
            _typeAllowed = typeAllowed;
        }

        public void HasBeenPlaced()
        {
            _placed = true;
        }
    }
}