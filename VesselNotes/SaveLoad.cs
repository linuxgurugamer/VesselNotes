using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VesselNotesNS.RegisterToolbar;

namespace VesselNotesNS
{
    internal partial class VesselNotesLogs
    {
        const string NODENAME = "VESSELNOTES";
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (HighLogic.CurrentGame == null)
                return;
            if (!HighLogic.LoadedSceneIsEditor && vessel != null)
            {
                if (vessel.protoVessel.protoPartSnapshots[0].partName == "PotatoRoid" || vessel.isEVA)
                    return;
            }
            noteList = new NOTE_LIST();
            ConfigNode vesselNode = node.GetNode(NODENAME);
            Guid listguid = Guid.Empty;
            if (node.TryGetValue("LISTGUID", ref listguid))
            {
                noteList.listGuid = listguid;

                bool log = false;
                if (node.TryGetValue("AUTOLOG", ref log))
                {
                    logList.autolog = log;

                    if (vesselNode != null)
                    {
                        ConfigNode[] nodes = vesselNode.GetNodes("NOTES");
                        if (nodes != null)
                        {
                            foreach (var n in nodes)
                            {
                                bool privateNote = false;
                                if (n.TryGetValue("PRIVATENOTE", ref privateNote))
                                {
                                    string title = "";
                                    if (n.TryGetValue("TITLE", ref title))
                                    {
                                        string note = "";
                                        if (n.TryGetValue("NOTE", ref note))
                                        {
                                            note = note.Replace("<EOL>", "\n");

                                            Guid guid = Guid.Empty;
                                            if (n.TryGetValue("GUID", ref guid))
                                            {
                                                Guid vesselId = Guid.Empty;
                                                if (n.TryGetValue("VESSEL_ID", ref vesselId))
                                                {
                                                    while (note.Length > 0 && note[note.Length - 1] == '\n')
                                                        note = note.Remove(note.Length - 1, 1);

                                                    double gameDateTime = 0;
                                                    n.TryGetValue("GAMEDATETIME", ref gameDateTime);

                                                    noteList.list.Add(new NOTE(title, note, guid, vesselId, privateNote, gameDateTime:gameDateTime));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            noteList.lastOnLoad = Planetarium.GetUniversalTime();

            if (vesselNode != null)
            {

                ConfigNode[] logs = vesselNode.GetNodes("VESSELLOG");

                if (logs != null)
                {
                    foreach (var n in logs)
                    {
                        bool privateNote = false;
                        if (n.TryGetValue("PRIVATENOTE", ref privateNote))
                        {
                            string title = "";
                            if (n.TryGetValue("TITLE", ref title))
                            {
                                string note = "";
                                if (n.TryGetValue("NOTE", ref note))
                                {
                                    note = note.Replace("<EOL>", "\n");
                                    Guid guid = Guid.Empty;
                                    if (n.TryGetValue("GUID", ref guid))
                                    {
                                        Guid vesselId = Guid.Empty;
                                        if (n.TryGetValue("VESSEL_ID", ref vesselId))
                                        {
                                            double gameDateTime = 0;
                                            n.TryGetValue("GAMEDATETIME", ref gameDateTime);
                                            logList.list.Add(new NOTE(title, note, guid, vesselId, privateNote, gameDateTime));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            if (!HighLogic.LoadedSceneIsEditor)
            {
                if (vessel == null || vessel.protoVessel == null || vessel.protoVessel.protoPartSnapshots[0].partName == "PotatoRoid" || vessel.isEVA)
                    return;
            }

            ConfigNode vesselNode = new ConfigNode(NODENAME);
            node.AddValue("LISTGUID", noteList.listGuid);
            node.AddValue("AUTOLOG", logList.autolog);
            foreach (var n in noteList.list)
            {
                ConfigNode note = new ConfigNode("NOTES");

                string s = "";
                if (n.note != null)
                    s = n.note.Replace("\n", "<EOL>");
                note.AddValue("NOTE", s);

                note.AddValue("TITLE", n.title);
                note.AddValue("GAMEDATETIME", n.gameDateTime);
                note.AddValue("GUID", n.guid);
                note.AddValue("VESSEL_ID", n.noteListGuid);
                note.AddValue("PRIVATENOTE", n.privateNote);
                vesselNode.AddNode(note);
            }

            if (!HighLogic.LoadedSceneIsEditor)
            {

                foreach (NOTE n in logList.list)
                {
                    ConfigNode note = new ConfigNode("VESSELLOG");

                    string s = "";
                    if (n.note != null)
                        s = n.note.Replace("\n", "<EOL>");
                    note.AddValue("NOTE", s);

                    note.AddValue("TITLE", n.title);
                    note.AddValue("GAMEDATETIME", n.gameDateTime);
                    note.AddValue("GUID", n.guid);
                    note.AddValue("VESSEL_ID", n.noteListGuid);
                    note.AddValue("PRIVATENOTE", n.privateNote);
                    vesselNode.AddNode(note);
                }
                node.AddNode(vesselNode);
            }

            base.OnSave(node);
        }
    }
}
