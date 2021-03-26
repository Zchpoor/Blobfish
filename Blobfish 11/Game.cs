using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public class Game
    {
        public static readonly Position startingPosition = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        private static readonly Exception playerVectorException = new Exception("Spelarvektorn måste innehålla exakt 2 element.");

        private GameTree firstGameTreeNode;
        private GameTree gameTree;
        private string[] playerNames;

        public Game()
        {
            gameTree = new GameTree();
            firstGameTreeNode = gameTree;
            players = new string[] { "Human player", "Blobfish 11" };
        }
        public Game(Position customStartingPosition)
        {
            gameTree = new GameTree(customStartingPosition);
            firstGameTreeNode = gameTree;
            players = new string[] { "Human player", "Blobfish 11" };
        }
        public Game(Position customStartingPosition, string[] players)
        {
            gameTree = new GameTree(customStartingPosition);
            firstGameTreeNode = gameTree;
            this.players = players;
        }
        public string[] players
        {
            get { return playerNames; }
            set
            {
                if (value.Length == 2)
                {
                    playerNames = value;
                }
                else
                {
                    throw playerVectorException;
                }
            }

        }
        public string result { get; set; } = "*";
        public Position currentPosition { get { return gameTree.position; } }
        public Position firstPosition { get { return firstGameTreeNode.position; } }
        public void addMove(Move move)
        {
            GameTree gt = gameTree.continuation(move);
            if (gt != null)
            {
                gameTree = gt;
            }
            else
            {
                gameTree = gameTree.addContinuation(move);
            }
        }
        public string scoresheet()
        {
            return firstGameTreeNode.ToString();
        }
        public void mainContinuation()
        {
            if (gameTree.continuations.Count > 0)
                gameTree = gameTree.continuations[0].Item2;
        }
        public void goBack()
        {
            if (gameTree.parent != null)
                gameTree = gameTree.parent;
        }
        public void goToLastPosition()
        {
            while (gameTree.continuations.Count != 0)
            {
                mainContinuation();
            }
        }
        public void goToFirstPosition()
        {
            gameTree = firstGameTreeNode;
        }
        public void takeback(int numberOfMoves)
        {
            if(gameTree.parent == null || numberOfMoves <= 0)
            {
                return;
            }
            else
            {
                gameTree.parent.removeContinuation(gameTree);
                gameTree = gameTree.parent;
                takeback(numberOfMoves - 1);
            }

        }
    }
}
