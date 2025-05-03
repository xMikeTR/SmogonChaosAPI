namespace PokemonChaosStatsApi.Models;

public class SmogonResponse
{
    public Info? Info { get; set; }
    public Dictionary<string, Pokemon> Data { get; set; }
}
