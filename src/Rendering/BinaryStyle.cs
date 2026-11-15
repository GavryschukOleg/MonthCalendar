namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // A binary-coded clock: every digit of the time is a column of dots (8-4-2-1).
    // Hours are white, minutes light, seconds green; the digital time is under the dots.

    internal sealed class BinaryStyle : CalendarStyle
    {
        public override String Name => "Binary";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            // The 12-hour editions show the hour as 1..12, like the digital time under the dots.
            var hour = d.Suffix.Length > 0 ? ((d.Hour + 11) % 12) + 1 : d.Hour;

            // Digit values and how many bits each column needs: tens of hours 2, minutes and seconds 3, units 4.
            var columns = new[]
            {
                (Value: hour / 10, Bits: 2, Color: Palette.White),
                (Value: hour % 10, Bits: 4, Color: Palette.White),
                (Value: d.Minute / 10, Bits: 3, Color: Palette.Light),
                (Value: d.Minute % 10, Bits: 4, Color: Palette.Light),
                (Value: d.Second / 10, Bits: 3, Color: Palette.Green),
                (Value: d.Second % 10, Bits: 4, Color: Palette.Green)
            };

            var pitch = 25f;
            var radius = Math.Max(2.5f, 9f * c.Scale);
            var left = 14f + (pitch / 2f);

            for (var col = 0; col < columns.Length; col++)
            {
                var x = c.X(left + (col * pitch));
                for (var bit = 0; bit < columns[col].Bits; bit++)
                {
                    // Row 0 is the top (weight 8); a column with fewer bits starts lower.
                    var row = 4 - columns[col].Bits + (columns[col].Bits - 1 - bit);
                    var y = c.Y(30f + (row * pitch));
                    var on = (columns[col].Value & (1 << bit)) != 0;
                    c.Builder.FillCircle(x, y, radius, on ? columns[col].Color : Palette.Track);
                }
            }

            var seconds = ":" + d.Second.ToString("00");
            var time = d.Time + seconds;
            var font = Canvas.FitFont(time, c.Font(24), c.X(166) - c.X(14));
            var timeLeft = c.X(90) - (Canvas.Measure(time, font) / 2f);
            var top = c.Y(142);
            c.Label(d.Time, timeLeft, top, font, Palette.White, Align.Left);
            c.Label(seconds, timeLeft + Canvas.Measure(d.Time, font), top, font, Palette.Green, Align.Left);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => CalendarStyles.Ring.DrawMonth(c, m);
    }
}
