using Azul.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Azul.Models
{
    public class RowToInsert
    {
        internal int _size;
        internal List<TileType> _typesAllowed;

        public int Position
        {
            get
            {
                return _size - 1;
            }
        }

        public TileType? TypeInUse { get; private set; }

        public int Inserted { get; private set; }

        public bool CanBeInserted
        {
            get
            {
                return Inserted.Equals(_size);
            }
        }

        public RowToInsert(int size)
        {
            _size = size;
            _typesAllowed = new List<TileType>
            {
                TileType.Yellow,
                TileType.Red,
                TileType.Ice,
                TileType.Black,
                TileType.Blue
            };
        }

        public int AddTiles(TileType tileType, int number)
        {
            // Se voglio aggiungere tessere di cui ho già messo il colore nellla board o non dello stesso colore di quelle già piazzate nella riga, sono tutte in eccedenza
            if (!_typesAllowed.Contains(tileType) || (TypeInUse.HasValue && !TypeInUse.Value.Equals(tileType)))
            {
                return number;
            }

            // Aggiungo le tessere e setto il tipo
            Inserted += number;
            TypeInUse = tileType;

            // Di base suppongo non ci sia eccedenza
            var exceeding = 0;

            // Se ne ho inserite di più, setto le inserite al massimo e calcolo l'eccedenza
            if (Inserted > _size)
            {
                exceeding = Inserted - _size;

                Inserted = _size;
            }

            // Ritorno l'eccedenza
            return exceeding;
        }

        public List<Tile> Empty()
        {
            if (TypeInUse.HasValue)
            {
                var list = Enumerable.Range(0, _size - 1).Select(q => new Tile(TypeInUse.Value)).ToList();

                _typesAllowed.Remove(TypeInUse.Value);
                TypeInUse = null;

                Inserted = 0;

                return list;
            }

            return new List<Tile>();
        }
    }
}