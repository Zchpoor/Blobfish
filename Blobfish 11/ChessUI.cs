using System;
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
        Position currentPosition;
        int displayedPly = 0;
        bool gameIsGoingOn = true;
        readonly Position startingPosition = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        List<Move> currentMoves = new List<Move>();
        bool flipped = false;
        Engine blobFish = new Engine();
        int minDepth = 4;
        int numberOfDots = 1;
        TimeSpan ponderingTime = new TimeSpan(0);
        Dictionary<char, Image> piecesPictures = new Dictionary<char, Image>(13);

        public ChessUI()
        {
            InitializeComponent();
            this.MinimumSize = this.Size;
            //this.MaximumSize = this.Size;
            int squareSize = 56;
            boardPanel.AutoSize = true;
            moveLabel.Text = "";
            ponderingLabel.Text = "Datorn tänker.";
            ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
            #region fetchImages
            
            try
            {
                string[] picNames = 
                    {"null.png", "Bp.png","WP.png","Bn.png","WN.png","Bb.png","WB.png","Br.png","WR.png","Bq.png","WQ.png","Bk.png","WK.png"};
                char[] pieceNames =
                    {'\0', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K' };
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
                    picBox.Image = piecesPictures['\0'];

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
        }
        private void reset()
        {
            evalBox.Text = "Nytt parti";
            game.result = "*";
            gameIsGoingOn = true;
            game = new Game();
            displayedPly = 0;
            display(startingPosition);
        }
        private void display(Position pos)
        {
            currentPosition = pos;
            currentMoves = blobFish.allValidMoves(pos, false);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char piece = flipped ? pos.board[7-i, 7-j] : pos.board[i, j];
                    Falt[i, j].Image = piecesPictures[piece];
                    Cursor cursor = moveablePiece(piece) ? dragCursor : Cursors.Default;
                    Falt[i, j].Cursor = cursor;
                }
            }

            toMoveLabel.Text = pos.whiteToMove ? "Vit vid draget." : "Svart vid draget.";

            //TODO: Faktorisera ut med liknande kod nedan.
            int res = blobFish.decisiveResult(pos, currentMoves);
            if (res != -2)
            {
                resultPopUp(res);
                gameIsGoingOn = false;
            }
            else if (!blobFish.mateableMaterial(currentPosition.board))
            {
                resultPopUp(-1);
                gameIsGoingOn = false;
            }
            else if (engineIsToMove())
            {
                playBestEngineMove();
            }
        }
        private void playBestEngineMove()
        {
            if (!ponderingWorker.IsBusy && gameIsGoingOn)
            {
                blobFish = choosePlayingStyle();
                //evalBox.Text = "";
                ponderingTime = new TimeSpan(0);
                ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
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
        private void flipBoard()
        {
            flipped = !flipped;
            display(currentPosition);
        }
        private void printEval(EvalResult result)
        {
            int decisiveResult = blobFish.decisiveResult(currentPosition, currentMoves);
            if (decisiveResult != -2)
            {
                resultPopUp(decisiveResult);
                gameIsGoingOn = false;
            }
            else if (!blobFish.mateableMaterial(currentPosition.board))
            {
                resultPopUp(-1);
                gameIsGoingOn = false;
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
                String completeString = "Bästa drag: " + result.bestMove.toString(currentPosition) +
                    Environment.NewLine + "Datorns evaluering: " + textEval;
                evalBox.Text = completeString;
            }
            else
            {
                throw new Exception("Odefinierat bästa drag.");
            }
        }
        private void playMove(Move move)
        {
            game.addMove(move);
            displayedPly = game.length;
            display(game.lastPosition());
        }
        public void takeback(int numberOfMoves)
        {
            try
            {
                game.takeback(numberOfMoves);
                displayedPly = game.length;
                display(game.lastPosition());
            }
            catch
            {
                
            }
        }
        private void resultPopUp(int result)
        {
            if (!gameIsGoingOn)
                return;
            if (result > 1000)
            {
                MessageBox.Show("Vit vann på schack matt!");
                game.result = "1-0";
            }
            else if (result < -1000)
            {
                MessageBox.Show("Svart vann på schack matt!");
                game.result = "0-1";
            }
            else if (result == 0)
            {
                MessageBox.Show("Partiet slutade remi!");
                game.result = "1/2-1/2";
            }
            else if (result == -1)
            {
                MessageBox.Show("Partiet slutade remi, på grund av ej mattbart material!");
                game.result = "1/2-1/2";
            }
            string[] tempPlayers = game.players; //Kommer ihåg vad players var innan.
            computerNoneToolStripMenuItem.Checked = true; //Stänger av datorn, för att underlätta om man vill backa i partiet.
            game.players = tempPlayers;          // Återställer player. Troligen detta man är intresserad av att spara.
        }
        private void displayGamePosition(int ply)
        {
            if (ply < 0) ply = 0;
            if (ply > game.length) ply = game.length;
            displayedPly = ply;

            display(game.getPosition(ply));
            if(ply == game.length)
            {
                setBoardDisabled(false);
            }
            else
            {
                setBoardDisabled(true);
            }
        }
        private bool engineIsToMove()
        {
            if (displayedPly != game.length)
                return false;
            return (computerBothToolStripMenuItem.Checked || computerWhiteToolStripMenuItem.Checked && currentPosition.whiteToMove) ||
                (computerBlackToolStripMenuItem.Checked && !currentPosition.whiteToMove);
        }

        private void ponderingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //TODO: Kolla så ställningen inte förändrats.
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
                engineStart(minDepth, currentPosition.boardCopy(), resultPlace);
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
                        if(gameIsGoingOn)
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
                    evalBox.Text = "Ett fel inträffade.";
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
                        char pieceOnSquare = currentPosition.board[rank, line];
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

/*
 * TODO:
 * Bekvämligheter:
 *  +/#
 *  Fler tester.
 *  Välja pjäs att promotera till.
 *  Se material.
 *  Se bästa variant
 *  Koordinater
 *  Få "dra nu" att fungera bättre.
 *  Träd för varianter.
 *  Spela forcerande drag omedelbart?
 *  Läsa in PGN.
 * 
 * Justera matriserna:
 *  Gör torn assymmetriska?
 *  Föredra Sc3 före Sd2
 *  Minska behov av terräng.
 * 
 * Effektiviseringar:
 *  Effektivisera algoritmer för dragberäkning.
 *  Tråd-pool?
 *  Beräkna nästa lager av drag tidigare.
 *  Gemensamt bräde i tråd?
 *  Klass med värden åt tråd.
 *  Bli av med delegates?
 *  
 * Förbättringar:
 *  Ta öppna linjer med torn.
 *  Dragupprepningar
 *  Gör kraftiga hot forcerande.
 *  Få schackar/forcerade drag att kräva beräkning två drag framåt.
 *  Avbryt inuti trådarna.
 *  
 *  Buggar:
 */
