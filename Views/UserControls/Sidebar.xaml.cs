using Accessibility;
using FGC_Stat_Analyzer_wpf.Services;
using FGC_Stat_Analyzer_wpf.Views.ChildWindows;
using System.Drawing.Imaging;
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
            // Initialize starting components
            InitializeComponent();
            _keyManager = new KeyManager();
            InitializeQueryManager();
            
            // Event subscriptions
            ApiKeyWindow.ApiKeySaved += ApiKeyWindow_ApiKeySaved;
            tournamentUrl.TournamentSelected += TournamentUrl_TournamentSelected;

            // Initialize default combobox values
            optionCombo.Items.Add("Top 8");
            optionCombo.Items.Add("Attendee Headcount");
        }

        private void InitializeQueryManager()
        {
            // Start test for API key, if not found, run in landing page
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

        private async void TournamentUrl_TournamentSelected(object? sender, string slug)
        {
            slugSearch(slug);
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
            // Disable input until query completes
            toggleInput(false);
            loadingGif.Visibility = Visibility.Visible;

            // Fault check for no API key
            string? apiKey = _keyManager.GetKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                testLabel.Content = "Please setup API key before entering.";
                loadingGif.Visibility = Visibility.Collapsed;
                toggleInput(true);
                return;
            }

            // check if there was a tournament entered into the search
            if (tournamentEntered == false)
            {
                testLabel.Content = "Please enter a URL before running query.";
                loadingGif.Visibility = Visibility.Collapsed;
                toggleInput(true);
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

            // Run the query
            switch (optionCombo.SelectedItem.ToString())
            {
                case "Top 8":
                    Dictionary<string, List<Analytics.Top8Analytics>> top8Results = await _queryManager.QueryTop8(_queryManager.TournamentList, queryStartDate, queryEndDate);
                    _resultsDataGrid?.DisplayTop8Results(top8Results);
                    loadingGif.Visibility = Visibility.Collapsed;
                    toggleInput(true);
                    break;
                case "Attendee Headcount":
                    Dictionary<string, List<Analytics.HeadcountAnalytics>> HeadcountResults = await _queryManager.QueryHeadcount(_queryManager.TournamentList, queryStartDate, queryEndDate);
                    _resultsDataGrid?.DisplayHeadcountResults(HeadcountResults);
                    loadingGif.Visibility = Visibility.Collapsed;
                    toggleInput(true);
                    break;
            } 
        }

        private void toggleInput(bool isEnabled)
        {
            // Controls functionality of sidebar elements
            optionCombo.IsEnabled = isEnabled;
            tournamentUrl.IsEnabled = isEnabled;
            if (btnYTD.IsChecked != true)
            { 
                startDate.IsEnabled = isEnabled;
                endDate.IsEnabled = isEnabled;
            }
            btnYTD.IsEnabled = isEnabled;
            queryButton.IsEnabled = isEnabled;
            testLabel.IsEnabled = isEnabled;

            // Set other parameters
            if (!isEnabled)
            {
                _resultsDataGrid?.Opacity = .5;
                optionCombo.Opacity = .5;
                btnYTD.Opacity = .5;
            }
            else
            {
                _resultsDataGrid?.Opacity = 1;
                optionCombo.Opacity = 1;
                btnYTD.Opacity = 1;
            }
        }

        private async void TournamentUrl_ValueChanged(object? sender, EventArgs e)
        {
            string tournamentValue = tournamentUrl.Value;

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
                await Task.Delay(700, _searchCts.Token);

                testLabel.Content = "Testing...";

                // Determine if entered value is a tournament URL, otherwise start lookup function
                if (tournamentValue.Contains("start.gg/tournament/", StringComparison.OrdinalIgnoreCase)) 
                {
                    string tournamentSlug = ExtractSlug(tournamentValue);
                    slugSearch(tournamentSlug);
                }
                else
                {
                    lookupSearch(tournamentValue);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private string ExtractSlug(string url)
        {
            // Extract tournament slug from url
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException("Invalid URL.", nameof(url));
            }

            const string tournamentPath = "/tournament/";
            var path = uri.AbsolutePath;
            var start = path.IndexOf(tournamentPath, StringComparison.OrdinalIgnoreCase);

            if (start == -1)
            {
                throw new ArgumentException("Argument does not appear to be a Start.gg tournament URL.", nameof(url));
            }

            start += tournamentPath.Length;

            var end = path.IndexOf('/', start);
            if (end == -1)
            {
                end = path.Length;
            }

            var slug = path[start..end];

            // Check if there is a valid slug
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentException("Tournament URL does not contain a slug.", nameof(url));
            }

            return slug;
        }

        private async void lookupSearch(string tournamentValue)
        {
            List<Parser.NameResult> lookupResult = await _queryManager.QueryTournamentName(tournamentValue);
            // Populate tournament value with list then show popup
            tournamentUrl.populatePopup(lookupResult);
        }

        private async void slugSearch(string tournamentValue)
        {
            // Run tournament query with submitted information
            try
            {
                await _queryManager.QueryTournamentOwner(tournamentValue);
                int tournamentCount = _queryManager.TournamentList.Count;

                if (tournamentCount <= 0)
                {
                    testLabel.Content = "Error: no results were found from tournament lookup.";
                    tournamentEntered = false;
                    return;
                }

                // Set banner based on first result
                var firstTournament = _queryManager.TournamentList.FirstOrDefault();
                _resultsDataGrid.ChangeBanner(firstTournament?.ImageUrl);

                // Confirm results to frontend
                testLabel.Content = "Success! Found: " + tournamentCount + " Results.";
                tournamentEntered = true;
            }
            catch
            {
                testLabel.Content = "Error: provided value is not correct.";
                tournamentEntered = false;
            }
        }

        private void tempColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TEMP function, will be replaced by a manager later
            if (tempColorPicker.SelectedItem is ComboBoxItem item)
            {
                ColorPaletteManager.ApplyPalette(item.Content?.ToString() ?? "");
            }
        }
    }
}
