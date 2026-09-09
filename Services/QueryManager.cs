using System.Data;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class QueryManager
    {
        private readonly StartGgClient _client;
        private readonly Parser _parser;
        private readonly Analytics _analytics;

        public List<Parser.OwnerResult> TournamentList { get; private set; } = new();

        public QueryManager()
        {
            // Setup client
            string? key = new KeyManager().GetKey();
            if (key == null)
            {
                throw new Exception("API Key is missing.");
            }

            _client = new StartGgClient(key);
            _parser = new Parser();
            _analytics = new Analytics();
        }

        public async Task QueryTournamentOwner(string url)
        {
            // Clear the old tournament list
            TournamentList.Clear();

            string slug = ExtractSlug(url);

            // Run Query for tournament owner and return owner ID
            GraphQLResult slugResult = await _client.ExecuteAsync(Queries.GetOwnerByTournamentSlug, "slug", slug);
            string ownerID = _parser.ParseOwner(slugResult.Json!);

            // Run Query for tournaments owner has ran
            GraphQLResult ownerResult = await _client.ExecuteAsync(Queries.SearchTournamentsByOwner, "user", ownerID);

            // Append data to Tournament List
            TournamentList = _parser.ParseOwnerTournaments(ownerResult.Json!);
        }

        private string ExtractSlug(string url)
        {
            // Extract tournament slug from url
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException("Invalid URL.", nameof(url));
            }

            const string tournamentPath = "/tournament/";
            var path = uri.AbsolutePath;
            var start = path.IndexOf(tournamentPath, StringComparison.OrdinalIgnoreCase);

            if (start == -1)
            {
                throw new ArgumentException("Arguement does not appear to be a Start.gg tournament URL.", nameof(url));
            }

            start += tournamentPath.Length;

            var end = path.IndexOf('/', start);
            if (end == -1)
            {
                end = path.Length;
            }

            var slug = path[start..end];

            // Check if there is a valid slug
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentException("Tournament URL does not contain a slug.", nameof(url));
            }
                
            return slug;
        }

        
        public async Task<Dictionary<string, List<Analytics.Top8Analytics>>> QueryTop8(List<Parser.OwnerResult> tournamentList, DateTime startDate, DateTime endDate)
        {
            // Filter tournament data by date
            List<Parser.OwnerResult> filteredTournaments = tournamentList.Where(
                t => DateTimeOffset.FromUnixTimeSeconds(t.StartAt).DateTime >= 
                startDate && DateTimeOffset.FromUnixTimeSeconds(t.StartAt).DateTime <= 
                endDate).ToList();

            List<Parser.Top8Result> totalParsedResults = new();

            foreach (Parser.OwnerResult tournament in filteredTournaments)
            {
                // Execute Query
                GraphQLResult result = await _client.ExecuteAsync(Queries.TournamentTop8, "tournamentSlug", tournament.Slug);

                if (result.Success != true)
                {
                    throw new Exception("Error in query: " + result.ErrorMessage);
                }

                // Parse query then append any results to the total list
                List<Parser.Top8Result> parsedTournament = _parser.ParseTop8(result.Json);
                totalParsedResults.AddRange(parsedTournament);
            }

            // Convert to useful data
            Dictionary<string, List<Analytics.Top8Analytics>> analytics = _analytics.BuildTop8Analytics(totalParsedResults);
            return analytics;
        }

        public async Task<Dictionary<string, List<Analytics.HeadcountAnalytics>>> QueryHeadcount(List<Parser.OwnerResult> tournamentList, DateTime startDate, DateTime endDate) 
        {
            // Filter tournament data by date
            List<Parser.OwnerResult> filteredTournaments = tournamentList.Where(
                t => DateTimeOffset.FromUnixTimeSeconds(t.StartAt).DateTime >=
                startDate && DateTimeOffset.FromUnixTimeSeconds(t.StartAt).DateTime <=
                endDate).ToList();

            List<Parser.HeadcountResult> totalParsedResults = new();

            foreach (Parser.OwnerResult tournament in filteredTournaments)
            {
                // Execute Query
                GraphQLResult result = await _client.ExecuteAsync(Queries.TournamentHeadCount, "tournamentSlug", tournament.Slug);

                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessage);
                }

                // Parse Query
                List<Parser.HeadcountResult> parsedHeadcount = _parser.ParseHeadcount(result.Json);
                totalParsedResults.AddRange(parsedHeadcount);
            }
            
            // Convert to useful data
            Dictionary<string, List<Analytics.HeadcountAnalytics>> analytics = _analytics.BuildHeadcountAnalytics(totalParsedResults);
            return analytics;
        }
    }
}
