using Draughts.Core.Board;
using Draughts.Core.Board.Enum;
using Draughts.Core.Engine;
using Draughts.Infrastructure.Repositories;

namespace Draughts.Core.Service;

public class GameService : IGameService
{
    private readonly IGameEngine _gameEngine;
    private readonly IGameRepository _gameRepository;

    public GameService(IGameEngine gameEngine, IGameRepository gameRepository)
    {
        _gameEngine = gameEngine;
        _gameRepository = gameRepository;
    }

    public async Task<Game> CreateGame(PlayerColor humanColor, int aiTimeLimit)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            BoardClass = new BoardClass(),
            CurrentTurn = PlayerColor.Black, // Black goes first in English Draughts
            GameStartTime = DateTime.UtcNow,
            LastMoveTime = DateTime.UtcNow,
            HumanPlayerColor = humanColor,
            AiTimeLimit = aiTimeLimit
        };

        await _gameRepository.SaveGame(game);

        // If AI goes first, calculate its move
        if (humanColor != PlayerColor.Black) await GetAiMove(game.Id);

        return game;
    }

    public async Task<Game> GetGame(Guid id)
    {
        return await _gameRepository.GetGame(id);
    }

    public async Task<Game> MakeMove(Guid gameId, Move move)
    {
        var game = await _gameRepository.GetGame(gameId);

        // Validate it's player's turn
        if (game.CurrentTurn != game.HumanPlayerColor)
            throw new InvalidOperationException("Not player's turn");

        // Validate move
        if (!game.BoardClass.IsValidMove(move, game.CurrentTurn))
            throw new InvalidOperationException("Invalid move");

        // Apply move
        game.BoardClass.ApplyMove(move);
        game.LastMoveTime = DateTime.UtcNow;

        // Check for win conditions
        CheckWinConditions(game);

        // Switch turns
        game.CurrentTurn = game.CurrentTurn == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;

        await _gameRepository.SaveGame(game);

        return game;
    }

    public async Task<Move> GetAiMove(Guid gameId)
    {
        var game = await _gameRepository.GetGame(gameId);

        // Validate it's AI's turn
        var aiColor = game.HumanPlayerColor == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;
        if (game.CurrentTurn != aiColor)
            throw new InvalidOperationException("Not AI's turn");

        // Calculate AI move
        var aiMove = await _gameEngine.CalculateBestMove(game.BoardClass, aiColor, game.AiTimeLimit);

        // Apply move
        game.BoardClass.ApplyMove(aiMove);
        game.LastMoveTime = DateTime.UtcNow;

        // Check for win conditions
        CheckWinConditions(game);

        // Switch turns
        game.CurrentTurn = game.CurrentTurn == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;

        await _gameRepository.SaveGame(game);

        return aiMove;
    }

    public async Task<Move> GetHint(Guid gameId)
    {
        var game = await _gameRepository.GetGame(gameId);
        return await _gameEngine.GetHint(game.BoardClass, game.HumanPlayerColor);
    }

    private void CheckWinConditions(Game game)
    {
        // Check if the current player has no valid moves or no pieces left
        var opponent = game.CurrentTurn == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;
        if (game.BoardClass.GetPossibleMoves(opponent).Count == 0) game.Winner = game.CurrentTurn;
    }
}