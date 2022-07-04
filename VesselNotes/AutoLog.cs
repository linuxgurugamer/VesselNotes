using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static VesselNotesNS.VesselNotesLogs;

namespace VesselNotesNS
{
    internal partial class VesselNotesLogs
    {
        public enum RespondEvents
        {
            CrewKilled,
            CrewModified,
            CrewOnEVA,
            CrewTransferred,
            DominantBodyChange,
            FinalFrontier,
            FlagPlant,
            FlightLogRecorded,
            KerbalPassedOutFromGeeForce,
            Landed,
            Splashdown,
            CrashOrSplashdown,
            Launch,
            ManualEntry,
            MiscExternal,
            OnFundsChanged,
            OnOrbitalSurveyCompleted,
            OnPartPurchased,
            OnPartUpgradePurchased,
            OnReputationChanged,
            OnScienceChanged,
            OnScienceReceived,
            OnTriggeredDataTransmission,
            OnVesselRollout,
            OrbitClosed,
            OrbitEscaped,
            PartCouple,
            PartDied,
            ProgressRecord,
            StageActivate,
            StageSeparation,
            VesselRecovered,

            OnReachingSpace,
            OnReEntries,
            OnReturnsFromOrbitSurface,
            OnVesselDestruction,
            OnUndocking,
            OnAnomalyDiscovery
        }



        public void InitializeLogEvents(bool init = true)
        {
            if (Log != null)
                Log.Info("initializeEvents, init: " + init.ToString());
            if (init)
            {
                GameEvents.onCrashSplashdown.Add(onCrashSplashdown);
                GameEvents.onLaunch.Add(onLaunch);
                GameEvents.onStageSeparation.Add(onStageSeperation);
                GameEvents.onStageActivate.Add(onStageActivate);
                GameEvents.onPartDie.Add(onPartDie);
                GameEvents.onVesselOrbitClosed.Add(onVesselOrbitClosed);
                GameEvents.onVesselOrbitEscaped.Add(onVesselOrbitEscaped);

                //GameEvents.onCrewKilled.Add(onCrewKilled);
                GameEvents.onDominantBodyChange.Add(onDominantBodyChange);

                GameEvents.onKerbalPassedOutFromGeeForce.Add(onKerbalPassedOutFromGeeForce);

                GameEvents.onVesselDocking.Add(onVesselDockingLog);
                GameEvents.onUndock.Add(onVesselUndock);

                GameEvents.onCrewOnEva.Add(onCrewOnEva);

                GameEvents.OnTriggeredDataTransmission.Add(OnTriggeredDataTransmission);

                GameEvents.VesselSituation.onReachSpace.Add(OnReachSpace);
                GameEvents.onVesselSituationChange.Add(onVesselSituationChanged);
                GameEvents.VesselSituation.onReturnFromOrbit.Add(OnReturnFromOrbit);
                GameEvents.VesselSituation.onReturnFromSurface.Add(OnReturnFromSurface);
                GameEvents.onVesselDestroy.Add(onVesselDestroy);

            }
            else
            {
                GameEvents.onCrashSplashdown.Remove(onCrashSplashdown);
                GameEvents.onLaunch.Remove(onLaunch);
                GameEvents.onStageSeparation.Remove(onStageSeperation);
                GameEvents.onStageActivate.Remove(onStageActivate);
                GameEvents.onPartDie.Remove(onPartDie);
                GameEvents.onVesselOrbitClosed.Remove(onVesselOrbitClosed);
                GameEvents.onVesselOrbitEscaped.Remove(onVesselOrbitEscaped);

                //GameEvents.onCrewKilled.Remove(onCrewKilled);
                GameEvents.onDominantBodyChange.Remove(onDominantBodyChange);

                GameEvents.onKerbalPassedOutFromGeeForce.Remove(onKerbalPassedOutFromGeeForce);

                GameEvents.onVesselDocking.Remove(onVesselDockingLog);
                GameEvents.onUndock.Remove(onVesselUndock);

                GameEvents.onCrewOnEva.Remove(onCrewOnEva);

                GameEvents.OnTriggeredDataTransmission.Remove(OnTriggeredDataTransmission);

                GameEvents.VesselSituation.onReachSpace.Remove(OnReachSpace);
                GameEvents.onVesselSituationChange.Remove(onVesselSituationChanged);
                GameEvents.VesselSituation.onReturnFromOrbit.Remove(OnReturnFromOrbit);
                GameEvents.VesselSituation.onReturnFromSurface.Remove(OnReturnFromSurface);
                GameEvents.onVesselDestroy.Remove(onVesselDestroy);

            }
        }

        void OnReachSpace(Vessel v)
        {
            Log.Info("OnReachSpace, logOnReachingSpace: ");
            if (v != part.vessel) return;
            string s = v.vesselName + " has reached space for the first time";

            CreateLogEntry(RespondEvents.OnReachingSpace, false, s);
        }
        void onVesselSituationChanged(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> fromTo)
        {
            if ((fromTo.from & (Vessel.Situations.SUB_ORBITAL | Vessel.Situations.ESCAPING | Vessel.Situations.ORBITING)) != 0)
            {
                if (fromTo.host != part.vessel) return;
                string s = fromTo.host.vesselName + " reentered the atmosphere";

                CreateLogEntry(RespondEvents.OnReEntries, false, s);
            }
        }
        void OnReturnFromOrbit(Vessel v, CelestialBody b)
        {
            if (v != part.vessel) return;
            string s = v.vesselName + " returned from orbit around " + b.displayName;

            CreateLogEntry(RespondEvents.OnReturnsFromOrbitSurface, false, s);
        }
        void OnReturnFromSurface(Vessel v, CelestialBody b)
        {
            if (v != part.vessel) return;
            string s = v.vesselName + " returned from surface of " + b.displayName;

            CreateLogEntry(RespondEvents.OnReturnsFromOrbitSurface, false, s);
        }
        void onVesselDestroy(Vessel v)
        {
            return;
#if false
            if (v != part.vessel) return;
            string s = v.vesselName + " was destroyed";

            CreateLogEntry(RespondEvents.OnVesselDestruction, false, s);
            SaveLogsToFile(v, this.part);
            ScreenMessages.PostScreenMessage("Logs saved to file", 5, ScreenMessageStyle.UPPER_CENTER);
#endif
        }

        void OnTriggeredDataTransmission(ScienceData sd, Vessel v, bool b)
        {
            if (v != part.vessel) return;
            string s;
            if (b)
                s = "Science data " + sd.title + " transmitted by vessel: " + v.vesselName;
            else
                s = "Incomplete science data " + sd.title + " transmitted by vessel: " + v.vesselName;
            CreateLogEntry(RespondEvents.OnTriggeredDataTransmission, false, s, "");
        }


        public static bool vesselInFlight
        {
            get
            {
                return (/* HighLogic.LoadedScene == GameScenes.SPACECENTER || */ HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.TRACKSTATION);
            }
        }

        void onOrbitalSurveyCompleted(Vessel v, CelestialBody body)
        {
            if (v != part.vessel) return;
            string s = v.vesselName + "completed orbital survey of " + body.bodyDisplayName;

            CreateLogEntry(RespondEvents.OnOrbitalSurveyCompleted, false, s, "");
        }

        void onVesselUndock(EventReport evt)
        {
            if (evt.origin.vessel != part.vessel) return;

            Log.Info("onUndock");
            Log.Info("onUndock, evt.origin: " + evt.origin.name + ",   " + evt.origin.partInfo.name);
            CreateLogEntry(RespondEvents.OnUndocking, false, "", "");
        }
        void onKerbalPassedOutFromGeeForce(ProtoCrewMember crewMember)
        {
            Log.Info("onKerbalPassedOutFromGeeForce");

            CreateLogEntry(RespondEvents.KerbalPassedOutFromGeeForce, false, crewMember.name, crewMember.name);
        }

        void onCrewOnEva(GameEvents.FromToAction<Part, Part> b)
        {
            Log.Info("onCrewOnEva");
            if (b.from != part.vessel) return;

            Log.Info("from: " + b.from.partInfo.name + "    to: " + b.to.partInfo.name);

            CreateLogEntry(RespondEvents.CrewOnEVA, false, "Crew went on EVA from vessel: " + b.to.vessel.vesselName + ", from.vessel: " + b.from.vessel.vesselName);

        }


        void onCrashSplashdown(EventReport evt)
        {
            if (evt.origin.vessel != part.vessel) return;
            Log.Info("onCrashSplashdown");

            if (evt.origin.vessel.Splashed)
            {
                CreateLogEntry(RespondEvents.Splashdown, false, "Splashdown");
            }
            else
            {
                CreateLogEntry(RespondEvents.CrashOrSplashdown, false, "Crashed");
            }
        }

        internal void onVesselLanded(Vessel v, bool splashed = false)
        {
            if (!vesselInFlight || v != part.vessel)
                return;
            Log.Info("onVesselLanded");


            CreateLogEntry(RespondEvents.Landed, false, splashed? "Landed":"Splashdown");
        }

        public string getCurrentCrew(Vessel v = null)
        {
            string crew = "";
            if (v == null)
                v = FlightGlobals.ActiveVessel;
            foreach (ProtoCrewMember kerbal in v.GetVesselCrew())
            {
                if (crew != "")
                    crew += ", ";
                crew += kerbal.name;
            }
            return crew;
        }

        void onLaunch(EventReport evt)
        {
            Log.Info("onLaunch 0, LoadedScene: " + HighLogic.LoadedScene.ToString());
            if (!vesselInFlight || FlightGlobals.ActiveVessel != part.vessel)
                return;
            Log.Info("onLaunch");

            CreateLogEntry(RespondEvents.Launch, false, "Vessel launched, current crew: " + getCurrentCrew());
        }

        void onStageSeperation(EventReport evt)
        {
            if (!vesselInFlight || evt.origin.vessel != part.vessel)
                return;
            Log.Info("onStageSeperation");

            CreateLogEntry(RespondEvents.StageSeparation, false, " Stage Seperation");
        }
        void onStageActivate(int stage)
        {
            if (!vesselInFlight)
                return;
            Log.Info("onStageActivate");

            CreateLogEntry(RespondEvents.StageActivate, false, "Activated stage #" + stage.ToString() +
                ", Staging #: " + FlightGlobals.ActiveVessel.currentStage);

        }

        void onPartDie(Part p)
        {
            if (p == null || !vesselInFlight || p.vessel != part.vessel)
                return;
            Log.Info("onPartDie");

            if (p.vessel.vesselType == VesselType.Debris)
                return;
            string s;
            if (p.vessel.rootPart != p)
                s = p.vessel.name + ", " + p.partInfo.name + " was destroyed";
            else
                s = p.partInfo.name + " was destroyed";
            if (p.vessel == FlightGlobals.ActiveVessel)
                CreateLogEntry(RespondEvents.PartDied, false, s);
            else
                CreateLogEntry(RespondEvents.PartDied, false, s);
        }
        void onVesselDockingLog(uint i1, uint i2)
        {
            if (!vesselInFlight || (i1 != this.vessel.persistentId && i2 != this.vessel.persistentId))
                return;

            Log.Info("onVesselDocking");

            CreateLogEntry(RespondEvents.PartCouple, false);
        }

#if false
        void onVesselCrewWasModified(Vessel v)
        {
            if (v == null || !vesselInFlight || v != part.vessel)
                return;
            Log.Info("onVesselCrewWasModified, kerbalGoingEVA: " + kerbalGoingEVA.ToString() + ",    kerbalTransfered: " + kerbalTransferred.ToString());
            if (kerbalGoingEVA)
                return;
            if (kerbalTransferred > 0)
            {
                Log.Info("onVesselCrewWasModified, not throwing because kerbalTransfered is true, vessel: " + v.vesselName);
                kerbalTransferred--;
                return;
            }
            CreateLogEntry(RespondEvents.CrewModified, false, "Crew modified on vessel: " + v.name);
        }
#endif
        void onVesselOrbitClosed(Vessel v)
        {
            if (v == null || !vesselInFlight || v != part.vessel)
                return;
            Log.Info("onVesselOrbitClosed");

            CreateLogEntry(RespondEvents.OrbitClosed, false, v.name + " achieved orbit");
        }
        void onVesselOrbitEscaped(Vessel v)
        {

            if (v == null || !vesselInFlight)
                return;
            Log.Info("onVesselOrbitEscaped, vessel: " + v.name);

            CreateLogEntry(RespondEvents.OrbitEscaped, false, v.name + " achieved escape velocity");
        }

#if false
        void onCrewKilled(EventReport report)
        {
            Log.Info("onCrewKilled");
            if (vessel.isActiveVessel)
                CreateLogEntry(RespondEvents.CrewKilled, false, report.sender + " killed");
        }
#endif

#if false
        void onCrewTransferred(GameEvents.HostedFromToAction<ProtoCrewMember, Part> data)
        {
            Log.Info("onCrewTransferred");

            Log.Info("onCrewTransferred, from: " + data.from.name + ", " + data.from.partInfo.name + "   to: " + data.to.name + ", " + data.to.partInfo.name);
            if (data.to.Modules.Contains<KerbalEVA>())
            {
                Log.Info("Kerbal going EVA");
                kerbalGoingEVA = true;
                return;
            }
            kerbalTransferred = 2;

        }
#endif

        void onDominantBodyChange(GameEvents.FromToAction<CelestialBody, CelestialBody> data)
        {
            Log.Info("onDominantBodyChange");

            // This happens when, for example, a kerbal goes EVA on a planet
            if (data.from.bodyName == data.to.bodyName || part.vessel != FlightGlobals.ActiveVessel)
                return;

            CreateLogEntry(RespondEvents.DominantBodyChange, false, "SOI change from: " + data.from.bodyName + " to " + data.to.bodyName);
        }


        public void CreateLogEntry(RespondEvents evt, bool manualEntryRequired, string notes = "", string noActiveVessel = null)
        {
            if (FlightGlobals.ActiveVessel == null && noActiveVessel == null)
            {
                Log.Info("CreateLogEntry, returning due to null ActiveVessel");
                return;
            }

            string logentry = VesselNotesNS.VesselLog.GetLogInfo(this.vessel);
            logentry += "\n"+ notes + "\n";

#if false
            string crew = getCurrentCrew(this.vessel);
#endif

            logList.list.Add(new NOTE("AutoLog #" + (logList.list.Count + 1).ToString(), logentry, logList.listGuid));
            SetSelectedLog(logList.list.Count + 1);
            selectedLog = logList.list.Count - 1;


        }

        //==================
        const float ChecksPerSecond = 5;

        void LandingMonitorStart()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().LandingMonitorEnabledForSave)
                return;
            Log.Info("LandingMonitor.Start");
            var notesModules = vessel.FindPartModulesImplementing<VesselNotesLogs>();
            if (notesModules.Count == 0 || notesModules[0].vessel != this.vessel)
                return;
            InvokeRepeating("WatchForLanding", 2.0f, 1f / ChecksPerSecond);
        }
        void LandingMonitorStop()
        {
            StopCoroutine("WatchForLanding");
        }

        void OnDestroy()
        {
            CancelInvoke();
            if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().autolog)
                InitializeLogEvents(false);
        }
        bool landed = true;
        double landedTime = 0;
        bool flying = false;
        double flyingtime;

        bool splashed = false;
        double splashedtime;
        
        internal void WatchForLanding()
        {
            if (!VesselNotesLogs.vesselInFlight)
                return;

            if (!landed && part.vessel.Landed)
            {
                flying = false;
                landedTime = Planetarium.GetUniversalTime();
            }
            if (!splashed && part.vessel.Splashed)
            {
                flying = false;
                splashedtime = Planetarium.GetUniversalTime();
            }

            /* Landing definition
                1.  Touching the ground
                2.  speed less than 0.05
                3.  Stable for 5 seconds
            */
            if (( !landed && part.vessel.Landed) || (!splashed  && part.vessel.Splashed) &&
                Planetarium.GetUniversalTime() - landedTime >= HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().landedStabilityTime && part.vessel.speed < 0.075f)
            {
                onVesselLanded(part.vessel, part.vessel.Splashed);
                landed = part.vessel.Landed;
                splashed = part.vessel.Splashed;
                flying = false;
                landedTime = Planetarium.GetUniversalTime();
            }

            /* Flying definition
               1.  Altitude > 50m
               2.  Not touching the ground for 5 seconds                   
             */
            if (part.vessel.heightFromTerrain >= HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().minFlyingAltitude)
            {
                Log.Info("LandingMonitor: v.heightFromTerrain: " + part.vessel.heightFromTerrain.ToString());
                if (!flying)
                {
                    flyingtime = Planetarium.GetUniversalTime();
                }
                if ( !flying && 
                   Planetarium.GetUniversalTime() - flyingtime >= HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().minFlyingTime)
                {
                    landed = false;
                    splashed = false;
                    flying = true;
                }
            }
        }
    }
}
