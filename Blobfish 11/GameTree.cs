using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    using Variation = Tuple<Move, GameTree>;
    public class GameTree
    {
        readonly Position pos;
        //Kommentarer?
        public GameTree parent { get; }
        public List<Variation> continuations = new List<Variation>();
        public GameTree()
        {
            this.pos = Game.startingPosition;
            this.parent = null;
        }
        public GameTree(Position position)
        {
            this.pos = position;
            this.parent = null;
        }
        public GameTree(Position position, GameTree parent)
        {
            this.pos = position;
            this.parent = parent;
        }
        public GameTree(Position position, GameTree parent, List<Variation> continuations)
        {
            this.pos = position;
            this.parent = parent;
            this.continuations = continuations;
        }

        public Position position { get { return pos; } }

        public GameTree addContinuation(Move move)
        {
            GameTree newTree = new GameTree(move.execute(pos), this);
            continuations.Add(new Variation(move, newTree));
            return newTree;
        }
        public GameTree continuation(int i)
        {
            if (continuations.Count > i)
                return (continuations[i]).Item2;
            else return null;
        }
        public GameTree continuation(Move move)
        {
            foreach (Variation item in continuations)
            {
                if (item.Item1.Equals(move))
                {
                    return item.Item2;
                }
            }
            return null;
        }
        public void removeContinuation(GameTree continuation)
        {
            for (int i = 0; i < continuations.Count; i++)
            {
                if(continuations[i].Item2 == continuation)
                {
                    continuations.RemoveAt(i);
                }
            }
        }
        private Move getMove(int i)
        {
            //TODO: behövs denna?
            return continuations[i].Item1;
        }
        public override string ToString()
        {
            string ret = "";
            if (continuations.Count > 0)
            {
                if (pos.whiteToMove)
                {
                    ret += pos.moveCounter + ".";
                }
                ret += getMove(0).toString(pos) + " ";
                if (continuations.Count > 1)
                {
                    for (int i = 1; i < continuations.Count; i++)
                    {
                        ret += " (";
                        ret += pos.moveCounter;
                        if (pos.whiteToMove)
                            ret += ".";
                        else
                            ret += "...";
                        ret += getMove(i).toString(pos);
                        string cont = continuation(i).ToString();
                        if (cont != "")
                            ret += " " + cont;
                        ret += ") ";
                    }
                    string mainLine = continuation(0).ToString();
                    if(mainLine != "")
                    {
                        if (pos.whiteToMove)
                        {
                            ret += pos.moveCounter;
                            ret += "...";
                        }
                        ret += mainLine;
                    }
                }
                else
                {
                    ret += continuation(0).ToString();
                }
            }
            return ret;
        }
    }
}
