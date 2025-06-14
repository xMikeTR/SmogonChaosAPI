//HTTP Service for Streaming data, Streaming was used here due to the size of each JSON
//Deserialization method also already implemented here for use in the data methods

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PokemonChaosStatsApi.Models;


public class RequestsService : IRequestsService
{
    private readonly HttpClient _httpClient;

    public RequestsService(HttpClient httpClient, ILogger<RequestsService> logger)
    {
        _httpClient= httpClient;

        _httpClient.BaseAddress = new Uri("https://www.smogon.com/");
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<SmogonResponse> GetStreamAsync(string url)
    {
        using Stream stream = await _httpClient.GetStreamAsync(url);
            SmogonResponse? smogonResponse = await JsonSerializer.DeserializeAsync<SmogonResponse>(
                stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return smogonResponse;
    }
}