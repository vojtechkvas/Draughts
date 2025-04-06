using Draughts.Core.Board.Enum;
using Draughts.Core.Models;

namespace Draughts.Core.Board;

public class BoardClass
{
    //  public Piece[] Pieces { get; set; }

    public readonly Piece[,] Grid = new Piece[8, 8];

    public BoardClass()
    {
        Initialize();
    }

    //  public Piece[] Pieces { get; set; }
    //  public HashSet<Piece> Pieces = new HashSet<Piece>();
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
        if (piece == null || piece.Color != currentPlayer)
            return false;

        // Check if destination is empty
        if (Grid[move.ToX, move.ToY] != null)
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
        // Simple implementation - in a real game, this would be much more complex
        var moves = new List<Move>();
        var possibleMoves = new List<Move>();
        // Find all pieces of the current player's color
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            Piece piece = Grid[x, y];
            if (piece != null && piece.Color == color)
                // Check for possible moves for this piece
                possibleMoves =  GetMovesForPiece(x, y);
                moves.AddRange(possibleMoves);
        }

        return moves;
    }

    private List<Move> GetMovesForPiece(int x, int y)
    {
        var piece = Grid[x, y];

        List<Move> moves = new List<Move>();

        if (piece == null || piece.Type == PieceType.Empty)
            return moves;
        if (piece.Type == PieceType.King)
        {
            int[] dx = { -1, 1, -1, 1 };
            int[] dy = { -1, -1, 1, 1 };

            for (int i = 0; i < 4; i++)
            {
                GetSlidingMoves(x, y, dx[i], dy[i], piece.Color, moves);
                GetSlidingJumps(x, y, dx[i], dy[i], piece.Color, moves);
            }
            
            
            
        }else if (piece.Type == PieceType.Man)
        {
            
            int forwardDirection = (piece.Color == PlayerColor.White) ? -1 : 1;
            // Regular man moves (forward diagonals)
            int[] dx = { -1, 1 };
            int[] dy = { forwardDirection, forwardDirection };

            for (int i = 0; i < 2; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsValidPosition(newX, newY) && Grid[newX, newY].Type == PieceType.Empty)
                {
                    moves.Add(new Move (  x,  y,  newX,  newY  ));
                }

                // Check for jumps (forward diagonals)
                int jumpX = x + 2 * dx[i];
                int jumpY = y + 2 * dy[i];
                int jumpedX = x + dx[i];
                int jumpedY = y + dy[i];

                if (IsValidPosition(jumpX, jumpY) && Grid[jumpX, jumpY].Type == PieceType.Empty &&
                    IsValidPosition(jumpedX, jumpedY) && Grid[jumpedX, jumpedY] .Type != PieceType.Empty &&
                    Grid[jumpedX, jumpedY].Color != piece.Color)
                {
                    moves.Add(new Move
                     ( 
                         x,
                         y,
                         jumpX,
                         jumpY,
                         Grid[jumpedX, jumpedY]
                     ));
                }
            }
            
        }
        
        return moves;

        
    }
    
    
    private void GetSlidingMoves(int startX, int startY, int deltaX, int deltaY, PlayerColor color, List<Move> moves)
    {
        int currentX = startX + deltaX;
        int currentY = startY + deltaY;

        while (IsValidPosition(currentX, currentY))
        {
            if (Grid[currentX, currentY].Type == PieceType.Empty)
            {
                moves.Add(new Move( startX,  startY,  currentX,  currentY ));
                currentX += deltaX;
                currentY += deltaY;
            }

        }
    }

    private void GetSlidingJumps(int startX, int startY, int deltaX, int deltaY, PlayerColor color, List<Move> moves)
    {
        int jumpOverX = startX + deltaX;
        int jumpOverY = startY + deltaY;
        int landingX = startX + 2 * deltaX;
        int landingY = startY + 2 * deltaY;

        if (IsValidPosition(jumpOverX, jumpOverY) && IsValidPosition(landingX, landingY) &&
            Grid[jumpOverX, jumpOverY].Type != PieceType.Empty && Grid[jumpOverX, jumpOverY].Color != color &&
            Grid[landingX, landingY].Type ==  PieceType.Empty)
        {
            moves.Add(new Move
            (
                 startX,
                 startY,
                 landingX,
                 landingY,
                 Grid[jumpOverX, jumpOverY]
            ));

            // For multiple jumps, you'd need a recursive or iterative approach here
            // to find all possible sequences of jumps for a king.
            
        }
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }
    private bool IsValidPositionAndEmpty(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size && Grid[x, y].Type == PieceType.Empty; ;
    }
    
    
}