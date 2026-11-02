namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;

    // Shared logic for the calendar commands: a day/clock view and a month grid view,
    // with the tap toggling between the two. The language, the clock format, and the
    // date format are configured by the derived commands.

    public abstract class CalendarCommandBase : PluginDynamicCommand
    {
        private Int32 _year;
        private Int32 _month;
        private volatile Boolean _isDayView;
        private Boolean _unloaded;
        private Timer _refreshTimer;
        private readonly Object _timerLock = new Object();

        private static CultureInfo _systemCulture;

        protected static readonly BitmapColor BackgroundColor = BitmapColor.FromArgb(0xFF1E1E1E);
        protected static readonly BitmapColor TitleColor = BitmapColor.FromArgb(0xFFFFFFFF);
        protected static readonly BitmapColor WeekdayColor = BitmapColor.FromArgb(0xFF9E9E9E);
        protected static readonly BitmapColor DayColor = BitmapColor.FromArgb(0xFFE0E0E0);
        protected static readonly BitmapColor WeekendColor = BitmapColor.FromArgb(0xFF4FC3F7);
        protected static readonly BitmapColor TodayBackgroundColor = BitmapColor.FromArgb(0xFF3D7EFF);
        protected static readonly BitmapColor TodayTextColor = BitmapColor.White;
        protected static readonly BitmapColor AccentColor = BitmapColor.FromArgb(0xFF4FC3F7);

        protected static readonly String[] UkrainianMonthsGenitive =
        {
            "січня", "лютого", "березня", "квітня", "травня", "червня",
            "липня", "серпня", "вересня", "жовтня", "листопада", "грудня"
        };

        protected static readonly String[] UkrainianWeekdayShort = { "пн", "вт", "ср", "чт", "пт", "сб", "нд" };

        protected CalendarCommandBase(String displayName, String description, String groupName)
            : base(displayName: displayName, description: description, groupName: groupName)
        {
        }

        // The culture used for rendering. The system command follows the operating system
        // language, while localized variants return a fixed culture.
        protected abstract CultureInfo DisplayCulture { get; }

        // True for a 24-hour clock ("18:45:07"), false for a 12-hour clock ("06:45 PM").
        protected abstract Boolean Use24HourClock { get; }

        // The full date shown at the bottom of the day view.
        protected abstract String LongDatePattern { get; }

        // The "weekday, day month" line of the day view, e.g. "пн, 12 серпня" or "Mon, Aug 11".
        protected abstract String FormatWeekdayLine(DateTime now, CultureInfo culture);

        protected override Boolean OnLoad()
        {
            var now = DateTime.Today;
            this._year = now.Year;
            this._month = now.Month;
            this._isDayView = true; // Start with the clock; the calendar is the second view.
            lock (this._timerLock)
            {
                this._unloaded = false;
            }

            this.ScheduleRefresh();
            return base.OnLoad();
        }

        protected override Boolean OnUnload()
        {
            lock (this._timerLock)
            {
                // Set under the lock so a tick that is already running cannot reschedule after unload.
                this._unloaded = true;
                this._refreshTimer?.Dispose();
                this._refreshTimer = null;
            }

            return base.OnUnload();
        }

        protected override void RunCommand(String actionParameter)
        {
            this._isDayView = !this._isDayView;

            // The month grid always opens on the current month.
            var today = DateTime.Today;
            this._year = today.Year;
            this._month = today.Month;
            this.ScheduleRefresh();
            this.ActionImageChanged();
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                return this._isDayView ? this.DrawDay(imageSize) : this.DrawCalendar(imageSize);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Calendar image drawing failed.");
                return this.DrawError(imageSize);
            }
        }

        // The default day-view line in the system culture. Ukrainian uses the genitive
        // form of the month ("12 серпня") and short weekday names.
        protected static String FormatSystemWeekdayLine(DateTime now, CultureInfo culture)
        {
            var isUkrainian = culture.TwoLetterISOLanguageName.Equals("uk", StringComparison.OrdinalIgnoreCase);

            String weekdayShort;
            String dayMonth;
            if (isUkrainian)
            {
                var idx = ((Int32)now.DayOfWeek + 6) % 7; // Monday-based: 0 = Monday ... 6 = Sunday
                weekdayShort = CalendarCommandBase.UkrainianWeekdayShort[idx];
                dayMonth = now.Day.ToString(culture) + " " + CalendarCommandBase.UkrainianMonthsGenitive[now.Month - 1];
            }
            else
            {
                weekdayShort = culture.DateTimeFormat.ShortestDayNames[(Int32)now.DayOfWeek];
                dayMonth = now.Day.ToString(culture) + " " + culture.DateTimeFormat.GetMonthName(now.Month);
            }

            return weekdayShort + ", " + dayMonth;
        }

        // The Logi service runs with an English thread culture regardless of the system
        // language, so the display language is resolved from the operating system itself.
        protected static CultureInfo GetSystemCulture()
        {
            var culture = CalendarCommandBase._systemCulture;
            if (culture != null)
            {
                return culture;
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // On Windows the installed UI culture reflects the system language.
                culture = CultureInfo.InstalledUICulture;
            }
            else
            {
                try
                {
                    var tag = CalendarCommandBase.ReadAppleLanguages();
                    if (!String.IsNullOrWhiteSpace(tag))
                    {
                        culture = new CultureInfo(tag);
                    }
                }
                catch
                {
                    culture = null;
                }

                culture = culture ?? CultureInfo.CurrentCulture;
            }

            // Russian is not supported; show Ukrainian instead.
            if (culture != null && culture.TwoLetterISOLanguageName.Equals("ru", StringComparison.OrdinalIgnoreCase))
            {
                culture = new CultureInfo("uk-UA");
            }

            CalendarCommandBase._systemCulture = culture;
            return CalendarCommandBase._systemCulture;
        }

        private static String ReadAppleLanguages()
        {
            var info = new ProcessStartInfo
            {
                FileName = "/usr/bin/defaults",
                Arguments = "read -g AppleLanguages",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(info))
            {
                var output = process.StandardOutput.ReadToEnd();
                if (!process.WaitForExit(5000))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                    }
                }

                var match = Regex.Match(output ?? String.Empty, "\"([^\"]+)\"");
                return match.Success ? match.Groups[1].Value : String.Empty;
            }
        }

        private BitmapImage DrawError(PluginImageSize imageSize)
        {
            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(CalendarCommandBase.BackgroundColor);
                builder.DrawText(":(", 0, 0, builder.Width, builder.Height, CalendarCommandBase.AccentColor, 12, 12, 0, null);
                return builder.ToImage();
            }
        }

        private BitmapImage DrawDay(PluginImageSize imageSize)
        {
            var now = DateTime.Now;
            var culture = this.DisplayCulture;
            var timePattern = this.Use24HourClock ? "HH:mm:ss" : "hh:mm tt";

            using (var builder = new BitmapBuilder(imageSize))
            {
                var w = builder.Width;
                var h = builder.Height;

                var scale = h / 80.0;
                var infoFont = Math.Max(11, (Int32)Math.Round(12 * scale));
                var dateFont = Math.Max(10, (Int32)Math.Round(11 * scale));

                builder.Clear(CalendarCommandBase.BackgroundColor);

                // Time, e.g. "18:45:07" or "06:45 PM".
                builder.DrawText(
                    now.ToString(timePattern, culture),
                    0,
                    (Int32)(h * 0.04),
                    w,
                    (Int32)(h * 0.36),
                    CalendarCommandBase.TitleColor,
                    26,
                    26,
                    0,
                    null);

                // Subtle separator below the time.
                builder.DrawLine(0, (Int32)(h * 0.42), w, (Int32)(h * 0.42), CalendarCommandBase.WeekdayColor, 1);

                // Short weekday and date, e.g. "пн, 12 серпня" or "Mon, Aug 11".
                builder.DrawText(
                    this.FormatWeekdayLine(now, culture),
                    0,
                    (Int32)(h * 0.46),
                    w,
                    (Int32)(h * 0.16),
                    CalendarCommandBase.AccentColor,
                    infoFont,
                    infoFont + 1,
                    0,
                    null);

                // Full date, e.g. "11.08.2026" or "08/11/2026".
                builder.DrawText(
                    now.ToString(this.LongDatePattern, culture),
                    0,
                    (Int32)(h * 0.70),
                    w,
                    (Int32)(h * 0.16),
                    CalendarCommandBase.DayColor,
                    dateFont,
                    dateFont + 1,
                    0,
                    null);

                return builder.ToImage();
            }
        }

        private BitmapImage DrawCalendar(PluginImageSize imageSize)
        {
            var firstDay = new DateTime(this._year, this._month, 1);
            var daysInMonth = DateTime.DaysInMonth(this._year, this._month);
            var offset = (((Int32)firstDay.DayOfWeek) + 6) % 7; // Monday-based: 0 = Monday ... 6 = Sunday
            var rows = (offset + daysInMonth + 6) / 7;
            var today = DateTime.Today;

            var culture = this.DisplayCulture;
            var weekdayNames = culture.DateTimeFormat.ShortestDayNames;
            // Some cultures (e.g. Ukrainian) return lower-case month names; capitalize for the title.
            var monthName = culture.TextInfo.ToTitleCase(culture.DateTimeFormat.MonthNames[this._month - 1]);

            using (var builder = new BitmapBuilder(imageSize))
            {
                var w = builder.Width;
                var h = builder.Height;

                // Scale the fonts with the image size so the numbers look similar on every screen size.
                var scale = h / 80.0;

                builder.Clear(CalendarCommandBase.BackgroundColor);

                var titleHeight = (Int32)(h * 0.16);
                var weekdayHeight = (Int32)(h * 0.13);
                var gridTop = titleHeight + weekdayHeight;
                var gridBottom = h - 1;
                var rowHeight = (gridBottom - gridTop) / rows;
                var colWidth = w / 7;

                var titleFont = Math.Max(9, (Int32)Math.Round(11 * scale));
                var weekdayFont = Math.Max(6, (Int32)Math.Round(7 * scale));
                // The day number must fit the cell both vertically and horizontally (two-digit days like 28..31).
                var dayFont = Math.Max(8, (Int32)Math.Round(Math.Min(rowHeight, colWidth * 0.78)));
                var todayFont = dayFont + 1;

                // Month and year title.
                builder.DrawText(
                    $"{monthName} {this._year}",
                    0,
                    0,
                    w,
                    titleHeight,
                    CalendarCommandBase.TitleColor,
                    titleFont,
                    titleFont + 1,
                    0,
                    null);

                builder.DrawLine(0, titleHeight, w, titleHeight, CalendarCommandBase.WeekdayColor, 1);

                // Weekday labels. Column 0 is Monday.
                for (var col = 0; col < 7; col++)
                {
                    var name = weekdayNames[(col + 1) % 7];
                    var color = (col == 5 || col == 6) ? CalendarCommandBase.WeekendColor : CalendarCommandBase.WeekdayColor;
                    builder.DrawText(name, col * colWidth, titleHeight, colWidth, weekdayHeight, color, weekdayFont, weekdayFont, 0, null);
                }

                // Day cells.
                for (var i = 0; i < rows * 7; i++)
                {
                    var dayNumber = i - offset + 1;
                    if (dayNumber < 1 || dayNumber > daysInMonth)
                    {
                        continue;
                    }

                    var col = i % 7;
                    var row = i / 7;
                    var x = col * colWidth;
                    var y = gridTop + row * rowHeight;

                    var date = new DateTime(this._year, this._month, dayNumber);
                    var text = dayNumber.ToString(culture);

                    if (date == today)
                    {
                        var cx = x + colWidth / 2;
                        var cy = y + rowHeight / 2;
                        var radius = Math.Min(colWidth, rowHeight) / 2 - 1;
                        builder.FillCircle(cx, cy, radius, CalendarCommandBase.TodayBackgroundColor);
                        builder.DrawText(text, x, y, colWidth, rowHeight, CalendarCommandBase.TodayTextColor, todayFont, todayFont, 0, null);
                    }
                    else
                    {
                        var color = (col == 5 || col == 6) ? CalendarCommandBase.WeekendColor : CalendarCommandBase.DayColor;
                        builder.DrawText(text, x, y, colWidth, rowHeight, color, dayFont, dayFont, 0, null);
                    }
                }

                return builder.ToImage();
            }
        }

        private void ScheduleRefresh()
        {
            lock (this._timerLock)
            {
                if (this._unloaded)
                {
                    return;
                }

                this._refreshTimer?.Dispose();

                // Only the 24-hour clock shows seconds; the 12-hour clock and the month grid
                // change once a minute, so there is no point redrawing them every second.
                var now = DateTime.Now;
                var everySecond = this._isDayView && this.Use24HourClock;
                var delay = everySecond
                    ? now.Date.AddSeconds(Math.Floor(now.TimeOfDay.TotalSeconds) + 1) - now
                    : now.Date.AddMinutes(Math.Floor(now.TimeOfDay.TotalMinutes) + 1) - now;

                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.Zero;
                }

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