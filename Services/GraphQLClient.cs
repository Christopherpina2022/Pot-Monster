using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class GraphQLResult
    {
        public bool Success { get; set; }
        public string Json { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class StartGgClient
    {
        private readonly HttpClient _httpClient;

        public StartGgClient(string apiKey)
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("https://api.start.gg/gql/alpha") };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<GraphQLResult> ExecuteAsync(string query, Dictionary<string, object?>? variables = null) {
            try 
            {
                var payload = new
                {
                    query,
                    variables
                };

                string json = JsonSerializer.Serialize(payload);

                var request = new HttpRequestMessage(HttpMethod.Post, "")
                {
                    Content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json")
                };

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(responseBody);

                bool hasErrors = document.RootElement.TryGetProperty(
                    "errors",
                    out JsonElement errors);

                if (!response.IsSuccessStatusCode || hasErrors)
                {
                    return new GraphQLResult
                    {
                        Success = false,
                        Json = string.Empty,
                        ErrorMessage =
                            $"GraphQL returned an error. " +
                            $"{errors[0].GetProperty("message").GetString()}"
                    };
                }

                return new GraphQLResult
                {
                    Success = true,
                    Json = responseBody,
                    ErrorMessage = null
                };
                }
                catch (Exception ex)
                {
                    return new GraphQLResult
                    {
                        Success = false,
                        Json = string.Empty,
                        ErrorMessage = ex.Message
                    };
                }
        }
    }
}
