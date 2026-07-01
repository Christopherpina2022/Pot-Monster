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
                startDate.txtInput.Clear();

                endDate.IsEnabled = false;
                endDate.txtInput.Clear();
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
    }
}
