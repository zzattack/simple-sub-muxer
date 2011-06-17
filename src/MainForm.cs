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
using Microsoft.Win32;

namespace SubsMuxer {
	public partial class MainForm : Form {

		public MainForm() {
			InitializeComponent();
			Logger.LogLine += new Logger.LogDelegate(Logger_LogLine);
		}

		static string FindMkvMerge() {
			RegistryKey rkey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\mmg.exe");
			if (rkey != null) {
				string mmg = rkey.GetValue("") as string;
				if (mmg != null)
					return mmg.Substring(0, mmg.Length - 7) + "mkvmerge.exe";
			}
			return null;
		}

		public static string MkvMergeExecutable = FindMkvMerge();
		private void MainForm_Load(object sender, EventArgs e) {
			if (MkvMergeExecutable == null || !File.Exists(MkvMergeExecutable)) {
				MessageBox.Show("Cannot find mkvmerge.exe, please install the MKVToolNix package from http://www.bunkus.org/videotools/mkvtoolnix/downloads.html");
				Close();
			}
			dgv.Columns.Add(new DataGridViewProgressBarColumn() { HeaderText = "Progress" });
			Logger.Info("Program started {0}", DateTime.Now);
			Logger.Info("--------------------------------------------------------------\r\n");
		}

		#region ui event handlers
		string defaultLanguage = "English";
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
			object o = e.Data.GetData(DataFormats.FileDrop, false);

			ParameterizedThreadStart pts = new ParameterizedThreadStart(BeginLoadDragDrop);
			Thread loadFilesThread = new Thread(pts);
			loadFilesThread.Start(o);
		}

		void BeginLoadDragDrop(object o) {
			string[] files = (string[])o;
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
			Invoke(new NewRowDelegate(NewRow), fi, mergeAction);
		}

		delegate void NewRowDelegate(FileInfo f, MkvMergeAction act);
		void NewRow(FileInfo f, MkvMergeAction act) {
			DataGridViewRow r = new DataGridViewRow();
			r.CreateCells(dgv);
			r.Cells[0].Value = f.Name;
			r.Cells[0].ToolTipText = act.JobDescription();
			r.Cells[1].Value = string.Join(",", act.MkvInfo.SubsAvailable.ToArray());
			r.Cells[2].Value = string.Join(",", act.FolderSubs.Keys.ToArray());
			r.Cells[3].Value = act.ToString();
			r.Cells[4].Value = act.Status.ToString();
			r.Tag = act;
			dgv.Rows.Add(r);
			act.Row = r;
			dgv.FirstDisplayedScrollingRowIndex = r.Index;
			if (act.Status == Status.Waiting && processing)
				progbarTotal.Maximum += 100;
		}


		#endregion

		#region processing
		bool processing = false;
		private void btnStartMuxing_Click(object sender, EventArgs e) {
			if (processing) {
				MessageBox.Show("Error, already processing!");
				return;
			}
			int numRows = 0;
			foreach (DataGridViewRow r in dgv.Rows) {
				MkvMergeAction act = r.Tag as MkvMergeAction;
				if (act.Status == Status.Waiting)
					numRows++;
			}
			progbarTotal.Maximum = numRows * 100;
			progbarTotal.Value = 0;
			finishedActions = 0;
			processing = QueueNext();
		}

		delegate bool QueueNextDel();
		private bool QueueNext() {
			if (worker == null) {
				worker = new Thread(DoWork);
				worker.Start();
			}

			progbarTotal.Value = finishedActions * 100;

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
				else if (mergeAction.Status == Status.Finished)
					mergeAction.Row.Cells[5].Value = "100";
			}
			processing = false;
			return false;
		}

		void mergeAction_Finished(object sender, EventArgs e) {
			if (appExit) return;
			finishedActions++;
			currentAction.Row.Cells[4].Value = currentAction.Status.ToString();
			currentAction = null;
			try {
				Invoke(new QueueNextDel(QueueNext));
			}
			catch (ObjectDisposedException) { }
		}

		Queue<MkvMergeAction> actionQueue = new Queue<MkvMergeAction>();
		bool appExit = false;
		int finishedActions = 0;
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
				mergeAction.Perform(defaultLanguage, this.rtbMkvMergeLog, this.progbarTotal);
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
			if (currentAction != null)
				currentAction.Abort();
			appExit = true;
			queueFilled.Set();
			if (worker != null)
				worker.Join();
		}

		#endregion

		private void usageInfoToolStripMenuItem_Click(object sender, EventArgs e) {
			MessageBox.Show(
@"This tool allows you to quickly mux in lots of .srt 
subtitles to your mkv video files. Simply drag a folder
of some files over the large grid in the middle. 
It's best not to load your entire media collection at once,
because every file is opened by mkvmerge to determine the 
available tracks, but loading just a TV season, or even an 
entire series should be fine.

The tool searches for subtitles in the same folder as that
holds the .mkv files it finds. If there are subtitles carrying
the same name as the video, then you can mux them in. If the
.srt has the same name as the video, the subs will be mapped to
the default language selected at the bottom. If the sub is 
named, for example, Show.S01E01.720p.HDTV-LOL.fr.srt then the
subtitle will be mapped to French after muxing.

Resulting video files are placed in the _subsmuxed_ folder in the
same dir as the original .mkv. Originals are never deleted, but
stuff that already exists in _subsmuxed_ will be overwritten!
", "Usage info", MessageBoxButtons.OK);
		}

		private void aboutToolStripMenuItem1_Click(object sender, EventArgs e) {
			MessageBox.Show(
@"Written by Frank Razenberg
17.06.2011", "Usage info", MessageBoxButtons.OK);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}

		private void applicationLogToolStripMenuItem_Click(object sender, EventArgs e) {
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				rtbAppLog.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.UnicodePlainText);
		}

		private void mkvmergeLogToolStripMenuItem_Click(object sender, EventArgs e) {
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				rtbMkvMergeLog.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.UnicodePlainText);
		}

	}
}