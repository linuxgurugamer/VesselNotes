using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ClickThroughFix;
using ToolbarControl_NS;
using static VesselNotesNS.RegisterToolbar;
using SpaceTuxUtility;
using System.Globalization;


namespace VesselNotesNS
{
    internal class ShowVesselLogs : MonoBehaviour
    {
        static ShowVesselLogs showVesselLogs = null;
        static int showVesselLogsWinId;
        const string TITLE = "Show Vessel Logs";

        bool resizingHelpWindow = false;
        internal static GUIStyle resizeButton = null;
        float minLeftWidth = 400;

        Vector2 listScrollPos, logScrollPos;

        StringBuilder logForDisplay = new StringBuilder();
        string strLogForDisplay = "";
        int displayedLog = -1;

        const float MINWIDTH = 400f;
        const float MINHEIGHT = 400f;
        const float MINLOGWIDTH = 400f;

        Rect logsWinRect = new Rect((Screen.width - MINWIDTH) / 2, (Screen.height - MINHEIGHT) / 2, MINWIDTH, MINHEIGHT);
        GUIStyle myButtonStyle;
        Texture2D styleOff, styleOn;

        bool logVisible = false;

        internal class LogCache
        {
            internal string filename;
            internal string vesselName;
            internal double gameTime;
            internal DateBreakdownInfo dbi;

            internal LogCache(string filename, string vesselName, double gameTime, DateBreakdownInfo dbi)
            {
                this.filename = filename;
                this.vesselName = vesselName;
                this.gameTime = gameTime;
                this.dbi = dbi;
            }
        }

        // need to load all logs into cache one time at start
        List<LogCache> logs = new List<LogCache>();
        ConfigNode currentLog;
        void LoadCache()
        {
            var files = Directory.GetFiles(VesselNotesLogs.SaveDir, "*.cfg");
            for (int i = 0; i < files.Length; i++)
            {
                ConfigNode node = ConfigNode.Load(files[i]);
                if (node != null)
                {
                    string vesselName = node.SafeLoad("VesselName", "");
                    double gameTime = node.SafeLoad("GameTime", 0d);

                    using (var dbi = new DateBreakdownInfo(gameTime))
                    {
                        logs.Add(new LogCache(files[i], vesselName, gameTime, dbi));
                    }
                }
            }
        }


        void LoadSelectedLog(int i)
        {
            displayedLog = i;
            logForDisplay = new StringBuilder();
            currentLog = ConfigNode.Load(logs[i].filename);
            foreach (var note in currentLog.GetNodes("VESSELLOG"))
            {
                VesselNotesLogs.NOTE n = new VesselNotesLogs.NOTE();

                string str = note.SafeLoad("NOTE", "");
                str = str.Replace("<EOL>", "\n");
                n.note = str;
                n.title = note.SafeLoad("TITLE", "");
                n.gameDateTime = note.SafeLoad("GAMEDATETIME", 0d);
                n.guid = note.SafeLoad("GUID", n.guid);
                n.noteListGuid = note.SafeLoad("VESSEL_ID", n.noteListGuid);
                n.privateNote = note.SafeLoad("PRIVATENOTE", false);

                AppendLine(ref logForDisplay, str);
            }
            strLogForDisplay = logForDisplay.ToString();
        }

        internal static void InstantiateShowVesselLogsWindow()
        {
            if (showVesselLogs == null)
            {
                GameObject gameObject = new GameObject();
                showVesselLogs = gameObject.AddComponent<ShowVesselLogs>();
            }
            else
                Destroy(showVesselLogs);
        }

        void Start()
        {
            showVesselLogsWinId = SpaceTuxUtility.WindowHelper.NextWindowId("showVesselLogsWinId");
            LoadCache();
        }

        public GUIStyle GetToggleButtonStyle(string styleName, int width, int height, bool hover)
        {
            Log.Info("GetToggleButtonStyle, styleName: " + styleName);

            styleOff = new Texture2D(2, 2);
            styleOn = new Texture2D(2, 2);
            myButtonStyle = new GUIStyle();

            ToolbarControl.LoadImageFromFile(ref styleOff, KSPUtil.ApplicationRootPath + "GameData/VesselNotes/PluginData/textures/" + styleName + "_off");
            ToolbarControl.LoadImageFromFile(ref styleOn, KSPUtil.ApplicationRootPath + "GameData/VesselNotes/PluginData/textures/" + styleName + "_on");

            myButtonStyle.name = styleName + "Button";
            myButtonStyle.padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 };
            myButtonStyle.border = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 };
            myButtonStyle.margin = new RectOffset() { left = 0, right = 0, top = 2, bottom = 2 };
            myButtonStyle.normal.background = styleOff;
            myButtonStyle.onNormal.background = styleOn;
            if (hover)
            {
                myButtonStyle.hover.background = styleOn;
            }
            myButtonStyle.active.background = styleOn;
            myButtonStyle.fixedWidth = width;
            myButtonStyle.fixedHeight = height;
            return myButtonStyle;
        }

        void OnGUI()
        {
            if (resizeButton == null)
                resizeButton = GetToggleButtonStyle("resize", 20, 20, true);
            if (GlobalConfig.KspSkin)
                GUI.skin = HighLogic.Skin;

            logsWinRect = ClickThruBlocker.GUILayoutWindow(showVesselLogsWinId, logsWinRect, ShowVesselLogsWindow, TITLE);
        }

        void OnDestroy()
        {
            showVesselLogs = null;
            logs = null;
        }

        void AppendLine(ref StringBuilder data, string str)
        {
            data.Append(str + "\n");
        }

        private void ShowVesselLogsWindow(int windowId)
        {
            if (! EnterExitGame.ShowControls(true, logsWinRect))
                Destroy(this);
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    if (!logVisible)
                        minLeftWidth = logsWinRect.width - 20;
                    listScrollPos = GUILayout.BeginScrollView(listScrollPos, GUILayout.Width(minLeftWidth));
                    if (logs.Count > 0)
                        minLeftWidth = 0;
                    for (int i = 0; i < logs.Count; i++)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            string vesselName = logs[i].vesselName;
                            double gameTime = logs[i].gameTime;

                            var dbi = logs[i].dbi;
                            string _formatted = string.Format(
                                  CultureInfo.CurrentCulture,
                                  "{0}y, {1}d, {2}:{3}:{4}",
                                  dbi._ryears,
                                  dbi._rdays,
                                  dbi._hours.ToString("00"),
                                  dbi._minutes.ToString("00"),
                                  dbi._seconds.ToString("00"));
                            if (GUILayout.Button("", GUI.skin.toggle))
                            {
                                if (!logVisible)
                                {
                                    logsWinRect.width += MINLOGWIDTH;

                                    logVisible = true;
                                }
                               LoadSelectedLog(i);
                            }
                            GUIContent t = new GUIContent("" );
                            var t1 = GUI.skin.toggle.CalcSize(t);
                            GUIContent g = new GUIContent(vesselName + "  " + _formatted);
                            var s = GUI.skin.label.CalcSize(g);
                            minLeftWidth = Math.Max(s.x + t1.x+20f, minLeftWidth);
                            GUILayout.Label(g);
                        }
                    }
                    GUILayout.EndScrollView();

                    GUILayout.Space(20);
                }
                if (logVisible)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        logScrollPos = GUILayout.BeginScrollView(logScrollPos);

                        GUILayout.TextArea(strLogForDisplay, EnterExitGame.myStyle);
                        GUILayout.EndScrollView();
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Copy Logs to clipboard", GUILayout.Width(200)))
                            {
                                strLogForDisplay.CopyToClipboard();
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", GUILayout.Width(90)))
                    Destroy(this);
                GUILayout.FlexibleSpace();
            }
            if (GUI.RepeatButton(new Rect(logsWinRect.width - 23f, logsWinRect.height - 23f, 16, 16), "", resizeButton))
            {
                resizingHelpWindow = true;
            }
            ResizeVesselLogsWindow();

            GUI.DragWindow();
        }

        void ResizeVesselLogsWindow()
        {
            if (Input.GetMouseButtonUp(0))
            {
                resizingHelpWindow = false;
            }

            if (resizingHelpWindow)
            {
                logsWinRect.width = Math.Max(MINWIDTH, Input.mousePosition.x - logsWinRect.x + 10);
                logsWinRect.height = Math.Max(MINHEIGHT, Screen.height - Input.mousePosition.y) - logsWinRect.y + 10;
            }
        }
    }
}
