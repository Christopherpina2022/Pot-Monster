using FGC_Stat_Analyzer_wpf.Services;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Views.ChildWindows
{
    public partial class ApiKeyWindow : Window
    {
        KeyManager keyManager = new KeyManager();
        public static event EventHandler? ApiKeySaved;
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

            GraphQLResult result = await client.ExecuteAsync(Queries.SmokeTest, null);

            if (result.Success)
            {
                statusText.Text = "API Key Succeeded!";
                keyManager.SaveKey(apiBox.Password);

                ApiKeySaved?.Invoke(this, EventArgs.Empty);
            }
            else 
            {
                statusText.Text = "API Key Failed!";
            }
        }
    }
}
