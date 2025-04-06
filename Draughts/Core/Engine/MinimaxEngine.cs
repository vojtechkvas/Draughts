using Draughts.Core.Board;
using Draughts.Core.Board.Enum;
using Draughts.Core.Engine;
using Draughts.Core.Models;

public class MinimaxEngine : IGameEngine
{
    private readonly ParallelOptions _parallelOptions;
    private readonly Random _random = new();

    public MinimaxEngine()
    {
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
    }

    public async Task<Move> CalculateBestMove(BoardClass board, PlayerColor color, int timeLimit)
    {

        var possibleMoves = board.GetPossibleMoves(color);
 
        // Simulate thinking time
        await Task.Delay(Math.Min(timeLimit, 500));

        // For now, just return a random move
        return possibleMoves[_random.Next(possibleMoves.Count)];
    }

    public double EvaluateBoard(BoardClass board, PlayerColor color)
    {
        // Simple evaluation: count pieces
        var score = 0;

        for (var y = 0; y < board.Size; y++)
        for (var x = 0; x < board.Size; x++)
        {
            var piece = board.Grid[x, y];
            if (piece.Type != PieceType.Empty)
            {
                var pieceValue = piece.Type == PieceType.King ? 3 : 1;
                if (piece.Color == color)
                    score += pieceValue;
                else
                    score -= pieceValue;
            }
        }

        return score;
    }

    public async Task<Move> GetHint(BoardClass board, PlayerColor color)
    {
        // For hint, use a shorter time limit
        return await CalculateBestMove(board, color, 1000);
    }
}