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
        public void save(Game game)
        {
            try
            {
                string gameResultText;
                if (game.result == GameResult.WhiteWin) gameResultText = "1-0";
                if (game.result == GameResult.BlackWin) gameResultText = "0-1";
                if (game.result == GameResult.Undecided) gameResultText = "*";
                else gameResultText = "1/2-1/2";
                
                //TODO: Use stringBuilder.
                string text = "";
                text += "[Event \"Blobfish game\"]\n";
                text += "[Site \"?\"]\n";
                text += "[Date \"" + DateTime.Now.ToString("yyyy.MM.dd") + "\"]\n";
                text += "[Round \"-\"]\n";
                text += "[White \""+ game.players[0]+"\"]\n";
                text += "[Black \"" + game.players[1] + "\"]\n";
                text += "[Result \"";
                text += gameResultText;
                text += "\"]\n";
                if(game.firstPosition.getFEN() != "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
                {
                    //Om partiet inte startade i utgångsställningen så läggs ett fält till för FEN.
                    text += "[SetUp \"1\"]\n";
                    text += "[FEN \"" + game.firstPosition.getFEN() + "\"]\n";
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
                        //TODO: Färre än 80 tecken per rad.
                        text += "\n" + game.scoresheet();
                        text += gameResultText;
                        byte[] byteArray = Encoding.ASCII.GetBytes(text);
                        fileStream.Write(byteArray, 0, text.Length);
                    }
                }
            }
            catch(IOException e)
            {
                MessageBox.Show("Ett fel inträffade när filen skulle sparas.\nFelmeddelande:\n" + e.Message);
            }
            catch(Exception e)
            {
                MessageBox.Show("Ett okänt inträffade.\nFelmeddelande:\n" + e.Message);
            }
        }
    }
}
