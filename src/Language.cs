using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SubsMuxer {
	class LanguageEntry {
		public string FullName;
		public string TwoLetterAbbr;
		public string ThreeLetterAbbr;
		public bool IsDefault = false;

		internal static LanguageEntry Parse(string lang) {
			string[] parts = lang.Split('|');
			LanguageEntry ret = new LanguageEntry();
			ret.FullName = parts[0].Trim();
			ret.ThreeLetterAbbr = parts[1].Trim();
			ret.TwoLetterAbbr = parts[2].Trim();
			return ret;
		}

		public override string ToString() {
			return ThreeLetterAbbr ?? "";
		}

		internal LanguageEntry Clone() {
			LanguageEntry ent = new LanguageEntry();
			ent.FullName = this.FullName;
			ent.TwoLetterAbbr = this.TwoLetterAbbr;
			ent.ThreeLetterAbbr = this.ThreeLetterAbbr;
			return ent;
		}
	}
	class Language {
		static List<LanguageEntry> Languages = new List<LanguageEntry>();
		public static void LoadLanguages() {
			Logger.Info("Loading languages (mkvmerge --list-languages)");

			Process p = new Process();
			ProcessStartInfo psi = new ProcessStartInfo(Utils.MkvMergeExecutable, "--list-languages");
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardOutput = true;
			p.StartInfo = psi;
			p.Start();
			string[] langs = p.StandardOutput.ReadToEnd().Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
			p.WaitForExit();
			for (int i = 2; i < langs.Length; i++) {
				LanguageEntry entry = LanguageEntry.Parse(langs[i]);
				if (entry != null)
					Languages.Add(entry);
			}
		}

		public static LanguageEntry Find(string match) {
			if (match.Length == 2) {
				foreach (LanguageEntry ent in Languages) {
					if (ent.TwoLetterAbbr == match) return ent;
				}
			}
			else if (match.Length == 3) {
				foreach (LanguageEntry ent in Languages) {
					if (ent.ThreeLetterAbbr == match) return ent;
				}
			}
			else {
				foreach (LanguageEntry ent in Languages) {
					if (ent.FullName == match) return ent;
				}
			}
			return null;
		}

		static internal List<LanguageEntry> All() {
			return Languages;
		}
	}
}
