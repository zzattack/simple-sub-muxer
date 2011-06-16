using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SubsMuxer {
	static class Logger {
		public delegate void LogDelegate(string line, Color c);
		public static event LogDelegate LogLine;

		public static Color SuccessColor = Color.Green;
		public static Color InfoColor = Color.Black;
		public static Color WarnColor = Color.Orange;
		public static Color ErrorColor = Color.Red;

		public static void Success(string line, params object[] parameters) {
			if (LogLine != null) LogLine(string.Format(line, parameters), SuccessColor);
		}
		public static void Info(string line, params object[] parameters) {
			if (LogLine != null) LogLine(string.Format(line, parameters), InfoColor);
		}
		public static void Warn(string line, params object[] parameters) {
			if (LogLine != null) LogLine(string.Format(line, parameters), WarnColor);
		}
		public static void Error(string line, params object[] parameters) {
			if (LogLine != null) LogLine(string.Format(line, parameters), ErrorColor);
		}

	}
}
