using Azul.Helpers;
using Newtonsoft.Json;
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

        public List<Player> Players { get; set; } = new List<Player>();
        public List<Tile> TilesBag { get; set; } = new List<Tile>();
        public List<Expositor> Expositors { get; set; } = new List<Expositor>();
        public List<Tile> CenterTable { get; set; } = new List<Tile>();
        public List<Tile> CommonReserve { get; set; } = new List<Tile>();
        public int RoundNo { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }

        [JsonIgnore]
        public bool FirstTakeFromCenterTable
        {
            get
            {
                return CenterTable.Any(tile => tile.Type.Equals(TileType.Player1));
            }
        }

        public GameVM()
        {
            // Creo la sesssione di gioco
            _id = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public bool Start()
        {
            if (!Started.HasValue && Players.Count > 1)
            {
                Started = DateTime.Now;

                RoundNo = 1;

                // Scelgo un primo giocatore a caso
                var firstPlayer = Players.OrderBy(q => Guid.NewGuid()).First();

                firstPlayer.IsFirstPlayer = true;
                firstPlayer.IsMyTurn = true;

                var tiles = new List<TileType>
                {
                    TileType.Blue,
                    TileType.Yellow,
                    TileType.Red,
                    TileType.Black,
                    TileType.Ice
                };

                // Tutte le tile nel sacco e mischiare
                foreach (var tile in tiles)
                {
                    TilesBag.AddRange(Enumerable.Range(0, 20).Select(x => new Tile(tile)));
                }

                for (var i = 0; i < 3; i++)
                {
                    TilesBag = TilesBag.OrderBy(q => Guid.NewGuid()).ToList();
                }

                // Setup del tavolo
                SetupTable(firstPlayer.Name);

                return true;
            }

            return false;
        }

        public void TakeFromExpositor(string playerName, int expositorNumber, int rowNumber, TileType typeSelected)
        {
            var player = Players.FirstOrDefault(p => p.Name.Equals(playerName, StringComparison.InvariantCultureIgnoreCase));

            if (player.IsMyTurn)
            {
                var expositor = Expositors.FirstOrDefault(e => e.Number.Equals(expositorNumber));

                player.AddTilesToRow(rowNumber, typeSelected, expositor.SelectedByPlayer(typeSelected), false);

                CenterTable.AddRange(expositor.ToCenterPlate(typeSelected));

                TurnDone(player);
            }
        }

        public void TakeFromCenterTable(string playerName, int rowNumber, TileType typeSelected)
        {
            var player = Players.FirstOrDefault(p => p.Name.Equals(playerName, StringComparison.InvariantCultureIgnoreCase));

            if (player.IsMyTurn)
            {
                player.AddTilesToRow(rowNumber, typeSelected, CenterTable.Count(t => t.Type.Equals(typeSelected)), FirstTakeFromCenterTable);

                foreach (var tile in CenterTable.Where(t => t.Type.Equals(typeSelected)).ToList())
                {
                    CenterTable.Remove(tile);
                }

                CenterTable.RemoveAll(t => t.Type.Equals(TileType.Player1));

                TurnDone(player);
            }
        }

        void SetupTable(string playerName)
        {
            // Mescolo le tile perché sono state passate al gioco lato client e devono essere nuovamente randomizzate
            for (var k = 0; k < 3; k++)
            {
                TilesBag = TilesBag.OrderBy(q => Guid.NewGuid()).ToList();
            }

            // Creo gli espositori
            Expositors = new List<Expositor>();

            var numbers = 2 * Players.Count + 1;

            for (var i = 0; i < numbers; i++)
            {
                var tiles = TilesBag.Take(4).ToList();

                if (tiles.Count < 4)
                {
                    // Metto la riserva nel sacchetto e lo rimescolo
                    TilesBag = CommonReserve;

                    for (var k = 0; k < 3; k++)
                    {
                        TilesBag = TilesBag.OrderBy(q => Guid.NewGuid()).ToList();
                    }

                    // Svuoto la riserva
                    CommonReserve = new List<Tile>();
                }

                Expositors.Add(new Expositor(i + 1) { Tiles = tiles });
                TilesBag.RemoveRange(0, 4);
            }

            // Solo la tile primo giocatore al centro
            CenterTable = new List<Tile>
            {
                new Tile(TileType.Player1)
            };

            // Imposto l'ordine di gioco
            Players = Players
                        .SkipWhile(q => !q.IsFirstPlayer)
                        .Union(Players.TakeWhile(q => !q.IsFirstPlayer))
                        .ToList();
        }

        void TurnDone(Player player)
        {
            // Se ci sono ancora espositori con tile o tile a centro tavola
            if (Expositors.Any(e => e.Tiles.Any()) || CenterTable.Any())
            {
                var nextPlayer = Players.Skip(Players.IndexOf(player) + 1).FirstOrDefault();

                if (nextPlayer == null)
                {
                    nextPlayer = Players.FirstOrDefault();
                }

                // Next Player
                nextPlayer.IsMyTurn = true;
            }
            // Se non ci sono più tile da prendere...
            else
            {
                // Tutti i giocatori fanno lo score di fine round
                Players.ForEach(p => p.UpdateBoard());

                // Se almeno un giocatore ha completato una riga
                if (Players.Any(p => p.HasCompletedARow()))
                {
                    // Punti di fine partita
                    Players.ForEach(p => p.EndGameVictoryPoints());

                    Ended = DateTime.Now;
                }
                // Se si può ancora giocare...
                else
                {
                    var firstPlayer = Players.First(p => p.IsFirstPlayer);

                    firstPlayer.IsMyTurn = true;

                    SetupTable(firstPlayer.Name);

                    RoundNo++;
                }
            }
        }
    }
}