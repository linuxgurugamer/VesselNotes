using UnityEngine;
using static VesselNotesNS.RegisterToolbar;
using static VesselNotesNS.VesselNotesLogs;

namespace VesselNotesNS
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class NotesPersistentData : MonoBehaviour
    {
        internal static NOTE_LIST noteList;
        public static string flightData;
        public static bool returningFromFlight;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        internal static void OnSceneLoadRequested(GameScenes scene, NOTE_LIST vesselNotesList)
        {
            if (HighLogic.LoadedSceneIsFlight &&
                scene == GameScenes.EDITOR)
            {
                returningFromFlight = true;

                // Capture whatever you need BEFORE flight is destroyed
                if (FlightGlobals.ActiveVessel != null)
                {
                    flightData = FlightGlobals.ActiveVessel.vesselName;
                    noteList = vesselNotesList;
                    Log.Info("Saving flight data before returning to editor: " +  flightData +", number of notes = " + noteList.list.Count);
                }
            }
        }

        
        internal static void OnSceneLoaded(GameScenes scene, ref NOTE_LIST vesselNotesList)
        {
            if (scene != GameScenes.EDITOR)
                return;

            if (!returningFromFlight)
                return;

            returningFromFlight = false;
            if (noteList != null)
                vesselNotesList = noteList;
            Log.Info("Returned to editor. Flight data = " + flightData + ", number of notes = " + noteList.list.Count);

            // Restore/use your data here.
        }
    }
}