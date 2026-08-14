namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Globalization;

    // The default command. It follows the operating system language and shows a
    // 24-hour clock.

    public class MonthCalendarCommand : CalendarCommandBase
    {
        public MonthCalendarCommand()
            : base(displayName: "Calendar", description: "Shows the current month or the current day in the system language. Tap to switch between the views.", groupName: "Calendar")
        {
        }

        protected override CultureInfo DisplayCulture => CalendarCommandBase.GetSystemCulture();

        protected override Boolean Use24HourClock => true;

        protected override String LongDatePattern => "dd.MM.yyyy";

        protected override String FormatWeekdayLine(DateTime now, CultureInfo culture) =>
            CalendarCommandBase.FormatSystemWeekdayLine(now, culture);
    }
}