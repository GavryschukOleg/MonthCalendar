namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Variant D. Date first: the day of the month is the big number, the time is secondary.
    // Nothing changes every second, so the button refreshes once a minute.

    internal sealed class DateStyle : CalendarStyle
    {
        public override String Name => "Date";

        public override Boolean TicksEverySecond => false;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var left = c.X(14);
            var width = c.X(166) - left;
            var day = d.Day.ToString();

            var weekday = d.Weekday.ToUpperInvariant();
            var rest = "· " + d.MonthShort.ToUpperInvariant();
            var lineGap = c.L(6);
            var lineFont = Canvas.FitFont(weekday + " " + rest, c.Font(20), width - lineGap);

            var gap = c.L(4);
            var timeFont = c.Font(30);
            var suffixFont = c.Font(16);
            while (timeFont > 8 && Canvas.Measure(d.Time, timeFont) + (d.Suffix.Length > 0 ? gap + Canvas.Measure(d.Suffix, suffixFont) : 0) > width)
            {
                timeFont--;
                suffixFont = Math.Max(8, Math.Min(suffixFont, timeFont / 2));
            }

            var timeTop = c.Y(166) - Canvas.Cap(timeFont);

            // The day number takes what is left between the top and the two lines under it.
            var dayTop = c.Y(16);
            var dayFont = Canvas.FitFont(day, c.Font(104), width);
            while (dayFont > 8 && dayTop + Canvas.Cap(dayFont) + c.L(14) + Canvas.Cap(lineFont) + c.L(8) > timeTop)
            {
                dayFont--;
            }

            c.Label(day, left, dayTop, dayFont, Palette.White, Align.Left);

            var lineTop = dayTop + Canvas.Cap(dayFont) + c.L(14);
            c.Label(weekday, left, lineTop, lineFont, Palette.Green, Align.Left);
            c.Label(rest, left + Canvas.Measure(weekday, lineFont) + lineGap, lineTop, lineFont, Palette.Gray, Align.Left);

            c.Label(d.Time, left, timeTop, timeFont, Palette.Light, Align.Left);
            if (d.Suffix.Length > 0)
            {
                c.Label(d.Suffix, left + Canvas.Measure(d.Time, timeFont) + gap, timeTop + Canvas.Cap(timeFont) - Canvas.Cap(suffixFont), suffixFont, Palette.Gray, Align.Left);
            }
        }

        public override void DrawMonth(Canvas c, MonthInfo m)
        {
            var left = c.X(10);
            var right = c.X(170);
            var nameFont = c.Font(22);
            var yearFont = c.Font(16, 8);
            var year = m.Year.ToString();
            var name = CalendarStyle.FitMonthName(m, nameFont, right - left - Canvas.Measure(year, yearFont) - c.L(10));
            var top = c.Y(10);

            c.Label(name, left, top, nameFont, Palette.White, Align.Left);
            c.Label(year, right, top + Canvas.Cap(nameFont) - Canvas.Cap(yearFont), yearFont, Palette.Gray, Align.Right);
            MonthGrid.Draw(c, m, 40, false, TodayMarker.Square);
        }
    }
}
