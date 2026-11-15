namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Variant C. The most reserved one: just the time with a blinking colon and a quiet
    // date line; the month is a bare grid with today in green.

    internal sealed class MinimalStyle : CalendarStyle
    {
        public override String Name => "Minimal";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var cx = c.X(90);
            var width = c.X(166) - c.X(14);
            var gap = c.L(4);

            var timeFont = c.Font(64);
            var suffixFont = c.Font(24);
            while (timeFont > 8 && Canvas.Measure(d.Time, timeFont) + (d.Suffix.Length > 0 ? gap + Canvas.Measure(d.Suffix, suffixFont) : 0) > width)
            {
                timeFont--;
                suffixFont = Math.Max(8, Math.Min(suffixFont, timeFont / 2));
            }

            var header = d.Header.ToUpperInvariant();
            var headerFont = Canvas.FitFont(header, c.Font(20), width);
            var spacing = c.L(22);
            var total = Canvas.Cap(timeFont) + spacing + Canvas.Cap(headerFont);
            var timeTop = c.Y(90) - (total / 2);

            var timeWidth = Canvas.Measure(d.Time, timeFont) + (d.Suffix.Length > 0 ? gap + Canvas.Measure(d.Suffix, suffixFont) : 0);
            var left = cx - (timeWidth / 2f);
            var colon = left + Canvas.Measure(d.Hours, timeFont);

            // The colon blinks once a second; it keeps its place so the digits never move.
            c.Label(d.Hours, left, timeTop, timeFont, Palette.White, Align.Left);
            c.Label(":", colon, timeTop, timeFont, d.Second % 2 == 0 ? Palette.Green : Palette.Track, Align.Left);
            c.Label(d.Minutes, colon + Canvas.Measure(":", timeFont), timeTop, timeFont, Palette.White, Align.Left);
            if (d.Suffix.Length > 0)
            {
                var suffixLeft = left + Canvas.Measure(d.Time, timeFont) + gap;
                c.Label(d.Suffix, suffixLeft, timeTop + Canvas.Cap(timeFont) - Canvas.Cap(suffixFont), suffixFont, Palette.Gray, Align.Left);
            }

            c.Label(header, cx, timeTop + Canvas.Cap(timeFont) + spacing, headerFont, Palette.Gray, Align.Center);
        }

        public override void DrawMonth(Canvas c, MonthInfo m)
        {
            var font = c.Font(16);
            var year = " " + m.Year;
            var title = CalendarStyle.FitMonthName(m, font, c.X(170) - c.X(10) - Canvas.Measure(year, font)) + year;

            c.Label(title, c.X(90), c.Y(12), font, Palette.Gray, Align.Center);
            MonthGrid.Draw(c, m, 36, false, TodayMarker.TextOnly);
        }
    }
}
