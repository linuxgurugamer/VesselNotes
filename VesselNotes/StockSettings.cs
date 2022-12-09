using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using ClickThroughFix;

namespace VesselNotesNS
{
    public class VN_Settings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "General"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Vessel Notes"; } }
        public override string DisplaySection { get { return "Vessel Notes"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } 
        }

        [GameParameters.CustomParameterUI("Default Auto-log",
            toolTip = "Default Autolog setting, can be set/unset for each vessel")]
        public bool autolog = true;

        [GameParameters.CustomParameterUI("Show Pre/Post game notes",
            toolTip = "Show game notes when starting up and allow entry of notes when exiting a save\nNote: This is a global setting affecting ALL saves in this install")]
        public bool showGameNotes = true;

        [GameParameters.CustomParameterUI("Highlight part",
            toolTip = "Highlights the part which the current window is attached to")]
        public bool highlightPart = true;


        [GameParameters.CustomParameterUI("Landing Monitor Enabled",
            toolTip = "Enables the landing monitor.  Disable if having performance issues (it only checks 5x/sec)")]
        public bool LandingMonitorEnabledForSave = true;

        [GameParameters.CustomParameterUI("Include crew in every log entry",
            toolTip = "Enables adding the current crew to every log entry")]
        public bool logCrewAlways = false;


        [GameParameters.CustomFloatParameterUI("Landed stability time", minValue = 0.5f, maxValue = 5.0f,
            toolTip = "How long a vessel needs to be stable after landing before being considered <b>Landed</b>")]
        public double landedStabilityTime = 1;
        [GameParameters.CustomFloatParameterUI("Min altitude to be flying", minValue = 25.0f, maxValue = 500.0f,
            toolTip = "Minimum altitude to be considered flying")]
        public double minFlyingAltitude = 50;
        [GameParameters.CustomFloatParameterUI("Min flying time ", minValue = 1.0f, maxValue = 50.0f,
            toolTip = "Minimum time not touching the ground to be considered flying")]
        public double minFlyingTime = 10;

        [GameParameters.CustomParameterUI("Always query when locking a note",
            toolTip = "Ask for confirmation when locking a not")]
        public bool confirmLock = true;

        [GameParameters.CustomParameterUI("Check for VesselNotesLogs module",
            toolTip = "Check vessel before launching for a VesselNotesLog (usually in the DataLog part)")]
        public bool checkForVesselNotesLog = true;






        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            //if (member.Name == "autolog" || member.Name == "showGameNotes")
                return true;
            //else
            //    return !autolog ;
        }

        bool initted = false;
        bool oldShowGameNotes;
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (!initted)
            {
                initted = true;
                oldShowGameNotes = showGameNotes;
            }
            if (oldShowGameNotes != showGameNotes)
            {
                oldShowGameNotes = showGameNotes;
                GlobalConfig.active = showGameNotes;
                GlobalConfig.SaveCfg();
            }

            if (member.Name == "autolog" || member.Name == "showGameNotes" || member.Name == "fontSize" || member.Name == "useKspSkin")
                return true;
            else
                return !autolog;

            //return true;
        }
    }


    public class VN_Settings2 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "AutoLog"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Vessel Notes"; } }
        public override string DisplaySection { get { return "Vessel Notes"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets
        {
            get { return false; }
        }

        [GameParameters.CustomParameterUI("Splashdown",
            toolTip = "Record log entry for splashdown")]
        public bool splashdown = true;

        [GameParameters.CustomParameterUI("Crash or Splashdown",
            toolTip = "Record log entry for a crash or splashdown")]
        public bool onCrashSplashdown = true;

        [GameParameters.CustomParameterUI("Launch",
            toolTip = "Record log entry for launch")]
        public bool onLaunch = true;

        [GameParameters.CustomParameterUI("Vessel landed",
            toolTip = "Record log entry for a landing")]
        public bool onVesselLanded = true;

        [GameParameters.CustomParameterUI("Stage Separation",
            toolTip = "Record log entry for stage separation")]
        public bool onStageSeperation = true;

        [GameParameters.CustomParameterUI("Stage Activate",
            toolTip = "Record log entry for stage activation")]
        public bool onStageActivate = true;

        [GameParameters.CustomParameterUI("Part Destroyed",
            toolTip = "Record log entry for part being destroyed")]
        public bool onPartDie = true;

        [GameParameters.CustomParameterUI("Orbit Closed",
            toolTip = "Record log entry for closing an orbit (from an escape trajectory")]
        public bool onVesselOrbitClosed = true;

        [GameParameters.CustomParameterUI("Orbit Escape",
            toolTip = "Record log entry for reaching an escape trajectory")]
        public bool onVesselOrbitEscape = true;

        [GameParameters.CustomParameterUI("Crew Killed",
            toolTip = "Record log entry for a crew member being killed")]
        public bool onCrewKilled = false;

        [GameParameters.CustomParameterUI("Crew Transferred",
            toolTip = "Record log entry for crew member being transferred")]
        public bool onCrewTransferred = false;

        [GameParameters.CustomParameterUI("SOI change",
            toolTip = "Record log entry for change of SOI")]
        public bool onDominantBodyChange = true;

        [GameParameters.CustomParameterUI("Kerbal Passed Out From Gee Force",
            toolTip = "Record log entry for a kerbal passing out from gee force")]
        public bool onKerbalPassedOutFromGeeForce = true;

        [GameParameters.CustomParameterUI("Vessel Docking",
            toolTip = "Record log entry for a vessel docking with another")]
        public bool onVesselDockingLog = true;

        [GameParameters.CustomParameterUI("Vessel UnDocking",
            toolTip = "Record log entry for a vessel undocking ")]
        public bool onVesselUndock = true;

        [GameParameters.CustomParameterUI("Crew On Eva",
            toolTip = "Record log entry for a crew member going on eva")]
        public bool onCrewOnEva = true;

        [GameParameters.CustomParameterUI("Triggered Data Transmission",
            toolTip = "Record log entry for doing a data transmission")]
        public bool OnTriggeredDataTransmission = true;

        [GameParameters.CustomParameterUI("Reaching Space",
            toolTip = "Record log entry for reaching space")]
        public bool OnReachSpace = true;

        [GameParameters.CustomParameterUI("Vessel Situation Changed",
            toolTip = "Record log entry for when the vessel situation changes")]
        public bool onVesselSituationChanged = true;

        [GameParameters.CustomParameterUI("Return From Orbit",
            toolTip = "Record log entry for returning from orbit")]
        public bool OnReturnFromOrbit = true;

        [GameParameters.CustomParameterUI("Return From Surface",
            toolTip = "Record log entry for returning from the surface")]
        public bool OnReturnFromSurface = true;

        [GameParameters.CustomParameterUI("Vessel Destroyed",
            toolTip = "Record log entry for a vessel being destroyed")]
        public bool onVesselDestroy = true;




        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }


    }
}
