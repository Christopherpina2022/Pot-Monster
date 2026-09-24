using FGC_Stat_Analyzer_wpf.Services;
using FGC_Stat_Analyzer_wpf.Views.ChildWindows;
using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class HeaderBar : UserControl
    {
        public HeaderBar()
        {
            InitializeComponent();
        }

        private void apiMenu_Click(object sender, RoutedEventArgs e)
        {
            // Create new window prompting for API Key
            var mainWindow = Window.GetWindow(this);
            var apiWindow = new ApiKeyWindow
            {
                Owner = mainWindow,
            };
            apiWindow.Show();
        }

        private void helpMenu_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new helpWindow();
            helpWindow.Show();
        }
    }
}
