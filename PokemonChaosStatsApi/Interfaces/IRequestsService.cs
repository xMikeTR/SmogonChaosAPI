using PokemonChaosStatsApi.Models;

public interface IRequestsService
{
    Task<SmogonResponse> GetStreamAsync(string url);
}