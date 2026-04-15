public enum PieceType
{
    None,
    Red,
    Blue,
    Green,
    White,
    Purple
}

public struct Piece
{
    public PieceType pieceType;

    public Piece(PieceType pieceType)
    {
        this.pieceType = pieceType;
    }
}