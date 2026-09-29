using FGC_Stat_Analyzer_wpf.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class TextInput : UserControl
    {
        public event EventHandler? ValueChanged;
        public event EventHandler<string>? TournamentSelected;
        public string Value
        {
            get => txtInput.Text; set => txtInput.Text = value;
        }
        public TextInput()
        {
            InitializeComponent();
        }

        private System.Windows.Media.Brush placeholderColor = System.Windows.Media.Brushes.LightGray;

        public System.Windows.Media.Brush PlaceholderColor
        {
            get { return placeholderColor; }
            set { placeholderColor = value; tbPlaceholder.Foreground = placeholderColor;}
        }

        private string placeholder = string.Empty;

        public string Placeholder
        {
            get { return placeholder; }
            set { placeholder = value; tbPlaceholder.Text = placeholder;}
        }

        public void populatePopup(List<Parser.NameResult> tournaments)
        {
            tournamentListBox.ItemsSource = tournaments;
            searchPopup.IsOpen = true;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Collapsed;
            }

            // Expose event
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void tournamentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tournamentListBox.SelectedItem is not Parser.NameResult tournament)
            {
                return;
            }

            // hide popup first then expose event
            searchPopup.IsOpen = false;
            TournamentSelected?.Invoke(this, tournament.Slug);
            tournamentListBox.SelectedItem = null;
        }
    }
}
