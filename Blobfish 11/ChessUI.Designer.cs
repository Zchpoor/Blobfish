namespace Blobfish_11
{
    partial class ChessUI
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
            this.compColorBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.ponderingWorker = new System.ComponentModel.BackgroundWorker();
            this.ponderingLabel = new System.Windows.Forms.Label();
            this.ponderingPanel = new System.Windows.Forms.Panel();
            this.ponderingTimeLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ponderingTimer = new System.Windows.Forms.Timer(this.components);
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.depthRB4 = new System.Windows.Forms.RadioButton();
            this.depthRB5 = new System.Windows.Forms.RadioButton();
            this.depthRB3 = new System.Windows.Forms.RadioButton();
            this.depthRB2 = new System.Windows.Forms.RadioButton();
            this.depthRB1 = new System.Windows.Forms.RadioButton();
            this.depthRB0 = new System.Windows.Forms.RadioButton();
            this.playStyleBox = new System.Windows.Forms.GroupBox();
            this.playStyleRB3 = new System.Windows.Forms.RadioButton();
            this.playStyleRB2 = new System.Windows.Forms.RadioButton();
            this.playStyleRB1 = new System.Windows.Forms.RadioButton();
            this.playStyleRB0 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.compColorBox1.SuspendLayout();
            this.ponderingPanel.SuspendLayout();
            this.settingsPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.playStyleBox.SuspendLayout();
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
            this.evalBox.Location = new System.Drawing.Point(844, 6);
            this.evalBox.Multiline = true;
            this.evalBox.Name = "evalBox";
            this.evalBox.ReadOnly = true;
            this.evalBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.evalBox.Size = new System.Drawing.Size(254, 543);
            this.evalBox.TabIndex = 7;
            this.evalBox.TabStop = false;
            // 
            // toMoveLabel
            // 
            this.toMoveLabel.AutoSize = true;
            this.toMoveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.toMoveLabel.Location = new System.Drawing.Point(550, 31);
            this.toMoveLabel.Name = "toMoveLabel";
            this.toMoveLabel.Size = new System.Drawing.Size(103, 20);
            this.toMoveLabel.TabIndex = 8;
            this.toMoveLabel.Text = "(default text)";
            // 
            // compColorBox1
            // 
            this.compColorBox1.Controls.Add(this.radioButton4);
            this.compColorBox1.Controls.Add(this.radioButton3);
            this.compColorBox1.Controls.Add(this.radioButton2);
            this.compColorBox1.Controls.Add(this.radioButton1);
            this.compColorBox1.Location = new System.Drawing.Point(9, 3);
            this.compColorBox1.Name = "compColorBox1";
            this.compColorBox1.Size = new System.Drawing.Size(130, 132);
            this.compColorBox1.TabIndex = 9;
            this.compColorBox1.TabStop = false;
            this.compColorBox1.Text = "Färg";
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(6, 102);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(70, 21);
            this.radioButton4.TabIndex = 6;
            this.radioButton4.Text = "Bägge";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
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
            this.ponderingTimer.Interval = 500;
            this.ponderingTimer.Tick += new System.EventHandler(this.ponderingTimer_Tick);
            // 
            // settingsPanel
            // 
            this.settingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsPanel.Controls.Add(this.groupBox1);
            this.settingsPanel.Controls.Add(this.playStyleBox);
            this.settingsPanel.Controls.Add(this.compColorBox1);
            this.settingsPanel.Location = new System.Drawing.Point(553, 211);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(285, 337);
            this.settingsPanel.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.depthRB4);
            this.groupBox1.Controls.Add(this.depthRB5);
            this.groupBox1.Controls.Add(this.depthRB3);
            this.groupBox1.Controls.Add(this.depthRB2);
            this.groupBox1.Controls.Add(this.depthRB1);
            this.groupBox1.Controls.Add(this.depthRB0);
            this.groupBox1.Location = new System.Drawing.Point(9, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 188);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Djup";
            // 
            // depthRB4
            // 
            this.depthRB4.AutoSize = true;
            this.depthRB4.Location = new System.Drawing.Point(6, 129);
            this.depthRB4.Name = "depthRB4";
            this.depthRB4.Size = new System.Drawing.Size(160, 21);
            this.depthRB4.TabIndex = 8;
            this.depthRB4.Text = "5   (Mycket långsam)";
            this.depthRB4.UseVisualStyleBackColor = true;
            // 
            // depthRB5
            // 
            this.depthRB5.AutoSize = true;
            this.depthRB5.Location = new System.Drawing.Point(6, 156);
            this.depthRB5.Name = "depthRB5";
            this.depthRB5.Size = new System.Drawing.Size(155, 21);
            this.depthRB5.TabIndex = 7;
            this.depthRB5.Text = "6   (Endast slutspel)";
            this.depthRB5.UseVisualStyleBackColor = true;
            this.depthRB5.CheckedChanged += new System.EventHandler(this.depthRB_CheckedChanged);
            // 
            // depthRB3
            // 
            this.depthRB3.AutoSize = true;
            this.depthRB3.Location = new System.Drawing.Point(6, 102);
            this.depthRB3.Name = "depthRB3";
            this.depthRB3.Size = new System.Drawing.Size(117, 21);
            this.depthRB3.TabIndex = 6;
            this.depthRB3.Text = "4+ (Långsam)";
            this.depthRB3.UseVisualStyleBackColor = true;
            this.depthRB3.CheckedChanged += new System.EventHandler(this.depthRB_CheckedChanged);
            // 
            // depthRB2
            // 
            this.depthRB2.AutoSize = true;
            this.depthRB2.Checked = true;
            this.depthRB2.Location = new System.Drawing.Point(6, 75);
            this.depthRB2.Name = "depthRB2";
            this.depthRB2.Size = new System.Drawing.Size(166, 21);
            this.depthRB2.TabIndex = 5;
            this.depthRB2.TabStop = true;
            this.depthRB2.Text = "4   (Rekommenderas)";
            this.depthRB2.UseVisualStyleBackColor = true;
            this.depthRB2.CheckedChanged += new System.EventHandler(this.depthRB_CheckedChanged);
            // 
            // depthRB1
            // 
            this.depthRB1.AutoSize = true;
            this.depthRB1.Location = new System.Drawing.Point(6, 48);
            this.depthRB1.Name = "depthRB1";
            this.depthRB1.Size = new System.Drawing.Size(96, 21);
            this.depthRB1.TabIndex = 4;
            this.depthRB1.Text = "3  (Snabb)";
            this.depthRB1.UseVisualStyleBackColor = true;
            this.depthRB1.CheckedChanged += new System.EventHandler(this.depthRB_CheckedChanged);
            // 
            // depthRB0
            // 
            this.depthRB0.AutoSize = true;
            this.depthRB0.Location = new System.Drawing.Point(6, 21);
            this.depthRB0.Name = "depthRB0";
            this.depthRB0.Size = new System.Drawing.Size(146, 21);
            this.depthRB0.TabIndex = 3;
            this.depthRB0.Text = "2   (Mycket snabb)";
            this.depthRB0.UseVisualStyleBackColor = true;
            this.depthRB0.CheckedChanged += new System.EventHandler(this.depthRB_CheckedChanged);
            // 
            // playStyleBox
            // 
            this.playStyleBox.Controls.Add(this.playStyleRB3);
            this.playStyleBox.Controls.Add(this.playStyleRB2);
            this.playStyleBox.Controls.Add(this.playStyleRB1);
            this.playStyleBox.Controls.Add(this.playStyleRB0);
            this.playStyleBox.Location = new System.Drawing.Point(145, 3);
            this.playStyleBox.Name = "playStyleBox";
            this.playStyleBox.Size = new System.Drawing.Size(135, 132);
            this.playStyleBox.TabIndex = 10;
            this.playStyleBox.TabStop = false;
            this.playStyleBox.Text = "Spelstil";
            // 
            // playStyleRB3
            // 
            this.playStyleRB3.AutoSize = true;
            this.playStyleRB3.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.playStyleRB3.Location = new System.Drawing.Point(6, 102);
            this.playStyleRB3.Name = "playStyleRB3";
            this.playStyleRB3.Size = new System.Drawing.Size(113, 21);
            this.playStyleRB3.TabIndex = 6;
            this.playStyleRB3.Text = "Experimentell";
            this.playStyleRB3.UseVisualStyleBackColor = true;
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
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(550, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "Inställningar:";
            // 
            // ChessUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1110, 560);
            this.Controls.Add(this.label2);
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
            this.Name = "ChessUI";
            this.Text = "Blobfish 11";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChessUI_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChessUI_KeyDown);
            this.compColorBox1.ResumeLayout(false);
            this.compColorBox1.PerformLayout();
            this.ponderingPanel.ResumeLayout(false);
            this.ponderingPanel.PerformLayout();
            this.settingsPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.playStyleBox.ResumeLayout(false);
            this.playStyleBox.PerformLayout();
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
        private System.Windows.Forms.GroupBox compColorBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.ComponentModel.BackgroundWorker ponderingWorker;
        private System.Windows.Forms.Label ponderingLabel;
        private System.Windows.Forms.Panel ponderingPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Timer ponderingTimer;
        private System.Windows.Forms.Label ponderingTimeLabel;
        private System.Windows.Forms.Panel settingsPanel;
        private System.Windows.Forms.GroupBox playStyleBox;
        private System.Windows.Forms.RadioButton playStyleRB2;
        private System.Windows.Forms.RadioButton playStyleRB1;
        private System.Windows.Forms.RadioButton playStyleRB0;
        private System.Windows.Forms.RadioButton playStyleRB3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton depthRB3;
        private System.Windows.Forms.RadioButton depthRB2;
        private System.Windows.Forms.RadioButton depthRB1;
        private System.Windows.Forms.RadioButton depthRB0;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton depthRB5;
        private System.Windows.Forms.RadioButton depthRB4;
        private System.Windows.Forms.Label label2;
    }
}

