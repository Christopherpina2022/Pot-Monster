using System.Text.Json;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class Parser
    {
        public class Top8Result
        {
            public string Game { get; set; } = "";
            public string GamerTag { get; set; } = "";
            public int Placement { get; set; }
        }

        public class HeadcountResult
        {
            public string Game { get; set; } = "";
            public string GamerTag { get; set; } = "";
        }

        public class AttendeeResult
        {
            public string GamerTag { get; set; } = "";
            public string Pronouns { get; set; } = "";
            public string Birthday { get; set; } = "";
        }

        public List<Top8Result> ParseTop8(List<string> pages)
        {
            List<Top8Result> results = new List<Top8Result>();

            // Parse data
            foreach (string page in pages)
            {
                using JsonDocument document = JsonDocument.Parse(page);

                // Read through JSON
                JsonElement tournaments = document.RootElement.GetProperty("data").GetProperty("tournaments").GetProperty("nodes");
                foreach (JsonElement tournament in tournaments.EnumerateArray())
                {
                    JsonElement events = tournament.GetProperty("events");
                    foreach (JsonElement evt in events.EnumerateArray())
                    {
                        string game = evt.GetProperty("videogame").GetProperty("name").GetString()!;
                        JsonElement standings = evt.GetProperty("standings").GetProperty("nodes");
                        foreach (JsonElement standing in standings.EnumerateArray())
                        {
                            // Safety catch in case player property is null
                            if (!standing.TryGetProperty("player", out JsonElement player) || player.ValueKind == JsonValueKind.Null)
                            {
                                continue;
                            }

                            string gamerTag = standing.GetProperty("player").GetProperty("gamerTag").GetString() ?? "Unknown";
                            int placement = standing.GetProperty("standing").GetInt32();

                            results.Add(new Top8Result
                            {
                                Game = game,
                                GamerTag = gamerTag,
                                Placement = placement
                            });
                        }
                    }
                }
            }
            return results;
        }

        public List<HeadcountResult> ParseHeadcount(List<string> pages)
        {
            List<HeadcountResult> results = new List<HeadcountResult>();

            // Parse Data
            foreach (string page in pages) 
            {
                using JsonDocument document = JsonDocument.Parse(page);

                // Read through JSON
                JsonElement tournaments = document.RootElement.GetProperty("data").GetProperty("tournaments").GetProperty("nodes");

                foreach (JsonElement tournament in tournaments.EnumerateArray())
                {
                    JsonElement events = tournament.GetProperty("events");
                    foreach (JsonElement evt in events.EnumerateArray())
                    {
                        string game = evt.GetProperty("videogame").GetProperty("name").GetString()!;
                        JsonElement entrants = evt.GetProperty("entrants").GetProperty("nodes");
                        foreach(JsonElement entrant in entrants.EnumerateArray())
                        {
                            // Safely catch in case participants property is null
                            if (!entrant.TryGetProperty("participants", out JsonElement participants) || participants.ValueKind == JsonValueKind.Null)
                            {
                                continue;
                            }

                            foreach (JsonElement participant in participants.EnumerateArray())
                            {
                                string gamerTag = participant.GetProperty("gamerTag").GetString() ?? "Unknown";

                                results.Add(new HeadcountResult
                                {
                                    Game = game,
                                    GamerTag = gamerTag
                                });
                            }
                        }
                    }
                }
            }

            return results;
        }

        public Dictionary<string, List<AttendeeResult>> ParseAttendees(List<string> pages)
        {
            Dictionary<string, List<AttendeeResult>> results = new Dictionary<string, List<AttendeeResult>>();
            results["Overall"] = new List<AttendeeResult>();

            // Parse Data
            foreach (string page in pages)
            {
                using JsonDocument document = JsonDocument.Parse(page);

                // Read through JSON
                JsonElement tournaments = document.RootElement.GetProperty("data").GetProperty("tournaments").GetProperty("nodes");
                if (tournaments.GetArrayLength() == 0)
                {
                    return results;
                }

                // Only need the first result since query returns one value
                JsonElement tournament = tournaments[0];
                if(!tournament.TryGetProperty("events", out JsonElement events) || events.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement evt in events.EnumerateArray())
                {
                    JsonElement entrants = evt.GetProperty("entrants").GetProperty("nodes");

                    foreach (JsonElement entrant in entrants.EnumerateArray()) {
                        // Safely catch in case participants property is null
                        if (!entrant.TryGetProperty("participants", out JsonElement participants) || participants.ValueKind == JsonValueKind.Null)
                        {
                            continue;
                        }

                        foreach (JsonElement participant in participants.EnumerateArray())
                        {
                            string gamerTag = participant.GetProperty("gamerTag").GetString() ?? "Unknown";

                            JsonElement user = participant.GetProperty("player").GetProperty("user");
                            string pronouns = user.GetProperty("genderPronoun").GetString() ?? "Unknown";
                            string birthday = user.GetProperty("birthday").GetString() ?? "Unknown";

                            results["overall"].Add(new AttendeeResult
                            {
                                GamerTag = gamerTag,
                                Pronouns = pronouns,
                                Birthday = birthday,
                            });
                        }
                    }
                }
            }
            return results;
        }
    }
}
