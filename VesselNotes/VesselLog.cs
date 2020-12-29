// -------------------------------------------------------------------------------------------------
// Code in this file copied from ksp_notes
//
// notes.cs 0.14.1
//
// Simple KSP plugin to take notes ingame.
// Copyright (C) 2016 Iván Atienza
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
//
// Email: mecagoenbush at gmail dot com
// Freenode & EsperNet: hashashin
//
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using KSP.IO;
using UnityEngine;

namespace VesselNotes
{
    public class VesselLog
    {
        // Get vessel log information.
        public static string GetLogInfo(Vessel v)
        {
            if (HighLogic.LoadedSceneIsEditor)
                return "";
            string _vesselInfo;

            double _seconds = Planetarium.GetUniversalTime();
            _seconds = Math.Abs(_seconds);

            const int _minuteL = 60;
            const int _hourL = 60 * _minuteL;
            int _dayL = 24 * _hourL;
            int _yearL = 365 * _dayL;
            if (GameSettings.KERBIN_TIME)
            {
                _dayL = 6 * _hourL;
                _yearL = 426 * _dayL;
            }
            int _years = (int)Math.Floor(_seconds / _yearL);
            int _ryears = _years + 1;
            int _tseconds = (int)Math.Floor(_seconds);
            _seconds = _tseconds - _years * _yearL;
            int _days = (int)Math.Floor(_seconds / _dayL);
            int _rdays = _days + 1;
            _seconds -= _days * _dayL;
            int _hours = (int)Math.Floor(_seconds / _hourL);
            _seconds -= _hours * _hourL;
            int _minutes = (int)Math.Floor(_seconds / _minuteL);
            _seconds -= _minutes * _minuteL;

            const string _separator =
                "------------------------------------------------------------------------------------------------";
            TimeSpan diff = TimeSpan.FromSeconds(FlightGlobals.ActiveVessel.missionTime);
            string _formatted = string.Format(
                  CultureInfo.CurrentCulture,
                  "{0}y, {1}d, {2}:{3}:{4}",
                  diff.Days / 365,
                  (diff.Days - (diff.Days / 365) * 365) - ((diff.Days - (diff.Days / 365) * 365) / 30) * 30,
                  diff.Hours.ToString("00"),
                  diff.Minutes.ToString("00"),
                  diff.Seconds.ToString("00"));
            string _situation = Vessel.GetSituationString(FlightGlobals.ActiveVessel);
            _vesselInfo =
                string.Format("\n{0}\n{1} --- Year: {2} Day: {3} Time: {4}:{5:00}:{6:00}\n" + "MET: {7} --- Status: {8}\n{0}\n",
                    _separator, v.GetDisplayName(), _ryears, _rdays, _hours, _minutes, _seconds, _formatted, _situation);
            return _vesselInfo;
        }

    }
}
