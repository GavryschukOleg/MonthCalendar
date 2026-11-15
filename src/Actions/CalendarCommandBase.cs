namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Globalization;
    using System.Threading;

    // Shared logic for the calendar commands: a clock/date view and a month grid view,
    // with a tap toggling between the two. The language, the clock format, and the
    // date format are configured by the derived commands; the look is drawn by a CalendarStyle.

    public abstract class CalendarCommandBase : PluginDynamicCommand
    {
        private static readonly String[] UkrainianMonthsShort =
        {
            "січ", "лют", "бер", "кві", "тра", "чер",
            "лип", "сер", "вер", "жов", "лис", "гру"
        };

        private static readonly String[] UkrainianWeekdayShort = { "пн", "вт", "ср", "чт", "пт", "сб", "нд" };

        private readonly Object _timerLock = new Object();

        private Int32 _year;
        private Int32 _month;
        private Boolean _isDayView;
        private Timer _refreshTimer;

        protected CalendarCommandBase(String displayName, String description, String groupName)
            : base(displayName: displayName, description: description, groupName: groupName)
        {
        }

        // The culture used for rendering. The system command follows the operating system
        // language, while localized variants return a fixed culture.
        protected abstract CultureInfo DisplayCulture { get; }

        // True for a 24-hour clock ("18:45"), false for a 12-hour clock ("6:45 PM").
        protected abstract Boolean Use24HourClock { get; }

        // The full date shown in the clock view.
        protected abstract String LongDatePattern { get; }

        // The short "weekday, day month" line, e.g. "пн, 12 сер" or "Mon, Aug 11".
        protected abstract String FormatWeekdayLine(DateTime now, CultureInfo culture);

        // The look of the button. The plain Calendar actions use the Classic design.
        internal virtual CalendarStyle Style => CalendarStyles.Classic;

        protected override Boolean OnLoad()
        {
            var now = DateTime.Today;
            this._year = now.Year;
            this._month = now.Month;
            this._isDayView = true; // Start with the clock; the calendar is the second view.
            this.ScheduleRefresh();
            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            lock (this._timerLock)
            {
                this._refreshTimer?.Dispose();
                this._refreshTimer = null;
            }

            return base.OnUnload();
        }

        protected override void RunCommand(String actionParameter)
        {
            this._isDayView = !this._isDayView;
            this.ScheduleRefresh();
            this.ActionImageChanged();
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (imageSize == PluginImageSize.None)
            {
                return null;
            }

            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(Palette.Black);

                try
                {
                    var canvas = new Canvas(builder);
                    if (this._isDayView)
                    {
                        this.Style.DrawDay(canvas, this.CreateDayInfo());
                    }
                    else
                    {
                        this.Style.DrawMonth(canvas, this.CreateMonthInfo());
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "Calendar image drawing failed.");
                    builder.Clear(Palette.Black);
                    builder.DrawText(":(", 0, 0, builder.Width, builder.Height, Palette.Gray, 12, 12, 0, null);
                }

                return builder.ToImage();
            }
        }

        // The default header line in the system culture. Ukrainian uses the genitive form
        // of the month ("12 лис"); other languages use the abbreviated names of the culture.
        protected static String FormatSystemWeekdayLine(DateTime now, CultureInfo culture) =>
            CalendarCommandBase.WeekdayShort(now, culture) + ", " + now.Day.ToString(culture) + " " + CalendarCommandBase.MonthShort(now.Month, culture);

        private static Boolean IsUkrainian(CultureInfo culture) =>
            culture.TwoLetterISOLanguageName.Equals("uk", StringComparison.OrdinalIgnoreCase);

        private static String WeekdayShort(DateTime date, CultureInfo culture) =>
            CalendarCommandBase.IsUkrainian(culture)
                ? CalendarCommandBase.UkrainianWeekdayShort[((Int32)date.DayOfWeek + 6) % 7]
                : culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek).TrimEnd('.');

        private static String MonthShort(Int32 month, CultureInfo culture) =>
            CalendarCommandBase.IsUkrainian(culture)
                ? CalendarCommandBase.UkrainianMonthsShort[month - 1]
                : culture.DateTimeFormat.GetAbbreviatedMonthName(month).TrimEnd('.');

        private static String Capitalize(String text, CultureInfo culture) =>
            text.Length == 0 ? text : Char.ToUpper(text[0], culture) + text.Substring(1);

        private DayInfo CreateDayInfo()
        {
            var now = DateTime.Now;
            var culture = this.DisplayCulture;
            var use24 = this.Use24HourClock;

            var weekdayColumn = ((Int32)now.DayOfWeek + 6) % 7; // Monday-based: 0 = Monday ... 6 = Sunday
            var monday = now.Date.AddDays(-weekdayColumn);

            var info = new DayInfo
            {
                Hours = now.ToString(use24 ? "HH" : "%h", culture),
                Minutes = now.ToString("mm", culture),
                Suffix = use24 ? String.Empty : now.ToString("tt", culture),
                Header = this.FormatWeekdayLine(now, culture),
                Weekday = CalendarCommandBase.WeekdayShort(now, culture),
                MonthShort = CalendarCommandBase.MonthShort(now.Month, culture),
                Day = now.Day,
                Year = now.Year,
                Date = now.ToString(this.LongDatePattern, culture),
                Hour = now.Hour,
                Minute = now.Minute,
                Second = now.Second,
                WeekToday = weekdayColumn,
                MoonPhase = CalendarCommandBase.MoonPhase(DateTime.UtcNow)
            };

            for (var col = 0; col < 7; col++)
            {
                info.WeekLetters[col] = CalendarCommandBase.WeekdayLetter(culture, col);
                info.WeekDays[col] = monday.AddDays(col).Day;
            }

            return info;
        }

        // The moon phase from the mean synodic month, counted from the new moon of 6 January 2000, 18:14 UTC.
        private static Double MoonPhase(DateTime utc)
        {
            var cycles = (utc - new DateTime(2000, 1, 6, 18, 14, 0, DateTimeKind.Utc)).TotalDays / 29.530588853;
            return cycles - Math.Floor(cycles);
        }

        // The one-letter weekday label; column 0 is Monday (ShortestDayNames starts with Sunday).
        private static String WeekdayLetter(CultureInfo culture, Int32 column) =>
            CalendarCommandBase.Capitalize(culture.DateTimeFormat.ShortestDayNames[(column + 1) % 7], culture).Substring(0, 1);

        private MonthInfo CreateMonthInfo()
        {
            var culture = this.DisplayCulture;
            var firstDay = new DateTime(this._year, this._month, 1);
            var today = DateTime.Today;

            var info = new MonthInfo
            {
                Year = this._year,
                MonthName = CalendarCommandBase.Capitalize(culture.DateTimeFormat.MonthNames[this._month - 1], culture),
                MonthShort = CalendarCommandBase.Capitalize(CalendarCommandBase.MonthShort(this._month, culture), culture),
                Offset = ((Int32)firstDay.DayOfWeek + 6) % 7, // Monday-based: 0 = Monday ... 6 = Sunday
                DaysInMonth = DateTime.DaysInMonth(this._year, this._month),
                Today = today.Year == this._year && today.Month == this._month ? today.Day : 0
            };

            for (var col = 0; col < 7; col++)
            {
                info.WeekdayLetters[col] = CalendarCommandBase.WeekdayLetter(culture, col);
            }

            return info;
        }

        private void ScheduleRefresh()
        {
            // Creating and disposing the timer must be atomic: if two threads (a button
            // tap and a timer callback) race here without a lock, one of the newly
            // created Timer instances can end up nowhere but a local variable for an
            // instant before being overwritten in the field - long enough for the GC to
            // collect it before it ever fires. The clock then silently stops updating
            // until the next tap creates a fresh timer.
            lock (this._timerLock)
            {
                this._refreshTimer?.Dispose();

                var now = DateTime.Now;
                var delay = this._isDayView && this.Style.TicksEverySecond
                    ? now.Date.AddSeconds(Math.Floor(now.TimeOfDay.TotalSeconds) + 1) - now
                    : now.Date.AddMinutes(Math.Floor(now.TimeOfDay.TotalMinutes) + 1) - now;

                this._refreshTimer = new Timer(state => this.OnRefresh(), null, delay, Timeout.InfiniteTimeSpan);
            }
        }

        private void OnRefresh()
        {
            // Never let an exception break the refresh chain: after a suspend/resume
            // (or any other glitch) the timer must keep rescheduling, otherwise the
            // clock freezes and never updates again.
            try
            {
                if (!this._isDayView)
                {
                    var now = DateTime.Today;
                    if (now.Year != this._year || now.Month != this._month)
                    {
                        this._year = now.Year;
                        this._month = now.Month;
                    }
                }

                this.ActionImageChanged();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Calendar refresh failed, will retry.");
            }
            finally
            {
                this.ScheduleRefresh();
            }
        }
    }
}
