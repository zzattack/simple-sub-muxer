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
		public Dictionary<string, LanguageEntry> FolderSubs { get; set; }
		public MkvInfo MkvInfo { get; set; }
		public FileInfo MkvFileInfo { get; set; }
		public Status Status { get; set; }
		public DataGridViewRow Row { get; set; }
		public Episode Episode { get; set; }

		public event EventHandler Finished;

		public MkvMergeAction() {
			FolderSubs = new Dictionary<string, LanguageEntry>();
		}

		public MkvMergeAction(Episode ep)
			: this() {
			MkvInfo = MkvInfo.Parse(ep.fileInfo.FullName);
			MkvFileInfo = ep.fileInfo;
			Episode = ep;
			LoadFolderSubs(MkvFileInfo.DirectoryName);
		}

		private void LoadFolderSubs(string file) {
			string pattern = "*.srt";
			foreach (string srt in Directory.GetFiles(MkvFileInfo.DirectoryName, pattern).OrderBy(x => x)) {
				if (srt.Substring(0, srt.Length - 4) == MkvFileInfo.Name.Substring(0, MkvFileInfo.Name.Length - 4)) {
					AddSubtitleFromFolder(srt);
				}
				else if (Episode.IsValid()) {
					Episode srtEpisode = new SubsMuxer.Episode(new FileInfo(srt));
					if (srtEpisode.IsValid() && srtEpisode.Equals(this.Episode)) {
						AddSubtitleFromFolder(srt);
					}
				}
			}

			Status = CanBeSkipped ? Status.Finished : Status.Waiting;
			if (!CanBeSkipped) {
				Logger.Success("Found new subs for {0}: {1}", MkvFileInfo.Name, SubLangsToMux());
			}
		}

		private void AddSubtitleFromFolder(string srt) {
			LanguageEntry lang;
			if (srt.Length > MkvFileInfo.FullName.Length) {
				string[] parts = srt.Split('.');
				lang = Language.Find(parts[parts.Length - 2].ToLower());
			}
			else {
				lang = Language.Find("Undetermined").Clone();
				lang.IsDefault = true;
			}
			FolderSubs[srt] = lang;
		}

		public bool CanBeSkipped {
			get { return FolderSubs.All(fs => MkvInfo.SubsAvailable.Contains(fs.Value)); }
		}

		private string SubLangsToMux() {
			List<LanguageEntry> toBeMerged = FolderSubs.Values.ToList();
			toBeMerged.RemoveAll(l => MkvInfo.SubsAvailable.Contains(l));
			return string.Join(",", toBeMerged.Select(x => x.ToString()).ToArray());
		}

		public override string ToString() {
			if (CanBeSkipped)
				return "Nothing to do";
			else
				return "Mux in " + SubLangsToMux();
		}

		public void Perform(RichTextBox logger, ProgressBar progBar) {
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
				if (MkvInfo.SubsAvailable.Contains(sub.Value)) continue;
				arguments.Append("--language 0:");
				arguments.Append(sub.Value.ThreeLetterAbbr);
				arguments.Append(" \"");
				arguments.Append(sub.Key);
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
					linestart = logger.TextLength + 1;
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

			var subsToMux = new Dictionary<string, LanguageEntry>(this.FolderSubs);
			var subsAvail = MkvInfo.SubsAvailable;

			foreach (var v in subsToMux.Where(x => (!subsAvail.Contains(x.Value)))) {
				sb.Append("\t");
				sb.AppendLine(v.Value.ThreeLetterAbbr);
			}
			sb.Append("This resulting mkv will be stored in _subsmuxed_ in the same dir as the original.");
			return sb.ToString();
		}


		internal void UpdateDefault(LanguageEntry defaultLanguage) {
			List<string> toBeUpdated = new List<string>();
			foreach (var v in FolderSubs)
				if (v.Value.IsDefault)
					toBeUpdated.Add(v.Key);
			foreach (var v in toBeUpdated)
				FolderSubs[v] = defaultLanguage;
			Row.Cells[3].Value = ToString();
		}
	}
}
