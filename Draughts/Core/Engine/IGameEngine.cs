using Draughts.Core.Board;
using Draughts.Core.Board.Enum;

namespace Draughts.Core.Engine;

public interface IGameEngine
{
    Task<Move> CalculateBestMove(BoardClass boardClass, PlayerColor color, int timeLimit);
    double EvaluateBoard(BoardClass boardClass, PlayerColor color);
    Task<Move> GetHint(BoardClass boardClass, PlayerColor color);
}