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
        private List<Move> allValidMoves(Position pos) 
        {
            List<Move> allMoves = new List<Move>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    List<Move> moves = pieceCalculation(pos, new Square(i, j));
                    if (moves != null)
                        allMoves.AddRange(moves);
                }
            }

            for (int i = 0; i < allMoves.Count; i++)
            {
                Position newPos = allMoves[i].execute(pos);
                if (newPos.whiteToMove && isControlledBy(newPos, new Square(pos.kingPositions[0, 0], pos.kingPositions[0, 1]), true))
                {
                    allMoves.RemoveAt(i);
                    i--;
                }
                else if (!newPos.whiteToMove && isControlledBy(newPos, new Square(pos.kingPositions[1, 0], pos.kingPositions[1, 1]), false))
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
                case "" : return null;
                case "P": return pawnMoves(pos, pieceSquare, pieceIsWhite);
                case "N": return knightMoves(pos, pieceSquare, pieceIsWhite);
                case "B": return bishopMoves(pos, pieceSquare, pieceIsWhite);
                case "R": return rookMoves(pos, pieceSquare, pieceIsWhite);
                case "Q": return queenMoves(pos, pieceSquare, pieceIsWhite);
                case "K": return kingMoves(pos, pieceSquare, pieceIsWhite);
                default: return null;
            }
        }
        private PieceData bishopCalculation(PieceData pieceData, Position pos, Square pieceSquare, char pieceChar, char oppositeKing)
        {
            //TODO: Ta bort.
            int rank = pieceSquare.rank, line = pieceSquare.line;
            for (int i = -1; i < 2; i+=2)
            {
                for (int j = -1; j < 2; j+=2)
                {
                    int counter = 1;
                    bool done = false;
                    Square currentSquare = new Square(rank + (i * counter), line + (j * counter));

                    while (validSquare(currentSquare) && !done)
                    {
                        pieceData.controlledSquares.Add(currentSquare);

                        char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                        if (pieceOnCurrentSquare == '\0')
                        {
                            pieceData.moves.Add(new Move(pieceSquare, currentSquare));
                        }
                        else if (pieceOnCurrentSquare == oppositeKing)
                        {
                            pieceData.givesCheck = true;
                            Square squareBehindKing = new Square(rank + (i * (counter + 1)), line + (j * (counter + 1)));
                            if (validSquare(squareBehindKing)) 
                            {
                                //Fältet bakom kungen markeras som kontrollerat.
                                pieceData.controlledSquares.Add(squareBehindKing);
                            }
                            done = true;
                        }
                        else 
                        {
                            done = true;
                            if(isWhite(pieceOnCurrentSquare) != isWhite(pieceChar))
                            {
                                //Om pjäsen är av motsatt färg, så går denna att slå.
                                pieceData.moves.Add(new Move(pieceSquare, currentSquare));
                            }
                        }
                        i++;
                    }
                }
            }
            return pieceData;
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
            foreachBishopSquare(pos, pieceSquare,addMoveIfValid);
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
                if (pieceOnCurrentSquare =='\0' || isWhite(pieceOnCurrentSquare) != pieceIsWhite)
                {
                    possibleMoves.Add(new Move(pieceSquare, currentSquare));
                }
            }
            foreachKingSquare(pos, pieceSquare, addMoveIfValid);

            int castlingRank = pieceIsWhite ? 7 : 0;
            if(pieceSquare.rank == castlingRank && pieceSquare.line == 4)
            {
                //Kungen står på ett fält där rockad skulle kunna vara möjligt.
                char correctRook = pieceIsWhite ? 'R' : 'r';
                int castlingRightOffset = pieceIsWhite ? 0 : 2;
                if(pos.board[castlingRank, 0] == correctRook && pos.castlingRights[castlingRightOffset +1])
                {
                    //Lång rockad
                    Square rookTo = new Square(castlingRank, 3);
                    Square kingTo = new Square(castlingRank, 2);
                    if (pos.board[rookTo.rank, rookTo.line] == '\0' && pos.board[kingTo.rank, kingTo.line] == '\0' &&
                        !isControlledBy(pos, rookTo, !pieceIsWhite) && !isControlledBy(pos, kingTo, !pieceIsWhite &&
                        !isControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, 0);
                        possibleMoves.Add(new Castle(pieceSquare, kingTo, rookFrom, rookTo));
                    }
                }
                if(pos.board[castlingRank, 7] == correctRook && pos.castlingRights[castlingRightOffset])
                {
                    //Kort rockad
                    Square rookTo = new Square(castlingRank, 5);
                    Square kingTo = new Square(castlingRank, 6);
                    if (pos.board[rookTo.rank, rookTo.line] == '\0' && pos.board[kingTo.rank, kingTo.line] == '\0' &&
                        !isControlledBy(pos, rookTo, !pieceIsWhite) && !isControlledBy(pos, kingTo, !pieceIsWhite &&
                        !isControlledBy(pos, pieceSquare, !pieceIsWhite)))
                    {
                        Square rookFrom = new Square(castlingRank, 7);
                        possibleMoves.Add(new Castle(pieceSquare, kingTo, rookFrom, rookTo));
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

            int moveDirection = pieceIsWhite ? -1 : 1;
            Square currentSquare = new Square(pieceSquare.rank + moveDirection, pieceSquare.line);
            if (validSquare(currentSquare))
            {
                char pieceOnCurrentSquare = pos.board[currentSquare.rank, currentSquare.line];
                if (pieceOnCurrentSquare == '\0')
                {

                    int promotionRank = pieceIsWhite ? 0 : 8;
                    if (currentSquare.rank == promotionRank)//Promotering.
                    {
                        addPromotions(pieceSquare, currentSquare);
                    }
                    else
                    {
                        //Vanligt bondedrag, ett steg framåt.
                        possibleMoves.Add(new Move(pieceSquare, currentSquare));
                        int startingRank = pieceIsWhite ? 6 : 1;
                        if (pieceSquare.rank == startingRank)
                        {
                            currentSquare.rank = pieceSquare.rank + (moveDirection * 2);
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
            for (int i = -1; i <= 1; i+=2) //Kommer bli -1 och 1.
            {
                currentSquare.rank = pieceSquare.rank + moveDirection;
                currentSquare.line = pieceSquare.line + i;
                if (validSquare(currentSquare))
                {
                    int promotionRank = pieceIsWhite ? 0 : 8;
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
            List<Move> possibleMoves = new List<Move>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    Square currentSquare = new Square(pieceSquare.rank + i, pieceSquare.line + j);
                    if (validSquare(currentSquare))
                    {
                        char pieceOnCurrentSquare = pos.board[pieceSquare.rank, pieceSquare.line];
                        callback(currentSquare);
                    }
                }
            }
        }
        private void foreachKnightSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    Square currentSquare = new Square(pieceSquare.rank + i, pieceSquare.line + j);
                    if (Math.Abs(i) + Math.Abs(j) == 3 && validSquare(currentSquare))
                    {
                        callback(currentSquare);
                    }
                }
            }
        }
        private void foreachBishopSquare(Position pos, Square pieceSquare, functionByPiece callback)
        {
            for (int i = -1; i < 2; i += 2) //Kommer att vara -1 eller 1.
            {
                for (int j = -1; j < 2; j += 2) //Kommer att vara -1 eller 1.
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
        private void foreachLineSquare(Position pos, Square pieceSquare, int rankOffset, int lineOffset, functionByPiece callback)
        {
            //Generell funktion som itererar över en linje/diagonal så länge det är giltiga fält.
            int rank = pieceSquare.rank, line = pieceSquare.line;
            int counter = 1;
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
                currentSquare.rank = rank + (rankOffset * counter);
                currentSquare.line = line + (lineOffset * counter);
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
        private bool validSquare(int rank, int line)
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
        private int[] calculateLine(Square firstSquare, Square secondSquare)
        {
            //TODO: Ta bort.
            int dr = firstSquare.rank - secondSquare.rank;
            int dl = firstSquare.line - secondSquare.line;
            if(dl == 0)
            {
                return new int[] { Math.Sign(dr), 0 };
            }
            else if(dr == 0)
            {
                return new int[] { 0, Math.Sign(dl) };
            }
            else if(Math.Abs(dr) == Math.Abs(dl))
            {
                return new int[] { Math.Sign(dr), Math.Sign(dl) };
            }
            else
            {
                return new int[] { 0, 0 }; //Ingen delad diagonal.
            }
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
            int riktning = byWhite ? -1 : 1;
            for (int i = -1; i <= 1; i++)
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
