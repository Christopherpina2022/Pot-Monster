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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
