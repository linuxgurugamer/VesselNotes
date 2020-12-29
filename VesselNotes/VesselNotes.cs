using UnityEngine;
using System.Collections.Generic;
using ClickThroughFix;
using KSP_Log;

namespace VesselNotes
{
    public class VesselNotes : PartModule
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
            internal NOTE(string title, string note)
            {
                this.title = title;
                this.note = note;
                id = noteCnt++;
            }
        }
        // Define the controls to block.
        private const ControlTypes _blockAllControls =
            ControlTypes.ALL_SHIP_CONTROLS | ControlTypes.ACTIONS_ALL | ControlTypes.EVA_INPUT | ControlTypes.TIMEWARP |
            ControlTypes.MISC | ControlTypes.GROUPS_ALL | ControlTypes.CUSTOM_ACTION_GROUPS;

        // The font size.
        [KSPField(isPersistant = true)]
        private int _fontSize = 13;

        // The scroll view vector.
        private Vector2 noteVector = Vector2.zero;
        private Vector2 titleVector;
        int selectedNote = 0;
        int selectedId = 0;

        List<NOTE> notes = new List<NOTE>();

        // true lock input, false to unlock.
        private bool _toggleInput;

        // The vessel info.
        public string _vesselInfo;

        // true to show the plugin window, false to hide.
        private bool _visible;

        const float HEIGHT = 500f;
        const float WIDTH = 500f;
        // The rectangle for main window.
        [KSPField(isPersistant = true)]
        private Rect _windowRect = new Rect(50f, 25f, WIDTH, HEIGHT);

        //true use ksp skin, false use unity stock.
        private bool _useKspSkin;


        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Vessel Notes")]
        public void ToggleVesselNotes()
        {
            Toggle();
        }

        const float NOTESELWIDTH = 150f;
        const float NOTESCROLLVIEWWIDTH = WIDTH - NOTESELWIDTH;
        const float SCROLLVIEWHEIGHT = HEIGHT - 30;
        const float NOTEWIDTH = NOTESCROLLVIEWWIDTH - 10;
        const float NOTEHEIGHT = SCROLLVIEWHEIGHT - 10;

        static Log Log;

        void Start()
        {
            if (notes.Count == 0)
                notes.Add(new NOTE());
            if (Log == null)
                Log = new Log("VesselNotes", Log.LEVEL.INFO);
        }

        // Notes main window.
        //
        // <param name="windowId">Identifier for the window.</param>
        GUIStyle myStyle = null;
        GUIStyle buttonFontSel, buttonFont;
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
                Log.Info("i: " + i + " id: " + notes[i].id);
                if (GUILayout.Button(notes[i].title, (selectedId == notes[i].id) ? buttonFontSel : buttonFont, GUILayout.Width(NOTESELWIDTH - 10)))
                {
                    selectedNote = i;
                    selectedId = notes[i].id;
                }
            }
            // following in case note is deleted
            if (selectedNote >= notes.Count)
            {
                selectedNote = 0;
                selectedId = notes[0].id;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(NOTESELWIDTH));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Title: ");
            notes[selectedNote].title = GUILayout.TextField(notes[selectedNote].title);
            GUILayout.EndHorizontal();
            Log.Info("selectedNote: " + selectedNote + ", title: " + notes[selectedNote].title);

            // Text area with scroll bar
            noteVector = GUILayout.BeginScrollView(noteVector, GUILayout.Width(NOTESCROLLVIEWWIDTH), GUILayout.Height(SCROLLVIEWHEIGHT - 25));

            // Configurable font size, independent from the skin.
            myStyle.fontSize = _fontSize;
            myStyle.richText = true;

            notes[selectedNote].note = GUILayout.TextArea(notes[selectedNote].note, myStyle, GUILayout.MinWidth(NOTEWIDTH), GUILayout.MinHeight(NOTEHEIGHT - 25));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //New file
            if (GUILayout.Button("New Note"))
            {
                notes.Add(new NOTE(notes.Count.ToString(), ""));
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
            }
            GUI.Label(new Rect(95f, 0f, 60f, 20f), "Font size");
            if (GUI.Button(new Rect(150f, 2f, 15f, 15f), "+"))
            {
                // Big big big!!!
                _fontSize++;
            }
            // If we are on flight show the vessel logs buttons
            // Workaround for http://bugs.kerbalspaceprogram.com/issues/1230
            if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                if (GUI.Toggle(new Rect(255f, 452f, 150f, 20f), _toggleInput, "Toggle input lock") != _toggleInput)
                {
                    toggleLock();
                }
            }
            // Make this window dragable
            GUI.DragWindow();
        }

        // Executes the graphical user interface action.
        private void OnGUI()
        {
            if (myStyle == null)
            {
                myStyle = new GUIStyle(GUI.skin.textArea);
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

        //internal const string MODID = "VesselNotes_NS";
        //internal const string MODNAME = "VesselNotes";
        // Toggles plugin visibility.
        private void Toggle()
        {
            _visible = !_visible;
        }

        const string NODENAME = "VESSELNOTES";
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode vesselNode = node.GetNode(NODENAME);
            if (vesselNode != null)
            {
                ConfigNode[] nodes = vesselNode.GetNodes("NOTES");
                if (nodes != null)
                {
                    foreach (var n in nodes)
                    {
                        string title = "";
                        if (n.TryGetValue("TITLE", ref title))
                        {
                            string note = "";
                            if (n.TryGetValue("NOTE", ref note))
                            {
                                notes.Add(new NOTE(title, note));
                            }
                        }
                    }
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            Debug.Log("VesselNotes.OnSave, vessel: " + this.vessel.name + ", notes.Count: " + notes.Count);
            base.OnSave(node);
            ConfigNode vesselNode = new ConfigNode(NODENAME);
            foreach (var n in notes)
            {
                ConfigNode note = new ConfigNode("NOTES");
                note.AddValue("NOTE", n.note);
                note.AddValue("TITLE", n.title);
                vesselNode.AddNode(note);
                Debug.Log("note: " + note);
            }
            node.AddNode(vesselNode);
        }

        private void toggleLock()
        {
            _toggleInput = !_toggleInput;
            if (_toggleInput)
            {
                InputLockManager.SetControlLock(_blockAllControls, "notes");
            }
            else
            {
                InputLockManager.RemoveControlLock("notes");
            }
        }
    }
}