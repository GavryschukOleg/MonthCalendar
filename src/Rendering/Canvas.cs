namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Collections.Generic;

    internal enum Align
    {
        Left,
        Center,
        Right
    }

    // The shared palette. It matches the HardwareMonitor plugin so both look like one family.
    internal static class Palette
    {
        public static readonly BitmapColor Black = BitmapColor.Black;
        public static readonly BitmapColor White = BitmapColor.White;
        public static readonly BitmapColor Light = new BitmapColor(201, 206, 214);
        public static readonly BitmapColor Gray = new BitmapColor(158, 158, 158);
        public static readonly BitmapColor Track = new BitmapColor(35, 39, 46);
        public static readonly BitmapColor Green = new BitmapColor(51, 196, 76);
    }

    // A drawing surface with a fixed 180 x 180 design grid scaled to the key size, plus
    // text helpers that measure the text, so it can be aligned and fitted precisely
    // (the text renderer wraps long lines instead of clipping them).

    internal sealed class Canvas
    {
        private static readonly Dictionary<Char, Single> Advance = new Dictionary<Char, Single>
        {
            ['0'] = 0.62f, ['1'] = 0.329f, ['2'] = 0.56f, ['3'] = 0.547f, ['4'] = 0.589f,
            ['5'] = 0.54f, ['6'] = 0.556f, ['7'] = 0.504f, ['8'] = 0.571f, ['9'] = 0.556f,
            ['%'] = 0.809f, ['°'] = 0.413f, ['.'] = 0.291f, [':'] = 0.3f, ['/'] = 0.447f,
            ['-'] = 0.404f, [' '] = 0.26f, [','] = 0.27f, ['·'] = 0.29f,
            ['A'] = 0.669f, ['B'] = 0.627f, ['C'] = 0.704f, ['D'] = 0.744f, ['E'] = 0.542f,
            ['F'] = 0.507f, ['G'] = 0.744f, ['H'] = 0.773f, ['I'] = 0.262f, ['J'] = 0.45f,
            ['K'] = 0.602f, ['L'] = 0.518f, ['M'] = 0.847f, ['N'] = 0.773f, ['O'] = 0.8f,
            ['P'] = 0.591f, ['Q'] = 0.804f, ['R'] = 0.62f, ['S'] = 0.524f, ['T'] = 0.511f,
            ['U'] = 0.744f, ['V'] = 0.649f, ['W'] = 0.956f, ['X'] = 0.58f, ['Y'] = 0.6f, ['Z'] = 0.564f,
            ['a'] = 0.591f, ['b'] = 0.591f, ['c'] = 0.52f, ['d'] = 0.591f, ['e'] = 0.533f,
            ['f'] = 0.347f, ['g'] = 0.591f, ['h'] = 0.584f, ['i'] = 0.231f, ['j'] = 0.24f,
            ['k'] = 0.493f, ['l'] = 0.231f, ['m'] = 0.924f, ['n'] = 0.584f, ['o'] = 0.576f,
            ['p'] = 0.591f, ['q'] = 0.591f, ['r'] = 0.344f, ['s'] = 0.427f, ['t'] = 0.373f,
            ['u'] = 0.578f, ['v'] = 0.507f, ['w'] = 0.744f, ['x'] = 0.469f, ['y'] = 0.529f, ['z'] = 0.476f
        };

        private readonly Single _scale;
        private readonly Single _offsetX;
        private readonly Single _offsetY;

        public Canvas(BitmapBuilder builder)
        {
            this.Builder = builder;
            this._scale = Math.Min(builder.Width, builder.Height) / 180f;
            this._offsetX = (builder.Width - (180f * this._scale)) / 2f;
            this._offsetY = (builder.Height - (180f * this._scale)) / 2f;
        }

        public BitmapBuilder Builder { get; }

        public Single Scale => this._scale;

        public Int32 X(Single v) => (Int32)Math.Round(this._offsetX + (v * this._scale));

        public Int32 Y(Single v) => (Int32)Math.Round(this._offsetY + (v * this._scale));

        public Int32 L(Single v) => Math.Max(1, (Int32)Math.Round(v * this._scale));

        public Int32 Font(Single v, Int32 min = 10) => Math.Max(min, (Int32)Math.Round(v * this._scale));

        // The height of capital letters and digits at the given font size.
        public static Int32 Cap(Int32 fontSize) => (Int32)Math.Round(fontSize * 0.75);

        public static Single Measure(String text, Int32 fontSize)
        {
            if (String.IsNullOrEmpty(text))
            {
                return 0f;
            }

            var width = 0f;
            foreach (var ch in text)
            {
                if (!Canvas.Advance.TryGetValue(ch, out var advance))
                {
                    // Letters outside the table (Cyrillic and others): capitals are wider than lowercase.
                    advance = Char.IsUpper(ch) ? 0.68f : 0.58f;
                }

                width += advance;
            }

            return width * fontSize;
        }

        public static Int32 FitFont(String text, Int32 fontSize, Single maxWidth, Int32 minFont = 8)
        {
            while (fontSize > minFont && Canvas.Measure(text, fontSize) > maxWidth)
            {
                fontSize--;
            }

            return fontSize;
        }

        // Draws the text with the top of its capitals at inkTop. x is the left edge, the center,
        // or the right edge, depending on the alignment.
        public void Label(String text, Single x, Int32 inkTop, Int32 fontSize, BitmapColor color, Align align)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            var width = Canvas.Measure(text, fontSize);
            var center = align == Align.Left ? x + (width / 2f) : align == Align.Right ? x - (width / 2f) : x;
            var boxWidth = (Int32)Math.Ceiling(width) + fontSize;
            var left = (Int32)Math.Round(center - (boxWidth / 2f));
            var top = inkTop - (Int32)Math.Round(4.0 + (0.27 * fontSize));
            this.Builder.DrawText(text, left, top, boxWidth, fontSize + 8, color, fontSize, fontSize, 0);
        }

        // A point on a circle; degrees are measured clockwise from 12 o'clock.
        public static (Single X, Single Y) Polar(Single cx, Single cy, Single radius, Double degrees)
        {
            var radians = degrees * Math.PI / 180.0;
            return (cx + (radius * (Single)Math.Sin(radians)), cy - (radius * (Single)Math.Cos(radians)));
        }
    }
}
