namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // The moon as it looks today (computed from the date), with the time and the date under it.
    // The moon moves slowly, so the button refreshes once a minute.

    internal sealed class MoonStyle : CalendarStyle
    {
        public override String Name => "Moon";

        public override Boolean TicksEverySecond => false;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var cx = c.X(90);
            var cy = c.Y(58);
            var radius = c.L(42);

            c.Builder.FillCircle(cx, cy, radius, Palette.Track);
            MoonStyle.DrawLitPart(c, cx, cy, radius, d.MoonPhase);

            var time = d.Suffix.Length > 0 ? d.Time + " " + d.Suffix : d.Time;
            var timeFont = Canvas.FitFont(time, c.Font(34), c.X(166) - c.X(14));
            c.Label(time, cx, c.Y(112), timeFont, Palette.White, Align.Center);

            var header = d.Header.ToUpperInvariant();
            var headerFont = Canvas.FitFont(header, c.Font(17), c.X(166) - c.X(14));
            c.Label(header, cx, c.Y(150), headerFont, Palette.Gray, Align.Center);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => CalendarStyles.Week.DrawMonth(c, m);


        // The phase is 0 at the new moon, 0.5 at the full moon. The lit part is on the right while
        // the moon waxes and on the left while it wanes; it is drawn scanline by scanline.
        private static void DrawLitPart(Canvas c, Int32 cx, Int32 cy, Int32 radius, Double phase)
        {
            var k = Math.Cos(2 * Math.PI * phase);
            for (var dy = -radius; dy <= radius; dy++)
            {
                var half = Math.Sqrt(Math.Max(0, ((Double)radius * radius) - ((Double)dy * dy)));
                var from = phase <= 0.5 ? k * half : -half;
                var to = phase <= 0.5 ? half : -k * half;
                var width = (Int32)Math.Round(to - from);
                if (width > 0)
                {
                    c.Builder.FillRectangle((Int32)Math.Round(cx + from), cy + dy, width, 1, Palette.Light);
                }
            }
        }
    }
}
