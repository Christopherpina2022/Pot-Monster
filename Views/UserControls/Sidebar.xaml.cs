using FGC_Stat_Analyzer_wpf.Services;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Sidebar.xaml
    /// </summary>
    public partial class Sidebar : UserControl
    {
        public Sidebar()
        {
            InitializeComponent();
            optionCombo.Items.Add("Top 8");
            optionCombo.Items.Add("Attendee Headcount");
            optionCombo.Items.Add("Get Attendee Info");
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
            // Setup client
            string? key = new KeyManager().GetKey();
            if (key == null)
            {
                MessageBox.Show("Please setup API key or 0Auth credentials before running query.");
                return;
            }
            var client = new StartGgClient(key);
            GraphQLResult result = new GraphQLResult();

            if (!constructVariables(out var variables, out var error))
            {
                MessageBox.Show(error);
                return;
            }

            // Run the query
            switch (optionCombo.SelectedItem.ToString())
            {
                case "Top 8":
                    result = await client.ExecuteAsync(Queries.TournamentTop8, variables);
                    if (result.Success == false) { MessageBox.Show(result.Json); }
                    return;
                case "Attendee Headcount":
                    result = await client.ExecuteAsync(Queries.TournamentHeadCount, variables);
                    if (result.Success == false) { MessageBox.Show(result.Json); }
                    return;
                case "Get Attendee Info":
                    result = await client.ExecuteAsync(Queries.TournamentGetUser, variables);
                    if (result.Success == false) { MessageBox.Show(result.Json); }
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
