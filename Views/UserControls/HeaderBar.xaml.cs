using FGC_Stat_Analyzer_wpf.Services;
using FGC_Stat_Analyzer_wpf.Views.ChildWindows;
using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class HeaderBar : UserControl
    {
        private ResultsDataGrid? _resultsDataGrid;
        private ExportManager? _exportManager;

        public void SetResultsDataGrid(ResultsDataGrid resultsDataGrid)
        {
            // Makes sure that the instance being referenced in this class is the same as the active running instance in main window
            _resultsDataGrid = resultsDataGrid;
            _exportManager = new ExportManager();
        }

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

        private async void oauthMenu_Click(object sender, RoutedEventArgs e)
        {
            Window? window = Window.GetWindow(this);
            var oauth = new StartGgOAuthService(window);
            await oauth.AuthenticateAsync();
        }

        private void exportMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_resultsDataGrid == null || _exportManager == null)
            {
                return;
            }

            var results = _resultsDataGrid.CurrentResults;

            // Run exporter
            _exportManager.ExportCSV(results, _resultsDataGrid.CurrentExportType);
        }
    }
}
