using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class GraphQLResult
    {
        public bool Success { get; set; }
        public string? Json { get; set; }
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

        public async Task<GraphQLResult> ExecuteAsync (string query, Dictionary<string, object?> variables)
        {
            try
            {
                int page = 1;
                while (true) {
                    variables["page"] = page;

                    // Assemble Request
                    var payload = new { query, variables };
                    string json = JsonSerializer.Serialize(payload);
                    var request = new HttpRequestMessage(HttpMethod.Post, "")
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    HttpResponseMessage response = await _httpClient.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Validate response
                    using var document = JsonDocument.Parse(responseBody);
                    bool hasErrors = document.RootElement.TryGetProperty("errors", out _);

                    // for now only return first result
                    return new GraphQLResult
                    {
                        Success = response.IsSuccessStatusCode && !hasErrors,
                        Json = responseBody,
                        ErrorMessage = "Authentication Failed"
                    };

                    // parse JSON to determine if max page is reached
                    //page++;
                }
            }
            catch (Exception ex)
            {
                return new GraphQLResult
                {
                    Success = false,
                    Json = null,
                    ErrorMessage = "GraphQL Client Failed."
                };
            }
        }
    }
}
