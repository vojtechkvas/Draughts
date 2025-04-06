using Draughts.Core.Board;

namespace Draughts.Infrastructure.Repositories;

public interface IGameRepository
{
    Task<Game> GetGame(Guid id);
    Task SaveGame(Game game);
    Task<List<Game>> GetAllGames();
    Task DeleteGame(Guid id);
}

public class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<Guid, Game> _games = new();

    public Task<Game> GetGame(Guid id)
    {
        if (_games.TryGetValue(id, out var game))
            return Task.FromResult(game);

        throw new KeyNotFoundException("Game not found");
    }

    public Task SaveGame(Game game)
    {
        _games[game.Id] = game;
        return Task.CompletedTask;
    }

    public Task<List<Game>> GetAllGames()
    {
        return Task.FromResult(_games.Values.ToList());
    }

    public Task DeleteGame(Guid id)
    {
        _games.Remove(id);
        return Task.CompletedTask;
    }
}