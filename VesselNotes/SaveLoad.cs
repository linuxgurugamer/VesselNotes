using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VesselNotes
{
    internal partial class VesselNotes
    {
        const string NODENAME = "VESSELNOTES";
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode vesselNode = node.GetNode(NODENAME);
            if (vesselNode != null)
            {
                ConfigNode[] nodes = vesselNode.GetNodes("NOTES");
                if (nodes != null)
                {
                    foreach (var n in nodes)
                    {
                        string title = "";
                        if (n.TryGetValue("TITLE", ref title))
                        {
                            string note = "";
                            if (n.TryGetValue("NOTE", ref note))
                            {
                                notes.Add(new NOTE(title, note));
                            }
                        }
                    }
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            ConfigNode vesselNode = new ConfigNode(NODENAME);
            foreach (var n in notes)
            {
                ConfigNode note = new ConfigNode("NOTES");
                note.AddValue("NOTE", n.note);
                note.AddValue("TITLE", n.title);
                vesselNode.AddNode(note);
            }
            node.AddNode(vesselNode);
        }
    }
}
