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
        int selectedNote = 0;
        int selectedId = 0;
        string currentNoteForComparision = "";
        string currentTitleForComparision = "";

        List<NOTE> notes = new List<NOTE>();

        private bool _visible;

        const float HEIGHT = 500f;
        const float WIDTH = 500f;



        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Vessel Notes")]
        public void ToggleVesselNotes()
        {
            Toggle();
        }

        const float NOTESELWIDTH = 150f;
        const float NOTES_SEL_DATA = NOTESELWIDTH - 10;
        const float NOTESCROLLVIEWWIDTH = WIDTH - NOTESELWIDTH;
        const float SCROLLVIEWHEIGHT = HEIGHT - 30;
        const float SCROLLVIEWENTRY = SCROLLVIEWHEIGHT - 25;
        const float NOTEWIDTH = NOTESCROLLVIEWWIDTH - 10;
        const float NOTEHEIGHT = SCROLLVIEWHEIGHT - 10;

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
            selectedId = notes[i].id;
            currentNoteForComparision = notes[i].note;
            currentTitleForComparision = notes[i].title;
        }

        GUIStyle myStyle = null;
        GUIStyle buttonFontSel, buttonFont;

        /// <summary>
        /// Notes window
        /// </summary>
        /// <param name="windowId"></param>
        private void NotesWindow(int windowId)
        {
            // Set the control name for later usage.
            GUI.SetNextControlName("notes");
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            titleVector = GUILayout.BeginScrollView(titleVector, GUILayout.Width(NOTESELWIDTH), GUILayout.Height(SCROLLVIEWHEIGHT));
            
            for (int i = 0; i < notes.Count; i++)
            {
                if (GUILayout.Button(notes[i].title, 
                    (selectedId == notes[i].id) ? buttonFontSel : buttonFont, GUILayout.Width(NOTES_SEL_DATA)))
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
                selectedId = notes[selectedNote].id;
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

            // Close the notes window.
            if (GUI.Button(new Rect(2f, 2f, 13f, 13f), "X"))
            {
                Toggle();
            }
            // Toggle current skin.
            if (GUI.Button(new Rect(20f, 2f, 22f, 16f), "S"))
            {
                _useKspSkin = !_useKspSkin;
            }
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

            GUI.DragWindow();
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