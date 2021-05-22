using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public static class PGNHandler
    {
        private static Engine engine = new Engine();
        public static void save(Game game)
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
        public static Game load()
        {
            Game game = new Game();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "pgn files (*.pgn)|*.pgn";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                string filename = ofd.FileName;
                string[] filelines = File.ReadAllLines(filename);

                List<string> moves = new List<string>();
                List<string> metaData = new List<string>();

                foreach (string line in filelines)
                {
                    if (line.Length == 0)
                        continue;
                    else if (line[0] == '[')
                        metaData.Add(line);
                    else
                        moves.Add(line);
                }


                List<string> moveNames = getMoveNames(moves);

                foreach (string moveName in moveNames)
                {
                    Move move = getMove(moveName, game.currentPosition);
                    game.addMove(move);
                }

                setMetaData(metaData, game);

                return game;
            }

            return null;
        }
        private static List<string> getMoveNames (List<string> inMoves)
        {
            string allMoves = String.Join(" ", inMoves);
            List<string> outMoves = new List<string>();

            string[] tokens = allMoves.Split(' ');

            string[] possibleResults = { "1-0", "0-1", "1/2-1/2", "*"};
            foreach (string token in tokens)
            {
                if(token.Contains('.'))
                {
                    int indexOfDot = token.IndexOf(".");
                    string moveName = token.Substring(indexOfDot+1, token.Length - indexOfDot-1);
                    if (moveName != "")
                        outMoves.Add(moveName);

                    continue;
                }
                if(possibleResults.Contains(token))
                {
                    continue;
                }
                outMoves.Add(token);
                Console.WriteLine(token);
            }
            return outMoves;
        }
        private static void setMetaData(List<string> gameStrings, Game game)
        {
            foreach (string line in gameStrings)
            {
                if (line[0] != '[' || line[line.Length-1] != ']')
                    break;

                string metaField = line.Substring(1, line.IndexOf(' '));
                metaField = metaField.ToLower().Trim();

                //Assuming the value goes on until the second last character.
                int firstQuote = line.IndexOf('\"')+1;
                string metaValue = line.Substring(firstQuote, (line.Length-2)-firstQuote);
                try
                {
                    switch (metaField)
                    {
                        case "event": game.gameEvent = metaValue; break;
                        case "result": game.result = getGameResult(metaValue); break;
                        case "white": game.players[0] = metaValue; break;
                        case "black": game.players[1] = metaValue; break;
                        case "site": game.site = metaValue; break;
                        case "date": game.date = metaValue; break;
                        case "round": game.round = metaValue; break;
                        case "eco": game.eco = metaValue; break;
                        case "whiteelo": game.eloRatings[0] = int.Parse(metaValue); break;
                        case "blackelo": game.eloRatings[1] = int.Parse(metaValue); break;
                        default: break;
                    }
                }catch (Exception e)
                {
                    MessageBox.Show("Error when parsing metadata: " + e.Message, "Exception!");
                }
            }
        }
        private static GameResult getGameResult(string result)
        {
            if (result == "1-0")
                return GameResult.WhiteWin;
            if (result == "0-1")
                return GameResult.BlackWin;
            if (result == "1/2-1/2")
                return GameResult.DrawByRepetition;
            return GameResult.Undecided;
        }

        private static Move getMove(string moveName, Position currentPosition) {
            moveName = moveName.Replace("+", "");
            moveName = moveName.Replace("#", "");
            List<Move> allMoves = engine.allValidMoves(currentPosition, false);
            foreach (Move move in allMoves)
            {
                if (move.toString(currentPosition) == moveName)
                {
                    Console.WriteLine(moveName);
                    return move;
                }
            }
            throw new Exception("No move with the name: " + moveName);
        }

        private static char getSymbol(int NAGNumber)
        {
            int symbolNumber(int number)
            {
                switch (number)
                {
                    case 1: return 0x0021;
                    case 2: return 0x003F;
                    case 3: return 0x203C;
                    case 4: return 0x2047;
                    case 5: return 0x2049;
                    case 6: return 0x2048;
                    case 7: return 0x25A1;

                    case 10: return 0x003D;

                    case 13: return 0x221E;
                    case 14: return 0x2A72;
                    case 15: return 0x2A71;
                    case 16: return 0x00B1;
                    case 17: return 0x2213;

                    default: return 36;
                }
            }
            return (char)symbolNumber(NAGNumber);
        }
    }
}
