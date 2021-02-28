using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    class PGNHandler
    {
        public void save(List<Position> gamePositions, List<Move> gameMoves, string result)
        {
            string text = "";
            text += "[Event \"Blobfish game\"]\n";
            text += "[Site \"?\"]\n";
            text += "[Date \"" + DateTime.Now.ToString("yyyy.MM.dd") + "\"]\n";
            text += "[Round \"-\"]\n";
            text += "[White \"?, ?\"]\n";
            text += "[Black \"?, ?\"]\n";
            text += "[Result \"" + result + "\"]\n";
            if(gamePositions[0].getFEN() != "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
            {
                //Om partiet inte startade i utgångsställningen så läggs ett fält till för FEN.
                text += "[SetUp \"1\"]";
                text += "[FEN \"" + gamePositions[0].getFEN() + "\"]\n";
            }

            using (var fileStream = new FileStream("MyGame.txt", FileMode.Create))
            {
                text += "\n" + moveText(gamePositions, gameMoves);
                text += " " + result;
                byte[] byteArray = Encoding.ASCII.GetBytes(text);
                fileStream.Write(byteArray, 0, text.Length);
            }
        }
        private string moveText(List<Position> gamePositions, List<Move> gameMoves)
        {
            if (gamePositions.Count != gameMoves.Count + 1)
            {
                throw new Exception("Fel antal drag/ställningar har spelats!");
            }
            string text = "";
            int initialMoveNumber = gamePositions[0].moveCounter;
            int i = 0;
            if (!gamePositions[0].whiteToMove)
            {
                text += initialMoveNumber.ToString() + "... " + gameMoves[i].toString(gamePositions[i]);
                i=1;
                text += " " + (initialMoveNumber + 1).ToString() + ".";
            }
            while (i < gameMoves.Count)
            {
                if (i % 2 == 0)
                {
                    if (i != 0)
                    {
                        text += " ";
                    }
                    text += ((i / 2) + initialMoveNumber).ToString() + ".";
                }
                text += " " + gameMoves[i].toString(gamePositions[i]);
                i++;
            }
            return text;
        }
    }
}
