using System.Windows;

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
    }
}
