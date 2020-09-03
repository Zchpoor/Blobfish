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
            return testNumberOfMoves() + Environment.NewLine + testForcedMates();
        }
        static private string testNumberOfMoves()
        {
            string detailedResult = "Antal drag-tester:" + Environment.NewLine;
            int testCounter = 1;
            bool testFailed = false;
            Engine blobfish = new Engine();

            bool makeTest(string FEN, int moves)
            {
                Position pos = new Position(FEN);
                List<Move> allMoves = blobfish.allValidMoves(pos);
                bool success = allMoves.Count == moves;
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
            makeTest("4k3/8/8/8/8/4b3/P6P/R3K2R w KQ - 0 1", 12); //Ingen rockad tillåten för vit.
            makeTest("4k3/8/8/8/8/3b4/P6P/R3K2R w KQ - 0 1", 13); //b1 garderat. Kort rockad ej tillåtet.
            makeTest("4k3/8/8/8/4b3/8/P6P/R3K2R w KQ - 0 1", 16); //b1 och h1 garderat. Bägge rockaderna tillåtna.
            makeTest("r3k2r/p6p/8/8/8/1Q6/8/4K3 b kq - 0 1", 14); //b8 garderat. Kort rockad otillåtet för svart.
            makeTest("r3k2r/p6p/8/8/8/8/4R3/4K3 b kq - 0 1", 4); //Svart står i schack, med rockadmöjligheter.
            //15
            makeTest("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 20); //Utgångsställningen
            makeTest("r1b1qr2/ppp1k1pp/2np4/2bnp1N1/2B1P3/8/PPPP1PPP/R1BQ1RK1 w - - 0 10", 31); //Traxler
            makeTest("r1bq1rk1/pppnn1bp/3p4/3Pp1p1/2P1Pp2/2N2P2/PP2BBPP/R2QNRK1 w - - 0 13", 33); //Mar del plata
            makeTest("r3k2r/3b1p1p/pq1ppp2/1p2bP2/4P3/3Q2P1/PPP1N2P/1K1R1B1R b kq - 0 16", 36); //Kozul
            makeTest("r3k2r/3b1p1p/pq1ppp2/1p2bP2/4P3/3Q2P1/PPP1N2P/1K1R1B1R b - - 4 18", 34); //Kozul utan rockader
            //20
            makeTest("rnbqkbnr/pp2pppp/3p4/1Bp5/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 3", 4); //Moskva
            makeTest("rnbqkbnr/pp1ppppp/2p5/8/2P5/8/PP1PPPPP/RNBQKBNR w KQkq - 0 2", 22); //1.c4 c6
            makeTest("1rb4Q/ppppk2p/2n2P2/4p3/2Bb4/2N5/PPP2PPP/R3K1NR b KQ - 0 1", 1); //Udda ställning. 1 giltigt drag.
            makeTest("8/p5pp/4p1k1/4P3/P3P2P/1P1K1b2/8/8 b - - 0 1", 14); //Slutspel med en bonde för pjäs.
            makeTest("r1bqk2r/pp2ppbp/2np1np1/8/3NP3/4BP2/PPPQ2PP/RN2KB1R w KQkq - 3 8", 42); //Sic, springare blockerar lång rockad.
            //25
            makeTest("6k1/8/8/8/8/8/4p3/1K6 b - - 0 1", 9); //Promotering för svart.


            if (testFailed)
                return detailedResult;
            else
                return "Antal drag-tester: Felfritt!";
        }
        static private string testForcedMates()
        {
            string testName = "Forcerad matt-tester";
            string detailedResult = testName + ": " + Environment.NewLine;
            int testCounter = 1;
            bool testFailed = false;
            Engine blobfish = new Engine();

            bool makeTest(string FEN, int movesToMate)
            {
                Position pos = new Position(FEN);
                EvalResult result = blobfish.eval(pos, movesToMate + 1);
                int plysToMate = (movesToMate * 2) -1;
                bool success = result.evaluation == (pos.whiteToMove ? (2001 - plysToMate) : (-2001 + plysToMate));
                if (success) detailedResult += "  Test " + testCounter.ToString() + ": Lyckades" + Environment.NewLine;
                else
                {
                    detailedResult += "  Test " + testCounter.ToString() + ": Misslyckades" + Environment.NewLine;
                    testFailed = true;
                }
                testCounter++;
                return success;
            }
            
            makeTest("r1bqkb1r/pppp1ppp/2n2n2/4p2Q/2B1P3/8/PPPP1PPP/RNB1K1NR w KQkq - 4 4", 1); //Skolmatt
            makeTest("r1b1k2r/ppppqppp/2n5/4n3/1PP2B2/5N2/1P1NPPPP/R2QKB1R b KQkq - 0 8", 1); //Budapestmatten
            makeTest("8/6k1/R7/1R6/8/8/8/4K3 w - - 0 1", 2); //Trappstegsmatt
            makeTest("r7/ppp2kpp/2nb4/5K2/2PP1P1q/2N5/PP1Q2PP/R4BNR b - - 0 16", 2); //Portugisiskt
            makeTest("r4rk1/pp3Npp/1b6/8/2Q5/P6P/1q3PP1/R4RK1 w - - 0 1", 3); //Kvävmatt

            if (testFailed)
                return detailedResult;
            else
                return testName + ": Felfritt!";
        }
    }
}
