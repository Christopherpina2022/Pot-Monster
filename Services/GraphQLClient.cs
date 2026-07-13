using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class GraphQLResult
    {
        public bool Success { get; set; }
        public List<string>? Json { get; set; }
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

        public async Task<GraphQLResult> ExecuteAsync (string query, Dictionary<string, object?> variables, bool paginate = false)
        {
            try
            {
                int page = 1;
                int totalPages = 1;
                List<string> pages = new();

                while (page <= totalPages) {
                    variables["page"] = page;

                    // Assemble Request
                    var payload = new { query, variables };
                    string json = JsonSerializer.Serialize(payload);
                    var request = new HttpRequestMessage(HttpMethod.Post, "")
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    // Run first page query
                    HttpResponseMessage response = await _httpClient.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Validate response
                    using var document = JsonDocument.Parse(responseBody);
                    bool hasErrors = document.RootElement.TryGetProperty("errors", out JsonElement errors);
                    if (!response.IsSuccessStatusCode || hasErrors)
                    {
                        return new GraphQLResult
                        {
                            Success = false,
                            Json = null,
                            ErrorMessage = $"GraphQL returned an error. {errors[0].GetProperty("message").GetString()}"
                        };
                    }

                    // Check if there are more pages
                    if (paginate && page == 1)
                    {
                        if (document.RootElement
                            .GetProperty("data")
                            .GetProperty("tournaments")
                            .GetProperty("pageInfo")
                            .TryGetProperty("totalPages", out JsonElement total))
                        {
                            totalPages = total.GetInt32();
                        }
                    }

                    // Append to responseBody
                    pages.Add(responseBody);
                    page++;
                }
                return new GraphQLResult
                {
                    Success = true,
                    Json = pages,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new GraphQLResult
                {
                    Success = false,
                    Json = null,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
