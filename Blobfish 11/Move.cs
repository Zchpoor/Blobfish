﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public class Move
    {
        public int[] from = new int[2];
        public int[] to = new int[2];
        public Move(int[] from, int[] to)
        {
            this.from = from;
            this.to = to;
        }
        public virtual string toString(char[,] board)
        {
            //TODO: Checks etc.
            string ret = "";
            if (board[from[0], from[1]] != 'p' && board[from[0], from[1]] != 'P')
                ret += board[from[0], from[1]];
            ret = ret.ToUpper();
            ret += ((Char)(from[1] + 'a')).ToString();
            ret += 8 - from[0];
            if (board[to[0], to[1]] != '\0')
            {
                ret += "x";
            }
            else
            {
                ret += "-";
            }
            ret += ((Char)(to[1] + 'a')).ToString();
            ret += 8 - to[0];
            return ret;
        }
        public virtual Position execute(Position oldPos)
        {
            //TODO: Dela upp denna i underfunktioner, som kan anropas av subklasser.

            //Makes a deep copy of the position.
            Position newPos = new Position(oldPos.board, oldPos.whiteToMove, 
                oldPos.castlingRights, oldPos.enPassantSquare, oldPos.halfMoveClock, oldPos.moveCounter);

            newPos.board[to[0], to[1]] = oldPos.board[from[0], from[1]];
            newPos.board[from[0], from[1]] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.whiteToMove = !oldPos.whiteToMove;
            if(oldPos.board[from[0], from[1]] == 'p' || oldPos.board[from[0], from[1]] == 'P' ||
                oldPos.board[to[0], to[1]] != '\0')
            {
                //Om ett bondedrag eller slag spelats, så skall räknaren för femtiodragsregelen sättas tilll 0.
                newPos.halfMoveClock = 0;
            }
            else
            {
                newPos.halfMoveClock = oldPos.halfMoveClock + 1;
            }
            if (oldPos.board[from[0], from[1]] == 'k')
            {
                newPos.castlingRights[2] = false; //Ta bort svarts rockadmöjligheter om kungen förflyttas.
                newPos.castlingRights[3] = false;
            }
            else if (oldPos.board[from[0], from[1]] == 'K')
            {
                newPos.castlingRights[0] = false; //Ta bort vits rockadmöjligheter om kungen förflyttas.
                newPos.castlingRights[1] = false;
            }
            else if (oldPos.board[from[0], from[1]] == 'r')
            {
                //Ta bort en av rockadmöjligheterna om ett av svarts torn förflyttas.

                if (from[1] == 0)//Om tornet står på a-linjen
                {
                    newPos.castlingRights[3] = false;
                }
                else if (from[1] == 7) //Om tornet står på h-linjen.
                {
                    newPos.castlingRights[2] = false;
                }
            }
            else if (oldPos.board[from[0], from[1]] == 'R')
            {
                //Ta bort en av rockadmöjligheterna om ett av vits torn förflyttas.

                if (from[1] == 0)//Om tornet står på a-linjen
                {
                    newPos.castlingRights[1] = false;
                }
                else if (from[1] == 7) //Om tornet står på h-linjen.
                {
                    newPos.castlingRights[0] = false;
                }
            }

            //Beräkna en passant-fält
            if(oldPos.board[from[0], from[1]].ToString().ToUpper() == "P") //Om den förflyttade pjäsen är en bonde
            {
               if(Math.Abs(from[0] - to[0]) == 2) //Om förflyttningen är två steg.
                {
                    newPos.enPassantSquare = new int[2] {(from[0] - to[0])/2, from[1]};
                }
            }
            else
            {
                newPos.enPassantSquare = new int[2] { -1, -1 };
            }
            return newPos;
        }
    }
    public class Castle : Move
    {
        int[] rookFrom, rookTo;
        public Castle(int[] kingFrom, int[] kingTo, int[] rookFrom, int[] rookTo) :
            base(kingFrom, kingTo)
        {
            this.rookFrom = rookFrom;
            this.rookTo = rookTo;
        }
        public override Position execute(Position oldPos)
        {
            //Makes a deep copy of the position.
            Position newPos = new Position(oldPos.board, oldPos.whiteToMove,
                oldPos.castlingRights, oldPos.enPassantSquare, oldPos.halfMoveClock, oldPos.moveCounter);

            newPos.board[to[0], to[1]] = oldPos.board[from[0], from[1]];
            newPos.board[from[0], from[1]] = '\0';
            newPos.board[rookTo[0], rookTo[1]] = oldPos.board[rookFrom[0], rookFrom[1]];
            newPos.board[rookFrom[0], rookFrom[1]] = '\0';
            if (oldPos.whiteToMove) //Ta bort ala rockadmöjligheter för spelaren som rockerar.
            {
                newPos.castlingRights[0] = false; 
                newPos.castlingRights[1] = false;
            }
            else
            {
                newPos.castlingRights[2] = false;
                newPos.castlingRights[3] = false;
            }
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare = new int[2] { -1, -1 };
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
            return newPos;
        }
        public override string toString(char[,] board)
        {
            if (rookFrom[1] == 7) return "0-0";
            else return "0-0-0";
        }
    }
    public class EnPassant : Move
    {
        int[] pawnToRemove;
        public EnPassant(int[] from, int[] to, int[] pawnToRemove) :
            base(from, to)
        {
            this.pawnToRemove = pawnToRemove;
        }
        public override Position execute(Position oldPos)
        {
            //Makes a deep copy of the position.
            Position newPos = new Position(oldPos.board, oldPos.whiteToMove,
                oldPos.castlingRights, oldPos.enPassantSquare, oldPos.halfMoveClock, oldPos.moveCounter);

            newPos.board[to[0], to[1]] = oldPos.board[from[0], from[1]];
            newPos.board[from[0], from[1]] = '\0';
            newPos.board[pawnToRemove[0], pawnToRemove[1]] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare = new int[2] { -1, -1 };
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
            return newPos;
        }
    }
    public class Promotion : Move
    {
        char promoteTo;
        public Promotion(int[] from, int[] to, char promoteTo) :
            base(from, to)
        {
            this.promoteTo = promoteTo;
        }
        public override Position execute(Position oldPos)
        {
            //Makes a deep copy of the position.
            Position newPos = new Position(oldPos.board, oldPos.whiteToMove,
                oldPos.castlingRights, oldPos.enPassantSquare, oldPos.halfMoveClock, oldPos.moveCounter);

            newPos.board[to[0], to[1]] = promoteTo;
            newPos.board[from[0], from[1]] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare = new int[2] { -1, -1 };
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
            return newPos;
        }
        public override string toString(char[,] board)
        {
            return base.ToString() + "=" + promoteTo.ToString().ToUpper();
        }
    }
}
