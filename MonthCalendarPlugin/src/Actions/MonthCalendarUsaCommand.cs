namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Globalization;

    // The US variant of the calendar command. It always uses the en-US culture, a
    // 12-hour clock with AM/PM, and US-style date formatting ("Mon, Aug 11",
    // "08/11/2026").

    public class MonthCalendarUsaCommand : CalendarCommandBase
    {
        private static readonly CultureInfo UsCulture = new CultureInfo("en-US");

        public MonthCalendarUsaCommand()
            : base(displayName: "Calendar (USA)", description: "Shows the current month or the current day in US format with a 12-hour clock (AM/PM). Tap to switch between the views.", groupName: "Calendar")
        {
        }

        protected override CultureInfo DisplayCulture => MonthCalendarUsaCommand.UsCulture;

        protected override Boolean Use24HourClock => false;

        protected override String LongDatePattern => "MM/dd/yyyy";

        protected override String FormatWeekdayLine(DateTime now, CultureInfo culture) =>
            culture.DateTimeFormat.GetAbbreviatedDayName(now.DayOfWeek) + ", " +
            culture.DateTimeFormat.GetAbbreviatedMonthName(now.Month) + " " +
            now.Day.ToString(culture);
    }
}