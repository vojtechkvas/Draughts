using Draughts.Core.Board;
using Draughts.Core.Board.Enum;
using Draughts.Core.Models;

namespace Draughts.Core.Service;

public interface IGameService
{
    Task<Game> CreateGame(PlayerColor humanColor = PlayerColor.Black, int aiTimeLimit = 10_000);
    Task<Game> GetGame(Guid id);
    Task<Game> MakeMove(Guid gameId, Move move);
    Task<Move> GetAiMove(Guid gameId);
    Task<Move> GetHint(Guid gameId);
}