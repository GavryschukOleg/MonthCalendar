namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // A seven-segment LED display: lit segments in green over faint "ghost" segments,
    // with a blinking colon.

    internal sealed class SegmentStyle : CalendarStyle
    {
        // Segments a (top), b, c (right), d (bottom), e, f (left), g (middle) as bits 0..6.
        private static readonly Int32[] Digits = { 63, 6, 91, 79, 102, 109, 125, 7, 127, 111 };

        public override String Name => "Segment";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var w = c.L(30);
            var h = c.L(70);
            var t = Math.Max(2, c.L(7));
            var gap = c.L(7);
            var colonWidth = c.L(14);
            var total = (4 * w) + (2 * gap) + colonWidth;
            var left = c.X(90) - (total / 2);
            var top = c.Y(26);

            var hours = d.Hours.PadLeft(2);
            var digits = new[] { hours[0], hours[1], d.Minutes[0], d.Minutes[1] };
            var xs = new[] { left, left + w + gap, left + (2 * w) + gap + colonWidth, left + (3 * w) + (2 * gap) + colonWidth };

            for (var i = 0; i < 4; i++)
            {
                SegmentStyle.DrawDigit(c, xs[i], top, w, h, t, digits[i]);
            }

            // The colon: two squares between the pairs, blinking once a second.
            var colonX = left + (2 * w) + gap + ((colonWidth - t) / 2);
            var colonColor = d.Second % 2 == 0 ? Palette.Green : Palette.Track;
            c.Builder.FillRectangle(colonX, top + (h / 3) - (t / 2), t, t, colonColor);
            c.Builder.FillRectangle(colonX, top + ((2 * h) / 3) - (t / 2), t, t, colonColor);

            var header = d.Header.ToUpperInvariant() + (d.Suffix.Length > 0 ? "  " + d.Suffix : string.Empty);
            var headerFont = Canvas.FitFont(header, c.Font(20), c.X(166) - c.X(14));
            c.Label(header, c.X(90), c.Y(116), headerFont, Palette.Gray, Align.Center);

            var dateFont = Canvas.FitFont(d.Date, c.Font(18), c.X(166) - c.X(14));
            c.Label(d.Date, c.X(90), c.Y(142), dateFont, Palette.Light, Align.Center);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => CalendarStyles.Minimal.DrawMonth(c, m);

        private static void DrawDigit(Canvas c, Int32 x, Int32 y, Int32 w, Int32 h, Int32 t, Char digit)
        {
            var lit = digit >= '0' && digit <= '9' ? SegmentStyle.Digits[digit - '0'] : 0;
            var vertical = (h - (3 * t)) / 2;
            var lowerTop = y + (2 * t) + vertical;

            // A 1 px gap keeps neighboring segments apart, but only where the segments are thick enough to afford it.
            var g = t >= 5 ? 1 : 0;

            // x, y, width, height of each segment, in the order of the bits.
            var segments = new[]
            {
                (x + t + g, y, w - (2 * t) - (2 * g), t),
                (x + w - t, y + t + g, t, vertical - (2 * g)),
                (x + w - t, lowerTop + g, t, y + h - t - lowerTop - (2 * g)),
                (x + t + g, y + h - t, w - (2 * t) - (2 * g), t),
                (x, lowerTop + g, t, y + h - t - lowerTop - (2 * g)),
                (x, y + t + g, t, vertical - (2 * g)),
                (x + t + g, y + t + vertical, w - (2 * t) - (2 * g), t)
            };

            for (var bit = 0; bit < 7; bit++)
            {
                var s = segments[bit];
                c.Builder.FillRectangle(s.Item1, s.Item2, s.Item3, s.Item4, (lit & (1 << bit)) != 0 ? Palette.Green : Palette.Track);
            }
        }
    }
}
