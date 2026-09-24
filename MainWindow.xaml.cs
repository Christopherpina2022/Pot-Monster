using System.Windows;
using System.Windows.Data;

namespace FGC_Stat_Analyzer_wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            sidebar.Initialize(resultsDataGrid);
        }
    }
}