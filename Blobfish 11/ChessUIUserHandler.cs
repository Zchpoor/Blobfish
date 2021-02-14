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
        private void ChessUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                if (e.KeyCode == Keys.Left)
                {
                    displayGamePosition(displayedPly - 1);
                }
                else if (e.KeyCode == Keys.Right)
                {
                    displayGamePosition(displayedPly + 1);
                }
                else if (e.KeyCode == Keys.Up)
                {
                    displayGamePosition(0);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    displayGamePosition(gamePositions.Count - 1);
                }
            }
            if (e.Modifiers == Keys.Control)
            {
                if(e.KeyCode == Keys.T)
                {
                    evalBox.Text = Tests.runTests();
                }
                if (e.KeyCode == Keys.S)
                {
                    PGNHandler handler = new PGNHandler();
                    handler.save(gamePositions, gameMoves, this.latestResult);
                }
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
                if (!ponderingWorker.IsBusy)
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
            if (e.Modifiers == (Keys.Control | Keys.Shift))
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
        private void fenBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                fenButton_Click(null, null);
            }
        }
        private void radioButton_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
                e.IsInputKey = true;
        }
        private void depthRB_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                if (depthRB2.Checked)
                    minDepth = 2;
                else if (depthRB3.Checked)
                    minDepth = 3;
                else if (depthRB4.Checked)
                    minDepth = 4;
                else if (depthRB5.Checked)
                    minDepth = 5;
                else if (depthRB6.Checked)
                    minDepth = 6;
                else if (depthRBAuto.Checked)
                    minDepth = -1;
                else
                    throw new Exception("Fel på djupinställningen!");
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            ponderingWorker.CancelAsync();
            if (!computerRBBoth.Checked)
                takeback(1);
            evalBox.Text = "Beräkningen avbröts.";
            ponderingTime = new TimeSpan(0);
            setPonderingMode(false);
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
                    evalBox.Text = "Omedelbar ställningsbedömning:" + Environment.NewLine + Math.Round(res, 2).ToString();
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
                        + "\r\n  Extra tid för att sortera: " + Math.Round((((float)t2 / (float)t1) - 1) * 100, 1) + "%";
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
            if ((sender as RadioButton).Checked) //Nödvändig för inte dubbla anrop ska ske.
            {
                if (engineIsToMove())
                    playBestEngineMove();
            }
        }
    }
}