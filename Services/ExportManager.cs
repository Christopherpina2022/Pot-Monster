using FGC_Stat_Analyzer_wpf.Views.UserControls;
using System.Collections;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class ExportManager
    {
        public enum ExportType
        {
            Top8,
            Headcount
        }

        public void ExportCSV(Dictionary<string, System.Collections.IEnumerable> results, ExportType type)
        {
            StringBuilder csv = new();
            foreach (var item in results) 
            {
                string game = item.Key;
                IEnumerable rows = item.Value;

                // Section title
                csv.AppendLine(game);

                // Append Headers
                switch (type)
                {
                    case ExportType.Top8:
                        csv.AppendLine("GamerTag,Top8Count,First,Second,Third,Fourth,Fifth,Sixth,Seventh,Eighth");
                        break;
                    case ExportType.Headcount:
                        csv.AppendLine("Gamertag,Headcount,Pronouns,Birthday");
                        break;
                }

                // Append data
                foreach (var row in rows)
                {
                    switch (type)
                    {
                        case ExportType.Top8 when row is Top8TableRow top8:
                            csv.AppendLine($"{escapeCSV(top8.GamerTag)},{top8.Top8Count},{top8.First},{top8.Second},{top8.Third},{top8.Fourth},{top8.Fifth},{top8.Sixth},{top8.Seventh},{top8.Eighth}");
                            break;

                        case ExportType.Headcount when row is HeadcountTableRow headcount:
                            csv.AppendLine($"{escapeCSV(headcount.GamerTag)},{headcount.HeadCount},{headcount.Pronouns},{headcount.Birthday}");
                            break;
                    }
                }

                // Blank line at end of section
                csv.AppendLine();
            }

            SaveFileDialog dialog = new()
            {
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = ".csv",
                AddExtension = true,
                FileName = "PotMonster_results.csv"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            File.WriteAllText(dialog.FileName, csv.ToString());
        }

        private string escapeCSV(string value)
        {
            // Prevents bugs caused by commas or formatting in a user's gamertag
            if (value.Contains(',') ||
                value.Contains('"') ||
                value.Contains('\n') ||
                value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
