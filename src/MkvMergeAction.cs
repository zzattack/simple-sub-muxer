using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace SubsMuxer {

	enum Status {
		Waiting,
		Started,
		Failed,
		Finished
	}

	class MkvMergeAction {
		public Dictionary<string, string> FolderSubs { get; set; }
		public MkvInfo MkvInfo { get; set; }
		public FileInfo MkvFileInfo { get; set; }
		public Status Status { get; set; }
		public DataGridViewRow Row { get; set; }

		public event EventHandler Finished;

		public MkvMergeAction() {
			FolderSubs = new Dictionary<string, string>();
		}

		public MkvMergeAction(string file)
			: this() {
			MkvInfo = MkvInfo.Parse(file);
			MkvFileInfo = new FileInfo(file);
			LoadFolderSubs(file);
		}

		private void LoadFolderSubs(string file) {
			string pattern = MkvFileInfo.Name.Substring(0, MkvFileInfo.Name.Length - 4) + "*.srt";
			foreach (string srt in Directory.GetFiles(MkvFileInfo.DirectoryName, pattern)) {
				string lang;
				if (srt.Length > MkvFileInfo.FullName.Length) {
					string[] parts = srt.Split('.');
					lang = parts[parts.Length - 2].ToLower();
				}
				else lang = "default";
				FolderSubs[lang] = srt;
			}

			Status = CanBeSkipped ? Status.Finished : Status.Waiting;
			if (!CanBeSkipped) {
				Logger.Success("Found new subs for {0}: {1}", MkvFileInfo.Name, SubLangsToMux());
			}
		}

		public bool CanBeSkipped {
			get { return FolderSubs.All(fs => MkvInfo.SubsAvailable.Contains(fs.Key)); }
		}

		private string SubLangsToMux() {
			List<string> toBeMerged = FolderSubs.Keys.ToList();
			toBeMerged.RemoveAll(l => MkvInfo.SubsAvailable.Contains(l));
			return string.Join(",", toBeMerged.ToArray());
		}

		public override string ToString() {
			if (CanBeSkipped)
				return "Nothing to do";
			else
				return "Mux in " + SubLangsToMux();
		}

		public void Perform(string defaultLanguage, RichTextBox logger, ProgressBar progBar) {
			string outputDir = MkvFileInfo.DirectoryName + "\\_subsmuxed_";
			if (!Directory.Exists(outputDir)) {
				Logger.Info("Creating output directory {0}", outputDir);
				Directory.CreateDirectory(outputDir);
			}

			StringBuilder arguments = new StringBuilder();
			// output
			arguments.Append("-o \"");
			arguments.Append(outputDir);
			arguments.Append("\\");
			arguments.Append(MkvFileInfo.Name);
			arguments.Append("\" ");

			// mkv in
			arguments.Append("\"");
			arguments.Append(MkvFileInfo.FullName);
			arguments.Append("\" ");

			// all subs
			foreach (var sub in FolderSubs) {
				if (MkvInfo.SubsAvailable.Contains(sub.Key)) continue;
				arguments.Append("--language 0:");
				arguments.Append(sub.Key == "default" ? defaultLanguage : sub.Key);
				arguments.Append(" \"");
				arguments.Append(sub.Value);
				arguments.Append("\" ");
			}
			this.logger = logger;
			this.progBar = progBar;
			this.pbInitVal = progBar.Value = (progBar.Value / 100) * 100;
			SpawnProcess(arguments.ToString());
		}

		RichTextBox logger;
		ProgressBar progBar;
		int pbInitVal = 0;
		Thread stdoutReaderThread, stderrReaderThread;
		string error;

		private void SpawnProcess(string arguments) {
			Process p = new Process();
			ProcessStartInfo psi = new ProcessStartInfo(Utils.MkvMergeExecutable, arguments);
			Logger.Info("Muxing {0}", MkvFileInfo.Name);

			psi.WindowStyle = ProcessWindowStyle.Hidden;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			p.Exited += new EventHandler(currentProcess_Exited);
			p.StartInfo = psi;
			currentProcess = p;
			p.Start();

			stdoutReaderThread = new Thread(StdOutReader);
			stdoutReaderThread.Start();
			stderrReaderThread = new Thread(StdErrReader);
			stderrReaderThread.Start();

			p.WaitForExit();

			stdoutReaderThread.Join();
			stderrReaderThread.Join();
			Status = string.IsNullOrEmpty(error) ? Status.Finished : Status.Failed;
			Finished(this, new EventArgs());
		}

		void StdOutReader() {
			try {
				StreamReader sr = currentProcess.StandardOutput;
				while (!sr.EndOfStream) {
					int bufflen;
					char[] buffer = new char[4096];
					bufflen = sr.Read(buffer, 0, buffer.Length);
					if (bufflen == 0) break;
					log(new string(buffer, 0, bufflen));
				}
			}
			catch (ThreadAbortException) { }
		}

		void StdErrReader() {
			try {
				StreamReader sr = currentProcess.StandardError;
				while (!sr.EndOfStream) {
					int bufflen;
					char[] buffer = new char[4096];
					bufflen = sr.Read(buffer, 0, buffer.Length);
					if (bufflen == 0) break;
					AppendToLogger(new string(buffer, 0, bufflen), Color.Red);
				}
			}
			catch (ThreadAbortException) { }
		}

		void currentProcess_Exited(object sender, EventArgs e) {
			currentProcess = null;
			stdoutReaderThread.Join();
			stderrReaderThread.Join();
		}

		Process currentProcess;
		internal void Abort() {
			if (currentProcess != null)
				currentProcess.Kill();
			if (stdoutReaderThread != null)
				stdoutReaderThread.Join();
			if (stdoutReaderThread != null)
				stderrReaderThread.Join();
		}

		#region logging/stdout parsing
		delegate void logstrdel(string s);
		int linestart = 0;
		bool flushIfNoNewline = false;
		void log(string txt) {
			if (logger.InvokeRequired) {
				logger.BeginInvoke(new logstrdel(log), txt);
				return;
			}

			if (txt.StartsWith("Error:") && txt.Length > 7)
				error = txt.Substring(7);
			else if (txt.StartsWith("Progress: ") && txt.Contains("%\r")) {
				string percentage = txt.Substring(10, txt.IndexOf("%\r", 10) - 10);
				Row.Cells[5].Value = percentage;
				int percentage_ = int.Parse(percentage);
				progBar.Value = pbInitVal + percentage_;
			}

			StringBuilder append = new StringBuilder();
			foreach (char c in txt) {
				if (c == '\r') {
					flushIfNoNewline = true;
					continue;
				}
				else if (flushIfNoNewline && c != '\n') {
					logger.SelectionStart = linestart;
					logger.SelectionLength = logger.TextLength - logger.SelectionStart;
					logger.SelectedText = "";
					append.Append(c);
				}
				else if (c == '\n') {
					append.Append("\r\n");
					linestart = logger.TextLength+1;
				}
				else
					append.Append(c);
				flushIfNoNewline = false;
			}

			AppendToLogger(append.ToString(), Color.Black);
		}

		delegate void AppendToLoggerDelegate(string append, Color c);
		private void AppendToLogger(string append, Color c) {
			if (logger.InvokeRequired) {
				logger.BeginInvoke(new AppendToLoggerDelegate(AppendToLogger), append, c);
				return;
			}
			logger.SelectionColor = c;
			logger.AppendText(append.ToString());
			logger.SelectionStart = logger.TextLength;
			logger.ScrollToBottom();
		}
		#endregion

		internal string JobDescription() {
			if (CanBeSkipped)
				return "Nothing will be done because there are no new subs waiting to be muxed for " + MkvFileInfo.Name;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("The following subtitles will be muxed in: ");


			Dictionary<string, string> subsToMux = new Dictionary<string, string>(this.FolderSubs);
			foreach (string sub in MkvInfo.SubsAvailable) {
				if (subsToMux.ContainsKey(sub))
					subsToMux.Remove(sub);
			}

			foreach (var v in subsToMux) {
				sb.Append("\t");
				sb.AppendLine(v.Value);
			}
			sb.Append("This resulting mkv will be stored in _subsmuxed_ in the same dir as the original.");
			return sb.ToString();
		}

	}
}
