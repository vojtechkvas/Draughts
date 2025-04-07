using Draughts.Core.Board.Enum;

namespace Draughts.Core.Models;

public class Piece
{
    public Piece(PlayerColor color, PieceType type = PieceType.Man)
    {
        Type = type;
        Color = color;
    }

    public PieceType Type { get; set; }

    public PlayerColor Color { get; set; }


    public void Delete()
    {
        Type = PieceType.Empty;
    }
}