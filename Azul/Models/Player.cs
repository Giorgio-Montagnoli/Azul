using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azul.Models
{
    public class Player
    {
        public string Name { get; set; }
        public bool IsFirstPlayer { get; set; }
        public int VictoryPoints { get; set; }
        public Tile[] FirstRow { get; set; }
        public Tile[] SecondRow { get; set; }
        public Tile[] ThirdRow { get; set; }
        public Tile[] FourthRow { get; set; }
        public Tile[] FifthRow { get; set; }
        public Tile[,] Board { get; set; }

        public Tile[] FloorRow { get; set; }

        public bool TurnDone { get; set; }

        public Player()
        {
            VictoryPoints = 0;

            FirstRow = new Tile[1];
            SecondRow = new Tile[2];
            ThirdRow  = new Tile[3];
            FourthRow = new Tile[4];
            FifthRow = new Tile[5];

            Board = new Tile[5,5];

            FloorRow = new Tile[7];
        }
    }
}