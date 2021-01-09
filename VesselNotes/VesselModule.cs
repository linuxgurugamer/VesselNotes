using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;
using static VesselNotesNS.VesselNotesLogs;


namespace VesselNotesNS
{
    //class MyVesselModule : VesselModule
    internal partial class VesselNotesLogs
    {
        int partCount = -1;

        public void StartVesselMonitoring()
        {
            //vessel = GetComponent<Vessel>();
            if (vessel.protoVessel.protoPartSnapshots[0].partName == "PotatoRoid" || vessel.isEVA)
                return;

            if (vessel.loaded)
            {
                InitVesselLoaded();
            }
        }

        void InitVesselLoaded()
        {
            GameEvents.onVesselLoaded.Add(onVesselLoaded);
            GameEvents.onVesselSwitchingToUnloaded.Add(onVesselSwitchingToUnloaded);

            partCount = vessel.parts.Count;
            ResetGuids();
            if (HighLogic.LoadedSceneIsFlight && vessel != null)
                StartCoroutine(SlowUpdate(1f));
        }
        void onVesselLoaded(Vessel v)
        {
            InitVesselLoaded();
        }

        void onVesselSwitchingToUnloaded(Vessel v1, Vessel v2)
        {
            GameEvents.onVesselLoaded.Remove(onVesselLoaded);
            GameEvents.onVesselSwitchingToUnloaded.Remove(onVesselSwitchingToUnloaded);

            StopCoroutine(SlowUpdate(1f));
        }
        private IEnumerator SlowUpdate(float delay)
        {
            while (true)
            {

                if (vessel == null)
                {
                    partCount = -1;
                }
                else
                {

                    int newPartCount = vessel.Parts.Count;
                    if (newPartCount != partCount)
                    {
                        partCount = newPartCount;
                        ResetGuids();
                    }
                }
                yield return new WaitForSeconds(delay);
            }
        }

        void ResetGuids()
        {
            if (HighLogic.LoadedSceneIsFlight && vessel != null)
                StartCoroutine(DoResetGuids());
        }


        IEnumerator DoResetGuids()
        {
            int cnt = 0;
            Guid g = Guid.NewGuid();
            if (Log != null)
                Log.Info("MyVesselModule.ResetGuids");
            var notesModules = vessel.FindPartModulesImplementing<VesselNotesLogs>();
            while (cnt < notesModules.Count)
            {
                foreach (var n in notesModules)
                {
                    if (n.noteList.lastOnLoad < Planetarium.GetUniversalTime())
                    {
                        n.noteList.ResetGuids(g);
                        cnt++;
                        n.noteList.lastOnLoad = double.MaxValue;

                        
                        n.logList.ResetGuids(g);
                        
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return null;
        }
    }
}
