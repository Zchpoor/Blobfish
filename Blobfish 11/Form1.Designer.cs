﻿namespace Blobfish_11
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.fenBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fenButton = new System.Windows.Forms.Button();
            this.boardPanel = new System.Windows.Forms.TableLayoutPanel();
            this.moveLabel = new System.Windows.Forms.Label();
            this.evalBox = new System.Windows.Forms.TextBox();
            this.toMoveLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.ponderingWorker = new System.ComponentModel.BackgroundWorker();
            this.ponderingLabel = new System.Windows.Forms.Label();
            this.ponderingPanel = new System.Windows.Forms.Panel();
            this.ponderingTimeLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.playStyleRB2 = new System.Windows.Forms.RadioButton();
            this.playStyleRB1 = new System.Windows.Forms.RadioButton();
            this.playStyleRB0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.ponderingPanel.SuspendLayout();
            this.settingsPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // fenBox
            // 
            this.fenBox.Location = new System.Drawing.Point(95, 6);
            this.fenBox.Name = "fenBox";
            this.fenBox.Size = new System.Drawing.Size(609, 22);
            this.fenBox.TabIndex = 0;
            this.fenBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fenBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ange FEN:";
            // 
            // fenButton
            // 
            this.fenButton.Location = new System.Drawing.Point(710, 5);
            this.fenButton.Name = "fenButton";
            this.fenButton.Size = new System.Drawing.Size(75, 24);
            this.fenButton.TabIndex = 2;
            this.fenButton.Text = "OK";
            this.fenButton.UseVisualStyleBackColor = true;
            this.fenButton.Click += new System.EventHandler(this.fenButton_Click);
            // 
            // boardPanel
            // 
            this.boardPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.boardPanel.ColumnCount = 8;
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.Location = new System.Drawing.Point(15, 31);
            this.boardPanel.Margin = new System.Windows.Forms.Padding(0);
            this.boardPanel.Name = "boardPanel";
            this.boardPanel.RowCount = 8;
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.boardPanel.Size = new System.Drawing.Size(480, 480);
            this.boardPanel.TabIndex = 5;
            // 
            // moveLabel
            // 
            this.moveLabel.AutoSize = true;
            this.moveLabel.Location = new System.Drawing.Point(704, 31);
            this.moveLabel.Name = "moveLabel";
            this.moveLabel.Size = new System.Drawing.Size(87, 17);
            this.moveLabel.TabIndex = 6;
            this.moveLabel.Text = "(default text)";
            // 
            // evalBox
            // 
            this.evalBox.AcceptsReturn = true;
            this.evalBox.AcceptsTab = true;
            this.evalBox.Location = new System.Drawing.Point(797, 5);
            this.evalBox.Multiline = true;
            this.evalBox.Name = "evalBox";
            this.evalBox.ReadOnly = true;
            this.evalBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.evalBox.Size = new System.Drawing.Size(258, 543);
            this.evalBox.TabIndex = 7;
            this.evalBox.TabStop = false;
            // 
            // toMoveLabel
            // 
            this.toMoveLabel.AutoSize = true;
            this.toMoveLabel.Location = new System.Drawing.Point(550, 31);
            this.toMoveLabel.Name = "toMoveLabel";
            this.toMoveLabel.Size = new System.Drawing.Size(87, 17);
            this.toMoveLabel.TabIndex = 8;
            this.toMoveLabel.Text = "(default text)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(130, 115);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dator";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(6, 75);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(62, 21);
            this.radioButton3.TabIndex = 5;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Svart";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 48);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(45, 21);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "Vit";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(101, 21);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.Text = "Ingen dator";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
            // 
            // ponderingWorker
            // 
            this.ponderingWorker.WorkerSupportsCancellation = true;
            this.ponderingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ponderingWorker_DoWork);
            this.ponderingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ponderingWorker_RunWorkerCompleted);
            // 
            // ponderingLabel
            // 
            this.ponderingLabel.AutoSize = true;
            this.ponderingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ponderingLabel.Location = new System.Drawing.Point(3, 9);
            this.ponderingLabel.Name = "ponderingLabel";
            this.ponderingLabel.Size = new System.Drawing.Size(120, 25);
            this.ponderingLabel.TabIndex = 10;
            this.ponderingLabel.Text = "(default text)";
            // 
            // ponderingPanel
            // 
            this.ponderingPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ponderingPanel.Controls.Add(this.ponderingTimeLabel);
            this.ponderingPanel.Controls.Add(this.cancelButton);
            this.ponderingPanel.Controls.Add(this.ponderingLabel);
            this.ponderingPanel.Location = new System.Drawing.Point(553, 65);
            this.ponderingPanel.Name = "ponderingPanel";
            this.ponderingPanel.Size = new System.Drawing.Size(200, 110);
            this.ponderingPanel.TabIndex = 11;
            this.ponderingPanel.Visible = false;
            // 
            // ponderingTimeLabel
            // 
            this.ponderingTimeLabel.AutoSize = true;
            this.ponderingTimeLabel.Location = new System.Drawing.Point(8, 43);
            this.ponderingTimeLabel.Name = "ponderingTimeLabel";
            this.ponderingTimeLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ponderingTimeLabel.Size = new System.Drawing.Size(44, 17);
            this.ponderingTimeLabel.TabIndex = 12;
            this.ponderingTimeLabel.Text = "00:00";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(8, 63);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(95, 39);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Avbryt";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // settingsPanel
            // 
            this.settingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsPanel.Controls.Add(this.groupBox2);
            this.settingsPanel.Controls.Add(this.groupBox1);
            this.settingsPanel.Location = new System.Drawing.Point(553, 181);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(238, 367);
            this.settingsPanel.TabIndex = 12;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.playStyleRB2);
            this.groupBox2.Controls.Add(this.playStyleRB1);
            this.groupBox2.Controls.Add(this.playStyleRB0);
            this.groupBox2.Location = new System.Drawing.Point(10, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(130, 115);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Spelstil";
            // 
            // playStyleRB2
            // 
            this.playStyleRB2.AutoSize = true;
            this.playStyleRB2.Location = new System.Drawing.Point(6, 75);
            this.playStyleRB2.Name = "playStyleRB2";
            this.playStyleRB2.Size = new System.Drawing.Size(110, 21);
            this.playStyleRB2.TabIndex = 5;
            this.playStyleRB2.Text = "Materialistisk";
            this.playStyleRB2.UseVisualStyleBackColor = true;
            this.playStyleRB2.CheckedChanged += new System.EventHandler(this.playStyleRB_CheckedChanged);
            // 
            // playStyleRB1
            // 
            this.playStyleRB1.AutoSize = true;
            this.playStyleRB1.Location = new System.Drawing.Point(6, 48);
            this.playStyleRB1.Name = "playStyleRB1";
            this.playStyleRB1.Size = new System.Drawing.Size(82, 21);
            this.playStyleRB1.TabIndex = 4;
            this.playStyleRB1.Text = "Försiktig";
            this.playStyleRB1.UseVisualStyleBackColor = true;
            this.playStyleRB1.CheckedChanged += new System.EventHandler(this.playStyleRB_CheckedChanged);
            // 
            // playStyleRB0
            // 
            this.playStyleRB0.AutoSize = true;
            this.playStyleRB0.Checked = true;
            this.playStyleRB0.Location = new System.Drawing.Point(6, 21);
            this.playStyleRB0.Name = "playStyleRB0";
            this.playStyleRB0.Size = new System.Drawing.Size(74, 21);
            this.playStyleRB0.TabIndex = 3;
            this.playStyleRB0.TabStop = true;
            this.playStyleRB0.Text = "Normal";
            this.playStyleRB0.UseVisualStyleBackColor = true;
            this.playStyleRB0.CheckedChanged += new System.EventHandler(this.playStyleRB_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 560);
            this.Controls.Add(this.settingsPanel);
            this.Controls.Add(this.ponderingPanel);
            this.Controls.Add(this.toMoveLabel);
            this.Controls.Add(this.evalBox);
            this.Controls.Add(this.moveLabel);
            this.Controls.Add(this.boardPanel);
            this.Controls.Add(this.fenButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fenBox);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Blobfish 11";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ponderingPanel.ResumeLayout(false);
            this.ponderingPanel.PerformLayout();
            this.settingsPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fenBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button fenButton;
        private System.Windows.Forms.TableLayoutPanel boardPanel;
        private System.Windows.Forms.Label moveLabel;
        private System.Windows.Forms.TextBox evalBox;
        private System.Windows.Forms.Label toMoveLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.ComponentModel.BackgroundWorker ponderingWorker;
        private System.Windows.Forms.Label ponderingLabel;
        private System.Windows.Forms.Panel ponderingPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label ponderingTimeLabel;
        private System.Windows.Forms.Panel settingsPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton playStyleRB2;
        private System.Windows.Forms.RadioButton playStyleRB1;
        private System.Windows.Forms.RadioButton playStyleRB0;
    }
}

