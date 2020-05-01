using Azul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azul.Helpers
{
    public static class Utility
    {
        public const string COOKIE_GAME_ID = "gameId";
        public const string COOKIE_GAME_NAME = "gameName";
        public static List<Tile> TilesBag
        {
            get
            {
                return CreateTilesBag();
            }
        }

        private static List<Tile> CreateTilesBag()
        {
            var ret = new List<Tile>();

            for (int i = 0; i < 20; i++)
            {
                ret.Add(new Tile { Type = TileType.Yellow });
                ret.Add(new Tile { Type = TileType.Red });
                ret.Add(new Tile { Type = TileType.Ice });
                ret.Add(new Tile { Type = TileType.Black });
                ret.Add(new Tile { Type = TileType.Blue });
            }

            return ret;
        }
    }
}