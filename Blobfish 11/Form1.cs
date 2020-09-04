﻿using System;
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
        bool flipped = false;
        Square firstSquare = new Square(-1, -1);
        Engine blobFish = new Engine();
        const int minDepth = 3;
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
                    char piece = flipped ? pos.board[7-i, 7-j] : pos.board[i, j];
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
            if (engineIsToMove())
            {
                playEngineMove();
            }
            else
            {
                currentMoves = blobFish.allValidMoves(pos);
                string temp = getMovesString(currentMoves, currentPosition.board);
                textBox1.Text = temp;

                int res = blobFish.decisiveResult(pos, currentMoves);
                if (res != -2)
                {
                    resultPopUp(res);
                }
            }
        }
        private void playEngineMove()
        {
            EvalResult result = blobFish.eval(currentPosition, minDepth);
            currentMoves = result.allMoves;
            string movesString = getMovesString(currentMoves, result.allEvals, currentPosition.board);
            textBox1.Text = movesString;
            int decisiveResult = blobFish.decisiveResult(currentPosition, currentMoves);
            if (decisiveResult != -2)
            {
                resultPopUp(decisiveResult);
            }
            else if (result.bestMove != null)
            {
                printEval(result);
                gameMoves.Add(result.bestMove);
                display(result.bestMove.execute(currentPosition));
            }
            else
            {
                throw new Exception("Odefinierat bästa drag.");
            }
        }
        private void printEval(EvalResult result)
        {
            int decisiveResult = blobFish.decisiveResult(currentPosition, currentMoves);
            if (decisiveResult != -2)
            {
                resultPopUp(decisiveResult);
            }
            else if (result.bestMove != null)
            {
                double eval = result.evaluation;
                string textEval;
                if (eval > 1000)
                {
                    int plysToMate = (int)(2001 - eval);
                    textEval = "M" + (plysToMate / 2).ToString();
                }
                else if (eval < -1000)
                {
                    int plysToMate = (int)(2001 + eval);
                    textEval = "m-" + (plysToMate / 2).ToString();
                }
                else
                {
                    textEval = Math.Round(eval, 2).ToString();
                }
                evalBox.Text = "Bästa drag: " + result.bestMove.toString(currentPosition.board) +
                    Environment.NewLine + "Datorns evaluering: " + textEval;
            }
            else
            {
                throw new Exception("Odefinierat bästa drag.");
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
                case "undo":
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
                case "fen":
                    fenBox.Text = currentPosition.getFEN();
                    break;
                case "flip":
                    flipBoard();
                    break;
                case "vänd":
                    flipBoard();
                    break;
                case "eval":
                    printEval(blobFish.eval(currentPosition, minDepth));
                    break;
                case "evaluate":
                    printEval(blobFish.eval(currentPosition, minDepth));
                    break;
                case "bedöm":
                    printEval(blobFish.eval(currentPosition, minDepth));
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
        private void squareClick(object sender, MouseEventArgs e)
        {
            int xVal = ((PictureBox)sender).Location.X; //a-h
            int yVal = ((PictureBox)sender).Location.Y; //1-8
            xVal = xVal / (boardPanel.Size.Width / 8);
            yVal = yVal / (boardPanel.Size.Height / 8);
            if (flipped)
            {
                xVal = 7 - xVal;
                yVal = 7 - yVal;
            }
            Square newSquare = new Square(yVal, xVal);
            if (firstSquare.rank == -1)  //-1 indikerar att ingen ruta tidigare markerats.
            {
                firstSquare = newSquare;
                moveLabel.Text = (char)(xVal + 'a') + (8 - yVal).ToString();
            }
            else
            {
                moveLabel.Text = (char)(firstSquare.line + 'a') + (8 - firstSquare.rank).ToString() + "-" +
                    (char)(xVal + 'a') + (8 - yVal).ToString();
                foreach (Move item in currentMoves)
                {
                    if (firstSquare.rank == item.from.rank && firstSquare.line == item.from.line &&
                        newSquare.rank == item.to.rank && newSquare.line == item.to.line)
                    {
                        Position newPosition = item.execute(currentPosition);
                        this.currentPosition = newPosition;
                        this.gameMoves.Add(item);
                        this.display(newPosition);
                        break;
                    }
                }
                firstSquare.rank = -1;
                firstSquare.line = -1;
                moveLabel.Text = "";
            }
        }
        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if((sender as RadioButton).Checked) //Nödvändig för inte dubbla anrop ska ske.
            {
                if (engineIsToMove())
                    playEngineMove();
            }
        }
        private void fenBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1_Click(null, null);
            }
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
                text += moves[i].toString(board) + "    " + Math.Round(evals[i].getValue(), 2).ToString() + Environment.NewLine;
            }
            return text;
        }
        private void reset()
        {
            gamePositions.Clear();
            gameMoves.Clear();
            display(new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));
        }
        private void flipBoard()
        {
            flipped = !flipped;
            display(currentPosition);
        }
        private string scoresheet()
        {
            string scoresheet = "";
            if (gamePositions.Count != gameMoves.Count + 1)
            {
                throw new Exception("Fel antal drag/ställningar har spelats!");
            }
            else if (gameMoves.Count == 0)
            {
                scoresheet = "Inga drag har spelats!";
            }
            else
            {
                int initialMoveNumber = gamePositions[0].moveCounter;
                for (int i = 0; i < gameMoves.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        if (i != 0)
                        {
                            scoresheet += Environment.NewLine;
                        }
                        scoresheet += ((i / 2) + initialMoveNumber).ToString() + ".";
                    }
                    scoresheet += " " + gameMoves[i].toString(gamePositions[i].board);
                }
            }
            return scoresheet;
        }
        private void resultPopUp(int result)
        { 
            if (result > 1000)
            {
                MessageBox.Show("Vit vann på schack matt!");
            }
            else if (result < -1000)
            {
                MessageBox.Show("Svart vann på schack matt!");
            }
            else if(result == 0)
            {
                MessageBox.Show("Partiet slutade remi!");
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
                evalBox.Text = "Ett drag har återtagits.";
                display(newCurrentPosition);
            }
            else
            {
                evalBox.Text = "För få drag har spelats!";
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.F)
                {
                    e.SuppressKeyPress = true;
                    flipBoard();
                }
                if (e.KeyCode == Keys.R)
                {
                    e.SuppressKeyPress = true;
                    DialogResult result = MessageBox.Show("Vill du starta ett nytt parti?", "Återställning av partiet", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        reset();
                    }
                }
                if(e.KeyCode == Keys.U)
                {
                    e.SuppressKeyPress = true;
                    takeback(2);
                }
                if (e.KeyCode == Keys.W)
                {
                    e.SuppressKeyPress = true;
                    DialogResult result = MessageBox.Show("Vill du stänga ned programmet?", "Avsluta", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
            }
        }
        private bool engineIsToMove()
        {
            return (radioButton2.Checked && currentPosition.whiteToMove) || (radioButton3.Checked && !currentPosition.whiteToMove);
        }
    }
}

/*
 * TODO:
 * Bekvämligheter:
 *  +/#
 *  Fler tester.
 *  Välja pjäs att promotera till.
 *  Se material.
 *  Se bästa variant
 *  Mattbart material
 *  Gör det möjligt att dra pjäserna
 *  Håll fönstret vid liv när drag beräknas.
 *  Gör fönstret skalbart.
 *  Går inte att återta drag om partier angetts via FEN?
 * 
 * Justera matriserna:
 *  Gör torn assymmetriska?
 *  Föredra springare före löpare.
 *  Mindre behov av kungen i hörnet.
 * 
 * Effektiviseringar:
 *  Sortera efter uppskattad kvalitet på draget.
 *  Effektivisera algoritmer för dragberäkning.
 *  Tråd-pool?
 *  Gör om system för att betckna forcerad matt.
 *  
 * Förbättringar:
 *  Variera djup utifrån antal pjäser.
 *  Ta öppna linjer med torn.
 *  Dragupprepningar
 *  Kungssäkerhet
 *  Gör kraftiga hot forcerande.
 *  Öka behov av terräng
 *  Få schackar/forcerade drag att kräva beräkning två drag framåt.
 *  Gör mer materialistisk.
 *  
 *  Kolla upp:
 *  Sb5: r3kb1r/ppp1pppp/3q1n2/3P1b2/8/P1N1BN2/1P2BPPP/Q4RK1 w kq - 1 12
 *  Sg4: r1b1k2r/2qpbpp1/p1n1pn2/1pp4p/4P3/1BNPBN2/PPP2PPP/2RQ1R1K b kq - 1 10
 */
