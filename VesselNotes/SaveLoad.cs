using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                                                    noteList.list.Add(new NOTE(title, note, guid, vesselId, privateNote));
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
                                        logList.list.Add(new NOTE(title, note, guid, vesselId, privateNote));
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
                if (vessel == null || vessel.protoVessel.protoPartSnapshots[0].partName == "PotatoRoid" || vessel.isEVA)
                    return;
            }
            if (Log == null)
                Debug.Log("VesselNotes: via Debug.Log  OnSave");
            else
                Log.Info("OnSave");

            ConfigNode vesselNode = new ConfigNode(NODENAME);
            node.AddValue("LISTGUID", noteList.listGuid);
            node.AddValue("AUTOLOG", logList.autolog);
            foreach (var n in noteList.list)
            {
                ConfigNode note = new ConfigNode("NOTES");

                string s = n.note.Replace("\n", "<EOL>");
                note.AddValue("NOTE", s);

                note.AddValue("TITLE", n.title);
                note.AddValue("GUID", n.guid);
                note.AddValue("VESSEL_ID", n.noteListGuid);
                note.AddValue("PRIVATENOTE", n.privateNote);
                vesselNode.AddNode(note);
            }

            foreach (var n in logList.list)
            {
                ConfigNode note = new ConfigNode("VESSELLOG");

                string s = n.note.Replace("\n", "<EOL>");
                note.AddValue("NOTE", s);

                note.AddValue("TITLE", n.title);
                note.AddValue("GUID", n.guid);
                note.AddValue("VESSEL_ID", n.noteListGuid);
                note.AddValue("PRIVATENOTE", n.privateNote);
                vesselNode.AddNode(note);
            }
            node.AddNode(vesselNode);
            base.OnSave(node);
        }
    }
}
