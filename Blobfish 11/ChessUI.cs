﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Blobfish_11
{
    public partial class ChessUI : Form
    {
        PictureBox[,] Falt = new PictureBox[8, 8];
        Game game = new Game();
        bool retrospectMode = false;
        List<Move> currentMoves = new List<Move>();
        bool flipped = false;
        Engine blobFish = new Engine();
        int minDepth = 4;
        int numberOfDots = 1;
        TimeSpan ponderingTime = new TimeSpan(0);
        Dictionary<Piece, Image> piecesPictures = new Dictionary<Piece, Image>(13);

        public ChessUI() 
        {
            InitializeComponent();
            this.Height = boardPanel.Height + 30;
            int squareSize = 60;
            //boardPanel.AutoSize = true;
            moveLabel.Text = "";
            ponderingLabel.Text = "Datorn tänker.";
            ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
            #region fetchImages
            
            try
            {
                string[] picNames = 
                    {"null.png", "Bp.png","WP.png","Bn.png","WN.png","Bb.png","WB.png","Br.png","WR.png","Bq.png","WQ.png","Bk.png","WK.png"};
                Piece[] pieceNames =
                    { Piece.None, Piece.Pawn, Piece.Pawn.AsWhite()
                    , Piece.Knight, Piece.Knight.AsWhite()
                    , Piece.Bishop, Piece.Bishop.AsWhite()
                    , Piece.Rook, Piece.Rook.AsWhite()
                    , Piece.Queen, Piece.Queen.AsWhite()
                    , Piece.King, Piece.King.AsWhite()};
                for (int i = 0; i < picNames.Length; i++)
                {
                    piecesPictures.Add(pieceNames[i], Image.FromFile(picNames[i]));
                }

                /*
                piecesPictures.Add('\0', Properties.Resources._null);
                piecesPictures.Add('p', Properties.Resources.Bp);
                piecesPictures.Add('P', Properties.Resources.Wp);
                piecesPictures.Add('b', Properties.Resources.Bb);
                piecesPictures.Add('B', Properties.Resources.WB);
                piecesPictures.Add('n', Properties.Resources.Bn);
                piecesPictures.Add('N', Properties.Resources.WN);
                piecesPictures.Add('r', Properties.Resources.Br);
                piecesPictures.Add('R', Properties.Resources.WR);
                piecesPictures.Add('q', Properties.Resources.Bq);
                piecesPictures.Add('Q', Properties.Resources.WQ);
                piecesPictures.Add('k', Properties.Resources.Bk);
                piecesPictures.Add('K', Properties.Resources.WK);*/
            }
            catch (Exception e)
            {
                MessageBox.Show("Ett fel inträffade vid inladdning av bilder. Programmet kommer att avslutas. " +
                    Environment.NewLine + "Felmeddelande: " + Environment.NewLine + e.ToString());
                this.Close();
            }
            #endregion
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
                    picBox.Image = piecesPictures[Piece.None];

                    picBox.AllowDrop = true;
                    picBox.MouseDown += new MouseEventHandler(squareMouseDown);
                    picBox.DragEnter += new DragEventHandler(squareDragEnter);
                    picBox.DragDrop += new DragEventHandler(squareDragDrop);
                    picBox.DragLeave += new EventHandler(squareDragLeave);
                    picBox.GiveFeedback += new GiveFeedbackEventHandler(squareGiveFeedBack);
                    if ((i + j) % 2 == 0)
                        picBox.BackColor = Color.WhiteSmoke;
                    else
                        picBox.BackColor = Color.SandyBrown;
                }
            }
            reset();
            this.Height = boardPanel.Height + menuStrip1.Height + statusStrip1.Height + 
                extraInfoTextBox.Height + fenBox.Height + 50;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
        }
        private void reset()
        {
            evalStatusLabel.Text = "";
            computerMoveStatusLabel.Text = "";
            timeSpentStatusLabel.Text = "";
            game.result = GameResult.Undecided;
            computerBlackToolStripMenuItem.Checked = true;
            game = new Game();
            display(game.currentPosition);
        }
        private void display(Position pos)
        {
            updateScoresheetBox();
            updateExtraInfoTextBox();
            toMoveLabel.Text = pos.whiteToMove ? "Vit vid draget." : "Svart vid draget.";

            currentMoves = PieceMovementHandler.AllValidMoves(pos, false);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = flipped ? pos.board[7-i, 7-j] : pos.board[i, j];
                    Falt[i, j].Image = piecesPictures[piece];
                    Cursor cursor = moveablePiece(piece) ? dragCursor : Cursors.Default;
                    Falt[i, j].Cursor = cursor;
                }
            }

            GameResult res = blobFish.decisiveResult(pos, currentMoves);
            if (res != GameResult.Undecided)
            {
                resultPopUp(res);
            }
            else if (engineIsToMove())
            {
                playBestEngineMove();
            }
        }
        private void updateScoresheetBox()
        {
            string rtfs = game.RTFScoresheet();
            scoresheetBox.Clear();
            if (rtfs.Equals(""))
            {
                scoresheetBox.Text = "Nytt parti.";
            }
            else
            {
                scoresheetBox.Rtf = game.RTFScoresheet(); //Uppdaterar protokollet.
            }
        }
        private void updateExtraInfoTextBox()
        {
            string[] players = new string[2];
            for (int i = 0; i < 2; i++)
            {
                players[i] = game.players[i];
                if (game.eloRatings[i] > 0)
                {
                    players[i] += " (" + game.eloRatings[i].ToString() + ")";
                }
            }
            string newText = players[0] + " - " + players[1] + Environment.NewLine;
            if (game.gameEvent != "")
                newText += game.gameEvent + Environment.NewLine;
            if (game.round != "")
                newText += game.round + Environment.NewLine;
            if (game.date != "")
                newText += game.date + Environment.NewLine;
            extraInfoTextBox.Clear();
            extraInfoTextBox.Text = newText;
        }

        private void playBestEngineMove()
        {
            if (!ponderingWorker.IsBusy)
            {
                blobFish.Data = choosePlayingStyle();
                ponderingTime = new TimeSpan(0);
                ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
                computerMoveStatusLabel.Text = "";
                //evalStatusLabel.Text = "";
                timeSpentStatusLabel.Text = "";
                ponderingWorker.RunWorkerAsync();
            }
            setPonderingMode(true);
        }
        private string getMovesString(List<Move> moves, Position pos)
        {
            string text = "";
            foreach (Move item in moves)
            {
                text += item.toString(pos) + Environment.NewLine;
            }
            return text;
        }
        private void printEval(EvalResult result)
        {
            GameResult decisiveResult = blobFish.decisiveResult(game.currentPosition, currentMoves);
            if (decisiveResult != GameResult.Undecided)
            {
                resultPopUp(decisiveResult);
            }
            else if (result.bestMove != null)
            {
                float eval = result.evaluation;
                string textEval;
                if (eval > 1000)
                {
                    int plysToMate = (int)(2002 - eval);
                    textEval = "M" + (plysToMate / 2).ToString();
                }
                else if (eval < -1000)
                {
                    int plysToMate = (int)(2002 + eval);
                    textEval = "m-" + (plysToMate / 2).ToString();
                }
                else
                {
                    textEval = Math.Round(eval, 2).ToString();
                }
                computerMoveStatusLabel.Text = "Datorn spelade: " + result.bestMove.toString(game.currentPosition);
                evalStatusLabel.Text = "Datorns evaluering: " + textEval;
                timeSpentStatusLabel.Text = "Förbrukad tid: " + ponderingTime.ToString(@"mm\:ss");
            }
            else
            {
                throw new Exception("Odefinierat bästa drag.");
            }
        }
        private void playMove(Move move)
        {
            game.addMove(move);
            retrospectMode = false;
            display(game.currentPosition);
        }
        private void takeback(int numberOfMoves)
        {
            try
            {
                game.takeback(numberOfMoves);
                display(game.currentPosition);
            }
            catch
            {
                
            }
        }
        private void resultPopUp(GameResult result)
        {
            if (retrospectMode)
                return;
            if (result == GameResult.WhiteWin)
            {
                MessageBox.Show("Vit vann på schack matt!");
            }
            else if (result == GameResult.BlackWin)
            {
                MessageBox.Show("Svart vann på schack matt!");
            }
            else if (result == GameResult.DrawBy50MoveRule)
            {
                MessageBox.Show("Partiet slutade remi, på grund av 50-dragsregeln!");
            }
            else if (result == GameResult.DrawByStaleMate)
            {
                MessageBox.Show("Partiet slutade remi, på grund av patt!");
            }
            else if (result == GameResult.DrawByRepetition)
            {
                MessageBox.Show("Partiet slutade remi, på grund av dragupprepning!");
            }
            else if (result == GameResult.DrawByInsufficientMaterial)
            {
                MessageBox.Show("Partiet slutade remi, på grund av ej mattbart material!");
            }
            game.result = result;
            retrospectMode = true;
        }
        private bool engineIsToMove()
        {
            if (retrospectMode)
            {
                return false;
            }
            return (computerBothToolStripMenuItem.Checked || computerWhiteToolStripMenuItem.Checked && game.currentPosition.whiteToMove) ||
                (computerBlackToolStripMenuItem.Checked && !game.currentPosition.whiteToMove);
        }

        private void ponderingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //TODO: Bli av med denna worker?
            //Flytta motorn hit?
            blobFish.cancelFlag.setValue(0);
            EvalResult resultPlace = new EvalResult();
            resultPlace.bestMove = null;
            if (depthAutoToolStripMenuItem.Checked)
            {
                minDepth = -1;
            }
            Thread thread = new Thread(delegate ()
            {
                engineStart(minDepth, game.currentPosition.boardCopy(), resultPlace);
            });
            thread.Name = "engineThread";
            thread.Start();
            while (thread.IsAlive)
            {
                if (ponderingWorker.CancellationPending)
                {
                    blobFish.cancelFlag.setValue(1);
                    e.Cancel = true;
                    break;
                }
                Thread.Sleep(50);
            }
            e.Result = resultPlace;
        }
        private void ponderingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                //Vidtag lämplig åtgärd
            }
            else
            {
                try
                {
                    EvalResult res = (EvalResult)e.Result;
                    if (res.bestMove == null)
                    {
                        //blobFish.cancelFlag.setValue(0);
                        //takeback(1);
                        setPonderingMode(false);
                        //throw new Exception("Inget bästa drag!");
                    }
                    else
                    {
                        setPonderingMode(false);
                        printEval(res);

                        if (engineIsToMove())
                        {
                            this.playMove(res.bestMove);
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                    scoresheetBox.Text = "Ett fel inträffade.";
                    takeback(1);
                    setPonderingMode(false);

                }
                //Samlar upp skräp då UI väntar på att användaren skall dra.
                if (!engineIsToMove())
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
        private void engineStart(int minDepth, Position pos, EvalResult resultPlace)
        {
            //TODO: Hitta något bättre sätt att göra detta på.
            EvalResult res = blobFish.eval(pos, minDepth);
            resultPlace.bestMove = res.bestMove;
            resultPlace.allMoves = res.allMoves;
            resultPlace.allEvals = res.allEvals;
            resultPlace.evaluation = res.evaluation;
            return;
        }
        private void ponderingTimer_Tick(object sender, EventArgs e)
        {
            numberOfDots = (numberOfDots % 3) + 1;
            ponderingLabel.Text = "Datorn tänker" + string.Join("", Enumerable.Repeat(".", numberOfDots));
            ponderingTime = ponderingTime.Add(new TimeSpan(0, 0, 0, 0, ponderingTimer.Interval));
            ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
        }
        private void ChessUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            ponderingWorker.CancelAsync();
        }
        private void setPonderingMode(bool setTo)
        {
            ponderingPanel.Visible = setTo;
            menuStrip1.Enabled = !setTo;
            moveNowButton.Enabled = setTo;
            fenBox.Enabled = !setTo;
            fenButton.Enabled = !setTo;
            if (setTo)
                ponderingTimer.Start();
            else
                ponderingTimer.Stop();
            setBoardDisabled(setTo);
        }
        private void setBoardDisabled(bool setTo)
        {
            for (int rank = 0; rank < 7; rank++)
            {
                for (int line = 0; line < 7; line++)
                {
                    if (setTo)
                    {
                        Falt[rank, line].Cursor = Cursors.Default;
                    }
                    else
                    {
                        Piece pieceOnSquare = game.currentPosition.board[rank, line];
                        Cursor cursor = moveablePiece(pieceOnSquare) ? dragCursor : Cursors.Default;
                        if (flipped)
                        {
                            Falt[7 - rank, 7 - line].Cursor = cursor;
                        }
                        else
                        {
                            Falt[rank, line].Cursor = cursor;
                        }
                    }
                }
            }
        }
    }
}
