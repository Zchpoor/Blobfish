using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public partial class Engine
    {
        public delegate void functionByPiece(Square square);

        public List<Move> allValidMoves(Position pos, bool sorted)
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
                    pieceCalculation(pos, currentSquare, allMoves);
                }
            }

            Position newPos;
            for (int i = 0; i < allMoves.Count; i++)
            {
                //Tar bort alla ogiltiga drag
                newPos = allMoves[i].execute(pos);

                if (newPos.whiteToMove && isControlledBy(newPos, newPos.kingPositions[0], true))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }
                else if (!newPos.whiteToMove && isControlledBy(newPos, newPos.kingPositions[1], false))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }

            }
            if(sorted) return sortByEstQuality(allMoves, pos);
            else return allMoves;
        }
        private void pieceCalculation(Position pos, Square pieceSquare, List<Move> allMoves)
        {
            Piece pieceChar = pos[pieceSquare];
            bool pieceIsWhite = pieceChar.IsWhite();
            if (pieceIsWhite != pos.whiteToMove)
                return;
            if (pieceChar == Piece.None) return;

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
            functionByPiece amiv = new functionByPiece(addMoveIfValid);
            switch (pieceChar)
            {
                case Piece.None: return;
                case Piece.Pawn: pawnMoves(pos, pieceSquare, pieceIsWhite, allMoves); break;
                case Piece.Knight: foreachKnightSquare(pos, pieceSquare, amiv); break;
                case Piece.Bishop: foreachBishopSquare(pos, pieceSquare, amiv); break;
                case Piece.Rook: foreachRookSquare(pos, pieceSquare, amiv); break;
                case Piece.Queen:
                    foreachBishopSquare(pos, pieceSquare, amiv);
                    foreachRookSquare(pos, pieceSquare, amiv); break;
                case Piece.King: kingMoves(pos, pieceSquare, pieceIsWhite, allMoves, amiv); break;
                default: return;
            }
        }
        private void pawnMoves(Position pos, Square pieceSquare, bool pieceIsWhite, List<Move> possibleMoves)
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
                foreach (Piece tkn in allPromotions)
                {
                    possibleMoves.Add(new Promotion(fromSquare, toSquare, tkn));
                }
            }

            sbyte moveDirection = pieceIsWhite ? (sbyte)-1 : (sbyte)1;
            Square currentSquare = new Square(pieceSquare.rank + moveDirection, pieceSquare.line);
            if (validSquare(currentSquare))
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
                            pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
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
                if (validSquare(currentSquare))
                {
                    sbyte promotionRank = pieceIsWhite ? (sbyte)0 : (sbyte)7;
                    Piece pieceOnCurrentSquare = pos[currentSquare];
                    if (pieceOnCurrentSquare.IsWhite() != pieceIsWhite && pieceOnCurrentSquare != Piece.None) //Om den är av motsatt färg.
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
        private void kingMoves(Position pos, Square pieceSquare, bool pieceIsWhite, List<Move> possibleMoves, functionByPiece addMoveIfValid)
        {
            foreachKingSquare(pos, pieceSquare, addMoveIfValid);

            sbyte castlingRank = pieceIsWhite ? (sbyte) 7: (sbyte) 0;
            if (pieceSquare.rank == castlingRank && pieceSquare.line == 4 && !isControlledBy(pos, pieceSquare, !pieceIsWhite))
            {
                //Kungen står på ett fält där rockad skulle kunna vara möjligt.
                Piece correctRook = Piece.Rook;
                if (pieceIsWhite) correctRook = correctRook.AsWhite();

                sbyte castlingRightOffset = pieceIsWhite ? (sbyte) 0: (sbyte) 2;
                if (pos.board[castlingRank, 0] == correctRook && pos.castlingRights[castlingRightOffset + 1])
                {
                    //Lång rockad
                    Square rookTo = new Square(castlingRank, (sbyte) 3);
                    Square kingTo = new Square(castlingRank, (sbyte) 2);
                    if (pos[rookTo] == Piece.None && pos[kingTo] == Piece.None &&
                        pos.board[kingTo.rank, kingTo.line - 1] == Piece.None && !isControlledBy(pos, rookTo, !pieceIsWhite) &&
                        !isControlledBy(pos, kingTo, !pieceIsWhite && !isControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, (sbyte) 0);
                        possibleMoves.Insert(0, new Castle(pieceSquare, kingTo, rookFrom, rookTo));
                    }
                }
                if (pos.board[castlingRank, 7] == correctRook && pos.castlingRights[castlingRightOffset])
                {
                    //Kort rockad
                    Square rookTo = new Square(castlingRank, (sbyte) 5);
                    Square kingTo = new Square(castlingRank, (sbyte) 6);
                    if (pos[rookTo] == Piece.None && pos[kingTo] == Piece.None &&
                        !isControlledBy(pos, rookTo, !pieceIsWhite) && !isControlledBy(pos, kingTo, !pieceIsWhite &&
                        !isControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, (sbyte) 7);
                        possibleMoves.Insert(0, new Castle(pieceSquare, kingTo, rookFrom, rookTo));
                    }
                }
            }
        }

        //Public för att kunna användas i Move. Bör hitta smidigare lösning för detta.
        public static void foreachKnightSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            Square currentSquare = new Square();
            int newRank, newLine;
            void callbackByOffset(sbyte rankOffset, sbyte lineOffset)
            {
                newRank = (pieceSquare.rank + rankOffset);
                newLine = (pieceSquare.line + lineOffset);
                if(newLine < 8 && newLine >= 0)
                {
                    currentSquare.rank = (sbyte)newRank;
                    currentSquare.line = (sbyte)newLine;
                    callback(currentSquare);
                }
            }
            if(pieceSquare.rank < 7)
            {
                callbackByOffset(1, 2);
                callbackByOffset(1, -2);

                if(pieceSquare.rank < 6)
                {
                    callbackByOffset(2, 1);
                    callbackByOffset(2, -1);
                }
            }
            if(pieceSquare.rank > 0)
            {
                callbackByOffset(-1, 2);
                callbackByOffset(-1, -2);
                if(pieceSquare.rank > 1)
                {
                    callbackByOffset(-2, 1);
                    callbackByOffset(-2, -1);
                }
            }
        }
        public static void foreachLineSquare(Position pos, Square pieceSquare, sbyte rankOffset, sbyte lineOffset, functionByPiece callback)
        {
            //Generell funktion som itererar över en linje/diagonal så länge det är giltiga fält.
            pieceSquare.rank += rankOffset;
            pieceSquare.line += lineOffset;

            while (validSquare(pieceSquare))
            {
                callback(pieceSquare);
                Piece pieceOnCurrentSquare = pos[pieceSquare];
                if (pieceOnCurrentSquare != Piece.None) //Ett fält där en pjäs står.
                {
                    break;
                }
                pieceSquare.rank += rankOffset;
                pieceSquare.line += lineOffset;
            }
        }
        public static void foreachBishopSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            foreachLineSquare(pos, pieceSquare, -1, -1, callback);
            foreachLineSquare(pos, pieceSquare, -1, 1, callback);
            foreachLineSquare(pos, pieceSquare, 1, -1, callback);
            foreachLineSquare(pos, pieceSquare, 1, 1, callback);
        }
        public static void foreachRookSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            foreachLineSquare(pos, pieceSquare, 0, 1, callback);
            foreachLineSquare(pos, pieceSquare, 0, -1, callback);
            foreachLineSquare(pos, pieceSquare, 1, 0, callback);
            foreachLineSquare(pos, pieceSquare, -1, 0, callback);
        }
        public static void foreachKingSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            Square currentSquare = new Square();
            for (sbyte i = -1; i <= 1; i++)
            {
                for (sbyte j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    currentSquare.rank = (sbyte)(pieceSquare.rank + i);
                    currentSquare.line = (sbyte)(pieceSquare.line + j);
                    if (validSquare(currentSquare))
                    {
                        callback(currentSquare);
                    }
                }
            }
        }

        public static bool validSquare(Square square)
        {
            return (square.rank < 8) && (square.rank >= 0) && (square.line < 8) && (square.line >= 0);
        }
        private bool isWhite(char piece)
        {
            return piece <= 'Z';
        }
        private bool isControlledBy(Position pos, Square relevantSquare, bool byWhite)
        {
            //Tab (\t) används om inget annat tecken skall sökas efter.
            bool isControlled = false;
            Piece pieceOnCurrentSquare;
            Piece[] piecesToLookFor = byWhite ? 
                new Piece[] { Piece.Bishop.AsWhite(), Piece.Queen.AsWhite() } : 
                new Piece[] { Piece.Bishop, Piece.Queen };
            void CheckIfPieceToLookFor(Square currentSquare)
            {
                pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (piecesToLookFor[0] == pieceOnCurrentSquare || piecesToLookFor[1] == pieceOnCurrentSquare)
                    isControlled = true;
            }
            functionByPiece ciptlf = new functionByPiece(CheckIfPieceToLookFor);
            foreachBishopSquare(pos, relevantSquare, ciptlf);
            if (isControlled) return true;

            piecesToLookFor[0] = byWhite ? Piece.Rook.AsWhite() : Piece.Rook;
            foreachRookSquare(pos, relevantSquare, ciptlf);
            if (isControlled) return true;

            piecesToLookFor[0] = byWhite ? Piece.King.AsWhite() : Piece.King;
            piecesToLookFor[1] = Piece.NonsensePiece; //Kommer aldrig återfinnas.
            foreachKingSquare(pos, relevantSquare, ciptlf);
            if (isControlled) return true;

            piecesToLookFor[0] = byWhite ? Piece.Knight.AsWhite() : Piece.Knight;
            foreachKnightSquare(pos, relevantSquare, ciptlf);
            if (isControlled) return true;

            //Bönder
            sbyte riktning = byWhite ? (sbyte)1 : (sbyte)-1;
            Piece pawnToLookFor = byWhite ? Piece.Pawn.AsWhite() : Piece.Pawn;
            Square currentPawnSquare;
            currentPawnSquare.rank = (sbyte)(relevantSquare.rank + riktning);
            for (sbyte i = -1; i <= 1; i+=2)
            {
                currentPawnSquare.line = (sbyte)(relevantSquare.line + i);
                if (validSquare(currentPawnSquare))
                {
                    if(pos[currentPawnSquare] == pawnToLookFor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private List<Move> sortByEstQuality(List<Move> moves, Position pos)
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
                listArray[priorityOf(currentMove, pos)].Add(currentMove);
            }

            List<Move> sortedList = new List<Move>();
            //Dragen läggs till i "omvänd ordning".
            for (int i = priorities-1; i >= 0; i--)
            {
                List<Move> currentList = listArray[i];
                if (currentList.Count > 0)
                    sortedList.AddRange(currentList);
            }
            if (sortedList.Count != moves.Count)
                throw new Exception("Fel i dragsorteringsfunktionen.");
            return sortedList;
        }
        private int priorityOf(Move move, Position pos)
        {
            Piece toPiece = pos[move.to];
            Piece fromPiece = pos[move.from];
            if (toPiece != Piece.None) //Slag
            {
                int valueOfCapturedPiece = roughValueOf(toPiece);
                int valueDiff = valueOfCapturedPiece - roughValueOf(fromPiece);
                
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
                else if (move is Promotion && (move as Promotion).promoteTo.AsBlack() == Piece.Queen)
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
        private int roughValueOf(Piece piece)
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