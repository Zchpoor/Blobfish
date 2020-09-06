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

namespace Blobfish_11
{
    public partial class Form1 : Form
    {
        PictureBox[,] Falt = new PictureBox[8, 8];
        List<Position> gamePositions = new List<Position>();
        List<Move> gameMoves = new List<Move>();
        Position currentPosition;
        readonly Position startingPosition = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        List<Move> currentMoves = new List<Move>();
        bool flipped = false;
        Square firstSquare = new Square(-1, -1);
        Engine blobFish = new Engine();
        const int minDepth = 4;
        int numberOfDots = 1;
        TimeSpan ponderingTime = new TimeSpan(0);

        public Form1()
        {
            InitializeComponent();
            int squareSize = 50;
            boardPanel.AutoSize = true;
            moveLabel.Text = "";
            ponderingLabel.Text = "Datorn tänker.";
            ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
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
            displayAndAddPosition(startingPosition);
        }
        private void displayAndAddPosition(Position pos)
        {
            gamePositions.Add(pos);
            display(pos);
        }
        private void display(Position pos)
        {
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

            toMoveLabel.Text = pos.whiteToMove ? "Vit vid draget." : "Svart vid draget.";

            currentPosition = pos;

            currentMoves = blobFish.allValidMoves(pos);

            int res = blobFish.decisiveResult(pos, currentMoves);
            if (res != -2)
            {
                resultPopUp(res);
            }
            else if (engineIsToMove())
            {
                playBestEngineMove();
            }
        }
        private void playBestEngineMove()
        {
            if (!ponderingWorker.IsBusy)
            {
                evalBox.Text = "";
                setPonderingMode(true);
                ponderingTime = new TimeSpan(0);
                ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
                ponderingWorker.RunWorkerAsync();
            }
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
                case "num":
                    double res = blobFish.numericEval(currentPosition);
                    evalBox.Text = "Omedelbar ställningsbedömning:" + Environment.NewLine + res.ToString();
                    break;
                default:
                    try
                    {
                        Position pos = new Position(inputText);
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
        private void squareClick(object sender, MouseEventArgs e)
        {
            //Om motorn räknar för närvarande, så bör ej andra drag tillåtas spelas.
            if (ponderingWorker.IsBusy) {return;}

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
                bool moveWasPlayed = false;
                foreach (Move item in currentMoves)
                {
                    if (firstSquare.rank == item.from.rank && firstSquare.line == item.from.line &&
                        newSquare.rank == item.to.rank && newSquare.line == item.to.line)
                    {
                        playMove(item);
                        moveWasPlayed = true;
                        break;
                    }
                }
                if (!moveWasPlayed && !(firstSquare.line == newSquare.line && firstSquare.rank == newSquare.rank))
                {
                    evalBox.Text = "Felaktigt drag!";
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
                displayAndAddPosition(newCurrentPosition);
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
                    if (e.KeyCode == Keys.U && e.Modifiers == Keys.Shift)
                    {
                        e.SuppressKeyPress = true;
                        takeback(1);
                    }
                    if (e.KeyCode == Keys.U)
                    {
                        e.SuppressKeyPress = true;
                        takeback(2);
                    }
                }
            }
        }
        private bool engineIsToMove()
        {
            return (radioButton2.Checked && currentPosition.whiteToMove) || (radioButton3.Checked && !currentPosition.whiteToMove);
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
                        printEval(res);
                        if (engineIsToMove())
                        {
                            playMove(res.bestMove);
                        }
                        setPonderingMode(false);
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
                GC.Collect();
                GC.WaitForPendingFinalizers();
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            numberOfDots = (numberOfDots % 3) + 1;
            ponderingLabel.Text = "Datorn tänker" + string.Join("", Enumerable.Repeat(".", numberOfDots));
            ponderingTime = ponderingTime.Add(new TimeSpan(0, 0, 0, 0, timer1.Interval));
            ponderingTimeLabel.Text = ponderingTime.ToString(@"mm\:ss");
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            ponderingWorker.CancelAsync();
            takeback(1);
            evalBox.Text = "Beräkningen avbröts.";
            setPonderingMode(false);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ponderingWorker.CancelAsync();
        }
        private void setPonderingMode(bool setTo)
        {
            ponderingPanel.Visible = setTo;
            groupBox1.Enabled = !setTo;
            fenBox.Enabled = !setTo;
            fenButton.Enabled = !setTo;
            if (setTo)
                timer1.Start();
            else
                timer1.Stop();
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
 *  Gör fönstret skalbart.
 *  Koordinater
 *  Double -> Float (Volatile)
 * 
 * Justera matriserna:
 *  Gör torn assymmetriska?
 *  Fixa ny matris för kung.
 *  Föredra springare före löpare.
 *  Mindre behov av kungen i hörnet.
 * 
 * Effektiviseringar:
 *  Sortera efter uppskattad kvalitet på draget.
 *  Effektivisera algoritmer för dragberäkning.
 *  Tråd-pool?
 *  Gör om system för att betckna forcerad matt.
 *  Jämför med global alfa/beta oftare.
 *  Beräkna nästa lager av drag tidigare.
 *  
 * Förbättringar:
 *  Variera djup utifrån antal pjäser.
 *  Ta öppna linjer med torn.
 *  Dragupprepningar
 *  Kungssäkerhet
 *  Gör kraftiga hot forcerande.
 *  Öka behov av terräng
 *  Få schackar/forcerade drag att kräva beräkning två drag framåt.
 *  
 */
