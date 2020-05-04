using Azul.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul.Models
{
    public class Player
    {
        public string Name { get; set; }

        public List<List<WallTile>> Wall { get; set; } = new List<List<WallTile>>();
        public List<RowToInsert> Rows { get; set; } = new List<RowToInsert>();
        public List<Tile> FloorRow { get; set; } = new List<Tile>();

        public bool IsFirstPlayer { get; set; }
        public int VictoryPoints { get; set; }

        public bool IsMyTurn { get; set; }

        public Player()
        {
            for (var i = 0; i < 5; i++)
            {
                Rows.Add(new RowToInsert(i + 1));
            }

            var typesAllowed = new List<TileType>
            {
                TileType.Blue,
                TileType.Yellow,
                TileType.Red,
                TileType.Black,
                TileType.Ice
            };

            for (var i = 0; i < 5; i++)
            {
                Wall.Add(typesAllowed.Select(type => new WallTile(type)).ToList());
                typesAllowed.Insert(0, typesAllowed.Last());
                typesAllowed.RemoveAt(typesAllowed.Count - 1);
            }
        }

        public void AddTilesToRow(int rowNumber, TileType tileType, int number, bool isFirstTakeFromCenterTable)
        {
            // Se è il mio turno
            if (IsMyTurn)
            {
                // Trovo la riga richiesta
                var rowToInsert = Rows.FirstOrDefault(q => q.Position.Equals(rowNumber));

                // Aggiungo le tile e calcolo le eccedenze
                var toFloor = rowToInsert.AddTiles(tileType, number);

                // Se è la prima presa, la tile primo giocatore va nel floor
                if (isFirstTakeFromCenterTable)
                {
                    FloorRow.Add(new Tile(TileType.Player1));
                }

                // Aggiungo le eccedenze al Floor
                FloorRow.AddRange(Enumerable.Range(0, toFloor).Select(q => new Tile(tileType)));

                // Segnalo che ho fatto
                IsMyTurn = false;
            }
        }

        public List<Tile> UpdateBoard()
        {
            var returnedTiles = new List<Tile>();

            foreach (var row in Rows)
            {
                // Controllo le righe complete
                if (row.CanBeInserted)
                {
                    // Trovo la riga/colonna corrispondente nella board
                    var tileOnBoard = Wall.ElementAt(row.Position).FirstOrDefault(q => q.TypeAllowed.Equals(row.TypeInUse.Value));

                    // Metto la tessera nella board
                    tileOnBoard.HasBeenPlaced();

                    // Rimetto le tessere nel sacchetto
                    returnedTiles.AddRange(row.Empty());

                    // Conto i punti per la tessera
                    // Aggiungo le tessere adicenti orizzontalmente e verticalmente
                    var colPosition = Wall.ElementAt(row.Position).IndexOf(tileOnBoard);

                    var points = NumAdjacent(Wall.ElementAt(row.Position), colPosition);

                    points += NumAdjacent(Wall.SelectMany(q => q.Select((r, col) => new { r, col }).Where(r => r.col.Equals(colPosition))).Select(q => q.r).ToList(), row.Position);

                    if (points.Equals(0))
                    {
                        points = 1;
                    }

                    VictoryPoints += points;
                }
            }

            // Controllo se divento primo giocatore
            IsFirstPlayer = FloorRow.Any(t => t.Type.Equals(TileType.Player1));

            // Segno le penalità
            if (FloorRow.Any())
            {
                var penalty = 14;

                switch (FloorRow.Count)
                {
                    case 1: penalty = 1; break;
                    case 2: penalty = 2; break;
                    case 3: penalty = 4; break;
                    case 4: penalty = 6; break;
                    case 5: penalty = 8; break;
                    case 6: penalty = 11; break;
                }

                VictoryPoints = Math.Max(0, VictoryPoints - penalty);

                // Rimetto le tessere penalità nel sacchetto
                returnedTiles.AddRange(FloorRow.Where(t => t.Type.Equals(TileType.Player1)));
                FloorRow = new List<Tile>();
            }

            return returnedTiles;
        }

        public void EndGameVictoryPoints()
        {
            // 2 Punti per ogni riga della Parete completa.
            VictoryPoints += 2 * Wall.Count(row => row.All(tile => tile.Placed));

            // 7 Punti per ogni colonna della Parete completa.
            VictoryPoints += 7 * Wall.SelectMany(row => row.Select((r, col) => new { r.Placed, col }).Where(r => r.Placed)).GroupBy(r => r.col).Count(g => g.Count().Equals(5));

            // 10 Punti per ogni colore di cui sono state posizionate tutte e 5 le Piastrelle nella Parete.
            VictoryPoints += 10 * Wall.SelectMany(row => row.Where(r => r.Placed).Select(r => r.TypeAllowed)).GroupBy(r => r).Count(g => g.Count().Equals(5));
        }

        public bool HasCompletedARow()
        {
            return Wall.Any(row => row.All(tile => tile.Placed));
        }

        int NumAdjacent(List<WallTile> list, int startPosition)
        {
            var count = 0;

            for (var i = startPosition - 1; i > -1; i--)
            {
                if (list.ElementAt(i).Placed)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            for (var i = startPosition + 1; i < list.Count; i++)
            {
                if (list.ElementAt(i).Placed)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count > 0)
            {
                count++;
            }

            return count;
        }
    }
}