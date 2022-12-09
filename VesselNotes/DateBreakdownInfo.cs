using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VesselNotesNS
{
    public class DateBreakdownInfo : IDisposable
    {
        public double _seconds;
        public int dayLength;
        public int yearLength;
        public int years;
        public int _ryears;
        public int _tseconds;
        public int _days;
        public int _tdays;
        public int _rdays;
        public int _hours;
        public int _minutes;

        const int _minuteL = 60;
        const int _hourL = 60 * _minuteL;
        public DateBreakdownInfo(double t)
        {
            _seconds = t;
            _seconds = Math.Abs(_seconds);

            dayLength = (int)FlightGlobals.GetHomeBody().solarDayLength;
            yearLength = (int)FlightGlobals.GetHomeBody().orbit.period;
            years = (int)Math.Floor(_seconds / yearLength);
            _ryears = years + 1;
            _tseconds = (int)Math.Floor(_seconds);
            _seconds = _tseconds - years * yearLength;
            _days = (int)Math.Floor(_seconds / dayLength);
            _rdays = _days + 1;
            _seconds -= _days * dayLength;
            _hours = (int)Math.Floor(_seconds / _hourL);
            _seconds -= _hours * _hourL;
            _minutes = (int)Math.Floor(_seconds / _minuteL);
            _seconds -= _minutes * _minuteL;
        }
        public void Dispose()
        { }
    }

}
