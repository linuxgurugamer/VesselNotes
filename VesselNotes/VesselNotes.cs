using System;
using UnityEngine;
using System.Collections.Generic;
using ClickThroughFix;
using KSP_Log;
using SpaceTuxUtility;
using System.Text;

namespace VesselNotesNS
{
    internal partial class VesselNotesLogs : PartModule
    {

        // The font size.
        [KSPField(isPersistant = true)]
        private int _fontSize = 13;

        // The rectangle for main window.
        [KSPField(isPersistant = true)]
        private Rect _windowRect = new Rect(50f, 25f, WIDTH, HEIGHT);

        [KSPField(isPersistant = true)]
        private bool _useKspSkin;


        private Vector2 noteVector = Vector2.zero;
        private Vector2 titleVector;
        private Vector2 logVector = Vector2.zero;
        private Vector2 logTitleVector;
        int selectedNote = -1;
        //int selectedNoteId = 0;
        string currentNoteForComparision = "";
        string currentTitleForComparision = "";

        int selectedLog = 0;
        string currentLogNoteForComparision = "";
        string currentLogTitleForComparision = "";

        // List<NOTE> notes = new List<NOTE>();
        internal NOTE_LIST logList = new NOTE_LIST();
        //List<NOTE> vesselLog = new List<NOTE>();

        internal NOTE_LIST noteList = new NOTE_LIST();
        //NOTE_LIST logList = new NOTE_LIST();


        private bool _visible;

        const float HEIGHT = 400;
        const float WIDTH = 500f;
        const float LOGWIDTHADD = 100f;


        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Vessel Notes")]
        public void ToggleVesselNotes()
        {
            Toggle();
        }
        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Vessel Logs")]
        public void ToggleVesselLogs()
        {
            Toggle();
            logMode = true;
        }

        const float NOTESELWIDTH = 150f;
        const float NOTES_SEL_DATA = NOTESELWIDTH - 30;
        const float NOTESCROLLVIEWWIDTH = WIDTH - NOTESELWIDTH;
        const float SCROLLVIEWHEIGHT = HEIGHT - 50;
        const float SCROLLVIEWENTRY = SCROLLVIEWHEIGHT - 25;
        const float NOTEWIDTH = NOTESCROLLVIEWWIDTH - 10;
        const float NOTEHEIGHT = SCROLLVIEWHEIGHT - 10;

        const float LOGSCROLLVIEWWIDTH = NOTESCROLLVIEWWIDTH + LOGWIDTHADD;
        const float LOGVIEWENTRY = SCROLLVIEWHEIGHT;


        internal static Log Log = null;
        static GUIStyle bstyle;

        int noteWinId;
        bool lastAutolog;
        void Start()
        {
            if (vessel == null || HighLogic.LoadedSceneIsEditor || vessel.protoVessel.protoPartSnapshots[0].partName == "PotatoRoid" || vessel.isEVA)
                return;

            noteWinId = WindowHelper.NextWindowId("Notes-" + this.part.persistentId.ToString());
#if DEBUG
            if (Log == null)
                Log = new Log("VesselNotes", Log.LEVEL.INFO);
#else
      if (Log == null)
                Log = new Log("VesselNotes", Log.LEVEL.ERROR);
#endif
            foreach (var n in noteList.list)
                if (n.noteListGuid == Guid.Empty)
                    n.noteListGuid = noteList.listGuid;
            foreach (var n in logList.list)
                if (n.noteListGuid == Guid.Empty)
                    n.noteListGuid = noteList.listGuid;

            ResetEvents();
            lastAutolog = logList.autolog;
            if (HighLogic.LoadedScene != GameScenes.EDITOR)
            {
                if (logList.autolog)
                {
                    InitializeLogEvents();
                    LandingMonitorStart();
                }
                StartVesselMonitoring();
            }
        }

        void ResetEvents()
        {

            GameEvents.onVesselDocking.Add(onVesselDocking);
            GameEvents.onDockingComplete.Add(onDockingComplete);

            GameEvents.onSameVesselUndock.Add(onSameVesselUndock);
            GameEvents.onUndock.Add(onUndock);
            GameEvents.onPartUndockComplete.Add(onPartUndockComplete);
            GameEvents.onPartUndock.Add(onPartUndock);
            GameEvents.onVesselsUndocking.Add(onVesselsUndocking);

            GameEvents.onPartDeCoupleNewVesselComplete.Add(onPartDeCoupleNewVesselComplete);

            GameEvents.OnVesselRecoveryRequested.Add(onVesselRecoveryRequested);
        }

        void onVesselRecoveryRequested(Vessel v)
        {
            if (v == this.vessel && logList.list.Count > 0)
            {
                //logList.list.RemoveAt(logList.list.Count - 1);
                SaveLogsToFile(v, this.part);
                ScreenMessages.PostScreenMessage("Logs saved to file", 5, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        void onVesselDocking(uint a, uint b)
        {
            if (a == this.vessel.persistentId || b == this.vessel.persistentId)
            {
                noteList.LockAllNotes();
                logList.LockAllNotes();
            }
        }
        void onSameVesselUndock(GameEvents.FromToAction<ModuleDockingNode, ModuleDockingNode> ftn)
        {
            GetAllNotesModules(true, "onPartUndock");
            Log.Info("onPartUndock");

        }

        void onPartUndock(Part p)
        {
            GetAllNotesModules(true, "onPartUndock");
            Log.Info("onPartUndock");
        }

        void onPartUndockComplete(Part p)
        {
            GetAllNotesModules(true, "onPartUndockComplete");
            Log.Info("onPartUndockComplete");
        }
        void onDockingComplete(GameEvents.FromToAction<Part, Part> ed)
        {
            GetAllNotesModules(true, "onDockingComplete");
            if (ed.from.vessel != ed.to.vessel)
                return;
            Log.Info("onDockingComplete");
        }

        void onVesselsUndocking(Vessel v1, Vessel v2)
        {
            if (vessel != null)
            {
                GetAllNotesModules(true, "onVesselsUndocking");
            }
            else
                Log.Error("onVesselsUndocking, vessel  is null (possibly new vessel)");
            if (v1 != vessel && v2 != vessel)
                return;

        }

        void onPartDeCoupleNewVesselComplete(Vessel v1, Vessel v2)
        {
            if (v1 != vessel && v2 != vessel)
                return;

            if (vessel != null)
            {
                GetAllNotesModules(true, "onPartDeCoupleNewVesselComplete");
            }
            else
                Log.Error("onPartDeCoupleNewVesselComplete, vessel  is null (possibly new vessel)");
            Log.Info("onPartDeCoupleNewVesselComplete");
        }

        void onUndock(EventReport er)
        {
            GetAllNotesModules(true, "onUndock");
            if (er.origin != this.vessel)
                return;
            Log.Info("onUndock");
        }
        void SetSelectedNote(int i)
        {
            selectedNote = i;
            if (i >= 0 && i < noteList.list.Count)
            {
                currentNoteForComparision = noteList.list[i].note;
                currentTitleForComparision = noteList.list[i].title;
            }
        }

        void SetSelectedLog(int i)
        {
            if (logList.list.Count == 0)
                return;
            selectedLog = i;
            if (i >= 0 && i < logList.list.Count)
            {
                currentLogNoteForComparision = logList.list[i].note;
                currentLogTitleForComparision = logList.list[i].title;
            }
        }

        void onDockingComplete(Part p1, Part p2)
        {

        }

        GUIStyle myStyle = null;
        GUIStyle buttonFontSel, buttonFont;

        bool logMode = false;

        /// <summary>
        /// Notes window
        /// </summary>
        /// <param name="windowId"></param>
        private void NotesWindow(int windowId)
        {
            if (logMode)
            {
                if (!HighLogic.LoadedSceneIsEditor)
                    LogEditWindow();
            }
            else
                NoteEditWindow();

            ShowControls();

            GUI.DragWindow();

        }

        private void LogEditWindow()
        {
            // Set the control name for later usage.
            GUI.SetNextControlName("notes");
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("New Log Entry", bstyle))
            {
                string currentCrew = "";
                if (HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().logCrewAlways)
                    currentCrew = "Crew: " + getCurrentCrew(vessel);

                logList.list.Add(new NOTE("Log #" + (logList.list.Count + 1).ToString(), VesselLog.GetLogInfo(vessel) + currentCrew, logList.listGuid));
                SetSelectedLog(logList.list.Count + 1);
                selectedLog = logList.list.Count - 1;
            }
            GUI.backgroundColor = oldColor;

            GUILayout.EndHorizontal();

            logTitleVector = GUILayout.BeginScrollView(logTitleVector, GUILayout.Width(NOTESELWIDTH), GUILayout.Height(SCROLLVIEWHEIGHT));

            for (int i = 0; i < logList.list.Count; i++)
            {
                if (GUILayout.Button(logList.list[i].title,
                    (selectedLog == i) ? buttonFontSel : buttonFont, GUILayout.Width(NOTES_SEL_DATA)))
                {
                    SetSelectedLog(i);
                }
            }
            // following in case note is deleted
            if (selectedLog >= logList.list.Count)
            {
                SetSelectedLog(0);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            if (logList.list.Count > 0)
            {
                GUILayout.Label("Title: ");
                logList.list[selectedLog].title = GUILayout.TextField(logList.list[selectedLog].title, GUILayout.Width(NOTESELWIDTH));
                //GUILayout.FlexibleSpace();
            }
            //else
            //    GUILayout.Label(" ");
            GUILayout.FlexibleSpace();
            noteList.autolog = GUILayout.Toggle(noteList.autolog, "Autolog");
            GUILayout.Label(" ");
            if (HighLogic.LoadedSceneIsFlight && GUILayout.Button("Vessel Notes", GUILayout.Width(90)))
            {
                logMode = false;
                _windowRect.width = WIDTH; ;
                _windowRect.height = HEIGHT;

            }
            GUILayout.EndHorizontal();

            // Text area with scroll bar
            logVector = GUILayout.BeginScrollView(noteVector,
                GUILayout.Width(LOGSCROLLVIEWWIDTH), GUILayout.Height(LOGVIEWENTRY));
            if (logList.list.Count > 0)
                logList.list[selectedLog].note = GUILayout.TextArea(logList.list[selectedLog].note, myStyle,
                GUILayout.MinWidth(NOTEWIDTH), GUILayout.MinHeight(LOGVIEWENTRY));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (logList.list.Count > 0 &&
                (currentLogNoteForComparision != logList.list[selectedLog].note ||
                currentLogTitleForComparision != logList.list[selectedLog].title))
            {
                SyncAllNotes(false, true);
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy Logs to clipboard", GUILayout.Width(200)))
            {
                CopyToClipboard(true, logList.list);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(90)))
            {
                CloseWin();

            }
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

        }
        private void NoteEditWindow()
        {
            // Set the control name for later usage.
            GUI.SetNextControlName("notes");
            GUILayout.BeginHorizontal();
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("New Note", bstyle, GUILayout.Width(120)))
            {
                noteList.list.Add(new NOTE("Note #" + (noteList.list.Count + 1).ToString(), "", noteList.listGuid));
                SetSelectedNote(noteList.list.Count - 1);
                SyncAllNotes(true, false);
            }
            GUI.backgroundColor = oldColor;

            GUILayout.FlexibleSpace();
            if (HighLogic.LoadedSceneIsFlight && GUILayout.Button("Vessel Log", GUILayout.Width(90)))
            {
                logMode = true;
                _windowRect.width += LOGWIDTHADD;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Private");
            GUILayout.EndHorizontal();
            titleVector = GUILayout.BeginScrollView(titleVector, GUILayout.Width(NOTESELWIDTH), GUILayout.Height(SCROLLVIEWHEIGHT));

            for (int i = 0; i < noteList.list.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (noteList.list[i].locked)
                {
                    GUILayout.Label("Lck");
                }
                else
                {
                    if (!noteList.list[i].privateNote)
                        noteList.list[i].privateNote = GUILayout.Toggle(noteList.list[i].privateNote, "");
                    else
                        GUILayout.Toggle(noteList.list[i].privateNote, "");
                }
                if (GUILayout.Button(noteList.list[i].title,
                    (selectedNote == i) ? buttonFontSel : buttonFont, GUILayout.Width(NOTES_SEL_DATA)))
                {
                    SetSelectedNote(i);
                }
                GUILayout.EndHorizontal();
            }
            // following in case note is deleted
            if (selectedNote >= noteList.list.Count && noteList.list.Count > 0)
            {
                SetSelectedNote(noteList.list.Count - 1);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Title: ");
            if (noteList.list.Count > 0 && selectedNote >= 0 && selectedNote < noteList.list.Count)
                noteList.list[selectedNote].title = GUILayout.TextField(noteList.list[selectedNote].title, GUILayout.Width(NOTESELWIDTH));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Text area with scroll bar
            noteVector = GUILayout.BeginScrollView(noteVector,
                GUILayout.Width(NOTESCROLLVIEWWIDTH), GUILayout.Height(SCROLLVIEWENTRY));
            if (noteList.list.Count > 0 && selectedNote >= 0 && selectedNote < noteList.list.Count)
                noteList.list[selectedNote].note = GUILayout.TextArea(noteList.list[selectedNote].note, myStyle,
                GUILayout.MinWidth(NOTEWIDTH), GUILayout.MinHeight(SCROLLVIEWENTRY));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (noteList.list.Count > 0 && selectedNote >= 0 && selectedNote < noteList.list.Count &&
                (currentNoteForComparision != noteList.list[selectedNote].note ||
                currentTitleForComparision != noteList.list[selectedNote].title))
            {
                if (!HighLogic.LoadedSceneIsEditor)
                    Log.Info("Part: " + part.partInfo.name + ", currentNoteForComparision: " + currentNoteForComparision +
                        ", noteList.list[selectedNote].note: " + noteList.list[selectedNote].note +
                        ", currentTitleForComparision: " + currentTitleForComparision +
                        ", noteList.list[selectedNote].title: " + noteList.list[selectedNote].title);
                SyncAllNotes(true, false);
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (noteList.list.Count > 0 && selectedNote >= 0 && selectedNote < noteList.list.Count)
            {
                if (GUILayout.Button("Delete Note", GUILayout.Width(90)))
                {
                    noteList.list.Remove(noteList.list[selectedNote]);
                    if (selectedNote > noteList.list.Count)
                        selectedNote = noteList.list.Count - 1;
                    SetSelectedNote(selectedNote);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy Notes to clipboard", GUILayout.Width(200)))
            {
                CopyToClipboard(false, noteList.list);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(90)))
            {
                CloseWin();
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        void CloseWin()
        {
            highlight = false;
            part.highlighter.ConstantOff();
            _visible = false;

        }
        void ShowControls()
        {
            // Close the notes window.
            if (GUI.Button(new Rect(2f, 2f, 22f, 16f), "X"))
                Toggle();

            // Toggle current skin.
            if (GUI.Button(new Rect(30f, 2f, 22f, 16f), "S"))
                _useKspSkin = !_useKspSkin;

            // buttons for change the font size.
            if (GUI.Button(new Rect(_windowRect.width + 10f - 115f, 2f, 15f, 15f), "-"))
            {
                // Who wants a 0 size font?
                if (_fontSize <= 1) return;
                _fontSize--;

                myStyle.fontSize = _fontSize;
                myStyle.richText = true;

            }
            GUI.Label(new Rect(_windowRect.width + 10f - 95f, 0f, 60f, 20f), "Font size");

            if (GUI.Button(new Rect(_windowRect.width + 10f - 95f + 60f, 2f, 15f, 15f), "+"))
            {
                // Big big big!!!
                _fontSize++;

                myStyle.fontSize = _fontSize;
                myStyle.richText = true;

            }
        }

        bool lastKspSkin;
        bool highlight = false;
        private void OnGUI()
        {
            if (myStyle == null || lastKspSkin != _useKspSkin)
            {
                myStyle = new GUIStyle(GUI.skin.textArea);

                myStyle.fontSize = _fontSize;
                myStyle.richText = true;

                buttonFont = new GUIStyle(GUI.skin.label);
                buttonFontSel = new GUIStyle(GUI.skin.label);
                buttonFontSel.normal = new GUIStyleState { textColor = Color.green };

                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                bstyle = new GUIStyle(HighLogic.Skin.button);
                bstyle.fixedHeight = GUI.skin.button.fixedHeight;
                bstyle.padding = GUI.skin.button.padding;
                bstyle.border = GUI.skin.button.border;
                bstyle.fontSize = GUI.skin.button.fontSize;
                bstyle.fontStyle = GUI.skin.button.fontStyle;
                bstyle.normal.textColor = Color.yellow;
                GUI.backgroundColor = oldColor;
                lastKspSkin = _useKspSkin;
            }
            if (_visible)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().highlightPart && ClickThruBlocker.CTBWin.MouseIsOverWindow(_windowRect))
                {
                    if (!highlight)
                    {
                        highlight = true;
                        part.highlighter.ConstantOn(XKCDColors.Yellow);
                    }
                }
                else
                {
                    if (highlight)
                    {
                        highlight = false;
                        part.highlighter.ConstantOff();
                    }
                }

                if (lastAutolog != logList.autolog)
                {
                    if (!HighLogic.LoadedSceneIsEditor)
                    {
                        if (lastAutolog)
                        {
                            InitializeLogEvents(false);
                            LandingMonitorStop();
                        }
                        else
                        {
                            InitializeLogEvents();
                            LandingMonitorStart();
                        }
                    }
                }
                if (_useKspSkin)
                    GUI.skin = HighLogic.Skin;
                _windowRect = ClickThruBlocker.GUILayoutWindow(noteWinId, _windowRect, NotesWindow, "Vessel Notes & Logs : " + part.partInfo.title);
            }
        }

        private void Toggle()
        {
            _visible = !_visible;
            logMode = false;
        }
    }
}