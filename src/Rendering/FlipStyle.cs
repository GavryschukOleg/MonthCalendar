namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Flip-clock cards: the hours and the minutes on two dark cards split by a thin line.
    // Nothing changes every second, so the button refreshes once a minute.

    internal sealed class FlipStyle : CalendarStyle
    {
        public override String Name => "Flip";

        public override Boolean TicksEverySecond => false;

        public override void DrawDay(Canvas c, DayInfo d)
        {
            var gap = c.L(8);
            var cardWidth = (c.X(166) - c.X(14) - gap) / 2;
            var cardHeight = c.L(98);
            var top = c.Y(20);
            var lineHeight = Math.Max(1, c.L(2));

            var cards = new[] { (Text: d.Hours, X: c.X(14)), (Text: d.Minutes, X: c.X(14) + cardWidth + gap) };
            var font = Canvas.FitFont("00", c.Font(66), cardWidth - c.L(10), 8);

            foreach (var card in cards)
            {
                c.Builder.FillRectangle(card.X, top, cardWidth, cardHeight, Palette.Track);
                c.Label(card.Text, card.X + (cardWidth / 2f), top + ((cardHeight - Canvas.Cap(font)) / 2), font, Palette.White, Align.Center);
                c.Builder.FillRectangle(card.X, top + ((cardHeight - lineHeight) / 2), cardWidth, lineHeight, Palette.Black);
            }

            var left = c.X(14);
            var width = c.X(166) - left;
            var suffixWidth = d.Suffix.Length > 0 ? Canvas.Measure(d.Suffix, c.Font(18)) + c.L(10) : 0;
            var header = d.Header.ToUpperInvariant();
            var headerFont = Canvas.FitFont(header, c.Font(18), width - suffixWidth);
            var headerTop = top + cardHeight + c.L(18);
            c.Label(header, left, headerTop, headerFont, Palette.Gray, Align.Left);
            if (d.Suffix.Length > 0)
            {
                c.Label(d.Suffix, c.X(166), headerTop, c.Font(18), Palette.Light, Align.Right);
            }
        }

        public override void DrawMonth(Canvas c, MonthInfo m) => CalendarStyles.Date.DrawMonth(c, m);
    }
}
