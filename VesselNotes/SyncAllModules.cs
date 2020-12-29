using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VesselNotes
{
    internal partial class VesselNotes
    {
        List<VesselNotes> allNotesModules;
        internal  void SyncAllNotes()
        {
            if (soloNote)
                return;

            if (allNotesModules == null)
                allNotesModules = vessel.FindPartModulesImplementing<VesselNotes>();

            for (int i = 0; i < allNotesModules.Count; i++)
            {
                if (!allNotesModules[i] == this && !allNotesModules[i].soloNote)
                {
                    allNotesModules[i].notes.Clear();
                    for (int n = 0; n < notes.Count; n++)
                    {
                        allNotesModules[i].notes.Add(new NOTE(notes[n].title, notes[n].note, notes[n].id));
                    }
                }
            }
        }
    }
}
