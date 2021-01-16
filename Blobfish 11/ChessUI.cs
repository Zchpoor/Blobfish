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
using System.Threading;
using System.Diagnostics;

namespace Blobfish_11
{
    public partial class ChessUI : Form
    {
        PictureBox[,] Falt = new PictureBox[8, 8];
        List<Position> gamePositions = new List<Position>();
        List<Move> gameMoves = new List<Move>();
        Position currentPosition;
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
            int squareSize = 50;
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
            displayAndAddPosition(startingPosition);
        }
        private void displayAndAddPosition(Position pos)
        {
            gamePositions.Add(pos);
            display(pos);
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
        private void playMove(Move move)
        {
            Position newPosition = move.execute(currentPosition);
            this.gameMoves.Add(move);
            displayAndAddPosition(newPosition);
        }
        private void fenButton_Click(object sender, EventArgs e)
        {
            string inputText = fenBox.Text;
            string lowerInput = inputText.ToLower();

            switch (lowerInput)
            {
                case "test":
                    evalBox.Text = Tests.runTests();
                    break;
                case "moves":
                    evalBox.Text = "Alla drag:\n" + Environment.NewLine + 
                        getMovesString(blobFish.allValidMoves(currentPosition, false), currentPosition.board);
                    break;
                case "drag":
                    evalBox.Text = "Alla drag:" + Environment.NewLine + 
                        getMovesString(blobFish.allValidMoves(currentPosition, false), currentPosition.board);
                    break;
                case "sorted":
                    evalBox.Text = "Alla drag:" + Environment.NewLine + 
                        getMovesString(blobFish.allValidMoves(currentPosition, true), currentPosition.board);
                    break;
                case "sorterade":
                    evalBox.Text = "Alla drag:" + Environment.NewLine + 
                        getMovesString(blobFish.allValidMoves(currentPosition, true), currentPosition.board);
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
                    fenBox.SelectAll();
                    break;
                case "flip":
                    flipBoard();
                    break;
                case "vänd":
                    flipBoard();
                    break;
                case "eval":
                    printEval(choosePlayingStyle().eval(currentPosition, minDepth));
                    break;
                case "evaluate":
                    printEval(choosePlayingStyle().eval(currentPosition, minDepth));
                    break;
                case "bedöm":
                    printEval(choosePlayingStyle().eval(currentPosition, minDepth));
                    break;
                case "num":
                    float res = choosePlayingStyle().numericEval(currentPosition);
                    evalBox.Text = "Omedelbar ställningsbedömning:" + Environment.NewLine + Math.Round(res,2).ToString();
                    break;
                case "time":
                    evalBox.Text = "Tid som förbrukades förra draget: " + ponderingTime.ToString(@"mm\:ss");
                    break;
                case "tid":
                    evalBox.Text = "Tid som förbrukades förra draget: " + ponderingTime.ToString(@"mm\:ss");
                    break;
                case "spec":
                    string posToEvaluate = "r1bq1rk1/pppnn1bp/3p4/3Pp1p1/2P1Pp2/2N2P2/PP2BBPP/R2QNRK1 w - - 0 13";
                    Stopwatch sw = new Stopwatch();
                    long t0, t1, t2;
                    sw.Start();
                    for (int i = 0; i < 10000; i++)
                    {
                        blobFish.numericEval(new Position(posToEvaluate));
                    }
                    sw.Stop();
                    t0 = sw.ElapsedMilliseconds;
                    sw.Restart();
                    for (int i = 0; i < 10000; i++)
                    {
                        blobFish.allValidMoves(new Position(posToEvaluate), false);
                    }
                    sw.Stop();
                    t1 = sw.ElapsedMilliseconds;
                    sw.Restart();
                    for (int i = 0; i < 10000; i++)
                    {
                        blobFish.allValidMoves(new Position(posToEvaluate), true);
                    }
                    sw.Stop();
                    t2 = sw.ElapsedMilliseconds;
                    evalBox.Text = "Tider för 10000 iterationer (ms): "
                        + "\r\n  Evaluering av ställning: " + t0.ToString()
                        + "\r\n  Alla drag (osorterade): " + t1.ToString()
                        + "\r\n  Alla drag (sorterade): " + t2.ToString()
                        + "\r\n  Extra tid för att sortera: " + Math.Round((((float)t2 / (float)t1)-1) * 100, 1) + "%";
                    break;
                default:
                    try
                    {
                        Position pos = new Position(inputText);
                        gameIsGoingOn = true;
                        this.gamePositions.Clear();
                        this.gameMoves.Clear();
                        displayAndAddPosition(pos);
                    }
                    catch
                    {
                        evalBox.Text = "Felaktig FEN!";
                        return;
                    }
                    break;
            }
        }
        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if((sender as RadioButton).Checked) //Nödvändig för inte dubbla anrop ska ske.
            {
                if (engineIsToMove())
                    playBestEngineMove();
            }
        }
        private void fenBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                fenButton_Click(null, null);
            }
        }
        private string getMovesString(List<Move> moves, char[,] board)
        {
            string text = "";
            foreach (Move item in moves)
            {
                text += item.toString(board) + Environment.NewLine;
            }
            return text;
        }
        private void reset()
        {
            gameIsGoingOn = true;
            gamePositions.Clear();
            gameMoves.Clear();
            displayAndAddPosition(startingPosition);
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
            else if (result == 0)
            {
                MessageBox.Show("Partiet slutade remi!");
            }
            else if (result == -1)
            {
                MessageBox.Show("Partiet slutade remi, på grund av ej mattbart material!");
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
                displayAndAddPosition(newCurrentPosition);
            }
            else
            {
                evalBox.Text = "För få drag har spelats!";
            }
        }
        private void ChessUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.F)
                {
                    e.SuppressKeyPress = true;
                    flipBoard();
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
                if(!ponderingWorker.IsBusy)
                {
                    //Kortkommandon som endast tillåts om motorn inte är igång.
                    if (e.KeyCode == Keys.R)
                    {
                        e.SuppressKeyPress = true;
                        DialogResult result = MessageBox.Show("Vill du starta ett nytt parti?", "Återställning av partiet", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            reset();
                        }
                    }
                    if (e.KeyCode == Keys.Z)
                    {
                        e.SuppressKeyPress = true;
                        takeback(2);
                    }
                }
            }
            if(e.Modifiers == (Keys.Control | Keys.Shift))
            {
                if (!ponderingWorker.IsBusy)
                {
                    //Kortkommandon som endast tillåts om motorn inte är igång.
                    if (e.KeyCode == Keys.Z)
                    {
                        e.SuppressKeyPress = true;
                        takeback(1);
                    }
                }
            }
        }
        private bool engineIsToMove()
        {
            return (radioButton4.Checked || radioButton2.Checked && currentPosition.whiteToMove) ||
                (radioButton3.Checked && !currentPosition.whiteToMove);
        }

        private void ponderingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //TODO: Kolla så ställningen inte förändrats.
            //TODO: Bli av med denna worker?
            //Flytta motorn hit?
            blobFish.cancelFlag.setValue(0);
            EvalResult resultPlace = new EvalResult();
            resultPlace.bestMove = null;
            Thread thread = new Thread(delegate ()
            {
                engineStart(minDepth, currentPosition.deepCopy(), resultPlace);
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
                            playMove(res.bestMove);
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
        private void cancelButton_Click(object sender, EventArgs e)
        {
            ponderingWorker.CancelAsync();
            if (!radioButton4.Checked)
                takeback(1);
            evalBox.Text = "Beräkningen avbröts.";
            ponderingTime = new TimeSpan(0);
            setPonderingMode(false);
        }
        private void ChessUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            ponderingWorker.CancelAsync();
        }
        private void setPonderingMode(bool setTo)
        {
            ponderingPanel.Visible = setTo;
            settingsPanel.Enabled = !setTo;
            fenBox.Enabled = !setTo;
            fenButton.Enabled = !setTo;
            if (setTo)
                ponderingTimer.Start();
            else
                ponderingTimer.Stop();
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
        private Engine choosePlayingStyle()
        {
            //Byt namn på funktionen?
            int[] MIL = { };
            if (depthRB3.Checked)
            {
                MIL = new int[] {8};
            }

            try
            {
                if (playStyleRB0.Checked) //Normal
                {
                    return new Engine(MIL);
                }
                else if (playStyleRB1.Checked) //Försiktig
                {
                    return new Engine(new float[] {1f, 3f, 3f, 5f, 9f }, 0.6f, new float[] { 1.2f, 2.2f, 1.4f, 0.4f, 0.1f }, 6, 1.15f, MIL);
                }
                else if (playStyleRB2.Checked) //Materialistisk
                {
                    return new Engine(new float[] {1.2f, 4f, 4f, 6.5f, 12f }, 0.4f, new float[] { 1, 2, 1.4f, 0.4f, 0.1f }, 8, 0.5f, MIL);
                }
                else if (playStyleRB3.Checked) //Experimentell
                {
                    return new Engine(new float[] {1f, 3f, 3.1f, 5f, 9f }, 0.4f, new float[] { 1, 1f, 0.8f, 0.1f, 0.05f }, 8, 0.85f, MIL);
                }
                else
                {
                    return new Engine();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + "Använder standardmotorn.");
                return new Engine();
            }
        }
        private void depthRB_CheckedChanged(object sender, EventArgs e)
        {
            if((sender as RadioButton).Checked)
            {
                if (depthRB0.Checked)
                    minDepth = 2;
                else if (depthRB1.Checked)
                    minDepth = 3;
                else if (depthRB2.Checked)
                    minDepth = 4;
                else if (depthRB3.Checked)
                    minDepth = 4;
                else if (depthRB4.Checked)
                    minDepth = 5;
                else if (depthRB5.Checked)
                    minDepth = 6;
                else
                    throw new Exception("Fel på djupinställningen!");
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
 *  Gör fönstret skalbart.
 *  Koordinater
 *  Gå framåt/bakåt i partiet.
 *  Dra nu!
 *  Förbättra validSquare()
 *  Träd för varianter.
 * 
 * Justera matriserna:
 *  Gör torn assymmetriska?
 *  Minska behov av att ställa ut damen.
 *  Öka behov av att flytta centrumbönder.
 * 
 * Effektiviseringar:
 *  Effektivisera algoritmer för dragberäkning.
 *  Tråd-pool?
 *  Gör om system för att beteckna forcerad matt.
 *  Beräkna nästa lager av drag tidigare.
 *  
 * Förbättringar:
 *  Variera djup utifrån antal pjäser.
 *  Ta öppna linjer med torn.
 *  Dragupprepningar
 *  Gör kraftiga hot forcerande.
 *  Få schackar/forcerade drag att kräva beräkning två drag framåt.
 *  
 *  
 *  Buggar:
 */
