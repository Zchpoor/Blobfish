using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public partial class Form1 : Form
    {
        PictureBox[,] Falt = new PictureBox[8, 8];
        List<Position> gamePositions = new List<Position>();
        List<Move> gameMoves = new List<Move>();
        Position currentPosition;
        List<Move> currentMoves = new List<Move>();
        int[] firstSquare = { -1, -1 };
        public Form1()
        {
            InitializeComponent();

            int squareSize = 50;
            boardPanel.AutoSize = true;
            moveLabel.Text = "";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox picBox = new PictureBox();
                    Falt[i, j] = picBox;
                    boardPanel.Controls.Add(picBox);

                    picBox.MinimumSize = new Size(squareSize, squareSize);
                    picBox.MaximumSize = new Size(squareSize, squareSize);
                    picBox.Dock = DockStyle.Fill;
                    picBox.SizeMode = PictureBoxSizeMode.Zoom;
                    picBox.Margin = new Padding(0);
                    picBox.Padding = new Padding(0);
                    picBox.Image = Image.FromFile("null.png");
                    picBox.MouseClick += new MouseEventHandler(squareClick);
                    if ((i + j) % 2 == 0)
                        picBox.BackColor = Color.WhiteSmoke;
                    else
                        picBox.BackColor = Color.SandyBrown;
                }
            }
            Position startingPostition = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            display(startingPostition);
        }
        private void display(Position pos)
        {
            gamePositions.Add(pos);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char piece = pos.board[i, j];
                    string picName = "null.png";
                    if (piece != '\0')
                    {
                        if (piece > 'Z')
                        {
                            picName = "B" + piece + ".png";
                        }
                        else
                        {
                            picName = "W" + piece + ".png";
                        }
                    }

                    Falt[i, j].Image = Image.FromFile(picName);
                }
            }

            //TEST
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //TEST

            toMoveLabel.Text = pos.whiteToMove ? "Vit vid draget." : "Svart vid draget.";

            //TODO: Flytta ut
            //TODO: Få bort "globala" variabler.
            currentPosition = pos;
            if ((radioButton2.Checked && pos.whiteToMove) || (radioButton3.Checked && !pos.whiteToMove))
            {
                Engine blobFish = new Engine();
                EvalResult result = blobFish.eval(pos, 3);
                double eval = result.evaluation;
                currentMoves = result.allMoves;
                string movesString = getMovesString(currentMoves, result.allEvals, currentPosition.board);
                textBox1.Text = movesString;
                int res = blobFish.decisiveResult(pos, currentMoves);
                if (res != -2)
                {
                    if (res == 1000)
                    {
                        MessageBox.Show("Vit vann på schack matt!");
                    }
                    else if (res == -1000)
                    {
                        MessageBox.Show("Svart vann på schack matt!");
                    }
                    else
                    {
                        MessageBox.Show("Partiet slutade remi!");
                    }
                }
                else if (result.bestMove != null)
                {
                    evalBox.Text = "Bästa drag: " + result.bestMove.toString(pos.board) + 
                        Environment.NewLine + "Evaluering: " + Math.Round(eval, 2);
                    gameMoves.Add(result.bestMove);
                    display(result.bestMove.execute(pos));
                }
                else
                {
                    throw new Exception("Odefinierat bästa drag.");
                }
            }
            else
            {
                Engine blobFish = new Engine();
                currentMoves = blobFish.allValidMoves(pos);
                string temp = getMovesString(currentMoves, currentPosition.board);
                textBox1.Text = temp;

                //TODO: Fakorera ut detta med identisk kod ovan.
                int res = blobFish.decisiveResult(pos, currentMoves);
                if (res != -2)
                {
                    if (res == 1000)
                    {
                        MessageBox.Show("Vit vann på schack matt!");
                    }
                    else if (res == -1000)
                    {
                        MessageBox.Show("Svart vann på schack matt!");
                    }
                    else
                    {
                        MessageBox.Show("Partiet slutade remi!");
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string inputText = fenBox.Text;
            string lowerInput = inputText.ToLower();

            switch (lowerInput)
            {
                case "test":
                    evalBox.Text = Tests.runTests();
                    break;
                case "takeback":
                    takeback(2);
                    break;
                case "tb":
                    takeback(2);
                    break;
                case "återta":
                    takeback(2);
                    break;
                case "stb":
                    takeback(1);
                    break;
                case "scoresheet":
                    evalBox.Text = scoresheet();
                    break;
                case "protokoll":
                    evalBox.Text = scoresheet();
                    break;
                case "reset":
                    reset();
                    break;
                case "omstart":
                    reset();
                    break;
                default:
                    try
                    {
                        Position pos = new Position(inputText);
                        this.gamePositions.Clear();
                        this.gameMoves.Clear();
                        display(pos);

                    }
                    catch
                    {
                        evalBox.Text = "Felaktig FEN!";
                        return;
                    }
                    break;
            }
        }
        private void takeback(int numberOfMoves)
        {
            if (gamePositions.Count > numberOfMoves)
            {
                for (int i = 0; i < numberOfMoves; i++)
                {
                    gamePositions.RemoveAt(gamePositions.Count - 1);
                    gameMoves.RemoveAt(gameMoves.Count - 1);
                }
                Position newCurrentPosition = gamePositions[gamePositions.Count - 1];
                gamePositions.RemoveAt(gamePositions.Count - 1);

                display(newCurrentPosition);
            }
            else
            {
                evalBox.Text = "För få drag har spelats!";
            }
            //TODO: Ta bort ytterligare ett drag?
        }
        private string scoresheet()
        {
            string scoresheet = "";
            if(gamePositions.Count != gameMoves.Count+1)
            {
                throw new Exception("Fel antal drag/ställningar har spelats!");
            }
            else if(gameMoves.Count == 0)
            {
                scoresheet = "Inga drag har spelats!";
            }
            else
            {
                for (int i = 0; i < gameMoves.Count; i++)
                {
                    scoresheet += gameMoves[i].toString(gamePositions[i].board) + Environment.NewLine;
                }
            }
            return scoresheet;
        }
        private void squareClick(object sender, MouseEventArgs e)
        {
            int xVal = ((PictureBox)sender).Location.X; //a-h
            int yVal = ((PictureBox)sender).Location.Y; //1-8
            xVal = xVal / (boardPanel.Size.Width / 8);
            yVal = yVal / (boardPanel.Size.Height / 8);
            int[] newSquare = { yVal, xVal };
            if (firstSquare[0] == -1)  //-1 indikerar att ingen ruta tidigare markerats.
            {
                firstSquare = newSquare;
                moveLabel.Text = (char)(xVal + 'a') + (8 - yVal).ToString();
            }
            else
            {
                moveLabel.Text = (char)(firstSquare[1] + 'a') + (8 - firstSquare[0]).ToString() + "-" +
                    (char)(xVal + 'a') + (8 - yVal).ToString();
                foreach (Move item in currentMoves)
                {
                    if (firstSquare[0] == item.from[0] && firstSquare[1] == item.from[1] &&
                        newSquare[0] == item.to[0] && newSquare[1] == item.to[1])
                    {
                        Position newPosition = item.execute(currentPosition);
                        this.currentPosition = newPosition;
                        this.gameMoves.Add(item);
                        this.display(newPosition);
                        break;
                    }
                }
                firstSquare[0] = -1;
                firstSquare[1] = -1;
                moveLabel.Text = "";
            }
        }
        private void fenBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                button1_Click(null, null);
        }
        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            gamePositions.RemoveAt(gamePositions.Count - 1);
            display(currentPosition);
        }
        public string getMovesString(List<Move> moves, char[,] board)
        {
            string text = "";
            foreach (Move item in moves)
            {
                text += item.toString(board) + Environment.NewLine;
            }
            return text;
        }
        public string getMovesString(List<Move> moves, List<SecureDouble> evals, char[,] board)
        {
            if (moves == null || evals == null) return "";
            if (moves.Count != evals.Count)
                throw new Exception("Olika antal evalueringar och drag!");

            string text = "";
            for (int i = 0; i < moves.Count; i++)
            {
                text += moves[i].toString(board) + "    " + Math.Round(evals[i].value, 2).ToString() + Environment.NewLine;
            }
            return text;
        }
        private void reset()
        {
            gamePositions.Clear();
            gameMoves.Clear();
            display(new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));
        }
    }
}

/*
 * TODO:
 * Bekvämligheter:
 *  +/#
 *  Byt ut: row -> rank, column -> line. 
 *  Byt ut int[] -> Square.
 *  Fler tester.
 *  Få FEN
 *  Välja pjäs att promotera till.
 *  Vända på brädet.
 *  Se matieral.
 *  Se bästa variant
 * 
 * Justera matriserna:
 *  Gör torn assymmetriska?
 *  Minska behov av terräng.
 *  Föredra springare före löpare.
 * 
 * Effektiviseringar:
 *  Sortera efter uppskattad kvalitet på draget.
 *  Effektivisera algoritmer för dragberäkning.
 *  Minimera minnesanvändning
 *  Kapa de längsta slagväxlingarna.
 *  Få alfa/beta mellan de olika trådarna.
 *  
 * Förbättringar:
 *  Variera djup utifrån antal pjäser.
 *  Kungssäkerhet
 *  Få schackar att också förlänga varianterna.
 */
