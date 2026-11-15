namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // The classic digital clock: the weekday and date line, the time with seconds, the full date.

    internal sealed class ClassicStyle : CalendarStyle
    {
        public override String Name => "Classic";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var cx = c.X(90);
            var width = c.X(166) - c.X(14);

            var header = d.Header.ToUpperInvariant();
            var headerFont = Canvas.FitFont(header, c.Font(20), width);
            c.Label(header, cx, c.Y(18), headerFont, Palette.Gray, Align.Center);

            // The hours and minutes in white, the seconds in green, with the AM/PM mark when there is one.
            var seconds = ":" + d.Second.ToString("00");
            var gap = c.L(4);
            var timeFont = c.Font(42);
            var suffixFont = c.Font(18);
            while (timeFont > 8 && Canvas.Measure(d.Time + seconds, timeFont) + (d.Suffix.Length > 0 ? gap + Canvas.Measure(d.Suffix, suffixFont) : 0) > width)
            {
                timeFont--;
                suffixFont = Math.Max(8, Math.Min(suffixFont, timeFont / 2));
            }

            var total = Canvas.Measure(d.Time + seconds, timeFont) + (d.Suffix.Length > 0 ? gap + Canvas.Measure(d.Suffix, suffixFont) : 0);
            var left = cx - (total / 2f);
            var timeTop = c.Y(64);
            c.Label(d.Time, left, timeTop, timeFont, Palette.White, Align.Left);
            c.Label(seconds, left + Canvas.Measure(d.Time, timeFont), timeTop, timeFont, Palette.Green, Align.Left);
            if (d.Suffix.Length > 0)
            {
                var suffixLeft = left + Canvas.Measure(d.Time + seconds, timeFont) + gap;
                c.Label(d.Suffix, suffixLeft, timeTop + Canvas.Cap(timeFont) - Canvas.Cap(suffixFont), suffixFont, Palette.Gray, Align.Left);
            }

            var dateFont = Canvas.FitFont(d.Date, c.Font(22), width);
            c.Label(d.Date, cx, c.Y(128), dateFont, Palette.Light, Align.Center);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => MonthGrid.DrawStandard(c, m);
    }
}
