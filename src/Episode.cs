using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace SubsMuxer {
	class Episode {
		public string seriesName;
		public string seasonNumber;
		public string episodeNumber;
		public FileInfo fileInfo;

		static List<Regex> regexes = new List<Regex>();
		static Episode() {
			regexes.Add(new Regex(@"\[[Ss]([0-9]+)\]_\[[Ee]([0-9]+)([^\\/]*)", RegexOptions.IgnoreCase));// foo_[s01]_[e01]
			regexes.Add(new Regex(@"[\._ \-]([0-9]+)x([0-9]+)([^\\/]*)", RegexOptions.IgnoreCase));// foo.1x09
			regexes.Add(new Regex(@"^(.+)[\._ \-][Ss]([0-9]+)[\. \-]?[Ee]([0-9]+)([^\\/]*)", RegexOptions.IgnoreCase));// foo, s01e01, foo.s01.e01, foo.s01-e01
			regexes.Add(new Regex(@"[\._ \-]([0-9]+)([0-9][0-9])([\._ \-][^\\/]*)", RegexOptions.IgnoreCase));// foo.103
		}

		public Episode(FileInfo fileInfo) {
			this.fileInfo = fileInfo;
			foreach (Regex r in regexes) {
				Match match = r.Match(fileInfo.Name);
				if (match.Success) {
					seriesName = match.Groups[1].Value.Replace('.', ' ');
					seasonNumber = match.Groups[2].Value;
					episodeNumber = match.Groups[3].Value;
					break;
				}
			}
		}

		public bool IsValid() {
			return !string.IsNullOrEmpty(seriesName) && !string.IsNullOrEmpty(seasonNumber) && !string.IsNullOrEmpty(episodeNumber);
		}

		public override bool Equals(object obj) {
			if (!(obj is Episode)) return false;
			Episode x = obj as Episode;
			return x.seriesName == seriesName && x.seasonNumber == seasonNumber && x.episodeNumber == episodeNumber;
		}

		public override string ToString() {
			if (IsValid())
				return string.Format("{0} {1:2}x{2:2}", seriesName, seasonNumber, episodeNumber);
			else
				return fileInfo.Name;
		}

	}
}