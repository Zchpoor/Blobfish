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
        private delegate void functionByPiece(Square square);
        public List<Move> allValidMoves(Position pos)
        {
            if (System.Threading.Thread.CurrentThread.Name == "Ke4-f3")
            {
                int a = 0;
            }
            //Public för att kunna användas av testerna.
            List<Move> allMoves = new List<Move>();
            for (sbyte i = 0; i < 8; i++)
            {
                for (sbyte j = 0; j < 8; j++)
                {
                    List<Move> moves = pieceCalculation(pos, new Square(i, j));
                    if (moves != null)
                        allMoves.AddRange(moves);
                }
            }

            for (int i = 0; i < allMoves.Count; i++)
            {
                //Tar bort alla ogiltiga drag
                Position newPos = allMoves[i].execute(pos);

                if (newPos.whiteToMove && isControlledBy(newPos, new Square(newPos.kingPositions[0, 0], newPos.kingPositions[0, 1]), true))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }
                else if (!newPos.whiteToMove && isControlledBy(newPos, new Square(newPos.kingPositions[1, 0], newPos.kingPositions[1, 1]), false))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }

            }
            return allMoves;
        }
        private List<Move> pieceCalculation(Position pos, Square pieceSquare)
        {
            char pieceChar = pos.board[pieceSquare.rank, pieceSquare.line];
            bool pieceIsWhite = isWhite(pieceChar);
            if (pieceIsWhite != pos.whiteToMove)
                return null;
            switch (pieceChar.ToString().ToUpper())
            {
                case "": return null;
                case "P": return pawnMoves(pos, pieceSquare, pieceIsWhite);
                case "N": return knightMoves(pos, pieceSquare, pieceIsWhite);
                case "B": return bishopMoves(pos, pieceSquare, pieceIsWhite);
                case "R": return rookMoves(pos, pieceSquare, pieceIsWhite);
                case "Q": return queenMoves(pos, pieceSquare, pieceIsWhite);
                case "K": return kingMoves(pos, pieceSquare, pieceIsWhite);
                default: return null;
            }
        }

        private List<Move> bishopMoves(Position pos, Square pieceSquare, bool pieceIsWhite)
        {
            List<Move> possibleMoves = new List<Move>();
            void addMoveIfValid(Square currentSquare)
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare == '\0' || isWhite(pieceOnCurrentSquare) != pieceIsWhite)
                {
                    possibleMoves.Add(new Move(pieceSquare, currentSquare));
                }
            }
            foreachBishopSquare(pos, pieceSquare, addMoveIfValid);
            return possibleMoves;
        }
        private List<Move> rookMoves(Position pos, Square pieceSquare, bool pieceIsWhite)
        {
            List<Move> possibleMoves = new List<Move>();
            void addMoveIfValid(Square currentSquare)
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare == '\0' || isWhite(pieceOnCurrentSquare) != pieceIsWhite)
                {
                    possibleMoves.Add(new Move(pieceSquare, currentSquare));
                }
            }
            foreachRookSquare(pos, pieceSquare, addMoveIfValid);
            return possibleMoves;
        }
        private List<Move> queenMoves(Position pos, Square pieceSquare, bool pieceIsWhite)
        {
            List<Move> possibleMoves = new List<Move>();
            possibleMoves.AddRange(rookMoves(pos, pieceSquare, pieceIsWhite));
            possibleMoves.AddRange(bishopMoves(pos, pieceSquare, pieceIsWhite));
            return possibleMoves;
        }
        private List<Move> knightMoves(Position pos, Square pieceSquare, bool pieceIsWhite)
        {
            List<Move> possibleMoves = new List<Move>();
            void addMoveIfValid(Square currentSquare)
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare == '\0' || isWhite(pieceOnCurrentSquare) != pieceIsWhite)
                {
                    possibleMoves.Add(new Move(pieceSquare, currentSquare));
                }
            }
            foreachKnightSquare(pos, pieceSquare, addMoveIfValid);
            return possibleMoves;
        }
        private List<Move> kingMoves(Position pos, Square pieceSquare, bool pieceIsWhite)
        {
            List<Move> possibleMoves = new List<Move>();
            void addMoveIfValid(Square currentSquare)
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare == '\0' || isWhite(pieceOnCurrentSquare) != pieceIsWhite)
                {
                    possibleMoves.Add(new Move(pieceSquare, currentSquare));
                }
            }
            foreachKingSquare(pos, pieceSquare, addMoveIfValid);

            sbyte castlingRank = pieceIsWhite ? (sbyte) 7: (sbyte) 0;
            if (pieceSquare.rank == castlingRank && pieceSquare.line == 4 && !isControlledBy(pos, pieceSquare, !pieceIsWhite))
            {
                //Kungen står på ett fält där rockad skulle kunna vara möjligt.
                char correctRook = pieceIsWhite ? 'R' : 'r';
                sbyte castlingRightOffset = pieceIsWhite ? (sbyte) 0: (sbyte) 2;
                if (pos.board[castlingRank, 0] == correctRook && pos.castlingRights[castlingRightOffset + 1])
                {
                    //Lång rockad
                    Square rookTo = new Square(castlingRank, (sbyte) 3);
                    Square kingTo = new Square(castlingRank, (sbyte) 2);
                    if (pos.board[rookTo.rank, rookTo.line] == '\0' && pos.board[kingTo.rank, kingTo.line] == '\0' &&
                        pos.board[kingTo.rank, kingTo.line - 1] == '\0' && !isControlledBy(pos, rookTo, !pieceIsWhite) &&
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
                    if (pos.board[rookTo.rank, rookTo.line] == '\0' && pos.board[kingTo.rank, kingTo.line] == '\0' &&
                        !isControlledBy(pos, rookTo, !pieceIsWhite) && !isControlledBy(pos, kingTo, !pieceIsWhite &&
                        !isControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, (sbyte) 7);
                        possibleMoves.Insert(0, new Castle(pieceSquare, kingTo, rookFrom, rookTo));
                    }
                }
            }
            return possibleMoves;
        }
        private List<Move> pawnMoves(Position pos, Square pieceSquare, bool pieceIsWhite)
        {
            List<Move> possibleMoves = new List<Move>();

            void addPromotions(Square fromSquare, Square toSquare)
            {
                string allPromotions = pieceIsWhite ? "QRBN" : "qrbn";
                foreach (char tkn in allPromotions)
                {
                    possibleMoves.Add(new Promotion(fromSquare, toSquare, tkn));
                }
            }

            sbyte moveDirection = pieceIsWhite ? (sbyte) - 1 : (sbyte) 1;
            Square currentSquare = new Square(pieceSquare.rank + moveDirection, pieceSquare.line);
            if (validSquare(currentSquare))
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare == '\0')
                {

                    sbyte promotionRank = pieceIsWhite ? (sbyte) 0: (sbyte) 7;
                    if (currentSquare.rank == promotionRank)//Promotering.
                    {
                        addPromotions(pieceSquare, currentSquare);
                    }
                    else
                    {
                        //Vanligt bondedrag, ett steg framåt.
                        possibleMoves.Add(new Move(pieceSquare, currentSquare));
                        sbyte startingRank = pieceIsWhite ? (sbyte) 6: (sbyte) 1;
                        if (pieceSquare.rank == startingRank)
                        {
                            currentSquare.rank = (sbyte) (pieceSquare.rank + (moveDirection * 2));
                            pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                            if (pieceOnCurrentSquare == '\0')
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
                currentSquare.rank = (sbyte) (pieceSquare.rank + moveDirection);
                currentSquare.line = (sbyte) (pieceSquare.line + i);
                if (validSquare(currentSquare))
                {
                    sbyte promotionRank = pieceIsWhite ? (sbyte) 0: (sbyte) 7;
                    char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                    if (isWhite(pieceOnCurrentSquare) != pieceIsWhite && pieceOnCurrentSquare != '\0') //Om den är av motsatt färg.
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
                    else if (currentSquare.rank == pos.enPassantSquare[0] && currentSquare.line == pos.enPassantSquare[1])
                    {
                        Square pawnToRemove = new Square(pieceSquare.rank, currentSquare.line);
                        possibleMoves.Add(new EnPassant(pieceSquare, currentSquare, pawnToRemove)); //En passant
                    }

                }
            }
            return possibleMoves;
        }
        private void foreachKingSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            for (sbyte i = -1; i <= 1; i++)
            {
                for (sbyte j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    Square currentSquare = new Square(pieceSquare.rank + i, pieceSquare.line + j);
                    if (validSquare(currentSquare))
                    {
                        callback(currentSquare);
                    }
                }
            }
        }
        private void foreachKnightSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            void callbackByOffset(sbyte rankOffset, sbyte lineOffset)
            {
                Square currentSquare = new Square(pieceSquare.rank + rankOffset, pieceSquare.line + lineOffset);
                if (validSquare(currentSquare))
                {
                    callback(currentSquare);
                }
            }
            callbackByOffset(2, 1);
            callbackByOffset(2, -1);
            callbackByOffset(1, 2);
            callbackByOffset(1, -2);
            callbackByOffset(-1, 2);
            callbackByOffset(-1, -2);
            callbackByOffset(-2, 1);
            callbackByOffset(-2, -1);
        }
        private void foreachBishopSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            for (sbyte i = -1; i < 2; i += 2) //Kommer att vara -1 eller 1.
            {
                for (sbyte j = -1; j < 2; j += 2) //Kommer att vara -1 eller 1.
                {
                    foreachLineSquare(pos, pieceSquare, i, j, callback);
                }
            }
        }
        private void foreachRookSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            foreachLineSquare(pos, pieceSquare, 0, 1, callback);
            foreachLineSquare(pos, pieceSquare, 0, -1, callback);
            foreachLineSquare(pos, pieceSquare, 1, 0, callback);
            foreachLineSquare(pos, pieceSquare, -1, 0, callback);
        }
        private void foreachLineSquare(Position pos, Square pieceSquare, sbyte rankOffset, sbyte lineOffset, functionByPiece callback)
        {
            //Generell funktion som itererar över en linje/diagonal så länge det är giltiga fält.
            sbyte rank = pieceSquare.rank, line = pieceSquare.line;
            sbyte counter = 1;
            bool done = false;
            Square currentSquare = new Square(rank + (rankOffset * counter), line + (lineOffset * counter));
            while (validSquare(currentSquare) && !done)
            {
                callback(currentSquare);
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare != '\0') //Ett fält där en pjäs står.
                {
                    done = true;
                }
                counter++;
                currentSquare.rank = (sbyte) (rank + (rankOffset * counter));
                currentSquare.line = (sbyte) (line + (lineOffset * counter));
            }
        }

        private bool validPosition(Position pos, SquareControl[,] squareControls)
        {
            if (pos.whiteToMove && squareControls[pos.kingPositions[0, 0], pos.kingPositions[0, 1]].wControl)
            {
                return false;
            }
            else if (!pos.whiteToMove && squareControls[pos.kingPositions[1, 0], pos.kingPositions[1, 1]].bControl)
            {
                return false;
            }
            else return true;
        }
        private bool accessableFor(bool forWhite, char piece)
        {
            if (forWhite)
            {
                if (piece == '\0' || piece > 'Z') return true;
                else return false;
            }
            else
            {
                if (piece < 'Z') return true;
                else return false;
            }
        }
        private bool validSquare(sbyte rank, sbyte line)
        {
            //TODO: Ta bort vid senare tillfälle.
            return (rank < 8) && (rank >= 0) && (line < 8) && (line >= 0);
        }
        private bool validSquare(Square square)
        {
            return validSquare(square.rank, square.line);
        }
        private bool isWhite(char piece)
        {
            return piece <= 'Z';
        }
        private bool isControlledBy(Position pos, Square relevantSquare, bool byWhite)
        {
            string piecesToLookFor = byWhite ? "BQ" : "bq";
            bool isControlled = false;
            void CheckIfPieceToLookFor(Square currentSquare)
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (piecesToLookFor.Contains(pieceOnCurrentSquare))
                    isControlled = true;
            }

            foreachBishopSquare(pos, relevantSquare, CheckIfPieceToLookFor);
            if (isControlled) return true;

            piecesToLookFor = byWhite ? "RQ" : "rq";
            foreachRookSquare(pos, relevantSquare, CheckIfPieceToLookFor);
            if (isControlled) return true;

            piecesToLookFor = byWhite ? "K" : "k";
            foreachKingSquare(pos, relevantSquare, CheckIfPieceToLookFor);
            if (isControlled) return true;

            piecesToLookFor = byWhite ? "N" : "n";
            foreachKnightSquare(pos, relevantSquare, CheckIfPieceToLookFor);
            if (isControlled) return true;

            //Bönder
            sbyte riktning = byWhite ? (sbyte)1 : (sbyte)-1;
            for (sbyte i = -1; i <= 1; i+=2)
            {
                Square currentSquare = new Square(relevantSquare.rank + riktning, relevantSquare.line + i);
                if (validSquare(currentSquare))
                {
                    char pawnToLookFor = byWhite ? 'P' : 'p';
                    if(pos.board[currentSquare.rank, currentSquare.line] == pawnToLookFor)
                    {
                        return true;
                    }
                }
            }
            return false;

        }
    }
}
