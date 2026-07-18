using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class QueryManager
    {
        private readonly StartGgClient _client;
        private readonly Parser _parser;
        private readonly Analytics _analytics;

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

        public async Task<Dictionary<string, List<Analytics.Top8Analytics>>> QueryTop8(Dictionary<string, object?> variables)
        {
            // Execute query
            GraphQLResult result = await _client.ExecuteAsync(Queries.TournamentTop8, variables, true);

            if (!result.Success)
            {
                throw new Exception(result.ErrorMessage);
            }

            // Parse query
            List<Parser.Top8Result> parsed = _parser.ParseTop8(result.Json);

            // Convert to useful data
            Dictionary<string, List<Analytics.Top8Analytics>> analytics = _analytics.BuildTop8Analytics(parsed);

            // Return data to be used by UI
            return analytics;
        }

        public async Task<Dictionary<string, List<Analytics.HeadcountAnalytics>>> QueryHeadcount(Dictionary<string, object?> variables) 
        {
            // Execute Query
            GraphQLResult result = await _client.ExecuteAsync(Queries.TournamentHeadCount, variables, true);

            if (!result.Success)
            {
                throw new Exception(result.ErrorMessage);
            }

            // Parse Query
            List<Parser.HeadcountResult> parsed = _parser.ParseHeadcount(result.Json);

            // Convert to useful data
            Dictionary<string, List<Analytics.HeadcountAnalytics>> analytics = _analytics.BuildHeadcountAnalytics(parsed);

            // Return data to be used by UI
            return analytics;
        }

        public async Task<Dictionary<string, List<Parser.AttendeeResult>>> QueryAttendees(Dictionary<string, object?> variables)
        {
            // Execute Query
            GraphQLResult result = await _client.ExecuteAsync(Queries.TournamentGetUser, variables, false);

            if (!result.Success)
            {
                throw new Exception(result.ErrorMessage);
            }

            // Parse Query as a dictionary
            Dictionary<string, List<Parser.AttendeeResult>> parsed = _parser.ParseAttendees(result.Json);

            // Return data to be used by UI
            return parsed;
        }
    }
}
