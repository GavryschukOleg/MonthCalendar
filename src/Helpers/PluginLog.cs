namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // A helper class that enables logging from the plugin code.

    internal static class PluginLog
    {
        private static PluginLogFile _pluginLogFile;

        public static void Init(PluginLogFile pluginLogFile)
        {
            pluginLogFile.CheckNullArgument(nameof(pluginLogFile));
            PluginLog._pluginLogFile = pluginLogFile;
        }

        public static void Warning(Exception ex, String text) => PluginLog._pluginLogFile?.Warning(ex, text);

        public static void Error(Exception ex, String text) => PluginLog._pluginLogFile?.Error(ex, text);
    }
}
