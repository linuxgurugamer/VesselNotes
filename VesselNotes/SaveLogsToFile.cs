using System;
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
    internal partial class VesselNotes
    {

        static internal ToolbarControl toolbarControl = null;
        internal const string MODID = "VesselNotes";
        internal const string MODNAME = "Vessel Notes & Logs";

        static Rect winRect = new Rect(0, 0, 200, 300);
        bool isVisible = false;
        static bool firstTime = true;
#if false
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
            return dir + name + ".txt";
        }
        internal void SaveLogsToFile(Vessel v)
        {
            StringBuilder sbPrint = new StringBuilder();

            var SaveDir = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/VesselLogs/";
            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);

            var allNotesModules = v.FindPartModulesImplementing<VesselNotes>();
            int cnt = 1;
            foreach (var m in allNotesModules)
            {
                sbPrint.Clear();
                foreach (var n in m.logList.list)
                {
                    sbPrint.Append(n.note);
                }
                while (File.Exists(GetFilename(SaveDir,v, ref cnt)))
                {
                    cnt++;
                }
                File.WriteAllText(GetFilename(SaveDir, v, ref cnt), sbPrint.ToString());

            }
            if (cnt == 1)
                ScreenMessages.PostScreenMessage("Logs saved to file", 5, ScreenMessageStyle.UPPER_CENTER);
            else
                ScreenMessages.PostScreenMessage(cnt + " logs saved to file", 5, ScreenMessageStyle.UPPER_CENTER);
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
