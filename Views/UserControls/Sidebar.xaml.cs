using Accessibility;
using FGC_Stat_Analyzer_wpf.Services;
using FGC_Stat_Analyzer_wpf.Views.ChildWindows;
using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class Sidebar : UserControl
    {
        private bool tournamentEntered = false;
        private QueryManager? _queryManager;
        private readonly KeyManager _keyManager;
        private ResultsDataGrid? _resultsDataGrid;
        private CancellationTokenSource? _searchCts;
        public Sidebar()
        {
            
            InitializeComponent();
            _keyManager = new KeyManager();
            InitializeQueryManager();
            
            // Event handler for when API key is changed during runtime
            ApiKeyWindow.ApiKeySaved += ApiKeyWindow_ApiKeySaved;

            optionCombo.Items.Add("Top 8");
            optionCombo.Items.Add("Attendee Headcount");
        }

        private void InitializeQueryManager()
        {
            string? apiKey = _keyManager.GetKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _queryManager = null;
                // Run landing page for first time users
                var landingPage = new LandingPage();

                landingPage.Loaded += (_, _) =>
                {
                    landingPage.Activate();
                    landingPage.Focus();
                };
                landingPage.ShowDialog();
                testLabel.Content = "API Not found, please setup in 'Profile' on the menubar.";
                return;
            }

            _queryManager = new QueryManager();
            testLabel.Content = "API key ready.";
        }

        private void ApiKeyWindow_ApiKeySaved(object? sender, EventArgs e)
        {
            InitializeQueryManager();
        }

        public void Initialize(ResultsDataGrid resultsDataGrid)
        {
            _resultsDataGrid = resultsDataGrid;
            tournamentUrl.ValueChanged += TournamentUrl_ValueChanged;
            startDate.ValueChanged += startDate_ValueChanged;
            endDate.ValueChanged += endDate_ValueChanged;
        }

        private void clearOptionals(bool hidden)
        {
            // adjust visibility of optional form items
            if (hidden)
            {
                startDate.IsEnabled = false;
                startDate.Value = null;

                endDate.IsEnabled = false;
                endDate.Value = null;
            }
            else
            {
                startDate.IsEnabled = true;
                endDate.IsEnabled = true;
            }
        }

        private void btnYTD_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            clearOptionals(true);
        }

        private void btnYTD_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            clearOptionals(false);
        }

        private void startDate_ValueChanged(object sender, EventArgs e)
        {
            // Compare to end date
            if (startDate.Value == null)
                return;

            DateTime maxEndDate = startDate.Value.Value.AddYears(1);
            DateTime today = DateTime.Today;

            if (endDate.Value == null)
            {
                endDate.Value = today.AddDays(6);
            }
            if (endDate.Value.Value > maxEndDate)
            {
                endDate.Value = maxEndDate;
            }
        }

        private void endDate_ValueChanged(object sender, EventArgs e)
        {
            // compare to start date
            if (endDate.Value == null)
                return;

            DateTime minStartDate = endDate.Value.Value.AddYears(-1);
            DateTime today = DateTime.Today;
            int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;

            if (startDate.Value == null)
            {
                startDate.Value = today.AddDays(-daysSinceMonday);
            }
            if (startDate.Value.Value < minStartDate)
            {
                startDate.Value = minStartDate;
            }

        }

        private async void queryButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Fault check for no API key
            string? apiKey = _keyManager.GetKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                testLabel.Content = "Please setup API key before entering.";
                return;
            }

            // check if there was a tournament entered into the search
            if (tournamentEntered == false)
            {
                testLabel.Content = "Please enter a URL before running query.";
                return;
            }
            
            DateTime today = DateTime.Today;
            int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;

            // default date range to current week
            DateTime queryStartDate = today.AddDays(-daysSinceMonday);
            DateTime queryEndDate = today.AddDays(6);

            // Overrides for time value
            if (btnYTD.IsChecked == true)
            {
                queryStartDate = new DateTime(DateTime.Now.Year, 1, 1);
                queryEndDate = DateTime.Now;
            }
            else if (startDate.Value != null && endDate.Value != null)
            {
                queryStartDate = startDate.Value.Value;
                queryEndDate = endDate.Value.Value;
            }

            // TODO: start a scroller to show that the app is running the query in the foreground

            // Run the query
            switch (optionCombo.SelectedItem.ToString())
            {
                case "Top 8":
                    Dictionary<string, List<Analytics.Top8Analytics>> top8Results = await _queryManager.QueryTop8(_queryManager.TournamentList, queryStartDate, queryEndDate);
                    _resultsDataGrid?.DisplayTop8Results(top8Results);
                    break;
                case "Attendee Headcount":
                    Dictionary<string, List<Analytics.HeadcountAnalytics>> HeadcountResults = await _queryManager.QueryHeadcount(_queryManager.TournamentList, queryStartDate, queryEndDate);
                    _resultsDataGrid?.DisplayHeadcountResults(HeadcountResults);
                    break;
            } 
        }

        private async void TournamentUrl_ValueChanged(object? sender, EventArgs e)
        {
            // Check if text box is empty
            if (string.IsNullOrWhiteSpace(tournamentUrl.Value))
            {
                tournamentEntered = false;
                return;
            }

            // Fault check for no API key
            string? apiKey = _keyManager.GetKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                testLabel.Content = "Please setup API key before entering.";
                return;
            }

            // Setup debouncing to delay query
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();

            try
            {
                // delay will be 400 ms
                await Task.Delay(400, _searchCts.Token);

                testLabel.Content = "Testing...";

                // TODO: Determine if entered value is a URL, otherwise start search

                // Run tournament query with submitted information
                try
                {
                    await _queryManager.QueryTournamentOwner(tournamentUrl.Value);
                    int tournamentCount = _queryManager.TournamentList.Count;

                    if (tournamentCount <= 0)
                    {
                        testLabel.Content = "Error: no results were found from tournament lookup.";
                        tournamentEntered = false;
                        return;
                    }
                    testLabel.Content = "Success! Found: " + tournamentCount + " Results.";
                    tournamentEntered = true;
                }
                catch
                {
                    testLabel.Content = "Error: provided value is not correct.";
                    tournamentEntered = false;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
