using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;

namespace FGC_Stat_Analyzer_wpf.Views.ChildWindows
{
    public partial class LandingPage : Window
    {
        public LandingPage()
        {
            InitializeComponent();
        }

        private void APIButton_Click(object sender, RoutedEventArgs e)
        {
            var apiKeyWindow = new ApiKeyWindow();
            apiKeyWindow.Show();
            this.Close();
        }

        private void OAuthButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });

            e.Handled = true;
        }
    }
}
