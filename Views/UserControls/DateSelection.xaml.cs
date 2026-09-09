using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class DateSelection : UserControl
    {
        public event EventHandler? ValueChanged;
        public DateTime? Value
        {
            get => dateSelector.SelectedDate;
            set => dateSelector.SelectedDate = value;
        }

        public string Label
        {
            get => dateLabel.Content?.ToString() ?? "";
            set => dateLabel.Content = value;
        }

        public DateSelection()
        {
            InitializeComponent();
        }

        private void dateSelector_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
