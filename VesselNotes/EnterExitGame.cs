using System.IO;
using System.Collections.Generic;
using KSP.UI.Screens;
using UnityEngine;
using ClickThroughFix;
using SpaceTuxUtility;

using ToolbarControl_NS;

using static VesselNotesNS.RegisterToolbar;

namespace VesselNotesNS
{

    [KSPAddon(KSPAddon.Startup.EveryScene, true)]
    public class EnterExitGame : MonoBehaviour
    {
        public static EnterExitGame Instance;

        GameScenes lastScene = GameScenes.LOADING;
        double lastGameTime = 0;
        public bool guiActive = false;

        public bool manualToggle = false;
        int enterExitWinId;

        const float HEIGHT = 400;
        const float WIDTH = 500f;

        private Rect _windowRect = new Rect(50f, 25f, WIDTH, HEIGHT);

        string prePostGameNotes = "";

        void SetUpWinRect()
        {
            _windowRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        }


        public void Start()
        {
            DontDestroyOnLoad(this);

            GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
            GameEvents.onGameStateLoad.Add(onGameStateLoad);
            lastScene = HighLogic.LoadedScene;

            enterExitWinId = WindowHelper.NextWindowId("EnterExitGame");
            GlobalConfig.LoadCfg();
            Start2();
        }

        static bool gameStarted = false;
        void onGameStateLoad(ConfigNode node)
        {
            //Log.Info("onGameStateLoad, lastScene: " + lastScene);
            if (HighLogic.CurrentGame != null)
            {
                if (lastScene == GameScenes.MAINMENU)
                {
                    //currentGame = HighLogic.CurrentGame.Title;
                    GameNote.SaveFolder = HighLogic.SaveFolder;
                    guiActive = true;
                    manualToggle = false;
                    gameStarted = true;
                    SetUpWinRect();
                    GameNote.LoadData();
                }
                lastScene = HighLogic.LoadedScene;
            }
        }

        internal void onGameSceneLoadRequested(GameScenes loadedScene)
        {
            //Log.Info("onGameSceneLoadRequested, loadedScene: " + loadedScene + ", lastScene: " + lastScene);
            if (lastScene >= GameScenes.SPACECENTER && lastScene <= GameScenes.TRACKSTATION && loadedScene == GameScenes.MAINMENU && !notesSaved)
            {
                guiActive = true;
                manualToggle = false;
                SetUpWinRect();
            }
            if (HighLogic.CurrentGame != null)
                lastGameTime = HighLogic.CurrentGame.UniversalTime;

            lastScene = HighLogic.LoadedScene;
        }

        GUIStyle kspWindow = null;
        GUIStyle unityStockWindow = null;
        GUIStyle window;
        Texture2D tex;

        public static GUIStyle myStyle = null;
        internal static void InitStyle()
        {
            if (myStyle == null)
            {
                myStyle = new GUIStyle(GUI.skin.textArea);
                myStyle.fontSize = GlobalConfig.FontSize;
                myStyle.richText = true;
            }
        }

        bool notesSaved = false;
        internal void OnGUI()
        {
            if (guiActive && GlobalConfig.active) //  && !notesSaved)
            {
                if (kspWindow == null)
                {
                    kspWindow = new GUIStyle(HighLogic.Skin.window);
                    unityStockWindow = new GUIStyle(GUI.skin.window);

                    tex = kspWindow.normal.background;
                    var pixels = tex.GetPixels32();

                    for (int i = 0; i < pixels.Length; ++i)
                        pixels[i].a = 255;

                    tex.SetPixels32(pixels); tex.Apply();

                    kspWindow.active.background =
                        kspWindow.focused.background =
                        kspWindow.normal.background = tex;

                    InitStyle();

                }
                if (GlobalConfig.KspSkin)
                {
                    GUI.skin = HighLogic.Skin;
                    window = kspWindow;
                }
                else
                {
                    window = unityStockWindow;
                }
                if (GameNote.notesList.Count > 0 || HighLogic.LoadedScene == GameScenes.MAINMENU || manualToggle)
                {
                    _windowRect = ClickThruBlocker.GUILayoutWindow(enterExitWinId, _windowRect, StartStopWin,
                        HighLogic.LoadedScene == GameScenes.MAINMENU ? "Post Game Notes" : "Game Notes", window);
                }
            }
            else
            {
                if (HighLogic.LoadedScene == GameScenes.MAINMENU)
                    notesSaved = false;
            }
        }


        public static bool ShowControls(bool guiActive, Rect _windowRect)
        {
            //if (GUI.Button(new Rect(_windowRect.width - 24, 3f, 23, 15f), new GUIContent("X")))

            // Close the notes window.
            //if (GUI.Button(new Rect(2f, 2f, 22f, 16f), "X"))
                if (GUI.Button(new Rect(_windowRect.width - 24, 3f, 23, 15f), new GUIContent("X")))
                    guiActive = false;

            // Toggle current skin.
            //if (GUI.Button(new Rect(30f, 2f, 22f, 16f), "S"))
              if (GUI.Button(new Rect(_windowRect.width - 24-28, 3f, 23, 15f), new GUIContent("S")))
                    GlobalConfig.KspSkin = !GlobalConfig.KspSkin;

            // buttons for change the font size.
            //if (GUI.Button(new Rect(_windowRect.width + 10f - 115f, 2f, 15f, 15f), "-"))
            if (GUI.Button(new Rect(2, 2f, 15f, 15f), "-"))
                {
                    // Who wants a 0 size font?
                    if (GlobalConfig.FontSize > GlobalConfig.MIN_FONT_SIZE)
                    GlobalConfig.FontSize--;

                myStyle.fontSize = GlobalConfig.FontSize;
                myStyle.richText = true;

            }
            //GUI.Label(new Rect(_windowRect.width + 10f - 95f, 0f, 60f, 20f), "Font size");
            GUI.Label(new Rect(22f, 0f, 60f, 20f), "Font size");

            //if (GUI.Button(new Rect(_windowRect.width + 10f - 95f + 60f, 2f, 15f, 15f), "+"))
            if (GUI.Button(new Rect(22f+ 60f, 2f, 15f, 15f), "+"))
                {
                    // Big big big!!!
                    if (GlobalConfig.FontSize < GlobalConfig.MAX_FONT_SIZE)
                    GlobalConfig.FontSize++;

                myStyle.fontSize = GlobalConfig.FontSize;
                myStyle.richText = true;

            }
            return guiActive;
        }

        Vector2 contractPos;
        bool enterNotes = false;
        internal void StartStopWin(int id)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GlobalConfig.KspSkin ? "KSP Skin" : "Alt Skin", GUILayout.Width(90)))
            {
                GlobalConfig.KspSkin = !GlobalConfig.KspSkin;
                GlobalConfig.SaveCfg();
            }
            if (HighLogic.LoadedScene >= GameScenes.SPACECENTER && lastScene <= GameScenes.TRACKSTATION)
            {
                if (GUILayout.Button(GlobalConfig.showAll ? "Show All" : "Show Last", GUILayout.Width(90)))
                { GlobalConfig.showAll = !GlobalConfig.showAll; GlobalConfig.SaveCfg(); }
                if (GlobalConfig.showAll)
                {
                    if (GUILayout.Button(GlobalConfig.showAscending ? "Ascending" : "Descending", GUILayout.Width(90)))
                    { GlobalConfig.showAscending = !GlobalConfig.showAscending; GlobalConfig.SaveCfg(); }
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Show Vessel Logs", GUILayout.Width(150)))
                {
                    ShowVesselLogs.InstantiateShowVesselLogsWindow();
                    //VesselNotesLogs.CopyToClipboard(GameNote.notesList, GlobalConfig.showAscending, GlobalConfig.showAll);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if ((HighLogic.LoadedScene >= GameScenes.SPACECENTER && lastScene <= GameScenes.TRACKSTATION) && !enterNotes)
            {
                contractPos = GUILayout.BeginScrollView(contractPos, GUILayout.Height(HEIGHT - 60));
                if (GameNote.notesList.Count > 0)
                {
                    if (GlobalConfig.showAll)
                    {
                        for (int i = 0; i < GameNote.notesList.Count; i++)
                        {
                            GameNote l = GameNote.notesList[GlobalConfig.showAscending ? i : GameNote.notesList.Count - i - 1];

                            var GameTimeText = KSPUtil.PrintDate(l.gameTime, includeTime: true);
                            GUILayout.BeginHorizontal();

                            l.visible = GUILayout.Toggle(l.visible, GameTimeText);
                            //GUILayout.Box(GameTimeText);
                            GUILayout.EndHorizontal();
                            //GUILayout.Label(GameTimeText);
                            if (l.visible)
                                GUILayout.TextArea(l.prePostGameNotes, myStyle);
                            GUILayout.Space(10);
                        }
                    }
                    else
                    {
                        GUILayout.TextArea(GameNote.notesList[GameNote.notesList.Count - 1].prePostGameNotes, myStyle);
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", GUILayout.Width(60)))
                {
                    guiActive = false;
                    manualToggle = false;
                    gameStarted = false;
                }
                if (!gameStarted)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Enter Notes", GUILayout.Width(120)))
                        enterNotes = true;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy to clipboard", GUILayout.Width(150)))
                {
                    VesselNotesLogs.CopyToClipboard(GameNote.notesList, GlobalConfig.showAscending, GlobalConfig.showAll);
                }
                GUILayout.FlexibleSpace();

            }
            else
            {
                prePostGameNotes = GUILayout.TextArea(prePostGameNotes, myStyle, GUILayout.Height(HEIGHT - 60));
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(60)))
                {
                    guiActive = false;
                    enterNotes = false;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Clear", GUILayout.Width(60)))
                {
                    prePostGameNotes = "";
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save & Close"))
                {
                    SaveNotes();
                    notesSaved = false;
                }
                if (HighLogic.LoadedScene != GameScenes.MAINMENU)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Save & Exit game", GUILayout.Width(150)))
                    {
                        SaveNotes();
                        notesSaved = true;
                        GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                        HighLogic.LoadScene(GameScenes.MAINMENU);
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            guiActive = EnterExitGame.ShowControls(guiActive, _windowRect);

            GUI.DragWindow();
        }

        void SaveNotes()
        {
            GameNote.notesList.Add(new GameNote(lastGameTime, prePostGameNotes));
            GameNote.SaveData();
            prePostGameNotes = "";
            guiActive = false;
            enterNotes = false;
        }
        static internal ToolbarControl toolbarControl = null;
        internal const string MODID = "VesselNotes";
        internal const string MODNAME = "Vessel Notes & Logs";

        static Rect winRect = new Rect(0, 0, 200, 300);
        //bool isVisible = false;
        static bool firstTime = true;

        void Start2()
        {
            if (firstTime)
            {
                winRect.x = (Screen.width - winRect.width) / 2;
                winRect.y = (Screen.height - winRect.height) / 2;
                firstTime = false;
                AddToolbarButton();
            }
        }

        void AddToolbarButton()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(WinToggle, WinToggle,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    MODID,
                    "VesselNotesBtn",
                    "VesselNotes/PluginData/VesselNotes-38",
                    "VesselNotes/PluginData/VesselNotes-24",
                    MODNAME
                );

            }
        }

        void WinToggle()
        {
            guiActive = !guiActive;
            manualToggle = !manualToggle;
            manualToggle = guiActive;
            Log.Info("WinToggle, guiActive: " + guiActive + ", manualToggle: " + manualToggle);
        }
    }
}
