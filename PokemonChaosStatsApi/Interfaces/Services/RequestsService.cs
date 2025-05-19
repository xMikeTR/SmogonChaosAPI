using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PokemonChaosStatsApi.Models;


public class RequestsService : IRequestsService
{
    private readonly HttpClient _httpClient;

    public RequestsService(HttpClient httpClient)
    {
        _httpClient= httpClient;

        _httpClient.BaseAddress = new Uri("https://www.smogon.com/");
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<SmogonResponse> GetStreamAsync(string url)
    {
        using Stream stream = await _httpClient.GetStreamAsync(url);
            Console.WriteLine("[DEBUG] Raw JSON:");
            Console.WriteLine(json);
            SmogonResponse? smogonResponse = await JsonSerializer.DeserializeAsync<SmogonResponse>(
                stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return smogonResponse;
    }
}