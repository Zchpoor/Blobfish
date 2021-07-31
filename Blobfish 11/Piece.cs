namespace Blobfish_11
{
    public enum Piece : byte
    {
        None = 0,
        Pawn = 1,
        Knight = 2,
        Bishop = 3,
        Rook = 4,
        Queen = 5,
        King = 6,
        NonsensePiece = 7,
        White = 8,
    }

    public static class PieceExtensions
    {
        public static bool Is(this Piece piece, Piece property)
        {
            return (piece & property) == property;
        }
        public static bool IsWhite(this Piece piece)
        {
            return (piece & Piece.White) == Piece.White;
        }
        public static bool IsEmpty(this Piece piece)
        {
            return (piece & Piece.None) == Piece.White;
        }
        //public static bool IsHeavy(this Piece piece)
        //{
        //    return (piece & Piece.Rook) == Piece.Rook;
        //}
        public static Piece AsWhite(this Piece piece)
        {
            return piece | Piece.White;
        }
        public static Piece AsBlack(this Piece piece)
        {
            return piece & ~Piece.White;
        }
        public static Piece Apply(this Piece piece, Piece property)
        {
            return piece | property;
        }
        public static Piece SwapColor(this Piece piece)
        {
            return piece ^ Piece.White;
        }
        public static char Name(this Piece piece)
        {
            switch (piece)
            {
                case Piece.Pawn:                  return 'p';
                case Piece.Pawn | Piece.White:    return 'P';
                case Piece.Knight:                return 'n';
                case Piece.Knight | Piece.White:  return 'N';
                case Piece.Bishop:                return 'b';
                case Piece.Bishop | Piece.White:  return 'B';
                case Piece.Rook:                  return 'r';
                case Piece.Rook   | Piece.White:  return 'R';
                case Piece.King:                  return 'k';
                case Piece.King   | Piece.White:  return 'K';
                case Piece.Queen:                 return 'q';
                case Piece.Queen  | Piece.White:  return 'Q';
                default: return ' ';
            }
        }
    }
}