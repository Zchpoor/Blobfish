using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    class Tests
    {
        static public string runTests()
        {
            string fullResult = "";
            int testCounter = 1;
            Engine blobfish = new Engine();

            bool testNumberOfMoves(string FEN, int moves)
            {
                Position pos = new Position(FEN);
                EvalResult result = blobfish.eval(pos, 0);
                bool success = result.allMoves.Count == moves;
                if (success) fullResult += "Test " + testCounter.ToString() + ": Success" + Environment.NewLine;
                else fullResult += "Test " + testCounter.ToString() + ": Fail" + Environment.NewLine;
                testCounter++;
                return success;
            }
            testNumberOfMoves("8/8/1b6/8/1k6/8/3rP1K1/8 w - - 0 1", 6); //Spikad vit bonde.
            testNumberOfMoves("kb5q/8/8/8/5R1B/8/r5PK/8 w - - 0 1", 4); //Tre spikar på vit
            testNumberOfMoves("K6Q/2B5/8/8/7r/6n1/R5pk/8 b - - 0 1", 8); //Tre spikar på svart
            testNumberOfMoves("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 20); //Utgångsställningen
            testNumberOfMoves("rnbqkbnr/pp2pppp/8/2ppP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", 31); //En passant

            //TODO: More tests.
            return fullResult;
        }
    }
}
