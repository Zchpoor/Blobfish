using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public static class PieceMovementHandler
    {
        public static List<Move> AllValidMoves(Position pos, bool sorted)
        {
            //Public för att kunna användas av testerna.
            List<Move> allMoves = new List<Move>();
            Square currentSquare = new Square();
            for (sbyte i = 0; i < 8; i++)
            {
                currentSquare.rank = i;
                for (sbyte j = 0; j < 8; j++)
                {
                    currentSquare.line = j;
                    PieceCalculation(pos, currentSquare, allMoves);
                }
            }

            Position newPos;
            for (int i = 0; i < allMoves.Count; i++)
            {
                //Tar bort alla ogiltiga drag
                newPos = allMoves[i].execute(pos);

                if (newPos.whiteToMove && IsControlledBy(newPos, newPos.kingPositions[0], true))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }
                else if (!newPos.whiteToMove && IsControlledBy(newPos, newPos.kingPositions[1], false))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }

            }
            if (sorted) return SortByEstQuality(allMoves, pos);
            else return allMoves;
        }
        private static void PieceCalculation(Position pos, Square pieceSquare, List<Move> allMoves)
        {
            Piece pieceChar = pos.board[pieceSquare.rank, pieceSquare.line];
            if (pieceChar == Piece.None)
                return;
            bool pieceIsWhite = pieceChar.IsWhite();
            if (pieceIsWhite != pos.whiteToMove)
                return;
            pieceChar = pieceChar.AsBlack();

            Piece pieceOnCurrentSquare;
            void addMoveIfValid(Square currentSquare)
            {
                pieceOnCurrentSquare = pos[currentSquare];
                if (pieceOnCurrentSquare == Piece.None || pieceOnCurrentSquare.IsWhite() != pieceIsWhite)
                {
                    allMoves.Add(new Move(pieceSquare, currentSquare));
                }
            }
            switch (pieceChar)
            {
                case Piece.None: return;
                case Piece.Pawn: AddPawnMovesToPossibleMoves(pos, pieceSquare, pieceIsWhite, allMoves); break;
                case Piece.Knight: foreach (Square square in KnightSquares(pieceSquare))
                    {
                        addMoveIfValid(square);
                    }; 
                    break;
                case Piece.Bishop:
                    foreach (Square square in BishopSquares(pos, pieceSquare))
                    {
                        addMoveIfValid(square);
                    };
                    break;
                case Piece.Rook:
                    foreach (Square square in RookSquares(pos, pieceSquare))
                    {
                        addMoveIfValid(square);
                    };
                    break;
                case Piece.Queen:
                    foreach (Square square in QueenSquares(pos, pieceSquare))
                    {
                        addMoveIfValid(square);
                    };
                    break;
                case Piece.King: AddKingMovesToPossibleMoves(pos, pieceSquare, pieceIsWhite, allMoves); break;
                default: return;
            }
        }
        private static List<Move> SortByEstQuality(List<Move> moves, Position pos)
        {
            //Högre placering i arrayen indikerar högre prioritet.
            const int priorities = 6;

            List<Move>[] listArray = new List<Move>[priorities];
            for (int i = 0; i < priorities; i++)
            {
                //Initierar vektorn.
                listArray[i] = new List<Move>();
            }

            foreach (Move currentMove in moves)
            {
                listArray[PriorityOf(currentMove, pos)].Add(currentMove);
            }

            List<Move> sortedList = new List<Move>();
            //Dragen läggs till i "omvänd ordning".
            for (int i = priorities - 1; i >= 0; i--)
            {
                List<Move> currentList = listArray[i];
                if (currentList.Count > 0)
                    sortedList.AddRange(currentList);
            }
            if (sortedList.Count != moves.Count)
                throw new Exception("Fel i dragsorteringsfunktionen.");
            return sortedList;
        }

        private static IEnumerable<Square> KnightSquares(Square pieceSquare)
        {
            Square currentSquare = new Square();
            int newRank, newLine;
            bool validOffset(sbyte rankOffset, sbyte lineOffset)
            {
                newRank = (pieceSquare.rank + rankOffset);
                newLine = (pieceSquare.line + lineOffset);
                if (newLine < 8 && newLine >= 0) // raden har redan kontrollerats.
                {
                    currentSquare.rank = (sbyte)newRank;
                    currentSquare.line = (sbyte)newLine;
                    return true;
                }
                return false;
            }
            if (pieceSquare.rank < 7)
            {
                if (validOffset(1, 2)) yield return currentSquare;
                if (validOffset(1, -2)) yield return currentSquare;

                if (pieceSquare.rank < 6)
                {
                    if (validOffset(2, 1)) yield return currentSquare;
                    if (validOffset(2, -1)) yield return currentSquare;
                }
            }
            if (pieceSquare.rank > 0)
            {
                if (validOffset(-1, 2)) yield return currentSquare;
                if (validOffset(-1, -2)) yield return currentSquare;

                if (pieceSquare.rank > 1)
                {
                    if (validOffset(-2, 1)) yield return currentSquare;
                    if (validOffset(-2, -1)) yield return currentSquare;
                }
            }
        }
        private static IEnumerable<Square> QueenSquares(Position pos, Square pieceSquare)
        {
            foreach (Square square in BishopSquares(pos, pieceSquare))
            {
                yield return square;
            }
            foreach (Square square in RookSquares(pos, pieceSquare))
            {
                yield return square;
            }
        }
        private static IEnumerable<Square> KingSquares(Square pieceSquare)
        {
            Square currentSquare = new Square();
            for (sbyte i = -1; i <= 1; i++)
            {
                for (sbyte j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    currentSquare.rank = (sbyte)(pieceSquare.rank + i);
                    currentSquare.line = (sbyte)(pieceSquare.line + j);
                    if (ValidSquare(currentSquare))
                    {
                        yield return currentSquare;
                    }
                }
            }
        }
        private static IEnumerable<Square> RookSquares(Position pos, Square pieceSquare)
        {
            foreach (Square square in LineSquares(pos, pieceSquare, 0, 1))
            {
                yield return square;
            }
            foreach (Square square in LineSquares(pos, pieceSquare, 0, -1))
            {
                yield return square;
            }
            foreach (Square square in LineSquares(pos, pieceSquare, 1, 0))
            {
                yield return square;
            }
            foreach (Square square in LineSquares(pos, pieceSquare, -1, 0))
            {
                yield return square;
            }
        }
        private static IEnumerable<Square> BishopSquares(Position pos, Square pieceSquare)
        {
            foreach (Square square in LineSquares(pos, pieceSquare, -1, -1))
            {
                yield return square;
            }
            foreach (Square square in LineSquares(pos, pieceSquare, -1, 1))
            {
                yield return square;
            }
            foreach (Square square in LineSquares(pos, pieceSquare, 1, -1))
            {
                yield return square;
            }
            foreach (Square square in LineSquares(pos, pieceSquare, 1, 1))
            {
                yield return square;
            }
        }
        private static IEnumerable<Square> LineSquares(Position pos, Square pieceSquare, sbyte rankOffset, sbyte lineOffset)
        {
            //Generell funktion som itererar över en linje/diagonal så länge det är giltiga fält.
            pieceSquare.rank += rankOffset;
            pieceSquare.line += lineOffset;

            while (ValidSquare(pieceSquare))
            {
                yield return pieceSquare;
                Piece pieceOnCurrentSquare = pos[pieceSquare];
                if (pieceOnCurrentSquare != Piece.None) //Ett fält där en pjäs står.
                {
                    break;
                }
                pieceSquare.rank += rankOffset;
                pieceSquare.line += lineOffset;
            }
        }

        private static void AddPawnMovesToPossibleMoves(Position pos, Square pieceSquare, bool pieceIsWhite, List<Move> possibleMoves)
        {
            void addPromotions(Square fromSquare, Square toSquare)
            {
                Piece[] allPromotions = pieceIsWhite ?
                    new[]
                    {
                        Piece.Queen.AsWhite(),
                        Piece.Rook.AsWhite(),
                        Piece.Bishop.AsWhite(),
                        Piece.Knight.AsWhite(),
                    } :
                    new[]
                    {
                        Piece.Queen,
                        Piece.Rook,
                        Piece.Bishop,
                        Piece.Knight,
                    };
                foreach (Piece piece in allPromotions)
                {
                    possibleMoves.Add(new Promotion(fromSquare, toSquare, piece));
                }
            }

            sbyte moveDirection = pieceIsWhite ? (sbyte)-1 : (sbyte)1;
            Square currentSquare = new Square(pieceSquare.rank + moveDirection, pieceSquare.line);
            if (ValidSquare(currentSquare))
            {
                Piece pieceOnCurrentSquare = pos[currentSquare];
                if (pieceOnCurrentSquare == Piece.None)
                {

                    sbyte promotionRank = pieceIsWhite ? (sbyte)0 : (sbyte)7;
                    if (currentSquare.rank == promotionRank)//Promotering.
                    {
                        addPromotions(pieceSquare, currentSquare);
                    }
                    else
                    {
                        //Vanligt bondedrag, ett steg framåt.
                        possibleMoves.Add(new Move(pieceSquare, currentSquare));
                        sbyte startingRank = pieceIsWhite ? (sbyte)6 : (sbyte)1;
                        if (pieceSquare.rank == startingRank)
                        {
                            currentSquare.rank = (sbyte)(pieceSquare.rank + (moveDirection * 2));
                            pieceOnCurrentSquare = pos[currentSquare];
                            if (pieceOnCurrentSquare == Piece.None)
                            {
                                //Två steg.
                                possibleMoves.Add(new Move(pieceSquare, currentSquare));
                            }
                        }
                    }
                }
            }
            for (sbyte i = -1; i <= 1; i += 2) //Kommer bli -1 och 1.
            {
                currentSquare.rank = (sbyte)(pieceSquare.rank + moveDirection);
                currentSquare.line = (sbyte)(pieceSquare.line + i);
                if (ValidSquare(currentSquare))
                {
                    sbyte promotionRank = pieceIsWhite ? (sbyte)0 : (sbyte)7;
                    Piece pieceOnCurrentSquare = pos[currentSquare];
                    if (pieceOnCurrentSquare .IsWhite() != pieceIsWhite && pieceOnCurrentSquare != Piece.None) //Om den är av motsatt färg.
                    {
                        if (currentSquare.rank == promotionRank) //Promotering.
                        {
                            addPromotions(pieceSquare, currentSquare);
                        }
                        else
                        {
                            possibleMoves.Add(new Move(pieceSquare, currentSquare)); //Slag
                        }
                    }
                    else if (currentSquare.rank == pos.enPassantSquare.rank && currentSquare.line == pos.enPassantSquare.line)
                    {
                        Square pawnToRemove = new Square(pieceSquare.rank, currentSquare.line);
                        possibleMoves.Add(new EnPassant(pieceSquare, currentSquare, pawnToRemove)); //En passant
                    }

                }
            }
        }
        private static void AddKingMovesToPossibleMoves(Position pos, Square pieceSquare, bool pieceIsWhite, List<Move> possibleMoves)
        {
            Piece pieceOnCurrentSquare;
            foreach (Square square in KingSquares(pieceSquare))
            {
                pieceOnCurrentSquare = pos[square];
                if (pieceOnCurrentSquare == Piece.None || pieceOnCurrentSquare.IsWhite() != pieceIsWhite)
                {
                    possibleMoves.Add(new Move(pieceSquare, square));
                }
            }

            sbyte castlingRank = pieceIsWhite ? (sbyte)7 : (sbyte)0;
            if (pieceSquare.rank == castlingRank && pieceSquare.line == 4 && !IsControlledBy(pos, pieceSquare, !pieceIsWhite))
            {
                //Kungen står på ett fält där rockad skulle kunna vara möjligt.
                Piece correctRook = pieceIsWhite ? Piece.Rook.AsWhite() : Piece.Rook;
                sbyte castlingRightOffset = pieceIsWhite ? (sbyte)0 : (sbyte)2;
                if (pos.board[castlingRank, 0] == correctRook && pos.castlingRights[castlingRightOffset + 1])
                {
                    //Lång rockad
                    Square rookTo = new Square(castlingRank, (sbyte)3);
                    Square kingTo = new Square(castlingRank, (sbyte)2);
                    if (pos[rookTo] == Piece.None && pos[kingTo] == Piece.None &&
                        pos.board[kingTo.rank, kingTo.line - 1] == Piece.None && !IsControlledBy(pos, rookTo, !pieceIsWhite) &&
                        !IsControlledBy(pos, kingTo, !pieceIsWhite && !IsControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, (sbyte)0);
                        possibleMoves.Insert(0, new Castle(pieceSquare, kingTo, rookFrom, rookTo));
                    }
                }
                if (pos.board[castlingRank, 7] == correctRook && pos.castlingRights[castlingRightOffset])
                {
                    //Kort rockad
                    Square rookTo = new Square(castlingRank, (sbyte)5);
                    Square kingTo = new Square(castlingRank, (sbyte)6);
                    if (pos[rookTo] == Piece.None && pos[kingTo] == Piece.None &&
                        !IsControlledBy(pos, rookTo, !pieceIsWhite) && !IsControlledBy(pos, kingTo, !pieceIsWhite &&
                        !IsControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, (sbyte)7);
                        possibleMoves.Insert(0, new Castle(pieceSquare, kingTo, rookFrom, rookTo));
                    }
                }
            }
        }

        private static bool ValidSquare(Square square)
        {
            return (square.rank < 8) && (square.rank >= 0) && (square.line < 8) && (square.line >= 0);
        }
        private static int PriorityOf(Move move, Position pos)
        {
            Piece toPiece = pos[move.to];
            Piece fromPiece = pos[move.from];
            if (toPiece != Piece.None) //Slag
            {
                int valueOfCapturedPiece = RoughValueOf(toPiece);
                int valueDiff = valueOfCapturedPiece - RoughValueOf(fromPiece);

                if (valueDiff <= 0)
                    return 0;
                else if (valueDiff <= 2)
                    return 2;
                else if (valueDiff <= 4)
                    return 3;
                else if (valueDiff <= 6)
                    return 4;
                else
                    return 5;
            }
            else //Ej slag
            {
                if (move is Castle)
                {
                    return 1;
                }
                else if (move is Promotion && (move as Promotion).promoteTo.Is(Piece.Queen))
                {
                    return 4;
                }
                else
                {
                    int secondLastRow = pos.whiteToMove ? 1 : 6;
                    if ((fromPiece == Piece.Pawn || fromPiece == Piece.Pawn.AsWhite()) && move.to.rank == secondLastRow)
                    {
                        return 1;
                    }
                    else
                    {
                        //TODO: Ytterligare uppdelning här.
                        return 0;
                    }
                }
            }
        }
        private static int RoughValueOf(Piece piece)
        {
            if (piece == Piece.None) return 0;
            piece = piece.AsBlack();
            switch (piece)
            {
                case Piece.Pawn: return 1;
                case Piece.Knight: return 3;
                case Piece.Bishop: return 3;
                case Piece.Rook: return 5;
                case Piece.Queen: return 9;
                default: return 0;
            }
        }
        private static bool IsWhite(char piece)
        {
            return piece <= 'Z';
        }
        private static bool IsControlledBy(Position pos, Square relevantSquare, bool byWhite)
        {
            //Tab (\t) används om inget annat tecken skall sökas efter.
            Piece pieceOnCurrentSquare;
            Piece[] piecesToLookFor = byWhite ?
                new Piece[] { Piece.Bishop.AsWhite(), Piece.Queen.AsWhite() } :
                new Piece[] { Piece.Bishop, Piece.Queen };
            bool CheckIfPieceToLookFor(Square currentSquare)
            {
                pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (piecesToLookFor[0] == pieceOnCurrentSquare || piecesToLookFor[1] == pieceOnCurrentSquare)
                {
                    return true;
                }
                return false;
            }

            foreach (Square square in BishopSquares(pos, relevantSquare))
            {
                if (CheckIfPieceToLookFor(square))
                {
                    return true;
                }
            }

            piecesToLookFor[0] = byWhite ? Piece.Rook.AsWhite() : Piece.Rook;
            foreach (Square square in RookSquares(pos, relevantSquare))
            {
                if (CheckIfPieceToLookFor(square))
                {
                    return true;
                }
            }

            piecesToLookFor[0] = byWhite ? Piece.King.AsWhite() : Piece.King;;
            piecesToLookFor[1] = Piece.NonsensePiece; //Kommer aldrig återfinnas.
            foreach (Square square in KingSquares(relevantSquare))
            {
                if (CheckIfPieceToLookFor(square))
                {
                    return true;
                }
            }

            piecesToLookFor[0] = byWhite ? Piece.Knight.AsWhite() : Piece.Knight; ;
            foreach (Square square in KnightSquares(relevantSquare))
            {
                if (CheckIfPieceToLookFor(square))
                {
                    return true;
                }
            }

            //Bönder
            sbyte riktning = byWhite ? (sbyte)1 : (sbyte)-1;
            Piece pawnToLookFor = byWhite ? Piece.Pawn.AsWhite() : Piece.Pawn; ;
            Square currentPawnSquare;
            currentPawnSquare.rank = (sbyte)(relevantSquare.rank + riktning);
            for (sbyte i = -1; i <= 1; i += 2)
            {
                currentPawnSquare.line = (sbyte)(relevantSquare.line + i);
                if (ValidSquare(currentPawnSquare))
                {
                    if (pos[currentPawnSquare] == pawnToLookFor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
/* Kategorisering:
 *   5. +8
 *   4. +6, promotering till dam
 *   3. +4
 *   2. +2
 *   1. Rockad, bonde till 7:de raden
 *   0. Övrigt
 */
