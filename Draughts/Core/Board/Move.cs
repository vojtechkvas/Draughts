using Draughts.Core.Models;

namespace Draughts.Core.Board;

public class Move
{
    public Move(int fromX, int fromY, int toX, int toY, Piece capturedPiece)
    {
        FromX = fromX;
        FromY = fromY;
        ToX = toX;
        ToY = toY;
        CapturedPieces.Add(capturedPiece);
    }

    public Move(int fromX, int fromY, int toX, int toY, List<Piece> capturedPieces)
    {
        FromX = fromX;
        FromY = fromY;
        ToX = toX;
        ToY = toY;
        CapturedPieces.AddRange(capturedPieces);
    }

    public Move(int fromX, int fromY, int toX, int toY)
    {
        FromX = fromX;
        FromY = fromY;
        ToX = toX;
        ToY = toY;
    }

    public int FromX { get; set; }
    public int FromY { get; set; }
    public int ToX { get; set; }
    public int ToY { get; set; }
    public List<Piece> CapturedPieces { get; set; } = new();
}