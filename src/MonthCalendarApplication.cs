namespace Loupedeck.MonthCalendarPlugin
{
    using System;

    // Required by the Logi Plugin Service: the plugin fails to load without a ClientApplication
    // class, even though this plugin is not linked to any application.

    public class MonthCalendarApplication : ClientApplication
    {
        public MonthCalendarApplication()
        {
        }

        protected override String GetProcessName() => "";

        protected override String GetBundleName() => "";

        public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Unknown;
    }
}
