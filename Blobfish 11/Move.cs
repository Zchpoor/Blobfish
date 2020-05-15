using System;
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
        public virtual string toString(Square[,] board)
        {
            //TODO: Checks etc.
            string ret = "";
            if (board[from[0], from[1]].piece != 'p' && board[from[0], from[1]].piece != 'P')
                ret += board[from[0], from[1]].piece;
            ret = ret.ToUpper();
            ret += ((Char)(from[1] + 'a')).ToString();
            ret += 8 - from[0];
            if (board[to[0], to[1]].piece != '\0')
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
        public virtual Square[,] execute(Square[,] board)
        {
            //TODO: return position, or other solution.
            board[to[0], to[1]] = board[from[0], from[1]];
            board[from[0], from[1]].piece = '\0';
            return board;
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
        public override Square[,] execute(Square[,] board)
        {
            board[to[0], to[1]] = board[from[0], from[1]];
            board[from[0], from[1]].piece = '\0';
            board[rookTo[0], rookTo[1]] = board[rookFrom[0], rookFrom[1]];
            board[rookFrom[0], rookFrom[1]].piece = '\0';
            return board;
        }
        public override string toString(Square[,] board)
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
        public override Square[,] execute(Square[,] board)
        {
            board[to[0], to[1]] = board[from[0], from[1]];
            board[from[0], from[1]].piece = '\0';
            board[pawnToRemove[0], pawnToRemove[1]].piece = '\0';
            return board;
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
        public override Square[,] execute(Square[,] board)
        {
            board[to[0], to[1]].piece = promoteTo;
            board[from[0], from[1]].piece = '\0';
            return board;
        }
        public override string toString(Square[,] board)
        {
            return base.ToString() + "=" + promoteTo.ToString().ToUpper();
        }
    }
}

