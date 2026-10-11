using FGC_Stat_Analyzer_wpf.Services;
using System.Windows;
using System.Windows.Data;

namespace FGC_Stat_Analyzer_wpf
{
    public partial class MainWindow : Window
    {
        ExportManager exportManager = new ExportManager();
        public MainWindow()
        {
            InitializeComponent();
            sidebar.Initialize(resultsDataGrid);
            exportManager.SetResultsDataGrid(resultsDataGrid);
        }
    }
}