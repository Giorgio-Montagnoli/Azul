﻿using Azul.Helpers;

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
                        return "black";
                    case TileType.Blue:
                        return "blue";
                    case TileType.Ice:
                        return "ice";
                    case TileType.Red:
                        return "red";
                    case TileType.Yellow:
                        return "yellow";
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
                        return "/Content/images/black.png";
                    case TileType.Blue:
                        return "/Content/images/blue.png";
                    case TileType.Ice:
                        return "/Content/images/ice.png";
                    case TileType.Red:
                        return "/Content/images/red.png";
                    case TileType.Yellow:
                        return "/Content/images/yellow.png";
                    case TileType.Player1:
                        return "/Content/images/player1.png";
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