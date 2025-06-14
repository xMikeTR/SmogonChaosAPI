//Interface to abstract the Streaming HTTP request
using PokemonChaosStatsApi.Models;

public interface IRequestsService
{
    Task<SmogonResponse> GetStreamAsync(string url);
}