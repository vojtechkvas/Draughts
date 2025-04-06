using System.Diagnostics;
using Draughts.Core.Board;
using Draughts.Core.Board.Enum;
using Draughts.Core.Engine;
using Draughts.Core.Models;
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
        // Arrange
        var board = new BoardClass();
        // Clear the board

        // Act
        var score = _gameEngine.EvaluateBoard(board, PlayerColor.Black);

        // Assert
        Assert.Equal(0, score);
    }

    [Fact]
    public async Task CalculateBestMove_TimeLimit_RespectsLimit()
    {
        // Arrange
        var board = new BoardClass();
        var timeLimit = 500; // 500ms
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        var move = await _gameEngine.CalculateBestMove(board, PlayerColor.Black, timeLimit);
        stopwatch.Stop();

        // Assert - allow small buffer for task scheduling
        Assert.True(stopwatch.ElapsedMilliseconds < timeLimit + 100);
    }

    [Fact]
    public void IsValidMove_ValidCapture_ReturnsTrue()
    {
        // Arrange
        var board = new BoardClass();
        // Set up board with a capture position

        var move = new Move
        ( 
               2,
              2,
             4,
             4
        );

        // Act
        var isValid = board.IsValidMove(move, PlayerColor.Black);

        // Assert
        Assert.True(isValid);
    }
}