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
    internal class GameNote
    {
        internal static List<GameNote> notesList = new List<GameNote>();

        internal string prePostGameNotes;
        internal double gameTime;
        internal bool visible;
        internal GameNote(double savedGameTime, string notes)
        {
            visible = true;
            prePostGameNotes = notes;
            gameTime = savedGameTime;
        }

        const string GAMETIME = "GameTime";
        static string DataFileCfgName { get { return KSPUtil.ApplicationRootPath + "saves/" + SaveFolder + "/" + "VesselNotes" + ".cfg"; } }
        static ConfigNode configFile, configFileNode;
        internal static string SaveFolder { set; get; }        

        public static void LoadData()
        {
            notesList.Clear();
            if (File.Exists(DataFileCfgName))
            {
                configFile = ConfigNode.Load(DataFileCfgName);
                if (configFile != null)
                {
                    configFileNode = configFile.GetNode(GlobalConfig.NODENAME);
                    if (configFileNode != null)
                    {
                        var nodes = configFileNode.GetNodes("Note");
                        foreach (var node in nodes)
                        {
                            var values = node.GetValuesList("Line");
                            string notes = "";
                            foreach (var v in values)
                                notes += v + "\n";
                            while (notes.Length>0 && notes[notes.Length - 1] == '\n')
                                notes = notes.Remove(notes.Length -1,1);
                            notesList.Add(new GameNote(node.SafeLoad(GAMETIME, (double)0), notes));
                        }
                    }
                }
            }
        }

        public static void SaveData()
        {
            configFile = new ConfigNode();
            configFileNode = new ConfigNode(GlobalConfig.NODENAME);
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
            configFile.AddNode(GlobalConfig.NODENAME, configFileNode);
            configFile.Save(DataFileCfgName);
        }

    }
}
