using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPieces
{
    abstract class Piece
    {
        abstract public int X { get; set; }
        abstract public int Y { get; set; }
        abstract public int PieceType { get; set; }
    }
}
