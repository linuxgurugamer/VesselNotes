using System;
using System.Collections.Generic;

namespace VesselNotesNS
{
    internal partial class VesselNotesLogs
    {
        internal class NOTE_LIST
        {
            internal Guid listGuid;
            internal List<NOTE> list;
            internal bool autolog;

            // Following are not saved
            internal double lastOnLoad = double.MaxValue;
            internal bool printed;

            internal NOTE_LIST()
            {
                list = new List<NOTE>();
                if (HighLogic.CurrentGame !=  null)
                    autolog = HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().autolog;
                printed = false;
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
            internal double gameDateTime;
            internal bool privateNote;
            internal bool locked;

            internal NOTE() { }
            internal NOTE(string title, string note, Guid listGuid, bool privateNote, int id = -1, double gameDateTime = -1)
            {
                this.title = title;
                this.note = note;
                if (id == -1)
                    this.id = noteCnt++;
                else
                    this.id = id;
                this.noteListGuid = listGuid;
                this.privateNote = privateNote;
                if (gameDateTime == -1)
                    this.gameDateTime = Planetarium.GetUniversalTime();
                else
                    this.gameDateTime = gameDateTime; ;
                locked = false;
                guid = Guid.NewGuid();
            }

            internal NOTE(string title, string note, Guid id, Guid listGuid, bool privateNote, double gameDateTime = -1)
            {
                this.title = title;
                this.note = note;
                this.id = noteCnt++;
                guid = id;
                noteListGuid = listGuid;
                if (gameDateTime == -1)
                    this.gameDateTime = Planetarium.GetUniversalTime();
                else
                    this.gameDateTime = gameDateTime; ;
                locked = false;
                this.privateNote = privateNote;
            }
            internal NOTE(string title, string note, Guid listGuid, double gameDateTime = -1)
            {
                this.title = title;
                this.note = note;
                this.id = noteCnt++;
                guid = Guid.NewGuid();
                noteListGuid = listGuid;
                if (gameDateTime == -1)
                    this.gameDateTime = Planetarium.GetUniversalTime();
                else
                    this.gameDateTime = gameDateTime; 
                locked = false;
                privateNote = false;
            }
        }
    }
}
