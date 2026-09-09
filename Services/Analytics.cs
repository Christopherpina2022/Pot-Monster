using static FGC_Stat_Analyzer_wpf.Services.Parser;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class Analytics
    {
        public class Top8Analytics
        {
            public string GamerTag { get; set; } = "";
            public int Top8Count { get; set; }
            public Dictionary<int, int> Placements { get; set; } = new();
        }

        public class HeadcountAnalytics
        {
            public string GamerTag { get; set; } = "";
            public int HeadCount { get; set; }
            public string Pronouns { get; set; } = "";
            public string Birthday { get; set; } = "";
        }

        public Dictionary<string, List<Top8Analytics>> BuildTop8Analytics(List<Top8Result> parsedResults)
        {
            // Create temp dictionaries
            Dictionary<string, Top8Analytics> overall = new();
            Dictionary<string, Dictionary<string, Top8Analytics>> byGame = new();

            foreach (Top8Result result in parsedResults)
            {
                string game = result.Game;
                string gamerTag = result.GamerTag;
                int placement = result.Placement;

                // Append overall data
                if (!overall.TryGetValue(gamerTag, out Top8Analytics? player))
                {
                    player = new Top8Analytics
                    {
                        GamerTag = gamerTag,
                    };

                    overall[gamerTag] = player;
                }

                player.Top8Count++;
                player.Placements.TryAdd(placement, 0);
                player.Placements[placement]++;

                // Append data by game
                if (!byGame.TryGetValue(game, out var gamePlayers))
                {
                    gamePlayers = new Dictionary<string, Top8Analytics>();
                    byGame[game] = gamePlayers;
                }

                if (!gamePlayers.TryGetValue(gamerTag, out Top8Analytics? gamePlayer))
                {
                    gamePlayer = new Top8Analytics
                    {
                        GamerTag = gamerTag
                    };

                    gamePlayers[gamerTag] = gamePlayer;
                }

                gamePlayer.Top8Count++;
                gamePlayer.Placements.TryAdd(placement, 0);
                gamePlayer.Placements[placement]++;
            }

            // Construct final result
            Dictionary<string, List<Top8Analytics>> results = new();

            results["Overall"] = overall.Values.ToList();

            foreach (var game in byGame)
            {
                results[game.Key] = game.Value.Values.ToList();
            }

            return results;
        }

        public Dictionary<string, List<HeadcountAnalytics>> BuildHeadcountAnalytics(List<HeadcountResult> parsedResults)
        {
            // Create temp dictionaries
            Dictionary<string, HeadcountAnalytics> overall = new();
            Dictionary<string, Dictionary<string, HeadcountAnalytics>> byGame = new();

            foreach (var result in parsedResults)
            {
                string game = result.Game;
                string gamerTag = result.GamerTag;
                string pronouns = result.Pronouns;
                string birthday = result.Birthday;

                // Append overall data
                if (!overall.TryGetValue(gamerTag, out HeadcountAnalytics? player))
                {
                    player = new HeadcountAnalytics
                    {
                        GamerTag = gamerTag,
                        Pronouns = pronouns,
                        Birthday = birthday
                    };

                    overall[gamerTag] = player;
                }

                player.HeadCount++;

                // Append data by game
                if (!byGame.TryGetValue(game, out var gamePlayers))
                {
                    gamePlayers = new Dictionary<string, HeadcountAnalytics>();
                    byGame[game] = gamePlayers;
                }

                if (!gamePlayers.TryGetValue(gamerTag, out HeadcountAnalytics? gamePlayer))
                {
                    gamePlayer = new HeadcountAnalytics
                    {
                        GamerTag = gamerTag
                    };

                    gamePlayers[gamerTag] = gamePlayer;
                }

                gamePlayer.HeadCount++;
            }

            // Construct final result
            Dictionary<string, List<HeadcountAnalytics>> results = new();

            results["Overall"] = overall.Values.ToList();

            foreach (var game in byGame)
            {
                results[game.Key] = game.Value.Values.ToList();
            }

            return results;
        }
    }
}
