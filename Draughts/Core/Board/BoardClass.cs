using Draughts.Core.Board.Enum;
using Draughts.Core.Models;

namespace Draughts.Core.Board;

public class BoardClass
{
    public readonly Piece[,] Grid = new Piece[8, 8];

    public BoardClass()
    {
        Initialize();
    }

    public int Size { get; } = 8; // Standard 8x8 board

    public void Initialize()
    {
        var Rows = Size;
        var Columns = Size;


        // Initialize board with starting pieces
        for (var row = 0; row < Rows; row++)
        for (var col = 0; col < Columns; col++)
            Grid[row, col] = new Piece(PlayerColor.Black, PieceType.Empty);

        // Place Black pieces (usually at the top)
        for (var row = 0; row < 3; row++)
        for (var col = 0; col < Columns; col++)
            // Place a piece on the black squares
            if ((row + col) % 2 == 1) // Black squares have row + col as odd
                Grid[row, col] = new Piece(PlayerColor.Black);

        // Place White pieces (usually at the bottom)
        for (var row = Rows - 3; row < Rows; row++)
        for (var col = 0; col < Columns; col++)
            // Place a piece on the black squares
            if ((row + col) % 2 == 1) // Black squares have row + col as odd
                Grid[row, col] = new Piece(PlayerColor.White);
    }

    public bool IsValidMove(Move move, PlayerColor currentPlayer)
    {
        if (move.FromX < 0 || move.FromY < 0 || move.ToX < 0 || move.ToY < 0 ||
            move.FromX >= Size || move.FromY >= Size || move.ToX >= Size || move.ToY >= Size)
            return false;

        // Check if source has a piece of current player's color
        var piece = Grid[move.FromX, move.FromY];
        if (piece.Color != currentPlayer)
            return false;

        // Check if destination is empty
        if (Grid[move.ToX, move.ToY].Type != PieceType.Empty)
            return false;

        return true;
    }

    public void ApplyMove(Move move)
    {
        var piece = Grid[move.FromX, move.FromY];
        Grid[move.FromX, move.FromY] = new Piece(PlayerColor.Black, PieceType.Empty);

        Grid[move.ToX, move.ToY] = piece;

        foreach (var capturedPiece in move.CapturedPieces) capturedPiece.Delete();

        // Check for king promotion
        if (piece.Type == PieceType.Man)
            if ((piece.Color == PlayerColor.White && move.ToY == 0) ||
                (piece.Color == PlayerColor.Black && move.ToY == Size - 1))
                piece.Type = PieceType.King;
    }

    public List<Move> GetPossibleMoves(PlayerColor color)
    {
        var moves = new List<Move>();
        var possibleMoves = new List<Move>();

        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            var piece = Grid[x, y];
            if (piece.Type != PieceType.Empty && piece.Color == color)
                possibleMoves = GetMovesForPiece(x, y);
            moves.AddRange(possibleMoves);
        }

        return moves;
    }

    private List<Move> GetMovesForPiece(int x, int y)
    {
        var piece = Grid[x, y];

        var moves = new List<Move>();

        if (piece.Type == PieceType.Empty)
            return moves;

        if (piece.Type == PieceType.King)
        {
            int[] dx = { -1, 1, -1, 1 };
            int[] dy = { -1, -1, 1, 1 };

            for (var i = 0; i < 4; i++)
            {
                GetSlidingMoves(x, y, dx[i], dy[i], piece.Color, moves);
                GetSlidingJumps(x, y, dx[i], dy[i], piece.Color, moves);
            }
        }
        else if (piece.Type == PieceType.Man)
        {
            var forwardDirection = piece.Color == PlayerColor.White ? -1 : 1;
            // Regular man moves (forward diagonals)
            int[] dx = { -1, 1 };
            int[] dy = { forwardDirection, forwardDirection };

            for (var i = 0; i < 2; i++)
            {
                var newX = x + dx[i];
                var newY = y + dy[i];

                if (IsValidPosition(newX, newY) && Grid[newX, newY].Type == PieceType.Empty)
                    moves.Add(new Move(x, y, newX, newY));

                // Check for jumps (forward diagonals)
                var jumpX = x + 2 * dx[i];
                var jumpY = y + 2 * dy[i];
                var jumpedX = x + dx[i];
                var jumpedY = y + dy[i];

                if (IsValidPosition(jumpX, jumpY) && Grid[jumpX, jumpY].Type == PieceType.Empty &&
                    IsValidPosition(jumpedX, jumpedY) && Grid[jumpedX, jumpedY].Type != PieceType.Empty &&
                    Grid[jumpedX, jumpedY].Color != piece.Color)
                    moves.Add(new Move
                    (
                        x,
                        y,
                        jumpX,
                        jumpY,
                        Grid[jumpedX, jumpedY]
                    ));
                // TODO multiple JUMPS
            }
        }

        return moves;
    }


    private void GetSlidingMoves(int startX, int startY, int deltaX, int deltaY, PlayerColor color, List<Move> moves)
    {
        var currentX = startX + deltaX;
        var currentY = startY + deltaY;

        while (IsValidPosition(currentX, currentY))
            if (Grid[currentX, currentY].Type == PieceType.Empty)
            {
                moves.Add(new Move(startX, startY, currentX, currentY));
                currentX += deltaX;
                currentY += deltaY;
            }
    }

    private void GetSlidingJumps(int startX, int startY, int deltaX, int deltaY, PlayerColor color, List<Move> moves)
    {
        var jumpOverX = startX + deltaX;
        var jumpOverY = startY + deltaY;
        var landingX = startX + 2 * deltaX;
        var landingY = startY + 2 * deltaY;

        if (IsValidPosition(jumpOverX, jumpOverY) && IsValidPosition(landingX, landingY) &&
            Grid[jumpOverX, jumpOverY].Type != PieceType.Empty && Grid[jumpOverX, jumpOverY].Color != color &&
            Grid[landingX, landingY].Type == PieceType.Empty)
            moves.Add(new Move
            (
                startX,
                startY,
                landingX,
                landingY,
                Grid[jumpOverX, jumpOverY]
            ));
        // TODO multiple JUMPS
        // For multiple jumps, you'd need a recursive or iterative approach here
        // to find all possible sequences of jumps for a king.
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }

    private bool IsValidPositionAndEmpty(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size && Grid[x, y].Type == PieceType.Empty;
    }
}