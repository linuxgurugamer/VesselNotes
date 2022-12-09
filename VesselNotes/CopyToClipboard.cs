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
        static StringBuilder sbPrint = new StringBuilder();

        static void CopyToClipboard(bool log, List<NOTE> notes, int i = -1)
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
            ScreenMessages.PostScreenMessage((log ? "Log" :
                (i == -1 ? "Notes" : "Note")
                ) + " copied to clipboard", 5, ScreenMessageStyle.UPPER_CENTER);
        }

        static internal void CopyToClipboard(List<GameNote> notesList, bool showAscending, bool showAll)
        {
            sbPrint.Clear();
            if (!showAll)
            {
                var GameTimeText = KSPUtil.PrintDate(notesList[notesList.Count - 1].gameTime, includeTime: true);
                sbPrint.AppendLine(GameTimeText);
                sbPrint.AppendLine(notesList[notesList.Count - 1].prePostGameNotes);
                sbPrint.ToString().CopyToClipboard();
            }
            else
            {
                for (int i = 0; i < notesList.Count; i++)
                {
                    GameNote n = notesList[showAscending ? i : notesList.Count - i - 1];
                    if (n.visible)
                    {
                        var GameTimeText = KSPUtil.PrintDate(n.gameTime, includeTime: true);
                        sbPrint.AppendLine(GameTimeText);
                        sbPrint.AppendLine(n.prePostGameNotes);
                        sbPrint.AppendLine("\n-----------------------------------------------");
                    }
                }
                sbPrint.ToString().CopyToClipboard();
            }
            ScreenMessages.PostScreenMessage("Post-game notes copied to clipboard", 5, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
