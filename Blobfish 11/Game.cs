using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public class Game
    {
        private static readonly Position startingPosition = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        private static readonly Exception playerVectorException = new Exception("Spelarvektorn måste innehålla exakt 2 element.");

        List<Move> moves = new List<Move>();
        List<Position> positions = new List<Position>();
        private string[] playerNames;
        private string resultString = "*";

        public Game()
        {
            positions.Add(startingPosition);
            players = new string[] { "Human player", "Blobfish 11" };
        }
        public Game(Position customStartingPosition)
        {
            positions.Add(customStartingPosition);
            players = new string[] { "Human player", "Blobfish 11" };
        }
        public Game(Position customStartingPosition, string[] players)
        {
            positions.Add(customStartingPosition);
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
        public string result
        {
            get { return resultString; }
            set
            {
                resultString = value;
            }
        }
        public Position getPosition(int posNumber)
        {
            return positions[posNumber];
        }
        public Move getMove(int moveNumber)
        {
            return moves[moveNumber];
        }
        public int length
        {
            get { return moves.Count; }
        }
        public void playMove(Move move)
        {
            Position newPosition = move.execute(positions[positions.Count-1]);
            positions.Add(newPosition);
            this.moves.Add(move);
        }
        public string scoresheet()
        {
            string scoresheet = "";
            if (positions.Count != moves.Count + 1)
            {
                throw new Exception("Fel antal drag/ställningar har spelats!");
            }
            else if (moves.Count == 0)
            {
                scoresheet = "Inga drag har spelats!";
            }
            else
            {
                int initialMoveNumber = positions[0].moveCounter;
                for (int i = 0; i < moves.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        if (i != 0)
                        {
                            scoresheet += Environment.NewLine;
                        }
                        scoresheet += ((i / 2) + initialMoveNumber).ToString() + ".";
                    }
                    scoresheet += " " + moves[i].toString(positions[i]);
                }
            }
            return scoresheet;
        }
        public Position lastPosition()
        {
            return positions[positions.Count - 1];
        }
        public void takeback(int numberOfMoves)
        {
            if (positions.Count > numberOfMoves)
            {
                for (int i = 0; i < numberOfMoves; i++)
                {
                    positions.RemoveAt(positions.Count - 1);
                    moves.RemoveAt(moves.Count - 1);
                }
                Position newCurrentPosition = positions[positions.Count - 1];
                positions.RemoveAt(positions.Count - 1);
            }
            else
            {
                throw new Exception("För få drag har spelats!");
            }
        }
    }
}
