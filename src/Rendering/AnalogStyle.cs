namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // A classic analog dial: hour and minute hands, a thin green seconds hand, and the date inside.

    internal sealed class AnalogStyle : CalendarStyle
    {
        public override String Name => "Analog";

        public override Boolean TicksEverySecond => true;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var builder = c.Builder;
            var cx = c.X(90);
            var cy = c.Y(90);
            var radius = c.L(80);
            var s = c.Scale;

            builder.DrawArc(cx, cy, radius, 0f, 360f, Palette.Track, Math.Max(1.5f, 3f * s));

            // Hour marks: the quarters are longer and brighter.
            for (var i = 0; i < 12; i++)
            {
                var quarter = i % 3 == 0;
                var outer = radius - (8f * s);
                var inner = outer - ((quarter ? 16f : 9f) * s);
                var a = Canvas.Polar(cx, cy, outer, i * 30);
                var b = Canvas.Polar(cx, cy, inner, i * 30);
                builder.DrawLine(a.X, a.Y, b.X, b.Y, quarter ? Palette.Light : Palette.Gray, Math.Max(1f, (quarter ? 3f : 2f) * s));
            }

            var date = d.Weekday.ToUpperInvariant() + " " + d.Day;
            var dateFont = Canvas.FitFont(date, c.Font(15, 8), 54f * s);
            c.Label(date, cx, cy + c.L(40), dateFont, Palette.Gray, Align.Center);

            var hourAngle = ((d.Hour % 12) + (d.Minute / 60.0)) * 30;
            var minuteAngle = (d.Minute + (d.Second / 60.0)) * 6;
            var secondAngle = d.Second * 6;

            var hand = Canvas.Polar(cx, cy, 46f * s, hourAngle);
            builder.DrawLine(cx, cy, hand.X, hand.Y, Palette.White, Math.Max(2.5f, 7f * s));
            hand = Canvas.Polar(cx, cy, 66f * s, minuteAngle);
            builder.DrawLine(cx, cy, hand.X, hand.Y, Palette.White, Math.Max(2f, 4.5f * s));

            var tail = Canvas.Polar(cx, cy, -14f * s, secondAngle);
            hand = Canvas.Polar(cx, cy, 70f * s, secondAngle);
            builder.DrawLine(tail.X, tail.Y, hand.X, hand.Y, Palette.Green, Math.Max(1f, 2f * s));
            builder.FillCircle(cx, cy, Math.Max(2.5f, 5.5f * s), Palette.Green);
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => CalendarStyles.Ring.DrawMonth(c, m);
    }
}
