using FGC_Stat_Analyzer_wpf.Services;
using System.Collections;
using System.Windows.Controls;

namespace FGC_Stat_Analyzer_wpf.Views.UserControls
{
    internal class Top8TableRow
    {
        public string GamerTag { get; set; } = "";
        public int Top8Count { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public int Third { get; set; }
        public int Fourth { get; set; }
        public int Fifth { get; set; }
        public int Sixth { get; set; }
        public int Seventh { get; set; }
        public int Eighth { get; set; }
    }

    internal class HeadcountTableRow
    {
        public string GamerTag { get; set; } = "";
        public int HeadCount { get; set;}
        public string Pronouns { get; set; } = "";
        public string Birthday { get; set; } = "";
    }

    public partial class ResultsDataGrid : UserControl
    {
        private Dictionary<string, IEnumerable> currentResults = new();

        public ResultsDataGrid()
        {
            InitializeComponent();
        }

        public void DisplayTop8Results(Dictionary<string, List<Analytics.Top8Analytics>> results)
        {
            foreach (var group in results) 
            {
                List<Top8TableRow> rows = new();

                foreach (var player in group.Value)
                {
                    rows.Add(new Top8TableRow
                    {
                        GamerTag = player.GamerTag,
                        Top8Count = player.Top8Count,
                        First = player.Placements.GetValueOrDefault(1),
                        Second = player.Placements.GetValueOrDefault(2),
                        Third = player.Placements.GetValueOrDefault(3),
                        Fourth = player.Placements.GetValueOrDefault(4),
                        Fifth = player.Placements.GetValueOrDefault(5),
                        Sixth = player.Placements.GetValueOrDefault(6),
                        Seventh = player.Placements.GetValueOrDefault(7),
                        Eighth = player.Placements.GetValueOrDefault(8)
                    });
                }

                // Insert sorted descending by how many times player was in Top 8
                currentResults[group.Key] = rows.OrderByDescending(x => x.Top8Count).ToList();
            }

            setupTable();
        }

        public void DisplayHeadcountResults(Dictionary<string, List<Analytics.HeadcountAnalytics>> results)
        {
            foreach (var group in results)
            {
                List<HeadcountTableRow> rows = new();

                foreach (var player in group.Value)
                {
                    rows.Add(new HeadcountTableRow
                    {
                        GamerTag= player.GamerTag,
                        HeadCount = player.HeadCount,
                        Pronouns = player.Pronouns,
                        Birthday = player.Birthday
                    });
                }

                // Insert sorted descending by how many times player was in Top 8
                currentResults[group.Key] = rows.OrderByDescending(x => x.HeadCount).ToList();
            }

            setupTable();
        }

        public void setupTable()
        {
            // Clear any data that might be on table already
            resultsTable.ItemsSource = null;
            resultsTable.Columns.Clear();
            filterCombo.ItemsSource = null;

            // Setup the filter Combo box
            filterCombo.ItemsSource = currentResults.Keys;
            string? defaultSelection = currentResults.ContainsKey("Overall") ? "Overall" : currentResults.Keys.FirstOrDefault();

            filterCombo.SelectedItem = defaultSelection;

            if (defaultSelection != null && currentResults.TryGetValue(defaultSelection, out var data))
            {
                resultsTable.ItemsSource = data;
            }
        }

        private void filterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterCombo.SelectedItem is string game && currentResults.TryGetValue(game, out var data))
            {
                resultsTable.ItemsSource = data;
            }
        }
    }
}
