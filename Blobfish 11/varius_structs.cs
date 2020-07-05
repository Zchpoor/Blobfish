using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public struct ControlAndCheckingPieces
    {
        public SquareControl[,] board;
        public int[] checkingPieces;
    }
    public struct EvalResult
    {
        public double evaluation;
        public List<Move> allMoves;
        public List<SecureDouble> allEvals;
        public Move bestMove;
    }
    public struct Square
    {
        public int rank;
        public int line;
        public Square(int rank, int line)
        {
            this.rank = rank;
            this.line = line;
        }
    }
    public struct PieceData
    {
        public List<Move> moves;

        //TODO: Byt ut mot bräde av fält?
        public List<Square> controlledSquares;

        public bool givesCheck;
    }
    public class SecureDouble
    {
        public Mutex mutex;
        public double value;
        public SecureDouble(){
            this.value = double.NaN;
            this.mutex = new Mutex();
        }
    }
}