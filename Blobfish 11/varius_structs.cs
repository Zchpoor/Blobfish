using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public struct EvalResult
    {
        public double evaluation;
        public List<Move> allMoves;
        public List<SecureDouble> allEvals;
        public Move bestMove;
    }
    public struct Square
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
    }
    public abstract class DoubleContainer
    {
        protected double val;
        public abstract double getValue();
        public abstract void setValue(double value);
    }
    public class OrdinaryDouble : DoubleContainer
    {
        public OrdinaryDouble()
        {
            this.val = double.NaN;
        }
        public OrdinaryDouble(double value)
        {
            this.val = value;
        }
        public override double getValue()
        {
            return val;
        }
        public override void setValue(double value)
        {
            this.val = value;
        }
    }
    public class SecureDouble : DoubleContainer
    {
        public Mutex mutex;
        public SecureDouble()
        {
            this.val = double.NaN;
            this.mutex = new Mutex();
        }
        public override double getValue()
        {
            double ret;
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
        public override void setValue(double value)
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
}