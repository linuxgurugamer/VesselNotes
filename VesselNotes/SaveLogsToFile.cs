﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.UI.Screens;
using System.Threading.Tasks;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace VesselNotesNS
{
    // [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    //internal class LogsToFile:MonoBehaviour
    internal partial class VesselNotesLogs
    {

#if false
        static internal ToolbarControl toolbarControl = null;
        internal const string MODID = "VesselNotes";
        internal const string MODNAME = "Vessel Notes & Logs";

        static Rect winRect = new Rect(0, 0, 200, 300);
        bool isVisible = false;
        static bool firstTime = true;
        void Start()
        {
            if (firstTime)
            {
                winRect.x = (Screen.width - winRect.width) / 2;
                winRect.y = (Screen.height - winRect.height) / 2;
                firstTime = false;
            }
        }
#endif
        string GetFilename(string dir, Vessel v, ref int cnt)
        {
            string name = v.vesselName;
            var dateAndTime = DateTime.Now;
            name += "-" + (dateAndTime.Year - 2000).ToString() + "." + dateAndTime.Month.ToString("D2") + "." + dateAndTime.Day.ToString("D2");
            name += "-" + (dateAndTime.Hour).ToString("D2") + "." + (dateAndTime.Minute).ToString("D2");
            if (cnt > 0)
                name += "-" + cnt.ToString();
            return dir + name;
        }
        internal void SaveLogsToFile(Vessel v, Part p)
        {
            StringBuilder sbPrint = new StringBuilder();

            var SaveDir = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/VesselLogs/";
            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);

            //var allNotesModules = v.FindPartModulesImplementing<VesselNotes>();
            var m = p.FindModuleImplementing<VesselNotesLogs>();

            int cnt = 0;
            sbPrint.Clear();
            if (m.logList.list.Count > 0)
            {
                sbPrint.AppendLine("Part: " + p.partInfo.title);
                sbPrint.AppendLine("Vessel: " + vessel.vesselName);

                foreach (var n in m.logList.list)
                {
                    var s = n.note.Split('\n');

                    foreach (var s1 in s)
                        sbPrint.AppendLine(s1);
                }
                while (File.Exists(GetFilename(SaveDir, v, ref cnt) + ".txt"))
                {
                    Log.Info("File found: " + GetFilename(SaveDir, v, ref cnt) + ".txt");
                    cnt++;
                }
                File.WriteAllText(GetFilename(SaveDir, v, ref cnt) + ".txt", sbPrint.ToString());
            }
            ScreenMessages.PostScreenMessage("Logs saved to file", 5, ScreenMessageStyle.UPPER_CENTER);
        }
#if false
    void AddToolbarButton()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (toolbarControl == null)
                {
                    toolbarControl = gameObject.AddComponent<ToolbarControl>();
                    toolbarControl.AddToAllToolbars(Toggle, Toggle,
                        ApplicationLauncher.AppScenes.SPACECENTER,
                        MODID,
                        "airparkButton",
                        "AirPark/VesselNotes/PluginData/VesselNotes-38",
                        "AirPark/VesselNotes/PluginData/VesselNotes-24",
                        MODNAME
                    );

                }
            }
        }

        void Toggle()
        {
            isVisible = !isVisible;
        }

        void OnGUI()
        {
            if (isVisible)
            {
                winRect = ClickThruBlocker.GUILayoutWindow(12342312, winRect, DisplayWin, "Vessel Notes & Logs");
            }
        }
        // vesselname+ uniqueID

        void DisplayWin(int id)
        {
            GUILayout.BeginHorizontal();

        }
#endif
    }
}
