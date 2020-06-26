using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    class Tests
    {
        //TODO: Lägg till fler tester.
        static public string runTests()
        {
            return testNumberOfMoves();
        }
        static private string testNumberOfMoves()
        {
            string detailedResult = "Antal drag:" + Environment.NewLine;
            int testCounter = 1;
            bool testFailed = false;
            Engine blobfish = new Engine();

            bool makeTest(string FEN, int moves)
            {
                Position pos = new Position(FEN);
                EvalResult result = blobfish.eval(pos, 1);
                bool success = result.allMoves.Count == moves;
                if (success) detailedResult += "  Test " + testCounter.ToString() + ": Lyckades" + Environment.NewLine;
                else
                {
                    detailedResult += "  Test " + testCounter.ToString() + ": Misslyckades" + Environment.NewLine;
                    testFailed = true;
                }
                testCounter++;
                return success;
            }

            makeTest("8/8/1b6/8/1k6/8/3rP1K1/8 w - - 0 1", 6); //Spikad vit bonde.
            makeTest("5k2/8/3r4/3B4/3K4/8/8/8 w - - 0 1", 7); //Spikad vit löpare.
            makeTest("kb5q/8/8/8/5R1B/8/r5PK/8 w - - 0 1", 4); //Tre spikar på vit
            makeTest("K6Q/2B5/8/8/7r/6n1/R5pk/8 b - - 0 1", 8); //Tre spikar på svart
            makeTest("rnbqkbnr/pp2pppp/8/2ppP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", 31); //En passant
            //5
            makeTest("rnbqkb1r/pppp1ppp/5n2/3PpP2/8/8/PPP1P1PP/RNBQKBNR w KQkq e6 0 5", 30); //Dubbel en passant för vit
            makeTest("rnbqkbnr/ppp1p1pp/8/8/3pPp2/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 5", 31); //Dubbel en passant för svart
            makeTest("8/5P2/8/8/k7/8/8/5K2 w - - 0 1", 9); //Enkel promotering.
            makeTest("4nbn1/5P2/8/8/k7/8/8/5K2 w - - 0 1", 13); //Dubbel promotering.
            makeTest("4n1n1/5P2/8/8/k7/8/8/5K2 w - - 0 1", 17); //Trippel promotering.
            //10
            makeTest("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 20); //Utgångsställningen
            makeTest("r1b1qr2/ppp1k1pp/2np4/2bnp1N1/2B1P3/8/PPPP1PPP/R1BQ1RK1 w - - 0 10", 31); //Traxler
            makeTest("r1bq1rk1/pppnn1bp/3p4/3Pp1p1/2P1Pp2/2N2P2/PP2BBPP/R2QNRK1 w - - 0 13", 33); //Mar del plata
            makeTest("r3k2r/3b1p1p/pq1ppp2/1p2bP2/4P3/3Q2P1/PPP1N2P/1K1R1B1R b kq - 0 16", 36); //Kozul
            makeTest("r3k2r/3b1p1p/pq1ppp2/1p2bP2/4P3/3Q2P1/PPP1N2P/1K1R1B1R b - - 4 18", 34); //Kozul utan rockader
            //15

            if (testFailed)
                return detailedResult;
            else
                return "Antal drag-tester: Felfritt!";

        }

    }
}
