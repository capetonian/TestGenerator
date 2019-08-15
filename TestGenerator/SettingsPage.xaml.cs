using System.Windows.Controls;

namespace TestGenerator
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private readonly Settings _settings;

        public SettingsPage(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            TestFramework.Text = _settings.TestFramework;
        }

        private void Framework_Changed(object sender, TextChangedEventArgs e)
        {
            _settings.TestFramework = TestFramework.Text;
        }
    }
}
