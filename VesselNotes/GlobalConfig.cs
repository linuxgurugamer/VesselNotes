using System.Collections.Generic;
using KSP.UI.Screens;
using UnityEngine;
using ClickThroughFix;
using SpaceTuxUtility;

using ToolbarControl_NS;

using static VesselNotesNS.RegisterToolbar;

namespace VesselNotesNS
{
    internal static class GlobalConfig
    {
        static ConfigNode configFile, configFileNode;
        internal static string PLUGINDATADIR { get { return KSPUtil.ApplicationRootPath + "GameData/VesselNotes/PluginData/"; } }
        internal static string PLUGINDATA { get { return PLUGINDATADIR + "VesselNotes.cfg"; } }
        internal const string NODENAME = "VesselNotes";

        internal static bool active = true;
        internal static bool KspSkin = true;
        internal static bool showAscending = true;
        internal static bool showAll = true;
        internal static bool showInstructions = true;
        internal static int FontSize = 13;

        public const int MIN_FONT_SIZE = 6;
        public const int MAX_FONT_SIZE = 36;


        public static void LoadCfg()
        {
            configFile = ConfigNode.Load( PLUGINDATA);
            if (configFile != null)
            {
                configFileNode = configFile.GetNode(NODENAME);
                if (configFileNode != null)
                {
                    active = configFileNode.SafeLoad("active", true);
                    KspSkin = configFileNode.SafeLoad("KspSkin", true);
                    FontSize = configFileNode.SafeLoad("FontSize", 13);

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
            configFileNode.AddValue("FontSize", FontSize);

            configFileNode.AddValue("showAscending", showAscending);
            configFileNode.AddValue("showAll", showAll);
            configFileNode.AddValue("showInstructions", showInstructions);
            configFile.AddNode(NODENAME, configFileNode);
            configFile.Save( PLUGINDATA);
        }

    }
}
