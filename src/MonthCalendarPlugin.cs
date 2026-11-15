namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // The plugin class. It has no application: the calendar commands work on their own.

    public class MonthCalendarPlugin : Plugin
    {
        // This is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // This is a Universal plugin, not tied to an application.
        public override Boolean HasNoApplication => true;

        public MonthCalendarPlugin()
        {
            PluginLog.Init(this.Log);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }
    }
}
