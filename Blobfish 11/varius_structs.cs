using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public class EvalResult
    {
        public float evaluation;
        public List<Move> allMoves;
        public List<SecureFloat> allEvals;
        public Move bestMove;
    }
    public struct Square : IEquatable<Square>
    {
        public sbyte rank;
        public sbyte line;
        public Square(sbyte rank, sbyte line)
        {
            this.rank = rank;
            this.line = line;
        }
        public Square(int rank, int line)
        {
            this.rank = (sbyte)rank;
            this.line = (sbyte)line;
        }

        public bool Equals(Square other)
        {
            return this.rank == other.rank && this.line == other.line;
        }
    }
    public abstract class FloatContainer
    {
        protected float val;
        public abstract float getValue();
        public abstract void setValue(float value);
    }
    public class OrdinaryFloat : FloatContainer
    {
        public OrdinaryFloat()
        {
            this.val = float.NaN;
        }
        public OrdinaryFloat(float value)
        {
            this.val = value;
        }
        public override float getValue()
        {
            return val;
        }
        public override void setValue(float value)
        {
            this.val = value;
        }
    }
    public class SecureFloat : FloatContainer
    {
        public Mutex mutex;
        public SecureFloat()
        {
            this.val = float.NaN;
            this.mutex = new Mutex();
        }
        public SecureFloat(float val)
        {
            this.val = val;
            this.mutex = new Mutex();
        }
        public override float getValue()
        {
            float ret;
            try
            {
                mutex.WaitOne();
                ret = val;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
            return ret;
        }
        public override void setValue(float value)
        {
            try
            {
                mutex.WaitOne();
                this.val = value;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
    public class ThreadStartArguments
    {
        public Position pos;
        public sbyte depth;
        public SecureFloat ansPlace;
        public SecureFloat globalAlpha;
        public SecureFloat globalBeta;
        public ThreadStartArguments(Position pos, sbyte depth, SecureFloat ansPlace, SecureFloat globalAlpha, SecureFloat globalBeta)
        {
            this.pos = pos;
            this.depth = depth;
            this.ansPlace = ansPlace;
            this.globalAlpha = globalAlpha;
            this.globalBeta = globalBeta;
        }
    }
}