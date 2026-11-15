namespace Loupedeck.MonthCalendarPlugin
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;

    // Resolves the display language from the operating system. The Logi service runs with
    // an English thread culture regardless of the system language, so CurrentCulture is useless here.

    internal static class SystemCulture
    {
        private static CultureInfo _cached;

        public static CultureInfo Get()
        {
            var culture = SystemCulture._cached;
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
                    var tag = SystemCulture.ReadAppleLanguages();
                    if (!String.IsNullOrWhiteSpace(tag))
                    {
                        culture = new CultureInfo(tag);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Warning(ex, "Could not read the macOS system language.");
                }

                culture = culture ?? CultureInfo.CurrentCulture;
            }

            // Russian is not supported; show Ukrainian instead.
            if (culture.TwoLetterISOLanguageName.Equals("ru", StringComparison.OrdinalIgnoreCase))
            {
                culture = new CultureInfo("uk-UA");
            }

            SystemCulture._cached = culture;
            return culture;
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
                    catch (Exception)
                    {
                        // The process may have exited in the meantime.
                    }
                }

                var match = Regex.Match(output ?? String.Empty, "\"([^\"]+)\"");
                return match.Success ? match.Groups[1].Value : String.Empty;
            }
        }
    }
}
