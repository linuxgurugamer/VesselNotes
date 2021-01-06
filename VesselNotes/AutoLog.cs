using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static VesselNotesNS.VesselNotes;

namespace VesselNotesNS
{
#if false
    internal partial class VesselNotes
    {
        public enum Events
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
            Revert,
            ScreenMsgRecord,
            StageActivate,
            StageSeparation,
            VesselModified,
            VesselRecovered,

            OnReachingSpace,
            OnReEntries,
            OnReturnsFromOrbitSurface,
            OnVesselDestruction,
            OnUndocking,
            OnAnomalyDiscovery,
            OnBuildingUpgrades,
            OnBuildingDamaged,
            OnTechnologyResearch
        }



        public void InitializeLogEvents(bool init = true)
        {
            Log.Info("initializeEvents, init: " + init.ToString());
            if (init)
            {
                GameEvents.onCrashSplashdown.Add(onCrashSplashdown);
                GameEvents.onLaunch.Add(onLaunch);
                GameEvents.onStageSeparation.Add(onStageSeperation);
                GameEvents.onStageActivate.Add(onStageActivate);
                GameEvents.onPartDie.Add(onPartDie);
                GameEvents.onVesselCrewWasModified.Add(onVesselCrewWasModified);
                GameEvents.onVesselOrbitClosed.Add(onVesselOrbitClosed);
                GameEvents.onVesselOrbitEscaped.Add(onVesselOrbitEscaped);

                GameEvents.onCrewKilled.Add(onCrewKilled);
                GameEvents.onCrewTransferred.Add(onCrewTransferred);
                GameEvents.onDominantBodyChange.Add(onDominantBodyChange);
                GameEvents.onFlagPlant.Add(onFlagPlant);

                GameEvents.onKerbalPassedOutFromGeeForce.Add(onKerbalPassedOutFromGeeForce);

                GameEvents.onVesselDocking.Add(onVesselDockingLog);
                GameEvents.onUndock.Add(onVesselUndock);
                GameEvents.onCommandSeatInteractionEnter.Add(onCommandSeatInteraction);

                GameEvents.onCrewOnEva.Add(onCrewOnEva);
                GameEvents.onCrewBoardVessel.Add(onCrewBoardVessel);

                GameEvents.OnScienceRecieved.Add(onScienceReceived);


                GameEvents.OnTriggeredDataTransmission.Add(OnTriggeredDataTransmission);


                //GameEvents.onShowUI.Add(onShowUI);
                //GameEvents.onHideUI.Add(onHideUI);

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
                GameEvents.onVesselDocking.Remove(onVesselDockingLog);
                GameEvents.onVesselCrewWasModified.Remove(onVesselCrewWasModified);
                GameEvents.onVesselOrbitClosed.Remove(onVesselOrbitClosed);
                GameEvents.onVesselOrbitEscaped.Remove(onVesselOrbitEscaped);

                GameEvents.onKerbalPassedOutFromGeeForce.Remove(onKerbalPassedOutFromGeeForce);

                GameEvents.onCrewKilled.Remove(onCrewKilled);
                GameEvents.onCrewOnEva.Remove(onCrewOnEva);
                GameEvents.onCrewBoardVessel.Remove(onCrewBoardVessel);
                GameEvents.onCommandSeatInteractionEnter.Remove(onCommandSeatInteractionEnter);
                GameEvents.onCommandSeatInteractionEnter.Remove(onCommandSeatInteraction);

                GameEvents.onCrewTransferred.Remove(onCrewTransferred);
                GameEvents.onDominantBodyChange.Remove(onDominantBodyChange);
                GameEvents.onFlagPlant.Remove(onFlagPlant);


                GameEvents.onUndock.Remove(onVesselUndock);

                //GameEvents.onShowUI.Remove(onShowUI);
                //GameEvents.onHideUI.Remove(onHideUI);
                // GameEvents.onGameStateCreated.Remove(onGameStateCreated);


                GameEvents.OnScienceChanged.Remove(onScienceChanged);
                GameEvents.OnScienceRecieved.Remove(onScienceReceived);
                GameEvents.OnOrbitalSurveyCompleted.Remove(onOrbitalSurveyCompleted);

                GameEvents.OnReputationChanged.Remove(OnReputationChanged);
                GameEvents.OnTriggeredDataTransmission.Remove(OnTriggeredDataTransmission);

                GameEvents.VesselSituation.onReachSpace.Remove(OnReachSpace);
                GameEvents.onVesselSituationChange.Remove(onVesselSituationChanged);
                GameEvents.VesselSituation.onReturnFromOrbit.Remove(OnReturnFromOrbit);
                GameEvents.VesselSituation.onReturnFromSurface.Remove(OnReturnFromSurface);
                GameEvents.onVesselDestroy.Remove(onVesselDestroy);
                GameEvents.OnProgressComplete.Remove(OnProgressComplete);

            }
        }

        void OnReachSpace(Vessel v)
        {
            Log.Info("OnReachSpace, logOnReachingSpace: ");

            string s = v.vesselName + " has reached space for the first time";

            CreateLogEntry(Events.OnReachingSpace, false, s);
        }
        void onVesselSituationChanged(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> a)
        {
            if ((a.from & (Vessel.Situations.SUB_ORBITAL | Vessel.Situations.ESCAPING | Vessel.Situations.ORBITING)) != 0)
            {
                string s = a.host.vesselName + " reentered the atmosphere";

                CreateLogEntry(Events.OnReEntries, false, s);
            }
        }
        void OnReturnFromOrbit(Vessel v, CelestialBody b)
        {
            string s = v.vesselName + " returned from orbit around " + b.displayName;

            CreateLogEntry(Events.OnReturnsFromOrbitSurface, false, s);
        }
        void OnReturnFromSurface(Vessel v, CelestialBody b)
        {
            string s = v.vesselName + " returned from surface of " + b.displayName;

            CreateLogEntry(Events.OnReturnsFromOrbitSurface, false, s);
        }
        void onVesselDestroy(Vessel v)
        {
            string s = v.vesselName + " was destroyed";

            CreateLogEntry(Events.OnVesselDestruction, false, s);
        }
        void OnProgressComplete(ProgressNode n)
        {
            if (n is KSPAchievements.PointOfInterest)
            {
                string s = "Reached a point of interest: " + ((KSPAchievements.PointOfInterest)n).Id + " on " + ((KSPAchievements.PointOfInterest)n).body;
                CreateLogEntry(Events.OnAnomalyDiscovery, false, s, "");
            }
        }




        void OnReputationChanged(float f, TransactionReasons tr)
        {
            string s;
            if (f > 0)
                s = "Reputation increased by ";
            else
                s = "Reputation dropped by ";
            s += f.ToString("N1");

            CreateLogEntry(Events.OnReputationChanged, false, s, "");
        }

        void OnTriggeredDataTransmission(ScienceData sd, Vessel v, bool b)
        {
            string s;
            if (b)
                s = "Science data " + sd.title + " transmitted by vessel: " + v.vesselName;
            else
                s = "Incomplete science data " + sd.title + " transmitted by vessel: " + v.vesselName;
            CreateLogEntry(Events.OnTriggeredDataTransmission, false, s, "");
        }



        void AddReason(ref string current, string newreason)
        {
            if (current != "")
                current += ", ";
            current += newreason;
        }
        string GetTransReason(TransactionReasons tr)
        {
            string reason = "";
            if ((tr & TransactionReasons.ContractAdvance) != 0)
                AddReason(ref reason, "Contract advance");
            if ((tr & TransactionReasons.ContractReward) != 0)
                AddReason(ref reason, "Contract reward");
            if ((tr & TransactionReasons.ContractPenalty) != 0)
                AddReason(ref reason, "Contrace penalty");
            if ((tr & TransactionReasons.VesselRollout) != 0)
                AddReason(ref reason, "Vessel rollout");
            if ((tr & TransactionReasons.VesselRecovery) != 0)
                AddReason(ref reason, "Vessel recovery");
            if ((tr & TransactionReasons.VesselLoss) != 0)
                AddReason(ref reason, "Vessel loss");
            if ((tr & TransactionReasons.StrategyInput) != 0)
                AddReason(ref reason, "Strategy input");
            if ((tr & TransactionReasons.StrategyOutput) != 0)
                AddReason(ref reason, "Strategy output");
            if ((tr & TransactionReasons.StrategySetup) != 0)
                AddReason(ref reason, "Stragety setup");
            if ((tr & TransactionReasons.ScienceTransmission) != 0)
                AddReason(ref reason, "Science transmission");
            if ((tr & TransactionReasons.StructureRepair) != 0)
                AddReason(ref reason, "Structure repair");
            if ((tr & TransactionReasons.StructureCollapse) != 0)
                AddReason(ref reason, "Structure collapse");
            if ((tr & TransactionReasons.StructureConstruction) != 0)
                AddReason(ref reason, "Structure construction");
            if ((tr & TransactionReasons.RnDTechResearch) != 0)
                AddReason(ref reason, "RnD tech research ");
            if ((tr & TransactionReasons.RnDPartPurchase) != 0)
                AddReason(ref reason, "RnD part purchase");
            if ((tr & TransactionReasons.Cheating) != 0)
                AddReason(ref reason, "Cheating");
            if ((tr & TransactionReasons.CrewRecruited) != 0)
                AddReason(ref reason, "Crew recruited");
            if ((tr & TransactionReasons.ContractDecline) != 0)
                AddReason(ref reason, "Contract decline");
            if ((tr & TransactionReasons.Progression) != 0)
                AddReason(ref reason, "Progression");


            return reason;
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
            string s = v.vesselName + "completed orbital survey of " + body.bodyDisplayName;

            CreateLogEntry(Events.OnOrbitalSurveyCompleted, false, s, "");
        }

        void onScienceChanged(float f, TransactionReasons tr)
        {
            Log.Info("onScienceChanged");

            if ((tr & TransactionReasons.RnDs) == 0)
                return;
            string trs = GetTransReason(tr);

            trs = trs + ", " + f.ToString() + " science received";

            CreateLogEntry(Events.OnScienceChanged, false, trs, "");
        }

        void onScienceReceived(float f, ScienceSubject ss, ProtoVessel pv, bool b)
        {
            string s = f.ToString() + " science received for " + ss.title + ", transmitted by " + pv.vesselName;
            CreateLogEntry(Events.OnScienceReceived, false, s, "");
        }


        void onCrewBoardVessel(GameEvents.FromToAction<Part, Part> b)
        {
            Log.Info("onCrewBoardVessel");

            Log.Info("onCrewBoardVessel, from: " + b.from.partInfo.name + "    to: " + b.to.partInfo.name);
        }
        void onVesselUndock(EventReport evt)
        {
            Log.Info("onUndock");
            Log.Info("onUndock, evt.origin: " + evt.origin.name + ",   " + evt.origin.partInfo.name);
        }

        void onCommandSeatInteractionEnter(KerbalEVA k, bool entering)
        {
            Log.Info("onCommandSeatInteractionEnter, entering: " + entering.ToString());

        }
        void onCommandSeatInteraction(KerbalEVA k, bool entering)
        {
            Log.Info("onCommandSeatInteraction, entering: " + entering.ToString());

        }
        void onKerbalPassedOutFromGeeForce(ProtoCrewMember crewMember)
        {
            Log.Info("onKerbalPassedOutFromGeeForce");

            CreateLogEntry(Events.KerbalPassedOutFromGeeForce, false, crewMember.name, crewMember.name);
        }

        void onCrewOnEva(GameEvents.FromToAction<Part, Part> b)
        {
            Log.Info("onCrewOnEva");

            Log.Info("from: " + b.from.partInfo.name + "    to: " + b.to.partInfo.name);

            CreateLogEntry(Events.CrewOnEVA, false, "Crew went on EVA from vessel: " + b.to.vessel.vesselName);

        }


        void onCrashSplashdown(EventReport evt)
        {
            if (!vesselInFlight)
                return;

            Log.Info("onCrashSplashdown");

            if (evt.origin.vessel.Splashed)
            {
                CreateLogEntry(Events.Splashdown, false, "Splashdown");
            }
            else
            {
                CreateLogEntry(Events.CrashOrSplashdown, false, "Crashed");
            }
        }

        internal void onVesselLanded(Vessel v)
        {
            if (!vesselInFlight)
                return;
            Log.Info("onVesselLanded");


            CreateLogEntry(Events.Landed, false, "Landed");
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
            if (!vesselInFlight)
                return;
            Log.Info("onLaunch");

            CreateLogEntry(Events.Launch, false, "Vessel launched, current crew: " + getCurrentCrew());
        }

        void onStageSeperation(EventReport evt)
        {
            if (!vesselInFlight)
                return;
            Log.Info("onStageSeperation");

            CreateLogEntry(Events.StageSeparation, false, " Stage Seperation");
        }
        void onStageActivate(int stage)
        {
            if (!vesselInFlight)
                return;
            Log.Info("onStageActivate");

            CreateLogEntry(Events.StageActivate, false, "Activated stage #" + stage.ToString());
        }

        void onPartDie(Part p)
        {
            if (p == null || !vesselInFlight)
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
                CreateLogEntry(Events.PartDied, false, s);
            else
                CreateLogEntry(Events.PartDied, false, s);
        }
        void onVesselDockingLog(uint i1, uint i2)
        {
            if (!vesselInFlight)
                return;

            Log.Info("onVesselDocking");

            CreateLogEntry(Events.PartCouple, false);
        }

        void onVesselCrewWasModified(Vessel v)
        {
            if (v == null || !vesselInFlight)
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
            CreateLogEntry(Events.CrewModified, false, "Crew modified on vessel: " + v.name);
        }

        void onVesselOrbitClosed(Vessel v)
        {
            if (v == null || !vesselInFlight)
                return;
            Log.Info("onVesselOrbitClosed");

            CreateLogEntry(Events.OrbitClosed, false, v.name + " achieved orbit");
        }
        void onVesselOrbitEscaped(Vessel v)
        {

            if (v == null || !vesselInFlight)
                return;
            Log.Info("onVesselOrbitEscaped, vessel: " + v.name);

            CreateLogEntry(Events.OrbitEscaped, false, v.name + " achieved escape velocity");
        }

        void onCrewKilled(EventReport report)
        {
            Log.Info("onCrewKilled");

            CreateLogEntry(Events.CrewKilled, false, report.sender + " killed");
        }

        bool kerbalGoingEVA = false;
        int kerbalTransferred = 0;
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

        void onDominantBodyChange(GameEvents.FromToAction<CelestialBody, CelestialBody> data)
        {
            Log.Info("onDominantBodyChange");

            // This happens when, for example, a kerbal goes EVA on a planet
            if (data.from.bodyName == data.to.bodyName)
                return;

            CreateLogEntry(Events.DominantBodyChange, false, "SOI change from: " + data.from.bodyName + " to " + data.to.bodyName);
        }

        void onFlagPlant(Vessel v)
        {
            Log.Info("onFlagPlant");

            CreateLogEntry(Events.FlagPlant, false, v.vesselName + " planted flag on " + v.mainBody.name);
        }


        public void CreateLogEntry(Events evt, bool manualEntryRequired, string notes = "", string noActiveVessel = null)
        {
            if (FlightGlobals.ActiveVessel == null && noActiveVessel == null)
            {
                Log.Info("CreateLogEntry, returning due to null ActiveVessel");
                return;
            }

            string logentry = VesselNotesNS.VesselLog.GetLogInfo(this.vessel);
            logentry += "\r\n" + notes;

#if false
            foreach (ProtoCrewMember kerbal in FlightGlobals.ActiveVessel.GetVesselCrew())
                {
                    CrewMember cm = new CrewMember(kerbal.name, kerbal.type, kerbal.experienceLevel);
                    leLocal.crewList.Add(cm);
                }
#endif

            logList.list.Add(new NOTE("AutoLog #" + (logList.list.Count + 1).ToString(), logentry, logList.listGuid));
            SetSelectedLog(logList.list.Count + 1);
            selectedLog = logList.list.Count - 1;


        }

    }
#endif
}
