namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Everything a style needs to draw the clock view.
    internal sealed class DayInfo
    {
        public String Hours;        // "14" or "2"
        public String Minutes;      // "52"
        public String Suffix;       // "" for a 24-hour clock, "PM" for a 12-hour one
        public String Header;       // "чт, 12 лис" / "Thu, Nov 12"
        public String Weekday;      // "чт" / "Thu"
        public String MonthShort;   // "лис" / "Nov"
        public Int32 Day;
        public Int32 Year;
        public String Date;         // "12.11.2026" / "11/12/2026"
        public Int32 Hour;          // 0-23
        public Int32 Minute;
        public Int32 Second;

        public String[] WeekLetters = new String[7]; // Monday first
        public Int32[] WeekDays = new Int32[7];      // the day numbers of the current week
        public Int32 WeekToday;                      // 0 = Monday

        public Double MoonPhase;    // 0 = new moon, 0.5 = full moon

        public String Time => this.Hours + ":" + this.Minutes;
    }

    // Everything a style needs to draw the month view.
    internal sealed class MonthInfo
    {
        public Int32 Year;
        public String MonthName;      // "Листопад" / "November"
        public String MonthShort;     // "Лис" / "Nov"
        public String[] WeekdayLetters = new String[7]; // Monday first
        public Int32 Offset;          // the weekday column of the 1st: 0 = Monday
        public Int32 DaysInMonth;
        public Int32 Today;           // today's day number, or 0 when another month is shown

        public Int32 Rows => (this.Offset + this.DaysInMonth + 6) / 7;

        // The grid row of today, or -1 when another month is shown.
        public Int32 TodayRow => this.Today > 0 ? (this.Offset + this.Today - 1) / 7 : -1;
    }

    // One complete look of the button: a clock view and a month view.
    internal abstract class CalendarStyle
    {
        public abstract String Name { get; }

        // False when the style shows nothing that changes every second, so the button can refresh once a minute.
        public abstract Boolean TicksEverySecond { get; }

        public abstract void DrawDay(Canvas canvas, DayInfo day);

        public abstract void DrawMonth(Canvas canvas, MonthInfo month);

        // The month names are fitted to the width: the full name if it fits, the abbreviation otherwise.
        internal static String FitMonthName(MonthInfo month, Int32 fontSize, Single maxWidth, String suffix = "")
        {
            var full = month.MonthName + suffix;
            return Canvas.Measure(full, fontSize) <= maxWidth ? full : month.MonthShort + suffix;
        }
    }

    internal static class CalendarStyles
    {
        public static readonly CalendarStyle Classic = new ClassicStyle();
        public static readonly CalendarStyle Ring = new RingStyle();
        public static readonly CalendarStyle Minimal = new MinimalStyle();
        public static readonly CalendarStyle Date = new DateStyle();
        public static readonly CalendarStyle Analog = new AnalogStyle();
        public static readonly CalendarStyle Orbit = new OrbitStyle();
        public static readonly CalendarStyle Week = new WeekStyle();
        public static readonly CalendarStyle Stack = new StackStyle();
        public static readonly CalendarStyle Segment = new SegmentStyle();
        public static readonly CalendarStyle Flip = new FlipStyle();
        public static readonly CalendarStyle Binary = new BinaryStyle();
        public static readonly CalendarStyle Moon = new MoonStyle();
    }
}
