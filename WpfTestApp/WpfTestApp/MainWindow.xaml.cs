using System.Windows;
using WpfTestApp.Configuration;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings _settings;

        public MainWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            DisplaySettings();
        }

        private void DisplaySettings()
        {
            if (_settings is not null && _settings.BearinxFolder != string.Empty)
            {
                textBox.Text = _settings.BearinxFolder;
            }
        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeSettings();
        }

        private void ChangeSettings()
        {
            _settings.BearinxFolder = "Oh, that's nice...!";
            DisplaySettings();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplaySettings();
        }

        private void DefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_settings.TrySetDefault(nameof(Settings.BearinxFolder)))
            {
                textBox.Text = _settings.BearinxFolder;
            }
            DisplaySettings();
        }
    }
}