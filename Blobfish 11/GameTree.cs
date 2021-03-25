﻿using System;
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
        Position pos;
        //Kommentarer?
        GameTree parent;
        List<Variation> continuations = new List<Variation>();
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
        public GameTree addContinuation(Move move)
        {
            GameTree newTree = new GameTree(move.execute(pos), this);
            continuations.Add(new Variation(move, newTree));
            return newTree;
        }
        public GameTree goBack()
        {
            if(parent == null)
                return this;
            else
                return parent;
        }
        public GameTree continuation(int i)
        {
            return (continuations[i]).Item2;
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
        public Move getMove(int i)
        {
            return continuations[i].Item1;
        }
        public string toString()
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
                    ret += " (";
                    for (int i = 1; i < continuations.Count; i++)
                    {
                        ret += pos.moveCounter;
                        if (pos.whiteToMove)
                            ret += ".";
                        else
                            ret += "...";
                        ret += getMove(i).toString(pos);
                        string cont = continuation(i).toString();
                        if (cont != "")
                            ret += " " + cont;
                        if (i < continuations.Count - 1)
                            ret += "; ";
                    }
                    ret += ") ";
                    string mainLine = continuation(0).toString();
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
                    ret += continuation(0).toString();
                }
            }
            return ret;
        }
    }
}
