using System.Drawing;
using System.Printing;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class Parser
    {
        public class OwnerResult
        {
            public string Name { get; set; } = "";
            public string Slug { get; set; } = "";
            public int StartAt { get; set; }
            public int NumAttendees { get; set; }
            public List<(string Type, string Url)> Images { get; set; } = new();

        }

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
            public string Pronouns { get; set; } = "";
            public string Birthday { get; set; } = "";
        }

        public string ParseOwner(string json)
        {
            // Parse data
            using JsonDocument document = JsonDocument.Parse(json);
            return document.RootElement.GetProperty("data").GetProperty("tournament").GetProperty("owner").GetProperty("id").ToString(); ;
        }

        public List<OwnerResult> ParseOwnerTournaments(string json) 
        {
            List<OwnerResult> results = new List<OwnerResult>();

            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement nodes = document.RootElement.GetProperty("data").GetProperty("tournaments").GetProperty("nodes");
            foreach (JsonElement node in nodes.EnumerateArray())
            {
                if (!node.TryGetProperty("startAt", out JsonElement startAt) || startAt.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                if (!node.TryGetProperty("numAttendees", out JsonElement numAttendees) || numAttendees.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }

                // Required values so no need to null validate
                string name = node.GetProperty("name").GetString();
                string slug = node.GetProperty("slug").GetString();

                OwnerResult result = new OwnerResult
                {
                    Name = name,
                    Slug = slug,
                    StartAt = startAt.GetInt32(),
                    NumAttendees = numAttendees.GetInt32()
                };

                JsonElement images = node.GetProperty("images");
                foreach(JsonElement image in images.EnumerateArray())
                {
                    if (!image.TryGetProperty("type", out JsonElement type) || type.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    if (!image.TryGetProperty("url", out JsonElement url) || url.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    };

                    result.Images.Add((
                        type.GetString()!,
                        url.GetString()!
                    ));
                }
                results.Add(result);
            }
            return results;
        }

        public int ParseTotalPages(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            return document.RootElement.GetProperty("data").GetProperty("tournaments").GetProperty("pageInfo").GetProperty("totalPages").GetInt32();
        }

        public List<Top8Result> ParseTop8(string json)
        {
            List<Top8Result> results = new List<Top8Result>();

            using JsonDocument document = JsonDocument.Parse(json);

            // Read through JSON
            JsonElement events = document.RootElement.GetProperty("data").GetProperty("tournament").GetProperty("events");
            foreach (JsonElement evt in events.EnumerateArray())
            {
                string game = evt.GetProperty("videogame").GetProperty("name").GetString()!;

                JsonElement standings = evt.GetProperty("standings").GetProperty("nodes");
                foreach (JsonElement standing in standings.EnumerateArray())
                {
                    
                    JsonElement participants = standing.GetProperty("entrant").GetProperty("participants");
                    foreach (JsonElement participant in participants.EnumerateArray())
                    {
                        // Safety catch in case player property is null
                        JsonElement player = participant.GetProperty("player");
                        if (player.ValueKind == JsonValueKind.Null)
                        {
                            continue;
                        }
                    
                        string gamerTag = player.GetProperty("prefix").GetString() + " " + player.GetProperty("gamerTag").GetString();
                        int placement = standing.GetProperty("placement").GetInt32();

                        results.Add(new Top8Result
                        {
                            Game = game,
                            GamerTag = gamerTag,
                            Placement = placement
                        });
                    }
                }
            }

            return results;
        }

        public List<HeadcountResult> ParseHeadcount(string json)
        {
            List<HeadcountResult> results = new List<HeadcountResult>();

            using JsonDocument document = JsonDocument.Parse(json);

            // Read through JSON
            JsonElement events = document.RootElement.GetProperty("data").GetProperty("tournament").GetProperty("events");

            foreach (JsonElement evt in events.EnumerateArray())
            {
                string game = evt.GetProperty("videogame").GetProperty("name").GetString()!;

                JsonElement entrants = evt.GetProperty("entrants").GetProperty("nodes");
                foreach (JsonElement entrant in entrants.EnumerateArray())
                {
                    foreach (JsonElement participant in entrant.GetProperty("participants").EnumerateArray())
                    {
                        // safety catch in case participant or user is null
                        if (participant.ValueKind == JsonValueKind.Null || participant.GetProperty("user").ValueKind == JsonValueKind.Null)
                        {
                            continue;
                        }

                        string prefix = participant.GetProperty("prefix").GetString() ?? "";
                        string gamerTagValue = participant.GetProperty("gamerTag").GetString() ?? "Unknown";
                        string gamerTag = $"{prefix} {gamerTagValue}".Trim();

                        string pronouns = participant.GetProperty("user").GetProperty("genderPronoun").GetString() ?? "";
                        string birthday = participant.GetProperty("user").GetProperty("birthday").GetString() ?? "N/A";

                        results.Add(new HeadcountResult
                        {
                            Game = game,
                            GamerTag = gamerTag,
                            Pronouns = pronouns,
                            Birthday = birthday,
                        });
                    }   
                }
            }

            return results;
        }
    }
}
