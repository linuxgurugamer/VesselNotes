using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.UI.Screens;
using KSP.UI.Screens.SpaceCenter;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using ClickThroughFix;
using SpaceTuxUtility;

using static VesselNotesNS.VesselNotesLogs;

namespace VesselNotesNS
{
    internal class GameNote
    {
        internal string prePostGameNotes;
        internal double gameTime;
        internal bool visible;
        internal GameNote(double savedGameTime, string notes)
        {
            visible = true;
            prePostGameNotes = notes;
            gameTime = savedGameTime;
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class EnterExitGame : MonoBehaviour
    {
        GameScenes lastScene = GameScenes.LOADING;
        double lastGameTime = 0;
        string currentGame = "";
        string saveFolder = "";
        bool guiActive = false;
        int enterExitWinId;

        const float HEIGHT = 400;
        const float WIDTH = 500f;

        private Rect _windowRect = new Rect(50f, 25f, WIDTH, HEIGHT);

        const string PLUGINDATADIR = "GameData/VesselNotes/PluginData/";
        const string PLUGINDATA = PLUGINDATADIR + "VesselNotes.cfg";
        const string NODENAME = "VesselNotes";

        static ConfigNode configFile, configFileNode;

        internal static bool active = true;
        static bool KspSkin = true;
        static bool showAscending = true;
        static bool showAll = true;
        static bool showInstructions = true;


        static List<GameNote> notesList = new List<GameNote>();

        string prePostGameNotes = "";
        string DataFileCfgName { get { return KSPUtil.ApplicationRootPath + "saves/" + saveFolder + "/" + "VesselNotes" + ".cfg"; } }

        void SetUpWinRect()
        {
            _windowRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        }

        public void LoadCfg()
        {
            configFile = ConfigNode.Load(KSPUtil.ApplicationRootPath + PLUGINDATA);
            if (configFile != null)
            {
                configFileNode = configFile.GetNode(NODENAME);
                if (configFileNode != null)
                {
                    active = configFileNode.SafeLoad("active", true);
                    KspSkin = configFileNode.SafeLoad("KspSkin", true);

                    showAscending = configFileNode.SafeLoad("showAscending", true);
                    showAll = configFileNode.SafeLoad("showAll", true);
                    showInstructions = configFileNode.SafeLoad("showInstructions", true);
                }
            }
        }

        public static void SaveCfg()
        {
            configFile = new ConfigNode(NODENAME);
            configFileNode = new ConfigNode(NODENAME);

            configFileNode.AddValue("active", active);
            configFileNode.AddValue("KspSkin", KspSkin);

            configFileNode.AddValue("showAscending", showAscending);
            configFileNode.AddValue("showAll", showAll);
            configFileNode.AddValue("showInstructions", showInstructions);
            configFile.AddNode(NODENAME, configFileNode);
            configFile.Save(KSPUtil.ApplicationRootPath + PLUGINDATA);
        }

        const string GAMETIME = "GameTime";
        public void LoadData()
        {
            notesList.Clear();
            if (File.Exists(DataFileCfgName))
            {
                configFile = ConfigNode.Load(DataFileCfgName);
                if (configFile != null)
                {
                    configFileNode = configFile.GetNode(NODENAME);
                    if (configFileNode != null)
                    {
                        var nodes = configFileNode.GetNodes("Note");
                        foreach (var node in nodes)
                        {
                            var values = node.GetValuesList("Line");
                            string notes = "";
                            foreach (var v in values)
                                notes += v + "\n";
                            notesList.Add(new GameNote(node.SafeLoad(GAMETIME, (double)0), notes));
                        }
                    }
                }
            }
        }

        public void SaveData()
        {
            configFile = new ConfigNode();
            configFileNode = new ConfigNode(NODENAME);
            foreach (var note in notesList)
            {
                ConfigNode node = new ConfigNode("Note");
                string[] lines = note.prePostGameNotes.Split('\n');
                node.AddValue(GAMETIME, note.gameTime);
                foreach (var l in lines)
                {
                    node.AddValue("Line", l);
                }
                configFileNode.AddNode(node);
            }
            configFile.AddNode(NODENAME, configFileNode);
            configFile.Save(DataFileCfgName);
        }


        public void Start()
        {
            DontDestroyOnLoad(this);

            GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
            GameEvents.onGameStateLoad.Add(onGameStateLoad);
            lastScene = HighLogic.LoadedScene;

            enterExitWinId = WindowHelper.NextWindowId("EnterExitGame");
            LoadCfg();
        }

        void onGameStateLoad(ConfigNode node)
        {
            Log.Info("onGameStateLoad");
            if (HighLogic.CurrentGame != null)
            {
                if (lastScene == GameScenes.MAINMENU)
                {
                    currentGame = HighLogic.CurrentGame.Title;
                    saveFolder = HighLogic.SaveFolder;
                    guiActive = true;
                    SetUpWinRect();
                    LoadData();
                }
                lastScene = HighLogic.LoadedScene;
            }
        }

        internal void onGameSceneLoadRequested(GameScenes loadedScene)
        {
            Log.Info("onGameSceneLoadRequested, loadedScene: " + loadedScene + ", lastScene: " + lastScene);
            if (lastScene >= GameScenes.SPACECENTER && lastScene <= GameScenes.TRACKSTATION && loadedScene == GameScenes.MAINMENU)
            {
                guiActive = true;
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

        internal void OnGUI()
        {
            if (guiActive && active)
            {
                if (kspWindow == null)
                {
                    kspWindow = new GUIStyle(HighLogic.Skin.window);
                    unityStockWindow = new GUIStyle(GUI.skin.window);

#if false
                    tex = unityStockWindow.normal.background;
                    var pixels = tex.GetPixels32();

                    for (int i = 0; i < pixels.Length; ++i)
                        pixels[i].a = 255;

                    tex.SetPixels32(pixels); tex.Apply();
                    unityStockWindow.active.background =
                        unityStockWindow.focused.background =
                        unityStockWindow.normal.background = tex;
#endif
                    tex = kspWindow.normal.background;
                    var pixels = tex.GetPixels32();

                    for (int i = 0; i < pixels.Length; ++i)
                        pixels[i].a = 255;

                    tex.SetPixels32(pixels); tex.Apply();

                    kspWindow.active.background =
                        kspWindow.focused.background =
                        kspWindow.normal.background = tex;

                }
                if (KspSkin)
                {
                    GUI.skin = HighLogic.Skin;
                    window = kspWindow;
                }
                else
                {
                    window = unityStockWindow;
                }
                if (notesList.Count > 0 || HighLogic.LoadedScene == GameScenes.MAINMENU)
                    _windowRect = ClickThruBlocker.GUILayoutWindow(enterExitWinId, _windowRect, StartStopWin,
                        HighLogic.LoadedScene == GameScenes.MAINMENU ? "Post Game Notes" : "Game Notes", window);
            }
        }

        Vector2 contractPos;
        internal void StartStopWin(int id)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(KspSkin ? "KSP Skin" : "Alt Skin", GUILayout.Width(90)))
            { KspSkin = !KspSkin; SaveCfg(); }
            if (HighLogic.LoadedScene >= GameScenes.SPACECENTER && lastScene <= GameScenes.TRACKSTATION)
            {
                if (GUILayout.Button(showAll ? "Show All" : "Show Last", GUILayout.Width(90)))
                { showAll = !showAll; SaveCfg(); }
                if (showAll)
                {
                    if (GUILayout.Button(showAscending ? "Ascending" : "Descending", GUILayout.Width(90)))
                    { showAscending = !showAscending; SaveCfg(); }
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy to clipboard", GUILayout.Width(150)))
                {
                    VesselNotesLogs.CopyToClipboard(notesList, showAscending, showAll);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (HighLogic.LoadedScene >= GameScenes.SPACECENTER && lastScene <= GameScenes.TRACKSTATION)
            {
                contractPos = GUILayout.BeginScrollView(contractPos, GUILayout.Height(HEIGHT - 60));
                if (showAll)
                {
                    for (int i = 0; i < notesList.Count; i++)
                    {
                        GameNote l = notesList[showAscending ? i : notesList.Count - i - 1];

                        var GameTimeText = KSPUtil.PrintDate(l.gameTime, includeTime: true);
                        GUILayout.BeginHorizontal();

                        l.visible = GUILayout.Toggle(l.visible, GameTimeText);
                        //GUILayout.Box(GameTimeText);
                        GUILayout.EndHorizontal();
                        //GUILayout.Label(GameTimeText);
                        if (l.visible)
                            GUILayout.TextArea(l.prePostGameNotes);
                        GUILayout.Space(10);
                    }
                }
                else
                {
                    GUILayout.TextArea(notesList[notesList.Count - 1].prePostGameNotes);
                }
                GUILayout.EndScrollView();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", GUILayout.Width(60)))
                    guiActive = false;
                GUILayout.FlexibleSpace();
            }
            else
            {
                prePostGameNotes = GUILayout.TextArea(prePostGameNotes, GUILayout.Height(HEIGHT - 60));
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(60)))
                {
                    guiActive = false;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Clear", GUILayout.Width(60)))
                {
                    prePostGameNotes = "";
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save & Close"))
                {
                    notesList.Add(new GameNote(lastGameTime, prePostGameNotes));
                    SaveData();
                    prePostGameNotes = "";
                    guiActive = false;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }

}
