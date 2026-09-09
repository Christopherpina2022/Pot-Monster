using Accessibility;
using FGC_Stat_Analyzer_wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class Sidebar : UserControl
    {
        private bool tournamentEntered = false;
        private readonly QueryManager _queryManager;
        private ResultsDataGrid? _resultsDataGrid;

        public Sidebar()
        {
            InitializeComponent();
            _queryManager = new QueryManager();

            optionCombo.Items.Add("Top 8");
            optionCombo.Items.Add("Attendee Headcount");
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
            if (hidden)
            {
                //perPage.IsEnabled = false;
                //perPage.txtInput.Clear();

                startDate.IsEnabled = false;
                startDate.Value = null;

                endDate.IsEnabled = false;
                endDate.Value = null;
            }
            else
            {
                //perPage.IsEnabled = true;
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

        private void optionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (optionCombo.SelectedItem.ToString() == "Get Attendee Info")
            {
                clearOptionals(true);
            }
            else if (btnYTD.IsChecked == true)
            {
                return;
            }
            else { clearOptionals(false); }
        }

        private async void queryButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
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

            // Override will only apply if both dates are filled in form
            if (startDate.Value != null && endDate.Value != null)
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
                    break;
                case "Attendee Headcount":
                    Dictionary<string, List<Analytics.HeadcountAnalytics>> HeadcountResults = await _queryManager.QueryHeadcount(_queryManager.TournamentList, queryStartDate, queryEndDate);
                    _resultsDataGrid?.DisplayHeadcountResults(HeadcountResults);
                    break;
            } 
            
        }

        private async void TournamentUrl_ValueChanged(object? sender, EventArgs e)
        {
            testLabel.Content = "Testing...";

            // Check if box is empty
            if (string.IsNullOrWhiteSpace(tournamentUrl.Value))
            {
                testLabel.Content = "Error: Please enter a URL.";
                tournamentEntered = false;
                return;
            }

            // Run tournament query with submitted information
            try
            {
                await _queryManager.QueryTournamentOwner(tournamentUrl.Value);
                int tournamentCount = _queryManager.TournamentList.Count;

                if (tournamentCount <=0)
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
                testLabel.Content = "Error: Invalid URL.";
                tournamentEntered = false;
            }
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
    }
}
