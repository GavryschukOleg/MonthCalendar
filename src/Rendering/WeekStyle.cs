namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // The time, and the current week as a strip of seven days with today in green.
    // In the month view the current week is highlighted as a row.

    internal sealed class WeekStyle : CalendarStyle
    {
        public override String Name => "Week";

        public override Boolean TicksEverySecond => false;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var left = c.X(14);
            var right = c.X(166);
            var width = right - left;
            var gap = c.L(4);

            var timeFont = c.Font(54);
            var suffixFont = c.Font(20);
            while (timeFont > 8 && Canvas.Measure(d.Time, timeFont) + (d.Suffix.Length > 0 ? gap + Canvas.Measure(d.Suffix, suffixFont) : 0) > width)
            {
                timeFont--;
                suffixFont = Math.Max(8, Math.Min(suffixFont, timeFont / 2));
            }

            var timeTop = c.Y(14);
            c.Label(d.Time, left, timeTop, timeFont, Palette.White, Align.Left);
            if (d.Suffix.Length > 0)
            {
                c.Label(d.Suffix, left + Canvas.Measure(d.Time, timeFont) + gap, timeTop + Canvas.Cap(timeFont) - Canvas.Cap(suffixFont), suffixFont, Palette.Gray, Align.Left);
            }

            // The week strip.
            var letterFont = c.Font(13, 8);
            var column = width / 7f;
            var dayFont = Canvas.FitFont("00", c.Font(18, 7), column * 0.9f, 7);
            var letterTop = timeTop + Canvas.Cap(timeFont) + c.L(20);
            var dayTop = letterTop + Canvas.Cap(letterFont) + c.L(11);

            for (var i = 0; i < 7; i++)
            {
                var x = left + (column * (i + 0.5f));
                var isToday = i == d.WeekToday;

                c.Label(d.WeekLetters[i], x, letterTop, letterFont, Palette.Gray, Align.Center);
                if (isToday)
                {
                    c.Builder.FillCircle(x, dayTop + (Canvas.Cap(dayFont) / 2f), Math.Max(column / 2f, Canvas.Cap(dayFont) * 0.75f), Palette.Green);
                }

                c.Label(d.WeekDays[i].ToString(), x, dayTop, dayFont, isToday ? Palette.Black : (i >= 5 ? Palette.Gray : Palette.Light), Align.Center);
            }

            var month = (d.MonthShort + " " + d.Year).ToUpperInvariant();
            var monthFont = Canvas.FitFont(month, c.Font(16, 9), width);
            c.Label(month, left, c.Y(166) - Canvas.Cap(monthFont), monthFont, Palette.Gray, Align.Left);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => MonthGrid.DrawStandard(c, m, highlightWeek: true);
    }
}
