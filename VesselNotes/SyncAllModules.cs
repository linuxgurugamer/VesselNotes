﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;
using static VesselNotesNS.RegisterToolbar;

namespace VesselNotesNS
{
    internal partial class VesselNotesLogs
    {
        List<VesselNotesLogs> allNotesModules;

        void GetAllNotesModules(bool force = false, string from = "")
        {
            if (force && allNotesModules != null)
            {
                allNotesModules.Clear();
            }
            else
                allNotesModules = new List<VesselNotesLogs>();

            // Initialize the allNotesModules one time every time the module is initialized
            //if (allNotesModules == null || force)
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    if (vessel != null)
                    {
#if true
                        allNotesModules = vessel.FindPartModulesImplementing<VesselNotesLogs>();
                        if (allNotesModules == null)
                            allNotesModules = new List<VesselNotesLogs>();
#else
                        allNotesModules = new List<VesselNotes>();
                        foreach (var p in vessel.Parts)
                        {
                            //if (p.Modules.Contains<VesselNotes>())
                            {
                                foreach (var m in p.Modules)
                                {
                                    if (m is VesselNotes)
                                    {
                                        allNotesModules.Add(m as VesselNotes);
                                        break;
                                    }
                                }
                            }
                        }
#endif
                    }
                }
                else
                {
                    allNotesModules = new List<VesselNotesLogs>();
                    foreach (var p in EditorLogic.SortedShipList)
                    {
                        if (p.Modules.Contains<VesselNotesLogs>())
                        {
                            foreach (var m in p.Modules)
                            {
                                if (m is VesselNotesLogs)
                                {
                                    allNotesModules.Add(m as VesselNotesLogs);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        bool doNotes = false, doLogs = false;
        internal void SyncAllNotes(bool notes, bool log)
        {
            //Log.Info("SyncAllNotes");
            GetAllNotesModules();
            doNotes = notes;
            doLogs = log;
            StartCoroutine("DoIt");
        }

        static private System.Object thisLock = new System.Object();
        IEnumerator DoIt()
        {
            lock (thisLock)
            {
                // Sync the notes
                for (int i = 0; i < allNotesModules.Count; i++)
                {
                    var syncToNote = allNotesModules[i];
                    if (syncToNote != this)
                    {
                        if (doNotes)
                            SyncNotes(syncToNote);
                        if (doLogs)
                            SyncLogs(syncToNote);
                    }
                }
                SetSelectedNote(selectedNote);
                SetSelectedLog(selectedLog);
            }
            return null;
        }


        void SyncNotes(VesselNotesLogs syncToNote)
        {
            for (int n = 0; n < noteList.list.Count; n++)
            {
                if (!noteList.list[n].privateNote && !noteList.list[n].locked)
                {
                    NOTE_LIST syncToNotesList = syncToNote.noteList;
                    var foundNote = syncToNotesList.list.SingleOrDefault(r => r.guid == noteList.list[n].guid);
                    if (foundNote == null)
                    {
                        if (noteList.list[n].noteListGuid == syncToNotesList.listGuid)
                        {
                            syncToNotesList.list.Add(new NOTE(
                                noteList.list[n].title,
                                noteList.list[n].note,
                                noteList.list[n].guid,
                                syncToNotesList.listGuid,
                                false));
                        }
                    }
                    else
                    {
                        if (!foundNote.privateNote && !foundNote.locked)
                        {
                            Log.Info("guid matches");
                            foundNote.title = noteList.list[n].title;
                            foundNote.note = noteList.list[n].note;
                        }
                    }
                }
            }
            syncToNote.SetSelectedNote(syncToNote.selectedNote);
        }


        void SyncLogs(VesselNotesLogs syncToNote)
        {
            for (int n = 0; n < logList.list.Count; n++)
            {
                if (!logList.list[n].privateNote && !logList.list[n].locked)
                {
                    NOTE_LIST syncToLogList = syncToNote.logList;
                    NOTE foundLog = syncToLogList.list.SingleOrDefault(r => r.guid == logList.list[n].guid);
                    if (foundLog == null)
                    {
                        if (logList.list[n].noteListGuid == syncToLogList.listGuid)
                            syncToLogList.list.Add(new NOTE(
                                logList.list[n].title,
                                logList.list[n].note,
                                logList.list[n].guid,
                                syncToLogList.listGuid,
                                false));
                    }
                    else
                    {
                        foundLog.title = logList.list[n].title;
                        foundLog.note = logList.list[n].note;
                        foundLog.id = logList.list[n].id;
                    }
                }
            }
            syncToNote.SetSelectedLog(syncToNote.selectedLog);

        }
    }
}
