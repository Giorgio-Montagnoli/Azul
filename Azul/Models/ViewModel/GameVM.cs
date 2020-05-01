using Azul.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul.Models.ViewModel
{
    public class GameVM
    {
        internal string _id;
        public string Id
        {
            get
            {
                return _id;
            }
        }

        public List<Player> Players { get; set; }
        public List<Tile> TilesBag { get; set; }
        public List<Factory> Factories { get; set; }
        public List<Tile> CenterTable { get; set; }
        public int TurnNo { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }

        public GameVM()
        {
            // Creo la sesssione di gioco
            _id = DateTime.Now.ToString("yyyyMMddHHmmss");

            Players = new List<Player>();

            TilesBag = Utility.TilesBag;
            ShuffleTiles();
            Factories = new List<Factory>();
            CenterTable = new List<Tile>();
            CenterTable.Add(new Tile { Type = TileType.Player1 });
            TurnNo = 1;
        }

        public void SetupFactories()
        {
            if (Started.HasValue && Players.Count > 1)
            {
                var FactoriesNum = 2 * Players.Count + 1;

                for (var i = 0; i < FactoriesNum; i++)
                {
                    Factories.Add(new Factory { Tiles = TilesBag.Take(4).ToList() });
                    TilesBag.RemoveRange(0, 4);
                }
            }
        }

        public bool Start()
        {
            if (!Started.HasValue && Players.Any())
            {
                Started = DateTime.Now;
                SetupFactories();

                return true;
            }

            return false;
        }

        void ShuffleTiles()
        {
            for (var i = 0; i < 3; i++)
            {
                TilesBag = TilesBag.OrderBy(q => Guid.NewGuid()).ToList();
            }
        }
    }
}