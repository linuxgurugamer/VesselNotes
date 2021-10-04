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

        void CopyToClipboard(bool log, List<NOTE> notes, int i = -1 )
        {
            sbPrint.Clear();
            if (i >= 0 && i < notes.Count)
            {
                notes[i].note.CopyToClipboard();
            }
            else
            {
                foreach (var n in notes)
                {
                    sbPrint.AppendLine(n.note);
                    if (!log)
                        sbPrint.AppendLine("\n-----------------------------------------------");
                }
                sbPrint.ToString().CopyToClipboard();
            }
            ScreenMessages.PostScreenMessage((log?"Log":
                (i == -1?"Notes":"Note") 
                ) + " copied to clipboard", 5, ScreenMessageStyle.UPPER_CENTER);
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
