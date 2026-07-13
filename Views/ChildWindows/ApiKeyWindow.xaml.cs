using FGC_Stat_Analyzer_wpf.Services;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Views.ChildWindows
{
    public partial class ApiKeyWindow : Window
    {
        KeyManager keyManager = new KeyManager();
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
            var variables = new Dictionary<string, object?>();

            GraphQLResult result = await client.ExecuteAsync(Queries.SmokeTest, variables, false);

            if (result.Success)
            {
                statusText.Text = "API Key Succeeded!";
                keyManager.SaveKey(apiBox.Password);
            }
            else 
            {
                statusText.Text = "API Key Failed!";
            }
        }
    }
}
