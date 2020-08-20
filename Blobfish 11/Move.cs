using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public class Move
    {
        public sbyte[] from = new sbyte[2];
        public sbyte[] to = new sbyte[2];
        public Move(sbyte[] from, sbyte[] to)
        {
            this.from = from;
            this.to = to;
        }
        public Move(Square fromSquare, Square toSquare) : 
            this(new sbyte[] { fromSquare.rank, fromSquare.line }, new sbyte[] { toSquare.rank, toSquare.line })
        {

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
            if (isCapture(board))
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

            Position newPos = oldPos.deepCopy();

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
                //Om ett bondedrag eller slag spelats, så skall räknaren för femtiodragsregelen sättas till 0.
                newPos.halfMoveClock = 0;
            }
            else
            {
                newPos.halfMoveClock = (sbyte) (oldPos.halfMoveClock + 1);
            }
            if (oldPos.board[from[0], from[1]] == 'k')
            {
                newPos.castlingRights[2] = false; //Ta bort svarts rockadmöjligheter om kungen förflyttas.
                newPos.castlingRights[3] = false;
                newPos.kingPositions[0].rank = this.to[0]; //Sparar om kungens placering.
                newPos.kingPositions[0].line = this.to[1];
            }
            else if (oldPos.board[from[0], from[1]] == 'K')
            {
                newPos.castlingRights[0] = false; //Ta bort vits rockadmöjligheter om kungen förflyttas.
                newPos.castlingRights[1] = false;
                newPos.kingPositions[1].rank = this.to[0]; //Sparar om kungens placering.
                newPos.kingPositions[1].line = this.to[1];
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
                    newPos.enPassantSquare = new sbyte[2] {(sbyte) ((from[0] + to[0])/2), from[1]};
                }
            }
            else
            {
                newPos.enPassantSquare = new sbyte[2] { -1, -1 };
            }
            return newPos;
        }
        public virtual bool isCapture(char [,] board)
        {
            return board[to[0], to[1]] != '\0';
        }
    }
    public class Castle : Move
    {
        sbyte[] rookFrom, rookTo;
        public Castle(sbyte[] kingFrom, sbyte[] kingTo, sbyte[] rookFrom, sbyte[] rookTo) :
            base(kingFrom, kingTo)
        {
            this.rookFrom = rookFrom;
            this.rookTo = rookTo;
        }
        public Castle(Square kingFrom, Square kingTo, Square rookFrom, Square rookTo):
            this(new sbyte[] { kingFrom.rank, kingFrom.line }, new sbyte[] { kingTo.rank, kingTo.line },
                new sbyte[] { rookFrom.rank, rookFrom.line }, new sbyte[] { rookTo.rank, rookTo.line })
        {

        }
        public override Position execute(Position oldPos)
        {
            Position newPos = oldPos.deepCopy();

            newPos.board[to[0], to[1]] = oldPos.board[from[0], from[1]];
            newPos.board[from[0], from[1]] = '\0';
            newPos.board[rookTo[0], rookTo[1]] = oldPos.board[rookFrom[0], rookFrom[1]];
            newPos.board[rookFrom[0], rookFrom[1]] = '\0';
            if (oldPos.whiteToMove) //Ta bort ala rockadmöjligheter för spelaren som rockerar.
            {
                newPos.castlingRights[0] = false; 
                newPos.castlingRights[1] = false;
                newPos.kingPositions[1].rank = this.to[0]; //Sparar om kungens placering.
                newPos.kingPositions[1].line = this.to[1];
            }
            else
            {
                newPos.castlingRights[2] = false;
                newPos.castlingRights[3] = false;
                newPos.kingPositions[0].rank = this.to[0]; //Sparar om kungens placering.
                newPos.kingPositions[0].line = this.to[1];
            }
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare = new sbyte[2] { -1, -1 };
            newPos.halfMoveClock = 0;
            newPos.whiteToMove = !oldPos.whiteToMove;
            return newPos;
        }
        public override string toString(char[,] board)
        {
            string ret = rookFrom[1] == 7 ? "0-0" : "0-0-0";
            return ret;
        }
    }
    public class EnPassant : Move
    {
        sbyte[] pawnToRemove;
        public EnPassant(sbyte[] from, sbyte[] to, sbyte[] pawnToRemove) :
            base(from, to)
        {
            this.pawnToRemove = pawnToRemove;
        }
        public EnPassant(Square fromSquare, Square toSquare, Square pawnToRemove) :
            base(new sbyte[] { fromSquare.rank, fromSquare.line }, new sbyte[] { toSquare.rank, toSquare.line })
        {
            this.pawnToRemove = new sbyte[] { pawnToRemove.rank, pawnToRemove.line };
        }
        public override Position execute(Position oldPos)
        {
            Position newPos = oldPos.deepCopy();

            newPos.board[to[0], to[1]] = oldPos.board[from[0], from[1]];
            newPos.board[from[0], from[1]] = '\0';
            newPos.board[pawnToRemove[0], pawnToRemove[1]] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare = new sbyte[2] { -1, -1 };
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
        char promoteTo;
        public Promotion(sbyte[] from, sbyte[] to, char promoteTo) :
            base(from, to)
        {
            this.promoteTo = promoteTo;
        }
        public Promotion(Square fromSquare, Square toSquare, char promoteTo) :
            base(new sbyte[] { fromSquare.rank, fromSquare.line }, new sbyte[] { toSquare.rank, toSquare.line })
        {
            this.promoteTo = promoteTo;
        }
        public override Position execute(Position oldPos)
        {
            Position newPos = oldPos.deepCopy();

            newPos.board[to[0], to[1]] = promoteTo;
            newPos.board[from[0], from[1]] = '\0';
            if (!oldPos.whiteToMove)
            {
                newPos.moveCounter++; //Om det var svarts drag, så öka antalet spelade drag i partiet.
            }
            newPos.enPassantSquare = new sbyte[2] { -1, -1 };
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

