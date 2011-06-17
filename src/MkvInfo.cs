using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SubsMuxer {
	class MkvTrack {
		public int ID { get; set; }
		public string Type { get; set; }
		public string Format { get; set; }
		public Dictionary<string, string> Properties { get; set; }

		static Regex trackRegex = new Regex(@"Track ID (\d+): (.+?) \((.+)\) \[([^:]+:[^: ]+(?: )?)*\]");

		MkvTrack() {
			Properties = new Dictionary<string, string>();
		}

		public static MkvTrack Parse(string line) {
			// "Track ID 2: audio (A_AC3) [language:und default_track:1 forced_track:0]\r"
			Match m = trackRegex.Match(line);
			if (m.Success) {
				MkvTrack ret = new MkvTrack();
				ret.ID = int.Parse(m.Groups[1].Captures[0].Value);
				ret.Type = m.Groups[2].Captures[0].Value;
				ret.Format = m.Groups[3].Captures[0].Value;
				foreach (Capture c in m.Groups[4].Captures) {
					string[] parts = c.Value.Split(':');
					string prop = parts[0],
							val = parts[1].TrimEnd();
					ret.Properties[prop] = val;
				}
				return ret;
			}
			else return null;
		}
	}

	class MkvInfo {
		public List<MkvTrack> Tracks { get; set; }

		MkvInfo() {
			Tracks = new List<MkvTrack>();
		}

		public static MkvInfo Parse(string mkv) {
			Process p = new Process();
			ProcessStartInfo psi = new ProcessStartInfo(Utils.MkvMergeExecutable, "--identify-verbose \"" + mkv + "\"");
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardOutput = true;
			p.StartInfo = psi;			
			p.Start();
			p.WaitForExit();
			StreamReader sr = p.StandardOutput;
			string[] tracks = sr.ReadToEnd().Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
			var ret = new MkvInfo();
			foreach (string line in tracks) {
				var track = MkvTrack.Parse(line);
				if (track != null)
					ret.Tracks.Add(track);
			}
			return ret;
		}

		public List<LanguageEntry> SubsAvailable {
			get {
				List<LanguageEntry> ret = new List<LanguageEntry>();
				foreach (MkvTrack tr in Tracks) {
					if (tr.Type == "subtitles") {
						if (tr.Properties.ContainsKey("language"))
							ret.Add(Language.Find(tr.Properties["language"]));
					}
				}
				return ret;
			}
		}
		
	}
}
