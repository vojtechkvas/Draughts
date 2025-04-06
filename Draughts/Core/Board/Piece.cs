using Draughts.Core.Board.Enum;

namespace Draughts.Core.Models;




public class Piece
{
    public Piece(PlayerColor color /*, int x, int y*/, PieceType type = PieceType.Man)
    {
        Type = type;
        Color = color;
        //  X = x;
        //  Y = y;
    }

    public PieceType Type { get; set; }

    public PlayerColor Color { get; set; }
    //   public int X { get; set; }
    //   public int Y { get; set; }

    public void Delete()
    {
        Type = PieceType.Empty;
    }
}