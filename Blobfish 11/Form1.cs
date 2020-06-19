using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        Position currentPosition;
        List<Move> currentMoves = new List<Move>();
        int[] firstSquare = { -1, -1 };
        public Form1()
        {
            InitializeComponent();

            int squareSize = 50;
            boardPanel.AutoSize = true;
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
            Engine blobFish = new Engine();
            EvalResult result = blobFish.eval(pos, 0);
            currentMoves = result.allMoves;
            currentPosition = pos;
            string temp = getMovesString(currentMoves, currentPosition.board);
            textBox1.Text = temp;
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
        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: Remove arguments?
            if (fenBox.Text.ToLower() == "test")
            {
                bool successfulTests = runTests();
                if (!successfulTests)
                    fenBox.Text = "Tests failed!";
            }
            else
            {
                Position pos = new Position(fenBox.Text);
                evalBox.Text = "";
                Engine blobFish = new Engine();
                EvalResult result = blobFish.eval(pos, 0);
                double eval = result.evaluation;
                currentMoves = result.allMoves;
                evalBox.Text = "Evaluering: " + Math.Round(eval, 2);
                /*
                double value = pos.eval();
                evalBox.Text += "Evaluering:\n";
                evalBox.Text += "Pjäser: " + Math.Round(pos.material, 2) + "\n";
                evalBox.Text += "Bönder: " + Math.Round(pos.pawnValues[1] - pos.pawnValues[0], 2) + "\n";
                evalBox.Text += "    Vita: " + Math.Round(pos.pawnValues[1], 2) + "\n";
                evalBox.Text += "    Svarta: " + Math.Round(pos.pawnValues[0], 2) + "\n";
                evalBox.Text += "Totalt: " + Math.Round(value, 2) + "\n";
                */
                display(pos);
            }
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
                foreach (Move item in currentMoves)
                {
                    if (firstSquare[0] == item.from[0] && firstSquare[1] == item.from[1] &&
                        newSquare[0] == item.to[0] && newSquare[1] == item.to[1])
                    {
                        Position newPosition = item.execute(currentPosition);
                        this.currentPosition = newPosition;
                        this.display(newPosition);
                    }
                }
                moveLabel.Text = (char)(firstSquare[1] + 'a') + (8 - firstSquare[0]).ToString() + "-" +
                    (char)(xVal + 'a') + (8 - yVal).ToString();
                firstSquare[0] = -1;
                firstSquare[1] = -1;
            }
        }
        private void fenBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                button1_Click(null, null);
        }
        private bool runTests()
        {
            string fullResult = "";
            int testCounter = 1;
            Engine blobfish = new Engine();

            bool testNumberOfMoves(string FEN, int moves)
            {
                Position pos = new Position(FEN);
                EvalResult result = blobfish.eval(pos, 0);
                bool success = result.allMoves.Count == moves;
                if (success) fullResult+= "Test " + testCounter.ToString() + ": Success\n";
                else fullResult += "Test " + testCounter.ToString() + ": Fail\n";
                testCounter++;
                return success;
            }
            testNumberOfMoves("8/8/1b6/8/1k6/8/3rP1K1/8 w - - 0 1", 6); //Spikad vit bonde.
            testNumberOfMoves("kb5q/8/8/8/5R1B/8/r5PK/8 w - - 0 1", 4); //Tre spikar på vit
            testNumberOfMoves("K6Q/2B5/8/8/7r/6n1/R5pk/8 b - - 0 1", 8); //Tre spikar på svart
            testNumberOfMoves("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 20); //Utgångsställningen
            testNumberOfMoves("rnbqkbnr/pp2pppp/8/2ppP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", 31); //En passant

            //TODO: More tests.
            evalBox.Text = fullResult;
            return true;
        }
    }
}