using Draughts.Core.Board.Enum;

namespace Draughts.Core.Board;

public class Game
{
    public Game(BoardClass boardClass)
    {
        BoardClass = boardClass;
    }

    public Game()
    {
        BoardClass = new BoardClass();
    }

    public Guid Id { get; set; }
    public BoardClass BoardClass { get; set; }
    public PlayerColor CurrentTurn { get; set; }
    public DateTime GameStartTime { get; set; }
    public DateTime LastMoveTime { get; set; }
    public PlayerColor? Winner { get; set; }
    public int AiTimeLimit { get; set; } // Time limit for AI in milliseconds
    public PlayerColor HumanPlayerColor { get; set; }
}