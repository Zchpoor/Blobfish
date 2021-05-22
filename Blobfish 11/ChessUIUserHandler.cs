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
                    game.goBack();
                    retrospectMode = true;
                    display(game.currentPosition);
                }
                else if (e.KeyCode == Keys.Right)
                {
                    game.mainContinuation();
                    retrospectMode = true;
                    display(game.currentPosition);
                }
                else if (e.KeyCode == Keys.Up)
                {
                    game.goToFirstPosition();
                    retrospectMode = true;
                    display(game.currentPosition);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    game.goToLastPosition();
                    retrospectMode = true;
                    display(game.currentPosition);
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
        private void fenButton_Click(object sender, EventArgs e)
        {
            string inputText = fenBox.Text;
            string lowerInput = inputText.ToLower();

            switch (lowerInput)
            {
                case "test":
                    scoresheetBox.Text = Tests.runTests();
                    break;
                case "moves":
                    scoresheetBox.Text = "Alla drag:\n" + Environment.NewLine +
                        getMovesString(blobFish.allValidMoves(game.currentPosition, false), game.currentPosition);
                    break;
                case "drag":
                    scoresheetBox.Text = "Alla drag:" + Environment.NewLine +
                        getMovesString(blobFish.allValidMoves(game.currentPosition, false), game.currentPosition);
                    break;
                case "sorted":
                    scoresheetBox.Text = "Alla drag:" + Environment.NewLine +
                        getMovesString(blobFish.allValidMoves(game.currentPosition, true), game.currentPosition);
                    break;
                case "sorterade":
                    scoresheetBox.Text = "Alla drag:" + Environment.NewLine +
                        getMovesString(blobFish.allValidMoves(game.currentPosition, true), game.currentPosition);
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
                    scoresheetBox.Text = game.scoresheet();
                    break;
                case "protokoll":
                    scoresheetBox.Text = game.scoresheet();
                    break;
                case "reset":
                    reset();
                    break;
                case "omstart":
                    reset();
                    break;
                case "fen":
                    fenBox.Text = game.currentPosition.getFEN();
                    fenBox.SelectAll();
                    break;
                case "flip":
                    flipBoard();
                    break;
                case "vänd":
                    flipBoard();
                    break;
                case "eval":
                    printEval(choosePlayingStyle().eval(game.currentPosition, minDepth));
                    break;
                case "evaluate":
                    printEval(choosePlayingStyle().eval(game.currentPosition, minDepth));
                    break;
                case "bedöm":
                    printEval(choosePlayingStyle().eval(game.currentPosition, minDepth));
                    break;
                case "num":
                    float res = choosePlayingStyle().numericEval(game.currentPosition);
                    scoresheetBox.Text = "Omedelbar ställningsbedömning:" + Environment.NewLine + Math.Round(res, 2).ToString();
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
                    scoresheetBox.Text = "Tider för 10000 iterationer (ms): "
                        + "\r\n  Evaluering av ställning: " + t0.ToString()
                        + "\r\n  Alla drag (osorterade): " + t1.ToString()
                        + "\r\n  Alla drag (sorterade): " + t2.ToString()
                        + "\r\n  Extra tid för att sortera: " + Math.Round((((float)t2 / (float)t1) - 1) * 100, 1) + "%";
                    break;
                default:
                    try
                    {
                        Position pos = new Position(inputText);
                        game = new Game(pos);
                        display(pos);
                    }
                    catch
                    {
                        scoresheetBox.Text = "Felaktig FEN!";
                        return;
                    }
                    break;
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
        private void cancelButton_Click(object sender, EventArgs e)
        {
            ponderingWorker.CancelAsync();
            if (!computerBothToolStripMenuItem.Checked)
                takeback(1);
            scoresheetBox.Text = "Beräkningen avbröts.";
            ponderingTime = new TimeSpan(0);
            setPonderingMode(false);
        }
        private void engineColor_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked == false)
            {
                return;
            }
            foreach (ToolStripMenuItem item in engineColorMenuItem.DropDownItems)
            {
                if (!item.Name.Equals((sender as ToolStripMenuItem).Name))
                {
                    item.Checked = false;
                }
            }
            if (computerBothToolStripMenuItem.Checked)
            {
                game.players = new string[] { "Blobfish 11", "Blobfish 11" };
            }
            else if (computerWhiteToolStripMenuItem.Checked)
            {
                game.players = new string[] { "Blobfish 11", "Human player" };
            }
            else if (computerBlackToolStripMenuItem.Checked)
            {
                game.players = new string[] { "Human player", "Blobfish 11" };
            }
            else
            {
                game.players = new string[] { "Human player", "Human player" };
            }
            if (engineIsToMove())
                playBestEngineMove();
        }
        private void styleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked == false)
            {
                return;
            }
            foreach (ToolStripMenuItem item in styleToolStripMenuItem.DropDownItems)
            {
                if (!item.Name.Equals((sender as ToolStripMenuItem).Name))
                {
                    item.Checked = false;
                }
            }
        }
        private void depthToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Checked == false)
            {
                return;
            }
            foreach (ToolStripMenuItem item in depthToolStripMenuItem.DropDownItems)
            {
                if (!item.Name.Equals((sender as ToolStripMenuItem).Name))
                {
                    item.Checked = false;
                }
            }
            if (depth2ToolStripMenuItem.Checked)
                minDepth = 2;
            else if (depth3ToolStripMenuItem.Checked)
                minDepth = 3;
            else if (depth4ToolStripMenuItem.Checked)
                minDepth = 4;
            else if (depth5ToolStripMenuItem.Checked)
                minDepth = 5;
            else if (depth6ToolStripMenuItem.Checked)
                minDepth = 6;
            else if (depthAutoToolStripMenuItem.Checked)
                minDepth = -1;
            else
                throw new Exception("Fel på djupinställningen!");
        }
        private void moveNowButton_Click(object sender, EventArgs e)
        {
            blobFish.moveNowFlag.setValue(1);
            moveNowButton.Enabled = false;
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PGNHandler handler = new PGNHandler();
            PGNHandler.save(game);
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game = PGNHandler.load();
            game.goToFirstPosition();
            display(game.currentPosition);
        }
        private void startaOmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ponderingWorker.IsBusy)
            {
                //Kortkommando som endast tillåts om motorn inte är igång.
                DialogResult result = MessageBox.Show("Vill du starta ett nytt parti?", "Återställning av partiet", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    reset();
                }
            }
        }
        private void filpBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flipBoard();
        }
        private void takebackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ponderingWorker.IsBusy)
            {
                takeback(2);
            }
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Vill du stänga ned programmet?", "Avsluta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
        private void testsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ponderingWorker.IsBusy)
            {
                scoresheetBox.Text = Tests.runTests();
            }
        }
        private Engine choosePlayingStyle()
        {
            //Byt namn på funktionen?
            int[] MIL = { };

            try
            {
                if (style0ToolStripMenuItem.Checked) //Normal
                {
                    return new Engine(MIL);
                }
                else if (style1ToolStripMenuItem.Checked) //Försiktig
                {
                    return new Engine(new float[] { 1f, 3f, 3f, 5f, 9f }, 0.8f,
                        new float[] { 1.2f, 2.2f, 1.4f, 0.4f, 0.1f }, 6, 1.15f, 5f, MIL, 0.15f);
                }
                else if (style2ToolStripMenuItem.Checked) //Aggressiv
                {
                    return new Engine(new float[] { 1.2f, 4f, 4f, 6.5f, 12f }, 0.4f,
                        new float[] { 1, 2, 1.4f, 0.4f, 0.1f }, 8, 0.5f, 2.5f, MIL, 0.4f);
                }
                else if (style3ToolStripMenuItem.Checked) //Experimentell
                {
                    return new Engine(new float[] { 1f, 3f, 3f, 4.5f, 9f }, 0.4f,
                        new float[] { 1, 1f, 0.8f, 0.1f, 0.05f }, 8, 1f, 1f, MIL, 0.25f);
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
        private void flipBoard()
        {
            flipped = !flipped;
            display(game.currentPosition);
        }
    }
}