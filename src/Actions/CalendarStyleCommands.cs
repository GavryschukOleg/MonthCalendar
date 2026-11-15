namespace Loupedeck.MonthCalendarPlugin
{
    // The other designs. "Classic" and "Classic (AM/PM)" are the MonthCalendarCommand and MonthCalendarUsaCommand
    // classes; every other design comes in a system-language edition (the "Clock Calendar" group)
    // and an AM/PM edition (the "Clock Calendar (AM/PM)" group). The class names are the action
    // identifiers stored in users' profiles, so they must not be renamed.

    public class CalendarRingCommand : MonthCalendarCommand
    {
        public CalendarRingCommand()
            : base("Ring", "Shows the time inside a ring that fills over the minute, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Ring;
    }

    public class CalendarRingUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarRingUsaCommand()
            : base("Ring (AM/PM)", "Shows the time inside a ring that fills over the minute, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Ring;
    }

    public class CalendarMinimalCommand : MonthCalendarCommand
    {
        public CalendarMinimalCommand()
            : base("Minimal", "Shows just the time with a blinking colon, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Minimal;
    }

    public class CalendarMinimalUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarMinimalUsaCommand()
            : base("Minimal (AM/PM)", "Shows just the time with a blinking colon, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Minimal;
    }

    public class CalendarDateCommand : MonthCalendarCommand
    {
        public CalendarDateCommand()
            : base("Date", "Shows the day of the month first with the time under it, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Date;
    }

    public class CalendarDateUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarDateUsaCommand()
            : base("Date (AM/PM)", "Shows the day of the month first with the time under it, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Date;
    }

    public class CalendarAnalogCommand : MonthCalendarCommand
    {
        public CalendarAnalogCommand()
            : base("Analog", "Shows an analog dial with a green seconds hand, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Analog;
    }

    public class CalendarAnalogUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarAnalogUsaCommand()
            : base("Analog (AM/PM)", "Shows an analog dial with a green seconds hand, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Analog;
    }

    public class CalendarOrbitCommand : MonthCalendarCommand
    {
        public CalendarOrbitCommand()
            : base("Orbit", "Shows the time inside three rings for the seconds, minutes and hours, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Orbit;
    }

    public class CalendarOrbitUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarOrbitUsaCommand()
            : base("Orbit (AM/PM)", "Shows the time inside three rings for the seconds, minutes and hours, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Orbit;
    }

    public class CalendarWeekCommand : MonthCalendarCommand
    {
        public CalendarWeekCommand()
            : base("Week", "Shows the time and the current week as a strip of days, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Week;
    }

    public class CalendarWeekUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarWeekUsaCommand()
            : base("Week (AM/PM)", "Shows the time and the current week as a strip of days, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Week;
    }

    public class CalendarStackCommand : MonthCalendarCommand
    {
        public CalendarStackCommand()
            : base("Stack", "Shows the hours over the minutes in big type, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Stack;
    }

    public class CalendarStackUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarStackUsaCommand()
            : base("Stack (AM/PM)", "Shows the hours over the minutes in big type, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Stack;
    }

    public class CalendarSegmentCommand : MonthCalendarCommand
    {
        public CalendarSegmentCommand()
            : base("Segment", "Shows the time on a seven-segment LED display, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Segment;
    }

    public class CalendarSegmentUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarSegmentUsaCommand()
            : base("Segment (AM/PM)", "Shows the time on a seven-segment LED display, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Segment;
    }

    public class CalendarFlipCommand : MonthCalendarCommand
    {
        public CalendarFlipCommand()
            : base("Flip", "Shows the time on flip-clock cards, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Flip;
    }

    public class CalendarFlipUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarFlipUsaCommand()
            : base("Flip (AM/PM)", "Shows the time on flip-clock cards, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Flip;
    }

    public class CalendarBinaryCommand : MonthCalendarCommand
    {
        public CalendarBinaryCommand()
            : base("Binary", "Shows the time as a binary clock of dots, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Binary;
    }

    public class CalendarBinaryUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarBinaryUsaCommand()
            : base("Binary (AM/PM)", "Shows the time as a binary clock of dots, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Binary;
    }

    public class CalendarMoonCommand : MonthCalendarCommand
    {
        public CalendarMoonCommand()
            : base("Moon", "Shows today's moon phase with the time, or the current month, in the system language. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Moon;
    }

    public class CalendarMoonUsaCommand : MonthCalendarUsaCommand
    {
        public CalendarMoonUsaCommand()
            : base("Moon (AM/PM)", "Shows today's moon phase with the time, or the current month, in English with a 12-hour clock (AM/PM) and US dates. Tap to switch between the views.")
        {
        }

        internal override CalendarStyle Style => CalendarStyles.Moon;
    }
}
