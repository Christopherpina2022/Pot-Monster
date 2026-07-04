using FGC_Stat_Analyzer_wpf.Services;
using System.Security;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Views.ChildWindows
{
    public partial class ApiKeyWindow : Window
    {
        public ApiKeyWindow()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Run smoke test with API key before saving to computer
            statusText.Text = "Testing API Key...";

            var client = new StartGgClient(apiBox.Password);

            GraphQLResult result = await client.ExecuteAsync(Queries.SmokeTest);

            if (result.Success)
            {
                statusText.Text = "API Key Succeeded!";
            }
            else 
            {
                statusText.Text = result.ErrorMessage;
            }
        }
    }
}
