using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public class PGNHandler
    {
        public void save(Game game, string result, string[] players)
        {
            string text = "";
            text += "[Event \"Blobfish game\"]\n";
            text += "[Site \"?\"]\n";
            text += "[Date \"" + DateTime.Now.ToString("yyyy.MM.dd") + "\"]\n";
            text += "[Round \"-\"]\n";
            text += "[White \""+ players[0]+"\"]\n";
            text += "[Black \"" + players[1] + "\"]\n";
            text += "[Result \"" + result + "\"]\n";
            if(game.getPosition(0).getFEN() != "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
            {
                //Om partiet inte startade i utgångsställningen så läggs ett fält till för FEN.
                text += "[SetUp \"1\"]\n";
                text += "[FEN \"" + game.getPosition(0).getFEN() + "\"]\n";
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "pgn files (*.pgn)|*.pgn";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            sfd.FileName = "MyGame.pgn";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                using (var fileStream = new FileStream(sfd.FileName, FileMode.Create))
                {
                    text += "\n" + moveText(game);
                    text += " " + result;
                    byte[] byteArray = Encoding.ASCII.GetBytes(text);
                    fileStream.Write(byteArray, 0, text.Length);
                }
            }
        }

        private string moveText(Game game)
        {
            string text = "";
            int initialMoveNumber = game.getPosition(0).moveCounter;
            int i = 0;
            if (!game.getPosition(0).whiteToMove)
            {
                text += initialMoveNumber.ToString() + "... " + game.getMove(i).toString(game.getPosition(i));
                i=1;
                text += " " + (initialMoveNumber + 1).ToString() + ".";
            }
            while (i < game.length)
            {
                if (i % 2 == 0)
                {
                    if (i != 0)
                    {
                        text += " ";
                    }
                    text += ((i / 2) + initialMoveNumber).ToString() + ".";
                }
                text += " " + game.getMove(i).toString(game.getPosition(i));
                i++;
            }
            return text;
        }
    }
}
