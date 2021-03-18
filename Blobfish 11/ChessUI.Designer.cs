namespace Blobfish_11
{
    public partial class ChessUI
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
            this.evalBox = new System.Windows.Forms.TextBox();
            this.toMoveLabel = new System.Windows.Forms.Label();
            this.ponderingWorker = new System.ComponentModel.BackgroundWorker();
            this.ponderingLabel = new System.Windows.Forms.Label();
            this.ponderingPanel = new System.Windows.Forms.Panel();
            this.moveNowButton = new System.Windows.Forms.Button();
            this.ponderingTimeLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ponderingTimer = new System.Windows.Forms.Timer(this.components);
            this.moveLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.partiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.partiToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.filpBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.protokollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.taTillbakaDragToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.startaOmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.motorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.engineColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computerNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computerWhiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computerBlackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computerBothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depthAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depth2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depth3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depth4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depth5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depth6ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.style0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.style1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.style2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.style3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ponderingPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fenBox
            // 
            this.fenBox.Location = new System.Drawing.Point(95, 35);
            this.fenBox.Name = "fenBox";
            this.fenBox.Size = new System.Drawing.Size(685, 22);
            this.fenBox.TabIndex = 0;
            this.fenBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fenBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ange FEN:";
            // 
            // fenButton
            // 
            this.fenButton.Location = new System.Drawing.Point(786, 33);
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
            this.boardPanel.Location = new System.Drawing.Point(15, 60);
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
            this.boardPanel.Size = new System.Drawing.Size(600, 550);
            this.boardPanel.TabIndex = 5;
            // 
            // evalBox
            // 
            this.evalBox.AcceptsReturn = true;
            this.evalBox.AcceptsTab = true;
            this.evalBox.Location = new System.Drawing.Point(622, 199);
            this.evalBox.Multiline = true;
            this.evalBox.Name = "evalBox";
            this.evalBox.ReadOnly = true;
            this.evalBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.evalBox.Size = new System.Drawing.Size(239, 411);
            this.evalBox.TabIndex = 7;
            this.evalBox.TabStop = false;
            // 
            // toMoveLabel
            // 
            this.toMoveLabel.AutoSize = true;
            this.toMoveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.toMoveLabel.Location = new System.Drawing.Point(618, 60);
            this.toMoveLabel.Name = "toMoveLabel";
            this.toMoveLabel.Size = new System.Drawing.Size(103, 20);
            this.toMoveLabel.TabIndex = 8;
            this.toMoveLabel.Text = "(default text)";
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
            this.ponderingPanel.Controls.Add(this.moveNowButton);
            this.ponderingPanel.Controls.Add(this.ponderingTimeLabel);
            this.ponderingPanel.Controls.Add(this.cancelButton);
            this.ponderingPanel.Controls.Add(this.ponderingLabel);
            this.ponderingPanel.Location = new System.Drawing.Point(621, 83);
            this.ponderingPanel.Name = "ponderingPanel";
            this.ponderingPanel.Size = new System.Drawing.Size(240, 110);
            this.ponderingPanel.TabIndex = 11;
            this.ponderingPanel.Visible = false;
            // 
            // moveNowButton
            // 
            this.moveNowButton.Location = new System.Drawing.Point(135, 63);
            this.moveNowButton.Name = "moveNowButton";
            this.moveNowButton.Size = new System.Drawing.Size(95, 39);
            this.moveNowButton.TabIndex = 13;
            this.moveNowButton.Text = "Dra nu!";
            this.moveNowButton.UseVisualStyleBackColor = true;
            this.moveNowButton.Click += new System.EventHandler(this.moveNowButton_Click);
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
            // ponderingTimer
            // 
            this.ponderingTimer.Interval = 500;
            this.ponderingTimer.Tick += new System.EventHandler(this.ponderingTimer_Tick);
            // 
            // moveLabel
            // 
            this.moveLabel.AutoSize = true;
            this.moveLabel.Location = new System.Drawing.Point(775, 60);
            this.moveLabel.Name = "moveLabel";
            this.moveLabel.Size = new System.Drawing.Size(87, 17);
            this.moveLabel.TabIndex = 6;
            this.moveLabel.Text = "(default text)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.partiToolStripMenuItem,
            this.partiToolStripMenuItem1,
            this.motorToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ShowItemToolTips = true;
            this.menuStrip1.Size = new System.Drawing.Size(871, 28);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // partiToolStripMenuItem
            // 
            this.partiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.toolStripSeparator3,
            this.testsToolStripMenuItem,
            this.toolStripSeparator4,
            this.closeToolStripMenuItem});
            this.partiToolStripMenuItem.Name = "partiToolStripMenuItem";
            this.partiToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.partiToolStripMenuItem.Text = "Program";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(215, 26);
            this.saveToolStripMenuItem.Text = "Spara parti";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // testsToolStripMenuItem
            // 
            this.testsToolStripMenuItem.Name = "testsToolStripMenuItem";
            this.testsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.testsToolStripMenuItem.Size = new System.Drawing.Size(215, 26);
            this.testsToolStripMenuItem.Text = "Kör tester";
            this.testsToolStripMenuItem.Click += new System.EventHandler(this.testsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(215, 26);
            this.closeToolStripMenuItem.Text = "Stäng";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // partiToolStripMenuItem1
            // 
            this.partiToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filpBoardToolStripMenuItem,
            this.protokollToolStripMenuItem,
            this.toolStripSeparator1,
            this.taTillbakaDragToolStripMenuItem,
            this.toolStripSeparator2,
            this.startaOmToolStripMenuItem});
            this.partiToolStripMenuItem1.Name = "partiToolStripMenuItem1";
            this.partiToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.partiToolStripMenuItem1.Size = new System.Drawing.Size(52, 24);
            this.partiToolStripMenuItem1.Text = "Parti";
            // 
            // filpBoardToolStripMenuItem
            // 
            this.filpBoardToolStripMenuItem.Name = "filpBoardToolStripMenuItem";
            this.filpBoardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.filpBoardToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.filpBoardToolStripMenuItem.Text = "Vänd på brädet";
            this.filpBoardToolStripMenuItem.Click += new System.EventHandler(this.filpBoardToolStripMenuItem_Click);
            // 
            // protokollToolStripMenuItem
            // 
            this.protokollToolStripMenuItem.Name = "protokollToolStripMenuItem";
            this.protokollToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.protokollToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.protokollToolStripMenuItem.Text = "Protokoll";
            this.protokollToolStripMenuItem.Click += new System.EventHandler(this.protokollToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(242, 6);
            // 
            // taTillbakaDragToolStripMenuItem
            // 
            this.taTillbakaDragToolStripMenuItem.Name = "taTillbakaDragToolStripMenuItem";
            this.taTillbakaDragToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.taTillbakaDragToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.taTillbakaDragToolStripMenuItem.Text = "Ta tillbaka drag";
            this.taTillbakaDragToolStripMenuItem.Click += new System.EventHandler(this.taTillbakaDragToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(242, 6);
            // 
            // startaOmToolStripMenuItem
            // 
            this.startaOmToolStripMenuItem.Name = "startaOmToolStripMenuItem";
            this.startaOmToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.startaOmToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.startaOmToolStripMenuItem.Text = "Starta nytt";
            this.startaOmToolStripMenuItem.Click += new System.EventHandler(this.startaOmToolStripMenuItem_Click);
            // 
            // motorToolStripMenuItem
            // 
            this.motorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.engineColorMenuItem,
            this.depthToolStripMenuItem,
            this.styleToolStripMenuItem});
            this.motorToolStripMenuItem.Name = "motorToolStripMenuItem";
            this.motorToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.motorToolStripMenuItem.Text = "Motor";
            // 
            // engineColorMenuItem
            // 
            this.engineColorMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.computerNoneToolStripMenuItem,
            this.computerWhiteToolStripMenuItem,
            this.computerBlackToolStripMenuItem,
            this.computerBothToolStripMenuItem});
            this.engineColorMenuItem.Name = "engineColorMenuItem";
            this.engineColorMenuItem.Size = new System.Drawing.Size(140, 26);
            this.engineColorMenuItem.Text = "Färg";
            // 
            // computerNoneToolStripMenuItem
            // 
            this.computerNoneToolStripMenuItem.CheckOnClick = true;
            this.computerNoneToolStripMenuItem.Name = "computerNoneToolStripMenuItem";
            this.computerNoneToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.computerNoneToolStripMenuItem.Text = "Ingen motor";
            this.computerNoneToolStripMenuItem.CheckedChanged += new System.EventHandler(this.engineColor_CheckedChanged);
            // 
            // computerWhiteToolStripMenuItem
            // 
            this.computerWhiteToolStripMenuItem.CheckOnClick = true;
            this.computerWhiteToolStripMenuItem.Name = "computerWhiteToolStripMenuItem";
            this.computerWhiteToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.computerWhiteToolStripMenuItem.Text = "Vit";
            this.computerWhiteToolStripMenuItem.CheckedChanged += new System.EventHandler(this.engineColor_CheckedChanged);
            // 
            // computerBlackToolStripMenuItem
            // 
            this.computerBlackToolStripMenuItem.Checked = true;
            this.computerBlackToolStripMenuItem.CheckOnClick = true;
            this.computerBlackToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.computerBlackToolStripMenuItem.Name = "computerBlackToolStripMenuItem";
            this.computerBlackToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.computerBlackToolStripMenuItem.Text = "Svart";
            this.computerBlackToolStripMenuItem.CheckedChanged += new System.EventHandler(this.engineColor_CheckedChanged);
            // 
            // computerBothToolStripMenuItem
            // 
            this.computerBothToolStripMenuItem.CheckOnClick = true;
            this.computerBothToolStripMenuItem.Name = "computerBothToolStripMenuItem";
            this.computerBothToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.computerBothToolStripMenuItem.Text = "Bägge";
            this.computerBothToolStripMenuItem.CheckedChanged += new System.EventHandler(this.engineColor_CheckedChanged);
            // 
            // depthToolStripMenuItem
            // 
            this.depthToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.depthAutoToolStripMenuItem,
            this.depth2ToolStripMenuItem,
            this.depth3ToolStripMenuItem,
            this.depth4ToolStripMenuItem,
            this.depth5ToolStripMenuItem,
            this.depth6ToolStripMenuItem});
            this.depthToolStripMenuItem.Name = "depthToolStripMenuItem";
            this.depthToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            this.depthToolStripMenuItem.Text = "Djup";
            // 
            // depthAutoToolStripMenuItem
            // 
            this.depthAutoToolStripMenuItem.Checked = true;
            this.depthAutoToolStripMenuItem.CheckOnClick = true;
            this.depthAutoToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.depthAutoToolStripMenuItem.Name = "depthAutoToolStripMenuItem";
            this.depthAutoToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.depthAutoToolStripMenuItem.Text = "Automatisk";
            this.depthAutoToolStripMenuItem.CheckedChanged += new System.EventHandler(this.depthToolStripMenuItem_CheckedChanged);
            // 
            // depth2ToolStripMenuItem
            // 
            this.depth2ToolStripMenuItem.CheckOnClick = true;
            this.depth2ToolStripMenuItem.Name = "depth2ToolStripMenuItem";
            this.depth2ToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.depth2ToolStripMenuItem.Text = "2";
            this.depth2ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.depthToolStripMenuItem_CheckedChanged);
            // 
            // depth3ToolStripMenuItem
            // 
            this.depth3ToolStripMenuItem.CheckOnClick = true;
            this.depth3ToolStripMenuItem.Name = "depth3ToolStripMenuItem";
            this.depth3ToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.depth3ToolStripMenuItem.Text = "3";
            this.depth3ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.depthToolStripMenuItem_CheckedChanged);
            // 
            // depth4ToolStripMenuItem
            // 
            this.depth4ToolStripMenuItem.CheckOnClick = true;
            this.depth4ToolStripMenuItem.Name = "depth4ToolStripMenuItem";
            this.depth4ToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.depth4ToolStripMenuItem.Text = "4";
            this.depth4ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.depthToolStripMenuItem_CheckedChanged);
            // 
            // depth5ToolStripMenuItem
            // 
            this.depth5ToolStripMenuItem.CheckOnClick = true;
            this.depth5ToolStripMenuItem.Name = "depth5ToolStripMenuItem";
            this.depth5ToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.depth5ToolStripMenuItem.Text = "5";
            this.depth5ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.depthToolStripMenuItem_CheckedChanged);
            // 
            // depth6ToolStripMenuItem
            // 
            this.depth6ToolStripMenuItem.CheckOnClick = true;
            this.depth6ToolStripMenuItem.Name = "depth6ToolStripMenuItem";
            this.depth6ToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.depth6ToolStripMenuItem.Text = "6";
            this.depth6ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.depthToolStripMenuItem_CheckedChanged);
            // 
            // styleToolStripMenuItem
            // 
            this.styleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.style0ToolStripMenuItem,
            this.style1ToolStripMenuItem,
            this.style2ToolStripMenuItem,
            this.style3ToolStripMenuItem});
            this.styleToolStripMenuItem.Name = "styleToolStripMenuItem";
            this.styleToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            this.styleToolStripMenuItem.Text = "Spelstil";
            // 
            // style0ToolStripMenuItem
            // 
            this.style0ToolStripMenuItem.Checked = true;
            this.style0ToolStripMenuItem.CheckOnClick = true;
            this.style0ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.style0ToolStripMenuItem.Name = "style0ToolStripMenuItem";
            this.style0ToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.style0ToolStripMenuItem.Text = "Normal";
            this.style0ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.styleToolStripMenuItem_CheckedChanged);
            // 
            // style1ToolStripMenuItem
            // 
            this.style1ToolStripMenuItem.CheckOnClick = true;
            this.style1ToolStripMenuItem.Name = "style1ToolStripMenuItem";
            this.style1ToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.style1ToolStripMenuItem.Text = "Försiktig";
            this.style1ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.styleToolStripMenuItem_CheckedChanged);
            // 
            // style2ToolStripMenuItem
            // 
            this.style2ToolStripMenuItem.CheckOnClick = true;
            this.style2ToolStripMenuItem.Name = "style2ToolStripMenuItem";
            this.style2ToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.style2ToolStripMenuItem.Text = "Aggressiv";
            this.style2ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.styleToolStripMenuItem_CheckedChanged);
            // 
            // style3ToolStripMenuItem
            // 
            this.style3ToolStripMenuItem.CheckOnClick = true;
            this.style3ToolStripMenuItem.Name = "style3ToolStripMenuItem";
            this.style3ToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.style3ToolStripMenuItem.Text = "Experimentell";
            this.style3ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.styleToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(221, 6);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(221, 6);
            // 
            // ChessUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 617);
            this.Controls.Add(this.ponderingPanel);
            this.Controls.Add(this.toMoveLabel);
            this.Controls.Add(this.evalBox);
            this.Controls.Add(this.moveLabel);
            this.Controls.Add(this.boardPanel);
            this.Controls.Add(this.fenButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fenBox);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "ChessUI";
            this.Text = "Blobfish 11";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChessUI_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChessUI_KeyDown);
            this.ponderingPanel.ResumeLayout(false);
            this.ponderingPanel.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fenBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button fenButton;
        private System.Windows.Forms.TableLayoutPanel boardPanel;
        private System.Windows.Forms.TextBox evalBox;
        private System.Windows.Forms.Label toMoveLabel;
        private System.ComponentModel.BackgroundWorker ponderingWorker;
        private System.Windows.Forms.Label ponderingLabel;
        private System.Windows.Forms.Panel ponderingPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Timer ponderingTimer;
        private System.Windows.Forms.Label ponderingTimeLabel;
        private System.Windows.Forms.Label moveLabel;
        private System.Windows.Forms.Button moveNowButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem partiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem engineColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computerNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computerWhiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computerBlackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computerBothToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depthAutoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depth2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depth3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depth4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depth5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem depth6ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem style0ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem style1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem style2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem style3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem partiToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startaOmToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem taTillbakaDragToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem filpBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem protokollToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

