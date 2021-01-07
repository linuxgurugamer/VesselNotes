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
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Vessel Notes"; } }
        public override string DisplaySection { get { return "Vessel Notes"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } 
        }

        [GameParameters.CustomParameterUI("Auto-log Enabled",
            toolTip = "Enables the autolog")]
        public bool autolog = true;

        [GameParameters.CustomParameterUI("Highlight part",
            toolTip = "Highlights the part which the current window is attached to")]
        public bool highlightPart = true;


        [GameParameters.CustomParameterUI("Landing Monitor Enabled",
            toolTip = "Enables the landing monitor.  Disable if having performance issues (it only checks 5x/sec)")]
        public bool LandingMonitorEnabledForSave = true;

        [GameParameters.CustomFloatParameterUI("Landed stability time", minValue = 0.5f, maxValue = 5.0f,
            toolTip = "How long a vessel needs to be stable after landing before being considered <b>Landed</b>")]
        public double landedStabilityTime = 1;
        [GameParameters.CustomFloatParameterUI("Min altitude to be flying", minValue = 25.0f, maxValue = 500.0f,
            toolTip = "Minimum altitude to be considered flying")]
        public double minFlyingAltitude = 50;
        [GameParameters.CustomFloatParameterUI("Min flying time ", minValue = 1.0f, maxValue = 50.0f,
            toolTip = "Minimum time not touching the ground to be considered flying")]
        public double minFlyingTime = 10;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return autolog ;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }


    }
}
