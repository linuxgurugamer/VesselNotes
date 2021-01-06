using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VesselNotesNS
{
    internal partial class VesselNotes
    {
        internal class NOTE_LIST
        {
            internal Guid listGuid;
            internal List<NOTE> list;

            internal double lastOnLoad = double.MaxValue;

            internal NOTE_LIST()
            {
                list = new List<NOTE>();
            }
            internal void ResetGuids(Guid newGuid)
            {
                Guid lastGuid = listGuid;
                listGuid = newGuid;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].noteListGuid == lastGuid)
                        list[i].noteListGuid = listGuid;
                }
            }
            internal void LockAllNotes()
            {
                foreach (var n in list)
                {
                    n.locked = true;
                }
            }

        }

        internal class NOTE
        {
            static int noteCnt = 0;

            internal Guid noteListGuid;

            internal Guid guid;
            internal int id;
            internal string title;
            internal string note;
            internal bool privateNote;
            internal bool locked;

            internal NOTE(string title, string note, Guid listGuid, bool solo, int id = -1)
            {
                this.title = title;
                this.note = note;
                if (id == -1)
                    this.id = noteCnt++;
                else
                    this.id = id;
                this.noteListGuid = listGuid;
                privateNote = solo;
                locked = false;
                guid = Guid.NewGuid();
            }

            internal NOTE(string title, string note, Guid id, Guid listGuid, bool solo)
            {
                this.title = title;
                this.note = note;
                this.id = noteCnt++;
                guid = id;
                noteListGuid = listGuid;
                locked = false;
                privateNote = solo;
            }
            internal NOTE(string title, string note, Guid listGuid)
            {
                this.title = title;
                this.note = note;
                this.id = noteCnt++;
                guid = Guid.NewGuid();
                noteListGuid = listGuid;
                locked = false;
                privateNote = false;
            }
        }
    }
}
