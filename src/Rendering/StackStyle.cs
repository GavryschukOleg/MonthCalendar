namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Typographic: the hours in white over the minutes in green, as big as the key allows,
    // with the weekday, the day and the month in a narrow column on the right.
    // Nothing changes every second, so the button refreshes once a minute.

    internal sealed class StackStyle : CalendarStyle
    {
        public override String Name => "Stack";

        public override Boolean TicksEverySecond => false;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var left = c.X(14);
            var right = c.X(166);

            var font = Canvas.FitFont(d.Hours.Length > 1 ? d.Hours : "00", c.Font(90), c.X(112) - left);
            var hoursTop = c.Y(16);
            var minutesTop = hoursTop + Canvas.Cap(font) + c.L(14);

            c.Label(d.Hours, left, hoursTop, font, Palette.White, Align.Left);
            c.Label(d.Minutes, left, minutesTop, font, Palette.Green, Align.Left);

            // The right column.
            var small = c.Font(18, 8);
            c.Label(d.Weekday.ToUpperInvariant(), right, hoursTop, Canvas.FitFont(d.Weekday.ToUpperInvariant(), small, right - c.X(120), 7), Palette.Gray, Align.Right);

            var dayFont = Canvas.FitFont(d.Day.ToString(), c.Font(40, 12), right - c.X(120));
            c.Label(d.Day.ToString(), right, hoursTop + Canvas.Cap(small) + c.L(14), dayFont, Palette.White, Align.Right);

            var month = d.MonthShort.ToUpperInvariant();
            c.Label(month, right, hoursTop + Canvas.Cap(small) + c.L(14) + Canvas.Cap(dayFont) + c.L(14), Canvas.FitFont(month, small, right - c.X(120), 7), Palette.Gray, Align.Right);

            if (d.Suffix.Length > 0)
            {
                var suffixFont = Canvas.FitFont(d.Suffix, c.Font(20, 8), right - c.X(120), 7);
                c.Label(d.Suffix, right, minutesTop + Canvas.Cap(font) - Canvas.Cap(suffixFont), suffixFont, Palette.Gray, Align.Right);
            }
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => MonthGrid.DrawStandard(c, m);
    }
}
