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
                string text = "";
                text += "[Event \"Blobfish game\"]\n";
                text += "[Site \"?\"]\n";
                text += "[Date \"" + DateTime.Now.ToString("yyyy.MM.dd") + "\"]\n";
                text += "[Round \"-\"]\n";
                text += "[White \""+ game.players[0]+"\"]\n";
                text += "[Black \"" + game.players[1] + "\"]\n";
                text += "[Result \"" + game.result + "\"]\n";
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
                        //TODO: Hantera undantag
                        text += "\n" + game.scoresheet();
                        text += game.result;
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
