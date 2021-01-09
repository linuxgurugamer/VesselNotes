using System;
using UnityEngine;
using System.Collections.Generic;
using ClickThroughFix;
using KSP_Log;
using SpaceTuxUtility;
using System.Text;

namespace VesselNotesNS
{
    internal partial class VesselNotesLogs
    {
        StringBuilder sbPrint = new StringBuilder();

        void CopyToClipboard(bool log, List<NOTE> notes)
        {
            sbPrint.Clear();
            foreach (var n in notes)
            {
                sbPrint.AppendLine(n.note);
                if (!log)
                    sbPrint.AppendLine("\n-----------------------------------------------");
            }
            sbPrint.ToString().CopyToClipboard();
            ScreenMessages.PostScreenMessage((log?"Log ":"Notes " ) + " copied to clipboard", 5, ScreenMessageStyle.UPPER_CENTER);
        }

    }
    internal static class StringStuff
    {
        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }
    }

}
