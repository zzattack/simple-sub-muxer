namespace SubsMuxer {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dgv = new System.Windows.Forms.DataGridView();
			this.dgvcFilename = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvSubsInMkv = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvSubsAvailable = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvcAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvcStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.applicationLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mkvmergeLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.usageInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.progbarTotal = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.cbDefaultLanguage = new System.Windows.Forms.ComboBox();
			this.btnStartMuxing = new System.Windows.Forms.Button();
			this.pnlLogs = new System.Windows.Forms.Panel();
			this.rtbAppLog = new System.Windows.Forms.RichTextBox();
			this.rtbMkvMergeLog = new System.Windows.Forms.RichTextBox();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.pnlLogs.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgv
			// 
			this.dgv.AllowDrop = true;
			this.dgv.AllowUserToAddRows = false;
			this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcFilename,
            this.dgvSubsInMkv,
            this.dgvSubsAvailable,
            this.dgvcAction,
            this.dgvcStatus});
			this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgv.Location = new System.Drawing.Point(0, 24);
			this.dgv.Name = "dgv";
			this.dgv.Size = new System.Drawing.Size(1001, 452);
			this.dgv.TabIndex = 2;
			this.dgv.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgv_DragDrop);
			this.dgv.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgv_DragEnter);
			// 
			// dgvcFilename
			// 
			this.dgvcFilename.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dgvcFilename.FillWeight = 60F;
			this.dgvcFilename.HeaderText = "Filename";
			this.dgvcFilename.Name = "dgvcFilename";
			// 
			// dgvSubsInMkv
			// 
			this.dgvSubsInMkv.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dgvSubsInMkv.FillWeight = 15F;
			this.dgvSubsInMkv.HeaderText = "MKV subs";
			this.dgvSubsInMkv.Name = "dgvSubsInMkv";
			// 
			// dgvSubsAvailable
			// 
			this.dgvSubsAvailable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dgvSubsAvailable.FillWeight = 15F;
			this.dgvSubsAvailable.HeaderText = "Folder subs";
			this.dgvSubsAvailable.Name = "dgvSubsAvailable";
			// 
			// dgvcAction
			// 
			this.dgvcAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dgvcAction.FillWeight = 20F;
			this.dgvcAction.HeaderText = "Action";
			this.dgvcAction.Name = "dgvcAction";
			// 
			// dgvcStatus
			// 
			this.dgvcStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dgvcStatus.FillWeight = 15F;
			this.dgvcStatus.HeaderText = "Status";
			this.dgvcStatus.Name = "dgvcStatus";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1001, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationLogToolStripMenuItem,
            this.mkvmergeLogToolStripMenuItem});
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			// 
			// applicationLogToolStripMenuItem
			// 
			this.applicationLogToolStripMenuItem.Name = "applicationLogToolStripMenuItem";
			this.applicationLogToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.applicationLogToolStripMenuItem.Text = "&Application log";
			this.applicationLogToolStripMenuItem.Click += new System.EventHandler(this.applicationLogToolStripMenuItem_Click);
			// 
			// mkvmergeLogToolStripMenuItem
			// 
			this.mkvmergeLogToolStripMenuItem.Name = "mkvmergeLogToolStripMenuItem";
			this.mkvmergeLogToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.mkvmergeLogToolStripMenuItem.Text = "&Mkvmerge log";
			this.mkvmergeLogToolStripMenuItem.Click += new System.EventHandler(this.mkvmergeLogToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usageInfoToolStripMenuItem,
            this.aboutToolStripMenuItem1});
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.aboutToolStripMenuItem.Text = "Help";
			// 
			// usageInfoToolStripMenuItem
			// 
			this.usageInfoToolStripMenuItem.Name = "usageInfoToolStripMenuItem";
			this.usageInfoToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
			this.usageInfoToolStripMenuItem.Text = "Usage &info";
			this.usageInfoToolStripMenuItem.Click += new System.EventHandler(this.usageInfoToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem1
			// 
			this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
			this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(130, 22);
			this.aboutToolStripMenuItem1.Text = "A&bout";
			this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.progbarTotal);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.cbDefaultLanguage);
			this.panel1.Controls.Add(this.btnStartMuxing);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 573);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1001, 32);
			this.panel1.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(487, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Total progress";
			// 
			// progbarTotal
			// 
			this.progbarTotal.Location = new System.Drawing.Point(567, 4);
			this.progbarTotal.Name = "progbarTotal";
			this.progbarTotal.Size = new System.Drawing.Size(209, 21);
			this.progbarTotal.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(142, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Map <video>.srt to language";
			// 
			// cbDefaultLanguage
			// 
			this.cbDefaultLanguage.FormattingEnabled = true;
			this.cbDefaultLanguage.Location = new System.Drawing.Point(167, 4);
			this.cbDefaultLanguage.Name = "cbDefaultLanguage";
			this.cbDefaultLanguage.Size = new System.Drawing.Size(121, 21);
			this.cbDefaultLanguage.TabIndex = 2;
			this.cbDefaultLanguage.Text = "English";
			this.cbDefaultLanguage.SelectedIndexChanged += new System.EventHandler(this.cbDefaultLanguage_SelectedIndexChanged);
			// 
			// btnStartMuxing
			// 
			this.btnStartMuxing.Location = new System.Drawing.Point(294, 2);
			this.btnStartMuxing.Name = "btnStartMuxing";
			this.btnStartMuxing.Size = new System.Drawing.Size(117, 23);
			this.btnStartMuxing.TabIndex = 0;
			this.btnStartMuxing.Text = "Start muxing";
			this.btnStartMuxing.UseVisualStyleBackColor = true;
			this.btnStartMuxing.Click += new System.EventHandler(this.btnStartMuxing_Click);
			// 
			// pnlLogs
			// 
			this.pnlLogs.Controls.Add(this.rtbAppLog);
			this.pnlLogs.Controls.Add(this.rtbMkvMergeLog);
			this.pnlLogs.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlLogs.Location = new System.Drawing.Point(0, 476);
			this.pnlLogs.Name = "pnlLogs";
			this.pnlLogs.Size = new System.Drawing.Size(1001, 97);
			this.pnlLogs.TabIndex = 5;
			// 
			// rtbAppLog
			// 
			this.rtbAppLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbAppLog.Location = new System.Drawing.Point(0, 0);
			this.rtbAppLog.Name = "rtbAppLog";
			this.rtbAppLog.Size = new System.Drawing.Size(470, 97);
			this.rtbAppLog.TabIndex = 3;
			this.rtbAppLog.Text = "";
			// 
			// rtbMkvMergeLog
			// 
			this.rtbMkvMergeLog.Dock = System.Windows.Forms.DockStyle.Right;
			this.rtbMkvMergeLog.Location = new System.Drawing.Point(470, 0);
			this.rtbMkvMergeLog.Name = "rtbMkvMergeLog";
			this.rtbMkvMergeLog.Size = new System.Drawing.Size(531, 97);
			this.rtbMkvMergeLog.TabIndex = 4;
			this.rtbMkvMergeLog.Text = "Mkverge output\n-------------------------\n";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "log";
			this.saveFileDialog1.Filter = "Log file (*.log)|*.log";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1001, 605);
			this.Controls.Add(this.dgv);
			this.Controls.Add(this.pnlLogs);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.panel1);
			this.Name = "MainForm";
			this.Text = "Simple subs muxer by zzattack";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.pnlLogs.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgv;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnStartMuxing;
		private System.Windows.Forms.ComboBox cbDefaultLanguage;
		private System.Windows.Forms.Panel pnlLogs;
		private System.Windows.Forms.RichTextBox rtbMkvMergeLog;
		private System.Windows.Forms.RichTextBox rtbAppLog;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvcFilename;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvSubsInMkv;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvSubsAvailable;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvcAction;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgvcStatus;
		private System.Windows.Forms.ToolStripMenuItem usageInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ProgressBar progbarTotal;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem applicationLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mkvmergeLogToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;

	}
}

