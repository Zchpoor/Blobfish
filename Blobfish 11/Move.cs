﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public class Move
    {
        public Square from;
        public Square to;
        public Move(Square fromSquare, Square toSquare)
        {
            this.from = fromSquare;
            this.to = toSquare;
        }
        public virtual Position execute(Position oldPos)
        {
            Position newPos = oldPos.boardCopy();
            char pieceOnCurrentSquare = oldPos.board[from.rank, from.line];
            newPos.board[to.rank, to.line] = pieceOnCurrentSquare;
            newPos.board[from.rank, from.line] = '\0';
            plyForward(newPos);
            if (pieceOnCurrentSquare == 'p' || pieceOnCurrentSquare == 'P' ||
                oldPos.board[to.rank, to.line] != '\0')
            {
                //Om ett bondedrag eller slag spelats, så skall räknaren för femtiodragsregelen sättas till 0.
                newPos.halfMoveClock = 0;
            }
            else
            {
                newPos.halfMoveClock = (sbyte)(oldPos.halfMoveClock + 1);
            }
            if (pieceOnCurrentSquare == 'k')
            {
                //Ta bort svarts rockadmöjligheter om kungen förflyttas.
                newPos.castlingRights = new bool[] { oldPos.castlingRights[0],
                    oldPos.castlingRights[1], false, false};

                //Sparar om kungens placering.
                newPos.kingPositions = new Square[] { new Square(this.to.rank, this.to.line), newPos.kingPositions[1] };
            }
            else if (pieceOnCurrentSquare == 'K')
            {
                //Ta bort vits rockadmöjligheter om kungen förflyttas.
                newPos.castlingRights = new bool[] { false, false,
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};

                //Sparar om kungens placering.
                newPos.kingPositions = new Square[] { newPos.kingPositions[0], new Square(this.to.rank, this.to.line) };

            }
            else if (pieceOnCurrentSquare == 'r')
            {
                //Ta bort en av rockadmöjligheterna om ett av svarts torn förflyttas.

                if (from.line == 0)//Om tornet står på a-linjen
                {
                    newPos.castlingRights = new bool[] { oldPos.castlingRights[0], oldPos.castlingRights[1],
                    oldPos.castlingRights[2], false};
                }
                else if (from.line == 7) //Om tornet står på h-linjen.
                {
                    newPos.castlingRights = new bool[] { oldPos.castlingRights[0], oldPos.castlingRights[1],
                    false, oldPos.castlingRights[3]};
                }
            }
            else if (pieceOnCurrentSquare == 'R')
            {
                //Ta bort en av rockadmöjligheterna om ett av vits torn förflyttas.

                if (from.line == 0)//Om tornet står på a-linjen
                {
                    newPos.castlingRights = new bool[] { oldPos.castlingRights[0], false,
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};
                }
                else if (from.line == 7) //Om tornet står på h-linjen.
                {
                    newPos.castlingRights = new bool[] {false,  oldPos.castlingRights[1],
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};
                }
            }

            //Beräkna en passant-fält
            if (pieceOnCurrentSquare == 'P'
                || pieceOnCurrentSquare == 'p') //Om den förflyttade pjäsen är en bonde
            {
                if (Math.Abs(from.rank - to.rank) == 2) //Om förflyttningen är två steg.
                {
                    newPos.enPassantSquare.rank = (sbyte)((from.rank + to.rank) / 2);
                    newPos.enPassantSquare.line = from.line;
                }
            }
            else
            {
                newPos.enPassantSquare.rank = -1;
                newPos.enPassantSquare.line = -1;
            }
            return newPos;
        }
        protected void plyForward(Position pos)
        {
            if (!pos.whiteToMove)
            {
                pos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            pos.whiteToMove = !pos.whiteToMove;
        }
        public virtual bool isCapture(char[,] board)
        {
            return board[to.rank, to.line] != '\0';
        }
        public virtual string toString(Position pos)
        {
            //TODO: Checks etc.
            char[,] board = pos.board;
            string ret = "";
            char thisPiece = board[from.rank, from.line];
            if (thisPiece != 'p' && thisPiece != 'P')
            {
                if (thisPiece >= 'a')
                {
                    thisPiece = (char)(thisPiece - ('a' - 'A')); //Gör om tecknet till stor bokstav.
                }
                ret += thisPiece;
            }
            else if(isCapture(board))
            {
                ret += (Char)(from.line + 'a');
            }
            ret += extraDescription(pos);
            //ret += ((Char)(from.line + 'a')).ToString();
            //ret += 8 - from.rank;
            if (isCapture(board))
            {
                ret += "x";
            }
            ret += ((Char)(to.line + 'a')).ToString(); //?
            ret += 8 - to.rank;
            return ret;
        }
        private string extraDescription(Position pos)
        {
            char[,] board = pos.board;
            char thisPiece = board[from.rank, from.line];
            bool onLine = false, onRank = false, otherPiece = false;
            void determineExtras(Square square)
            {
                if (board[square.rank, square.line] == thisPiece)
                {
                    if (square.rank != from.rank || square.line != from.line)
                        //För att inte räkna med den flyttade pjäsen.
                    {
                        otherPiece = true;
                        onRank = onRank || (square.rank == from.rank);
                        onLine = onLine || (square.line == from.line);
                    }
                }
            }
            Engine.functionByPiece dE = new Engine.functionByPiece(determineExtras);
            char thisPieceToLower = thisPiece <= 'Z' ? (char)(thisPiece + ('a' - 'A')) : thisPiece;

            switch (thisPieceToLower)
            {
                case 'r': Engine.foreachRookSquare(pos, to, dE); break;
                case 'b': Engine.foreachBishopSquare(pos, to, dE); break;
                case 'n': Engine.foreachKnightSquare(pos, to, dE); break;
                case 'q': Engine.foreachRookSquare(pos, to, dE); Engine.foreachBishopSquare(pos, to, dE); break;
            }
            String ret = "";
            if (onRank && onLine)
            {
                ret += (char)(from.line + 'a');
                ret += 8 - from.rank;
            }
            else if (!onRank && onLine)
            {
                ret += 8 - from.rank;
            }
            else if (otherPiece)
            {
                ret += (char)(from.line + 'a');
            }
            return ret;
        }
    }
    public class Castle : Move
    {
        Square rookFrom, rookTo;
        public Castle(Square kingFrom, Square kingTo, Square rookFrom, Square rookTo):
            base(kingFrom, kingTo)
        {
            this.rookFrom = rookFrom;
            this.rookTo = rookTo;
        }
        public override Position execute(Position oldPos)
        {
            Position newPos = oldPos.boardCopy();

            newPos.board[to.rank, to.line] = oldPos.board[from.rank, from.line];
            newPos.board[from.rank, from.line] = '\0';
            newPos.board[rookTo.rank, rookTo.line] = oldPos.board[rookFrom.rank, rookFrom.line];
            newPos.board[rookFrom.rank, rookFrom.line] = '\0';
            if (oldPos.whiteToMove) //Ta bort alla rockadmöjligheter för spelaren som rockerar.
            {
                newPos.castlingRights = new bool[] { false, false,
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};

                //Sparar om kungens placering.
                newPos.kingPositions = new Square[] { newPos.kingPositions[0], new Square(this.to.rank, this.to.line) };
               
            }
            else
            {
                newPos.castlingRights = new bool[] { oldPos.castlingRights[0],
                    oldPos.castlingRights[1], false, false};

                //Sparar om kungens placering.
                newPos.kingPositions = new Square[] {new Square(this.to.rank, this.to.line), newPos.kingPositions[1] };
            }
            plyForward(newPos);
            newPos.enPassantSquare.rank = -1;
            newPos.enPassantSquare.line = -1;
            newPos.halfMoveClock = 0;
            return newPos;
        }
        public override string toString(Position pos)
        {
            string ret = rookFrom.line == 7 ? "O-O" : "O-O-O";
            return ret;
        }
    }
    public class EnPassant : Move
    {
        Square pawnToRemove;
        public EnPassant(Square fromSquare, Square toSquare, Square pawnToRemove) :
            base(fromSquare, toSquare)
        {
            this.pawnToRemove = pawnToRemove;
        }
        public override Position execute(Position oldPos)
        {
            Position newPos = oldPos.boardCopy();

            newPos.board[to.rank, to.line] = oldPos.board[from.rank, from.line];
            newPos.board[from.rank, from.line] = '\0';
            newPos.board[pawnToRemove.rank, pawnToRemove.line] = '\0';
            plyForward(newPos);
            newPos.enPassantSquare.rank = -1;
            newPos.enPassantSquare.line = -1;
            newPos.halfMoveClock = 0;
            return newPos;
        }
        public override bool isCapture(char[,] board)
        {
            return true;
        }
    }
    public class Promotion : Move
    {
        public char promoteTo;
        public Promotion(Square fromSquare, Square toSquare, char promoteTo) :
            base(fromSquare, toSquare)
        {
            this.promoteTo = promoteTo;
        }
        public override Position execute(Position oldPos)
        {
            Position newPos = oldPos.boardCopy();

            newPos.board[to.rank, to.line] = promoteTo;
            newPos.board[from.rank, from.line] = '\0';
            plyForward(newPos);
            newPos.enPassantSquare.rank = -1;
            newPos.enPassantSquare.line = -1;
            newPos.halfMoveClock = 0;
            return newPos;
        }
        public override string toString(Position pos)
        {
            string ret = base.toString(pos);
            if(ret[ret.Length-1] == '+' || ret[ret.Length-1] == '#')
            {
                ret = ret.Insert(ret.Length - 1, "=" + promoteTo.ToString().ToUpper());
            }
            else
                ret += "=" + promoteTo.ToString().ToUpper();
            return ret;

        }
    }
}

