namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Globalization;

    // The default command. It follows the operating system language and shows a
    // 24-hour clock.

    public class MonthCalendarCommand : CalendarCommandBase
    {
        public MonthCalendarCommand()
            : this("Classic", "Shows the classic digital clock with seconds, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        protected MonthCalendarCommand(String displayName, String description)
            : base(displayName: displayName, description: description, groupName: "Clock Calendar")
        {
        }

        protected override CultureInfo DisplayCulture => SystemCulture.Get();

        protected override Boolean Use24HourClock => true;

        protected override String LongDatePattern => "dd.MM.yyyy";

        protected override String FormatWeekdayLine(DateTime now, CultureInfo culture) =>
            CalendarCommandBase.FormatSystemWeekdayLine(now, culture);
    }
}
