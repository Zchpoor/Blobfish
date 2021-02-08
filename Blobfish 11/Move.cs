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
            //TODO: Dela upp denna i underfunktioner, som kan anropas av subklasser.

            Position newPos = oldPos.deepCopy();
            char pieceOnCurrentSquare = oldPos.board[from.rank, from.line];
            newPos.board[to.rank, to.line] = pieceOnCurrentSquare;
            newPos.board[from.rank, from.line] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.whiteToMove = !oldPos.whiteToMove;
            if(pieceOnCurrentSquare == 'p' || pieceOnCurrentSquare == 'P' ||
                oldPos.board[to.rank, to.line] != '\0')
            {
                //Om ett bondedrag eller slag spelats, så skall räknaren för femtiodragsregelen sättas till 0.
                newPos.halfMoveClock = 0;
            }
            else
            {
                newPos.halfMoveClock = (sbyte) (oldPos.halfMoveClock + 1);
            }
            if (pieceOnCurrentSquare == 'k')
            {
                newPos.castlingRights = new bool[] { oldPos.castlingRights[0], 
                    oldPos.castlingRights[1], false, false};
                //newPos.castlingRights[2] = false; //Ta bort svarts rockadmöjligheter om kungen förflyttas.
                //newPos.castlingRights[3] = false;
                newPos.kingPositions[0].rank = this.to.rank; //Sparar om kungens placering.
                newPos.kingPositions[0].line = this.to.line;
            }
            else if (pieceOnCurrentSquare == 'K')
            {
                newPos.castlingRights = new bool[] { false, false, 
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};
                //newPos.castlingRights[0] = false; //Ta bort vits rockadmöjligheter om kungen förflyttas.
                //newPos.castlingRights[1] = false;
                newPos.kingPositions[1].rank = this.to.rank; //Sparar om kungens placering.
                newPos.kingPositions[1].line = this.to.line;
            }
            else if (pieceOnCurrentSquare == 'r')
            {
                //Ta bort en av rockadmöjligheterna om ett av svarts torn förflyttas.

                if (from.line == 0)//Om tornet står på a-linjen
                {
                    newPos.castlingRights = new bool[] { oldPos.castlingRights[0], oldPos.castlingRights[1],
                    oldPos.castlingRights[2], false};
                    //newPos.castlingRights[3] = false;
                }
                else if (from.line == 7) //Om tornet står på h-linjen.
                {
                    newPos.castlingRights = new bool[] { oldPos.castlingRights[0], oldPos.castlingRights[1],
                    false, oldPos.castlingRights[3]};
                    //newPos.castlingRights[2] = false;
                }
            }
            else if (pieceOnCurrentSquare == 'R')
            {
                //Ta bort en av rockadmöjligheterna om ett av vits torn förflyttas.

                if (from.line == 0)//Om tornet står på a-linjen
                {
                    newPos.castlingRights = new bool[] { oldPos.castlingRights[0], false,
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};
                    //newPos.castlingRights[1] = false;
                }
                else if (from.line == 7) //Om tornet står på h-linjen.
                {
                    newPos.castlingRights = new bool[] {false,  oldPos.castlingRights[1],
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};
                    //newPos.castlingRights[0] = false;
                }
            }

            //Beräkna en passant-fält
            if(pieceOnCurrentSquare == 'P'
                || pieceOnCurrentSquare == 'p') //Om den förflyttade pjäsen är en bonde
            {
               if(Math.Abs(from.rank - to.rank) == 2) //Om förflyttningen är två steg.
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
        public virtual bool isCapture(char [,] board)
        {
            return board[to.rank, to.line] != '\0';
        }
        public virtual string toString(char[,] board)
        {
            //TODO: Checks etc.
            string ret = "";
            if (board[from.rank, from.line] != 'p' && board[from.rank, from.line] != 'P')
            {
                char piece = board[from.rank, from.line];
                if (piece > 'a')
                {
                    piece = (char)(piece - ('a' - 'A')); //Gör om tecknet till stor bokstav.
                }
                ret += piece;
            }
            ret += ((Char)(from.line + 'a')).ToString();
            ret += 8 - from.rank;
            if (isCapture(board))
            {
                ret += "x";
            }
            else
            {
                ret += "-";
            }
            ret += ((Char)(to.line + 'a')).ToString();
            ret += 8 - to.rank;
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
            Position newPos = oldPos.deepCopy();

            newPos.board[to.rank, to.line] = oldPos.board[from.rank, from.line];
            newPos.board[from.rank, from.line] = '\0';
            newPos.board[rookTo.rank, rookTo.line] = oldPos.board[rookFrom.rank, rookFrom.line];
            newPos.board[rookFrom.rank, rookFrom.line] = '\0';
            if (oldPos.whiteToMove) //Ta bort alla rockadmöjligheter för spelaren som rockerar.
            {
                newPos.castlingRights = new bool[] { false, false,
                    oldPos.castlingRights[2], oldPos.castlingRights[3]};
                //newPos.castlingRights[0] = false; 
                //newPos.castlingRights[1] = false;
                newPos.kingPositions[1].rank = this.to.rank; //Sparar om kungens placering.
                newPos.kingPositions[1].line = this.to.line;
            }
            else
            {
                newPos.castlingRights = new bool[] { oldPos.castlingRights[0],
                    oldPos.castlingRights[1], false, false};
                //newPos.castlingRights[2] = false;
                //newPos.castlingRights[3] = false;
                newPos.kingPositions[0].rank = this.to.rank; //Sparar om kungens placering.
                newPos.kingPositions[0].line = this.to.line;
            }
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare.rank = -1;
            newPos.enPassantSquare.line = -1;
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
            return newPos;
        }
        public override string toString(char[,] board)
        {
            string ret = rookFrom.line == 7 ? "0-0" : "0-0-0";
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
            Position newPos = oldPos.deepCopy();

            newPos.board[to.rank, to.line] = oldPos.board[from.rank, from.line];
            newPos.board[from.rank, from.line] = '\0';
            newPos.board[pawnToRemove.rank, pawnToRemove.line] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare.rank = -1;
            newPos.enPassantSquare.line = -1;
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
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
            Position newPos = oldPos.deepCopy();

            newPos.board[to.rank, to.line] = promoteTo;
            newPos.board[from.rank, from.line] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare.rank = -1;
            newPos.enPassantSquare.line = -1;
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
            return newPos;
        }
        public override string toString(char[,] board)
        {
            string ret = base.toString(board);
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

