namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Three concentric rings: seconds (green) outside, minutes in the middle, hours inside,
    // with the time in the center.

    internal sealed class OrbitStyle : CalendarStyle
    {
        public override String Name => "Orbit";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var cx = c.X(90);
            var cy = c.Y(90);
            var stroke = Math.Max(3f, 11f * c.Scale);

            var rings = new[]
            {
                (Radius: 77f, Fraction: (d.Second + 1) / 60.0, Color: Palette.Green),
                (Radius: 59f, Fraction: (d.Minute + (d.Second / 60.0)) / 60.0, Color: Palette.Light),
                (Radius: 41f, Fraction: ((d.Hour % 12) + (d.Minute / 60.0)) / 12.0, Color: Palette.White)
            };

            foreach (var ring in rings)
            {
                var radius = c.L(ring.Radius);
                c.Builder.DrawArc(cx, cy, radius, 0f, 360f, Palette.Track, stroke);
                c.Builder.DrawArc(cx, cy, radius, -90f, (Single)(360.0 * ring.Fraction), ring.Color, stroke);
            }

            var inner = (2f * (c.L(41f) - (stroke / 2f))) - c.L(8f);
            var time = d.Suffix.Length > 0 ? d.Time + " " + d.Suffix : d.Time;
            var font = Canvas.FitFont(time, c.Font(34, 9), inner);
            c.Label(time, cx, cy - (Canvas.Cap(font) / 2), font, Palette.White, Align.Center);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => MonthGrid.DrawStandard(c, m);
    }
}
