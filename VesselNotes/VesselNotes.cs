using UnityEngine;
using System.Collections.Generic;
using ClickThroughFix;
//using KSP_Log;

namespace VesselNotes
{
    internal  partial class VesselNotes : PartModule
    {
        internal class NOTE
        {
            static int noteCnt = 0;

            internal int id;
            internal string title;
            internal string note;

            internal NOTE()
            {
                title = note = "";
                id = noteCnt++;
            }
            internal NOTE(string title, string note, int id=-1)
            {
                this.title = title;
                this.note = note;
                if (id == -1)
                    this.id = noteCnt++;
                else
                    this.id = id;
            }
        }

      
        // The font size.
        [KSPField(isPersistant = true)]
        private int _fontSize = 13;

        // The rectangle for main window.
        [KSPField(isPersistant = true)]
        private Rect _windowRect = new Rect(50f, 25f, WIDTH, HEIGHT);

        [KSPField(isPersistant = true)]
        private bool _useKspSkin;

        [KSPField(isPersistant = true)]
        private bool soloNote;


        private Vector2 noteVector = Vector2.zero;
        private Vector2 titleVector;
        private Vector2 logVector = Vector2.zero;
        private Vector2 logTitleVector;
        int selectedNote = 0;
        int selectedNoteId = 0;
        string currentNoteForComparision = "";
        string currentTitleForComparision = "";

        int selectedLog = 0;
        int selectedLogId = 0;
        string currentLogNoteForComparision = "";
        string currentLogTitleForComparision = "";

        List<NOTE> notes = new List<NOTE>();
        List<NOTE> vesselLog = new List<NOTE>();

        private bool _visible;

        const float HEIGHT = 500f;
        const float WIDTH = 500f;
        const float LOGWIDTHADD = 200f;


        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Vessel Notes")]
        public void ToggleVesselNotes()
        {
            Toggle();
        }

        const float NOTESELWIDTH = 150f;
        const float NOTES_SEL_DATA = NOTESELWIDTH - 10;
        const float NOTESCROLLVIEWWIDTH = WIDTH - NOTESELWIDTH;
        const float SCROLLVIEWHEIGHT = HEIGHT - 50;
        const float SCROLLVIEWENTRY = SCROLLVIEWHEIGHT - 25;
        const float NOTEWIDTH = NOTESCROLLVIEWWIDTH - 10;
        const float NOTEHEIGHT = SCROLLVIEWHEIGHT - 10;

        const float LOGSCROLLVIEWWIDTH = NOTESCROLLVIEWWIDTH + LOGWIDTHADD;
        const float LOGVIEWENTRY = SCROLLVIEWENTRY + LOGWIDTHADD;

        //static Log Log;

        void Start()
        {
            if (notes.Count == 0)
                notes.Add(new NOTE("Note #1", ""));
            //if (Log == null)
            //    Log = new Log("VesselNotes", Log.LEVEL.INFO);
        }

        void SetSelectedNote(int i)
        {
            selectedNote = i;
            selectedNoteId = notes[i].id;
            currentNoteForComparision = notes[i].note;
            currentTitleForComparision = notes[i].title;
        }
        void SetSelectedLog(int i)
        {
            if (vesselLog.Count == 0)
                return;
            selectedLog = i;
            selectedLogId = vesselLog[i].id;
            currentLogNoteForComparision = vesselLog[i].note;
            currentLogTitleForComparision = vesselLog[i].title;
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
                LogEditWindow();
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
            logTitleVector = GUILayout.BeginScrollView(logTitleVector, GUILayout.Width(NOTESELWIDTH), GUILayout.Height(SCROLLVIEWHEIGHT));

            for (int i = 0; i < vesselLog.Count; i++)
            {
                if (GUILayout.Button(vesselLog[i].title,
                    (selectedLogId == vesselLog[i].id) ? buttonFontSel : buttonFont, GUILayout.Width(NOTES_SEL_DATA)))
                {
                    SetSelectedLog(i);
                }
            }
            // following in case note is deleted
            if (selectedLog >= vesselLog.Count)
            {
                SetSelectedLog(0);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            if (vesselLog.Count > 0)
            {
                GUILayout.Label("Title: ");
                vesselLog[selectedLog].title = GUILayout.TextField(vesselLog[selectedLog].title);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            // Text area with scroll bar
            logVector = GUILayout.BeginScrollView(noteVector,
                GUILayout.Width(LOGSCROLLVIEWWIDTH), GUILayout.Height(LOGVIEWENTRY));
            if (vesselLog.Count > 0)
                vesselLog[selectedLog].note = GUILayout.TextArea(vesselLog[selectedLog].note, myStyle,
                GUILayout.MinWidth(NOTEWIDTH), GUILayout.MinHeight(LOGVIEWENTRY));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (vesselLog.Count > 0 &&
                (currentLogNoteForComparision != vesselLog[selectedLog].note ||
                currentLogTitleForComparision != vesselLog[selectedLog].title))
            {
                SyncAllNotes();
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("New Log Entry"))
            {
                vesselLog.Add(new NOTE("Log #" + (vesselLog.Count+1).ToString(), VesselLog.GetLogInfo(vessel)));
                SetSelectedLog(vesselLog.Count + 1);
                selectedLog = vesselLog.Count - 1;
                selectedLogId = vesselLog[selectedLog].id;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close Log"))
            {
                logMode = false;
                _windowRect.width -= LOGWIDTHADD;

            }
            GUILayout.FlexibleSpace(); 
            GUILayout.EndHorizontal();

        }
        private void NoteEditWindow()
        { 
            // Set the control name for later usage.
            GUI.SetNextControlName("notes");
            GUILayout.BeginHorizontal();
            soloNote = GUILayout.Toggle(soloNote, "Solo note (not shared with other command modules)");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Vessel Log", GUILayout.Width(90)))
            {
                logMode = true;
                _windowRect.width += LOGWIDTHADD;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            titleVector = GUILayout.BeginScrollView(titleVector, GUILayout.Width(NOTESELWIDTH), GUILayout.Height(SCROLLVIEWHEIGHT));
            
            for (int i = 0; i < notes.Count; i++)
            {
                if (GUILayout.Button(notes[i].title, 
                    (selectedNoteId == notes[i].id) ? buttonFontSel : buttonFont, GUILayout.Width(NOTES_SEL_DATA)))
                {
                    SetSelectedNote(i);
                }
            }
            // following in case note is deleted
            if (selectedNote >= notes.Count)
            {
                SetSelectedNote(0);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Title: ");
            notes[selectedNote].title = GUILayout.TextField(notes[selectedNote].title);
            GUILayout.EndHorizontal();

            // Text area with scroll bar
            noteVector = GUILayout.BeginScrollView(noteVector, 
                GUILayout.Width(NOTESCROLLVIEWWIDTH), GUILayout.Height(SCROLLVIEWENTRY));

            notes[selectedNote].note = GUILayout.TextArea(notes[selectedNote].note, myStyle, 
                GUILayout.MinWidth(NOTEWIDTH), GUILayout.MinHeight(SCROLLVIEWENTRY));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (currentNoteForComparision != notes[selectedNote].note ||
                currentTitleForComparision != notes[selectedNote].title)
            {
                SyncAllNotes();
            }
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New Note"))
            {
                notes.Add(new NOTE("Note #" +notes.Count.ToString(), ""));
                SetSelectedNote(notes.Count + 1);
                selectedNote = notes.Count - 1;
                selectedNoteId = notes[selectedNote].id;
            }
            if (GUILayout.Button("Delete Note"))
            {
                if (notes.Count > 1)
                {
                    notes.Remove(notes[selectedNote]);
                    selectedNote = 0;
                }
                else
                {
                    notes[selectedNote].title = "title";
                    notes[selectedNote].note = "";
                }
            }
            if (GUILayout.Button("Close"))
                _visible = false;
            GUILayout.EndHorizontal();
        }

        void ShowControls()
        {
            // Close the notes window.
            if (GUI.Button(new Rect(2f, 2f, 13f, 13f), "X"))
                Toggle();

            // Toggle current skin.
            if (GUI.Button(new Rect(20f, 2f, 22f, 16f), "S"))
                _useKspSkin = !_useKspSkin;

            // buttons for change the font size.
            if (GUI.Button(new Rect(80f, 2f, 15f, 15f), "-"))
            {
                // Who wants a 0 size font?
                if (_fontSize <= 1) return;
                _fontSize--;

                myStyle.fontSize = _fontSize;
                myStyle.richText = true;

            }
            GUI.Label(new Rect(95f, 0f, 60f, 20f), "Font size");
            if (GUI.Button(new Rect(150f, 2f, 15f, 15f), "+"))
            {
                // Big big big!!!
                _fontSize++;

                myStyle.fontSize = _fontSize;
                myStyle.richText = true;

            }
        }

        private void OnGUI()
        {
            if (myStyle == null)
            {
                myStyle = new GUIStyle(GUI.skin.textArea);

                myStyle.fontSize = _fontSize;
                myStyle.richText = true;

                buttonFont = new GUIStyle(GUI.skin.label);
                buttonFontSel = new GUIStyle(GUI.skin.label);
                buttonFontSel.normal = new GUIStyleState { textColor = Color.green };
            }
            if (_visible)
            {
                if (_useKspSkin)
                    GUI.skin = HighLogic.Skin;
                _windowRect = ClickThruBlocker.GUILayoutWindow(1235678, _windowRect, NotesWindow, "Vessel Notes");
            }
        }

        private void Toggle()
        {
            _visible = !_visible;
        }
    }
}