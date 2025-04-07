using System.Diagnostics;
using Draughts.Core.Board;
using Draughts.Core.Board.Enum;
using Draughts.Core.Engine;
using Xunit;

namespace Draughts.Tests;

public class DraughtsEngineTests
{
    private readonly IGameEngine _gameEngine;

    public DraughtsEngineTests()
    {
        _gameEngine = new MinimaxEngine();
    }

    [Fact]
    public void EvaluateBoard_EmptyBoard_ReturnsZero()
    {
        var board = new BoardClass();

        var score = _gameEngine.EvaluateBoard(board, PlayerColor.Black);

        Assert.Equal(0, score);
    }

    [Fact]
    public async Task CalculateBestMove_TimeLimit_RespectsLimit()
    {
        var board = new BoardClass();
        var timeLimit = 500;
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var move = await _gameEngine.CalculateBestMove(board, PlayerColor.Black, timeLimit);
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < timeLimit + 100);
    }

    [Fact]
    public void IsValidMove_ValidCapture_ReturnsTrue()
    {
        var board = new BoardClass();

        var move = new Move
        (
            2,
            2,
            4,
            4
        );

        var isValid = board.IsValidMove(move, PlayerColor.Black);

        Assert.False(isValid);
    }
}