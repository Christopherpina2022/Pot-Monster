using System.Windows;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    public partial class DateSelection : UserControl
    {
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
    }
}
