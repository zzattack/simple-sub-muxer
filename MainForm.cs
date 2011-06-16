using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace SubsMuxer {
	public partial class MainForm : Form {

		public MainForm() {
			InitializeComponent();
			Logger.LogLine += new Logger.LogDelegate(Logger_LogLine);
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if (!File.Exists("mkvmerge.exe")) {
				MessageBox.Show("Cannot find mkvmerge.exe, place this in the same dir");
				Close();
			}
			dgv.Columns.Add(new DataGridViewProgressBarColumn() { HeaderText = "Progress" });
		}

		#region ui event handlers
		string defaultLanguage = "undefined";
		private void cbDefaultLanguage_SelectedIndexChanged(object sender, EventArgs e) {
			defaultLanguage = cbDefaultLanguage.Text;
		}

		private void dgv_DragEnter(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy; // Okay
			else
				e.Effect = DragDropEffects.None; // Unknown data, ignore it
		}

		private void dgv_DragDrop(object sender, DragEventArgs e) {

			// Extract the data from the DataObject-Container into a string list
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			// Do something with the data...
			foreach (string file in files)
				LoadPath(file);
		}

		void Logger_LogLine(string line, Color c) {
			if (InvokeRequired) {
				BeginInvoke(new Logger.LogDelegate(Logger_LogLine), line, c);
				return;
			}
			rtbAppLog.SelectionColor = c;
			rtbAppLog.SelectionStart = rtbAppLog.TextLength;
			rtbAppLog.AppendText(line + "\r\n");
			rtbAppLog.SelectionStart = rtbAppLog.TextLength;
			rtbAppLog.ScrollToCaret();
		}
		#endregion

		#region directory/file loading
		private void LoadPath(string file) {
			if (Directory.Exists(file))
				LoadDirectory(file);
			else if (File.Exists(file))
				LoadFile(file);
		}

		private void LoadDirectory(string dir) {
			// Logger.Info("Loading files in directory {0}", dir);
			foreach (string file in Directory.GetFiles(dir, "*.mkv", SearchOption.TopDirectoryOnly).OrderBy(f => f)) {
				LoadFile(file);
			}
			foreach (string dir2 in Directory.GetDirectories(dir)) {
				if (!dir2.EndsWith("_subsmuxed_"))
					LoadDirectory(dir2);
				else
					Logger.Warn("Not loading directory {0} because it should contain muxed files already", dir2);
			}
		}

		private void LoadFile(string file) {
			var fi = new FileInfo(file);
			if (fi.Extension != ".mkv") {
				MessageBox.Show("File '" + fi.Name + "' is not an mkv");
				return;
			}
			MkvMergeAction mergeAction = new MkvMergeAction(file);
			DataGridViewRow r = new DataGridViewRow();
			r.CreateCells(dgv);
			r.Cells[0].Value = fi.Name;
			r.Cells[0].ToolTipText = mergeAction.DetailedDescription();
			r.Cells[1].Value = string.Join(",", mergeAction.MkvInfo.SubsAvailable.ToArray());
			r.Cells[2].Value = string.Join(",", mergeAction.FolderSubs.Keys.ToArray());
			r.Cells[3].Value = mergeAction.ToString();
			r.Cells[4].Value = mergeAction.Status.ToString();
			r.Tag = mergeAction;
			dgv.Rows.Add(r);
			mergeAction.Row = r;
		}
		#endregion

		#region processing
		bool processing = false;
		private void btnStartMuxing_Click(object sender, EventArgs e) {
			if (processing) {
				MessageBox.Show("Error, already processing!");
				return;
			}
			processing = QueueNext();
		}

		delegate bool QueueNextDel();
		private bool QueueNext() {
			if (worker == null) {
				worker = new Thread(DoWork);
				worker.Start();
			}

			foreach (DataGridViewRow row in dgv.Rows) {
				MkvMergeAction mergeAction = row.Tag as MkvMergeAction;
				if (mergeAction.Status == Status.Waiting) {
					lock (actionQueue) {
						mergeAction.Finished += new EventHandler(mergeAction_Finished);
						actionQueue.Enqueue(mergeAction);
						queueFilled.Set();
					}
				return true;
				}
			}
			processing = false;
			return false;
		}

		void mergeAction_Finished(object sender, EventArgs e) {
			currentAction.Row.Cells[4].Value = currentAction.Status.ToString();
			currentAction = null;
			try {
				Invoke(new QueueNextDel(QueueNext));
			}
			catch (ObjectDisposedException) { }
		}

		Queue<MkvMergeAction> actionQueue = new Queue<MkvMergeAction>();
		bool appExit = false;
		Thread worker;
		MkvMergeAction currentAction;
		AutoResetEvent queueFilled = new AutoResetEvent(false);
		void DoWork() {
			while (!appExit) {
				queueFilled.WaitOne();
				MkvMergeAction mergeAction;
				lock (actionQueue) {
					if (actionQueue.Count > 0)
						mergeAction = actionQueue.Dequeue();
					else
						continue;
				}
				currentAction = mergeAction;
				mergeAction.Perform(defaultLanguage, this.tbMkvMergeLog);
			}
		}

		#endregion

		#region abortion

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (processing) {
				var dr = MessageBox.Show("Currently processing. Are you sure you want to exit?",
					"Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
				if (dr != System.Windows.Forms.DialogResult.OK)
					e.Cancel = true;
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			appExit = true;
			queueFilled.Set();
			if (worker != null) 
				worker.Join();			
			if (currentAction != null)
				currentAction.Abort();
		}

		#endregion

	}
}