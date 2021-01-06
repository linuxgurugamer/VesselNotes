using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VesselNotesNS
{
    internal partial class VesselNotes
    {
        const string NODENAME = "VESSELNOTES";
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (HighLogic.CurrentGame == null)
                return;
            noteList = new NOTE_LIST();
            Guid listguid = Guid.Empty;
            if (node.TryGetValue("LISTGUID", ref listguid))
            {
                ConfigNode vesselNode = node.GetNode(NODENAME);
                noteList.listGuid = listguid;
                if (vesselNode != null)
                {
                    ConfigNode[] nodes = vesselNode.GetNodes("NOTES");
                    if (nodes != null)
                    {
                        foreach (var n in nodes)
                        {
                            bool solo = false;
                            if (n.TryGetValue("SOLONOTE", ref solo))
                            {
                                string title = "";
                                if (n.TryGetValue("TITLE", ref title))
                                {
                                    string note = "";
                                    if (n.TryGetValue("NOTE", ref note))
                                    {
                                        Guid guid = Guid.Empty;
                                        if (n.TryGetValue("GUID", ref guid))
                                        {
                                            Guid vesselId = Guid.Empty;
                                            if (n.TryGetValue("VESSEL_ID", ref vesselId))
                                            {
                                                noteList.list.Add(new NOTE(title, note, guid, vesselId, solo));
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


            ConfigNode[] logs = node.GetNodes("VESSELLOG");
            if (logs != null)
            {

                foreach (var n in logs)
                {
                    bool solo = false;
                    if (n.TryGetValue("SOLONOTE", ref solo))
                    {
                        string title = "";
                        if (n.TryGetValue("TITLE", ref title))
                        {
                            string note = "";
                            if (n.TryGetValue("NOTE", ref note))
                            {
                                Guid guid = Guid.Empty;
                                if (n.TryGetValue("GUID", ref guid))
                                {
                                    Guid vesselId = Guid.Empty;
                                    if (n.TryGetValue("VESSEL_ID", ref vesselId))
                                    {
                                        noteList.list.Add(new NOTE(title, note, guid, vesselId, solo));
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
            ConfigNode vesselNode = new ConfigNode(NODENAME);
            node.AddValue("LISTGUID", noteList.listGuid);
            foreach (var n in noteList.list)
            {
                ConfigNode note = new ConfigNode("NOTES");
                note.AddValue("NOTE", n.note);
                note.AddValue("TITLE", n.title);
                note.AddValue("GUID", n.guid);
                note.AddValue("VESSEL_ID", n.noteListGuid);
                note.AddValue("SOLO", n.privateNote);
                vesselNode.AddNode(note);
            }

            foreach (var n in logList.list)
            {
                ConfigNode note = new ConfigNode("VESSELLOG");
                note.AddValue("NOTE", n.note);
                note.AddValue("TITLE", n.title);
                note.AddValue("GUID", n.guid);
                note.AddValue("VESSEL_ID", n.noteListGuid);
                note.AddValue("SOLO", n.privateNote);
                vesselNode.AddNode(note);
            }
            node.AddNode(vesselNode);
            base.OnSave(node);
        }
    }
}
