namespace Blobfish_11
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
            this.fenBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.evalBox = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.boardPanel = new System.Windows.Forms.TableLayoutPanel();
            this.moveLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fenBox
            // 
            this.fenBox.Location = new System.Drawing.Point(95, 6);
            this.fenBox.Name = "fenBox";
            this.fenBox.Size = new System.Drawing.Size(609, 22);
            this.fenBox.TabIndex = 0;
            this.fenBox.Text = "r1bqkb1r/pppp1ppp/5n2/4n3/4P3/2N5/PPPP1PPP/R1BQKB1R w KQkq - 0 1";
            this.fenBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fenBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter FEN:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(710, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 24);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // evalBox
            // 
            this.evalBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.evalBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.evalBox.Location = new System.Drawing.Point(1055, 0);
            this.evalBox.Name = "evalBox";
            this.evalBox.Size = new System.Drawing.Size(259, 560);
            this.evalBox.TabIndex = 3;
            this.evalBox.Text = "N/A";
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.AcceptsTab = true;
            this.textBox1.Location = new System.Drawing.Point(794, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(258, 560);
            this.textBox1.TabIndex = 4;
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
            this.moveLabel.Size = new System.Drawing.Size(0, 17);
            this.moveLabel.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1314, 560);
            this.Controls.Add(this.moveLabel);
            this.Controls.Add(this.boardPanel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.evalBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fenBox);
            this.Name = "Form1";
            this.Text = "Blobfish 11";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fenBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label evalBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TableLayoutPanel boardPanel;
        private System.Windows.Forms.Label moveLabel;
    }
}

