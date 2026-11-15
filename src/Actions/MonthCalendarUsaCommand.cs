namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Globalization;

    // The AM/PM variant of the command. It always uses the en-US culture, a
    // 12-hour clock with AM/PM, and US-style date formatting ("Mon, Aug 11",
    // "08/11/2026").

    public class MonthCalendarUsaCommand : CalendarCommandBase
    {
        private static readonly CultureInfo UsCulture = new CultureInfo("en-US");

        public MonthCalendarUsaCommand()
            : this("Classic (AM/PM)", "Shows the classic digital clock with seconds, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        protected MonthCalendarUsaCommand(String displayName, String description)
            : base(displayName: displayName, description: description, groupName: "Clock Calendar (AM/PM)")
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
