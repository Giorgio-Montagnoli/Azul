using Azul.Helpers;

namespace Azul.Models
{
    public class Tile
    {
        internal TileType _type;
        public TileType Type
        {
            get
            {
                return _type;
            }
        }

        public string Color
        {
            get
            {
                switch (Type)
                {
                    case TileType.Black:
                        return "Black";
                    case TileType.Blue:
                        return "Blue";
                    case TileType.Ice:
                        return "Ice";
                    case TileType.Red:
                        return "Red";
                    case TileType.Yellow:
                        return "Yellow";
                    case TileType.Player1:
                        return "Player1";
                    default:
                        return string.Empty;
                }
            }
        }
        public string ImgUrl
        {
            get
            {
                switch (Type)
                {
                    case TileType.Black:
                        return "/Content/images/Black.png";
                    case TileType.Blue:
                        return "/Content/images/Blue.png";
                    case TileType.Ice:
                        return "/Content/images/Ice.png";
                    case TileType.Red:
                        return "/Content/images/Red.png";
                    case TileType.Yellow:
                        return "/Content/images/Yellow.png";
                    case TileType.Player1:
                        return "/Content/images/Player1.png";
                    default:
                        return string.Empty;
                }
            }
        }

        public Tile(TileType type)
        {
            _type = type;
        }
    }
}