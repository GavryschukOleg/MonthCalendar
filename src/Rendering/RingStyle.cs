namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Variant B. Like the "Ring" card of HardwareMonitor: the time sits inside a gauge that
    // fills over the minute, the date is under it, and the title is at the bottom opening.

    internal sealed class RingStyle : CalendarStyle
    {
        public override String Name => "Ring";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var cx = c.X(90);
            var cy = c.Y(86);
            var radius = c.L(62);
            var stroke = Math.Max(3f, 12f * c.Scale);

            c.Builder.DrawArc(cx, cy, radius, 135f, 270f, Palette.Track, stroke);
            c.Builder.DrawArc(cx, cy, radius, 135f, (Single)(270.0 * (d.Second + 1) / 60.0), Palette.Green, stroke);

            var inner = (2f * (radius - (stroke / 2f))) - c.L(26);
            var time = d.Suffix.Length > 0 ? d.Time + " " + d.Suffix : d.Time;
            var timeFont = Canvas.FitFont(time, c.Font(40), inner);
            var dateFont = Canvas.FitFont(d.Date, c.Font(20), inner * 0.9f);
            var gap = c.L(9);
            var top = cy - ((Canvas.Cap(timeFont) + gap + Canvas.Cap(dateFont)) / 2) - c.L(4);

            c.Label(time, cx, top, timeFont, Palette.White, Align.Center);
            c.Label(d.Date, cx, top + Canvas.Cap(timeFont) + gap, dateFont, Palette.Light, Align.Center);

            var header = d.Header.ToUpperInvariant();
            var headerFont = Canvas.FitFont(header, c.Font(20), c.X(166) - c.X(14));
            c.Label(header, cx, c.Y(168) - Canvas.Cap(headerFont), headerFont, Palette.Gray, Align.Center);
        }

        public override void DrawMonth(Canvas c, MonthInfo m)
        {
            var font = c.Font(17);
            var maxWidth = c.X(170) - c.X(10);
            var year = " " + m.Year;
            var title = (CalendarStyle.FitMonthName(m, font, maxWidth - Canvas.Measure(year, font)) + year).ToUpperInvariant();

            c.Label(title, c.X(90), c.Y(12), font, Palette.Gray, Align.Center);
            MonthGrid.Draw(c, m, 38, true, TodayMarker.Ring);
        }
    }
}
