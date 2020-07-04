﻿using System;
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

            //TODO: Flytta ut
            //TODO: Få bort globala variabler.
            currentPosition = pos;
            if (radioButton1.Checked)
            {
                Engine blobFish = new Engine();
                currentMoves = blobFish.allValidMoves(pos);
                string temp = getMovesString(currentMoves, currentPosition.board);
                textBox1.Text = temp;
            }
            else{
                Engine blobFish = new Engine();
                EvalResult result = blobFish.eval(pos, 3);
                double eval = result.evaluation;
                currentMoves = result.allMoves;
                if (result.bestMove != null)
                    evalBox.Text = "Bästa drag: " + result.bestMove.toString(pos.board) + Environment.NewLine + "Evaluering: " + Math.Round(eval, 2);
                string temp = getMovesString(currentMoves, result.allEvals, currentPosition.board);
                textBox1.Text = temp;

                if (radioButton2.Checked && pos.whiteToMove)
                {
                    display(result.bestMove.execute(pos));
                }
                else if (radioButton3.Checked && !pos.whiteToMove)
                {
                    display(result.bestMove.execute(pos));
                }
            }
            toMoveLabel.Text = pos.whiteToMove ? "Vit vid draget." : "Svart vid draget.";
            
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
        public string getMovesString(List<Move> moves, List<double> evals, char[,] board)
        {
            if (moves == null || evals == null) return "";
            if (moves.Count != evals.Count)
                throw new Exception("Olika antal evalueringar och drag!");

            string text = "";
            for (int i = 0; i < moves.Count; i++)
            {
                text += moves[i].toString(board) + "    " + Math.Round(evals[i], 2).ToString() + Environment.NewLine;
            }
            return text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (fenBox.Text.ToLower() == "test")
            {
                string testString = Tests.runTests();
                evalBox.Text = testString;
            }
            else
            {
                Position pos = new Position(fenBox.Text);
                display(pos);
                /*
                try
                {
                    Position pos = new Position(fenBox.Text);
                    //evalBox.Text = "";
                    //Engine blobFish = new Engine();
                    //EvalResult result = blobFish.eval(pos, 1);
                    //double eval = result.evaluation;
                    //currentMoves = result.allMoves;
                    //evalBox.Text = "Evaluering: " + Math.Round(eval, 2);
                    display(pos);
                }
                catch
                {
                    evalBox.Text = "Felaktig FEN!";
                    return;
                }*/
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
                moveLabel.Text = (char)(firstSquare[1] + 'a') + (8 - firstSquare[0]).ToString() + "-" +
                    (char)(xVal + 'a') + (8 - yVal).ToString();
                foreach (Move item in currentMoves)
                {
                    if (firstSquare[0] == item.from[0] && firstSquare[1] == item.from[1] &&
                        newSquare[0] == item.to[0] && newSquare[1] == item.to[1])
                    {
                        Position newPosition = item.execute(currentPosition);
                        this.currentPosition = newPosition;
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
    }
}

/*
 * TODO:
 * +/#
 * Byt ut: row -> rank, column -> line. 
 * Byt ut int[] -> Square.
 * Fler tester.
 * Ta tillbaka drag.
 * Få FEN
 * Sortera efter ungefärlig kvalitet på draget.
 */
