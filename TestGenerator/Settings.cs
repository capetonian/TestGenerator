using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace TestGenerator
{
    [Guid("00000000-0000-0000-0000-000000000000")]
    public sealed class Settings : UIElementDialogPage
    {
        private SettingsPage _settingsPage;

        public string TestFramework { get; set; } = "Default";

        protected override UIElement Child => _settingsPage ?? (_settingsPage = new SettingsPage(this));

        protected override void OnApply(PageApplyEventArgs args)
        {
            if (args.ApplyBehavior == ApplyKind.Apply)
            {
                // TODO Update the setting
            }

            base.OnApply(args);
        }
    }
}
