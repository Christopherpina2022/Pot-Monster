using Accessibility;
using FGC_Stat_Analyzer_wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class Sidebar : UserControl
    {
        private readonly QueryManager _queryManager;
        private ResultsDataGrid? _resultsDataGrid;

        public Sidebar()
        {
            InitializeComponent();

            _queryManager = new QueryManager();

            optionCombo.Items.Add("Top 8");
            optionCombo.Items.Add("Attendee Headcount");
            optionCombo.Items.Add("Get Attendee Info");
        }

        public void Initialize(ResultsDataGrid resultsDataGrid)
        {
            _resultsDataGrid = resultsDataGrid;
        }

        private void clearOptionals(bool hidden)
        {
            if (hidden)
            {
                perPage.IsEnabled = false;
                perPage.txtInput.Clear();

                startDate.IsEnabled = false;
                startDate.Value = null;

                endDate.IsEnabled = false;
                endDate.Value = null;
            }
            else
            {
                perPage.IsEnabled = true;
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
            // Construct variables, if any
            if (!constructVariables(out var variables, out var error))
            {
                MessageBox.Show(error);
            }

            // Run the query
            switch (optionCombo.SelectedItem.ToString())
            {
                case "Top 8":
                    var top8Results = await _queryManager.QueryTop8(variables);
                    _resultsDataGrid?.DisplayTop8Results(top8Results);
                    break;
                case "Attendee Headcount":
                    var HeadcountResults = await _queryManager.QueryHeadcount(variables);
                    _resultsDataGrid?.DisplayHeadcountResults(HeadcountResults);
                    break;
                case "Get Attendee Info":
                    return;
            } 
            
        }

        private bool constructVariables(out Dictionary<string, object?> variables, out string error)
        {
            variables = new Dictionary<string, object?>();

            // Data validation
            if (string.IsNullOrEmpty(tournamentName.Value)) {
                error = "tournament name is required.";
                return false;
            }

            if (string.IsNullOrEmpty(stateCode.Value)) {
                error = "State code is required.";
                return false;
            }

            // Assign values to variables
            variables["tournamentName"] = tournamentName.Value;
            variables["stateCode"] = stateCode.Value;
            variables["perPage"] = 25;

            // Datetime values need to convert to Unix time for GraphQL
            if (btnYTD.IsChecked == true)
            {
                DateTimeOffset queryStartDate = new DateTimeOffset(DateTime.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                DateTimeOffset queryEndDate = new DateTimeOffset(DateTime.Now.Date);
                variables["startDate"] = queryStartDate.ToUnixTimeSeconds();
                variables["endDate"] = queryEndDate.ToUnixTimeSeconds();
            }
            else
            {
                variables["startDate"] = startDate.Value.HasValue ? new DateTimeOffset(startDate.Value.Value.ToUniversalTime()).ToUnixTimeSeconds() : null;
                variables["endDate"] = endDate.Value.HasValue ? new DateTimeOffset(endDate.Value.Value.ToUniversalTime()).ToUnixTimeSeconds() : null;
            }

            error = "";
            return true;
        }
    }
}
