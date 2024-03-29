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
using System.Threading;

namespace Blobfish_11
{
    public partial class ChessUI : Form
    {
        private Square dragFromSquare = new Square(-1, -1);
        private Image toOldImage = null;
        private Image fromImage = null;
        //Byt ut till annan pekare?
        private Cursor dragCursor = Cursors.Hand;

        private bool moveablePiece(Piece piece)
        {
            if (piece == Piece.None)
            {
                return false;
            }
            else if (game.currentPosition.whiteToMove && piece.IsWhite())
            {
                return true;
            }
            else if (!game.currentPosition.whiteToMove && !piece.IsWhite())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool possibleToMove()
        {
            return !ponderingWorker.IsBusy;
        }
        private void squareMouseDown(object sender, MouseEventArgs e)
        {
            if (!possibleToMove())
                return;

            PictureBox from = sender as PictureBox;
            dragFromSquare = picBoxSquare(from);
            Piece piece = game.currentPosition[dragFromSquare];
            if (!moveablePiece(piece))
                return;

            fromImage = from.Image;
            from.Image = piecesPictures[Piece.None];
            from.DoDragDrop(fromImage, DragDropEffects.Copy);
        }
        private void squareDragEnter(object sender, DragEventArgs e)
        {
            if (!(sender is PictureBox) || fromImage == null)
                return;
            PictureBox to = sender as PictureBox;
            e.Effect = DragDropEffects.Copy;

            toOldImage = to.Image;
            Image cpy = (Image)fromImage.Clone();
            using (Graphics g = Graphics.FromImage(cpy))
            {
                using (SolidBrush br =
                new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
                {
                    g.FillRectangle(br, 0, 0, cpy.Width, cpy.Height);
                }
            }
            to.Image = cpy;
        }
        private void squareDragDrop(object sender, DragEventArgs e)
        {
            if (!(sender is PictureBox) || fromImage == null)
                return;
            bool moveWasPlayed = false;
            Square newSquare = picBoxSquare(sender as PictureBox);
            foreach (Move item in currentMoves)
            {
                if (dragFromSquare.rank == item.from.rank && dragFromSquare.line == item.from.line &&
                    newSquare.rank == item.to.rank && newSquare.line == item.to.line)
                {
                    (sender as PictureBox).Image = fromImage;
                    this.playMove(item);
                    moveWasPlayed = true;
                    break;
                }
            }
            if (!moveWasPlayed)
            {
                if (!(dragFromSquare.line == newSquare.line && dragFromSquare.rank == newSquare.rank))
                    scoresheetBox.Text = "Felaktigt drag!";
                (sender as PictureBox).Image = toOldImage;
                if (!flipped)
                    Falt[dragFromSquare.rank, dragFromSquare.line].Image = fromImage;
                else
                    Falt[7 - dragFromSquare.rank, 7 - dragFromSquare.line].Image = fromImage;
            }
            dragFromSquare = new Square(-1, -1);
            fromImage = null;
            moveLabel.Text = "";
        }
        private void squareDragLeave(object sender, EventArgs e)
        {
            if (!(sender is PictureBox) || fromImage == null)
                return;
            (sender as PictureBox).Image = toOldImage;
        }
        private void squareGiveFeedBack(object sender, GiveFeedbackEventArgs e)
        {
            if (!(sender is PictureBox) || fromImage == null)
                return;
            if (e.Effect == DragDropEffects.Copy)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = dragCursor;
            }
            else
                e.UseDefaultCursors = true;

        }
        private Square picBoxSquare(PictureBox picBox)
        {
            int xVal = picBox.Location.X; //a-h
            int yVal = picBox.Location.Y; //1-8
            xVal = xVal / (boardPanel.Size.Width / 8);
            yVal = yVal / (boardPanel.Size.Height / 8);
            if (flipped)
            {
                xVal = 7 - xVal;
                yVal = 7 - yVal;
            }
            return new Square(yVal, xVal);
        }
    }
}