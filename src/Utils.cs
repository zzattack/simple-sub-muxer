using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SubsMuxer {
	static class Utils {

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		private const int WM_VSCROLL = 0x115;
		private const int SB_BOTTOM = 7;

		/// <summary>
		/// Scrolls the vertical scroll bar of a multi-line text box to the bottom.
		/// </summary>
		/// <param name="tb">The text box to scroll</param>
		public static void ScrollToBottom(this TextBoxBase tb) {
			SendMessage(tb.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
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
	}

}
