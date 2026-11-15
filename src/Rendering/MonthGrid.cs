namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    internal enum TodayMarker
    {
        Disc,
        Ring,
        Square,
        TextOnly
    }

    // The month grid shared by the styles: the styles differ in the header, the weekday
    // letters, and how today is marked.
    internal static class MonthGrid
    {
        // The standard month view: the month on the left, the year on the right, the weekday
        // letters, and a green disc for today.
        public static void DrawStandard(Canvas c, MonthInfo m, Boolean highlightWeek = false)
        {
            var left = c.X(10);
            var right = c.X(170);
            var font = c.Font(20, 9);
            var year = m.Year.ToString();
            var name = CalendarStyle.FitMonthName(m, font, right - left - Canvas.Measure(year, font) - c.L(10));
            var top = c.Y(12);

            c.Label(name, left, top, font, Palette.White, Align.Left);
            c.Label(year, right, top, font, Palette.Gray, Align.Right);
            MonthGrid.Draw(c, m, 38, true, TodayMarker.Disc, highlightWeek);
        }

        public static void Draw(Canvas canvas, MonthInfo month, Int32 gridTop, Boolean showWeekdays, TodayMarker marker, Boolean highlightWeek = false)
        {
            const Single Left = 6f;
            const Single Right = 174f;
            const Single Bottom = 172f;

            var builder = canvas.Builder;
            var rows = month.Rows;

            // The weekday letters would not leave room for the days on the smallest keys.
            showWeekdays = showWeekdays && canvas.Scale >= 0.4f;
            var weekdaysHeight = showWeekdays ? 16f : 0f;
            var cellsTop = gridTop + weekdaysHeight;
            var colWidth = (Right - Left) / 7f;
            var rowHeight = (Bottom - cellsTop) / rows;
            var fontSize = canvas.Font(Math.Min(rowHeight * 0.8f, colWidth * 0.7f), 7);

            if (showWeekdays)
            {
                var letterFont = canvas.Font(13f, 7);
                for (var col = 0; col < 7; col++)
                {
                    var cx = canvas.X(Left + (colWidth * (col + 0.5f)));
                    canvas.Label(month.WeekdayLetters[col], cx, canvas.Y(gridTop), letterFont, Palette.Gray, Align.Center);
                }
            }

            if (highlightWeek && month.TodayRow >= 0)
            {
                var top = canvas.Y(cellsTop + (rowHeight * month.TodayRow));
                var bottom = canvas.Y(cellsTop + (rowHeight * (month.TodayRow + 1)));
                builder.FillRectangle(canvas.X(Left - 3), top, canvas.X(Right + 3) - canvas.X(Left - 3), bottom - top, Palette.Track);
            }

            for (var day = 1; day <= month.DaysInMonth; day++)
            {
                var index = month.Offset + day - 1;
                var col = index % 7;
                var row = index / 7;
                var cx = canvas.X(Left + (colWidth * (col + 0.5f)));
                var cy = canvas.Y(cellsTop + (rowHeight * (row + 0.5f)));
                var isToday = day == month.Today;

                var color = col >= 5 ? Palette.Gray : Palette.Light;
                var font = fontSize;
                if (isToday)
                {
                    var radius = Math.Max(colWidth, rowHeight) * canvas.Scale / 2f;
                    switch (marker)
                    {
                        case TodayMarker.Disc:
                            builder.FillCircle(cx, cy, radius, Palette.Green);
                            color = Palette.Black;
                            break;
                        case TodayMarker.Ring:
                            builder.DrawArc(cx, cy, (Int32)Math.Round(radius - 1), 0f, 360f, Palette.Green, Math.Max(1.5f, 2.5f * canvas.Scale));
                            color = Palette.White;
                            break;
                        case TodayMarker.Square:
                            builder.FillRectangle((Int32)Math.Round(cx - radius), (Int32)Math.Round(cy - radius), (Int32)Math.Round(2 * radius), (Int32)Math.Round(2 * radius), Palette.Track);
                            color = Palette.Green;
                            break;
                        default:
                            color = Palette.Green;
                            font = fontSize + 1;
                            break;
                    }
                }

                var text = day.ToString();
                var inkTop = cy - (Canvas.Cap(font) / 2);
                canvas.Label(text, cx, inkTop, font, color, Align.Center);
            }
        }
    }
}
