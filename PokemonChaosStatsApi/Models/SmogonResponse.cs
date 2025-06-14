//Since Smogon data is comprised of two parts, a model had to be created for each
//Which is then combined on this model to bring proper data

namespace PokemonChaosStatsApi.Models;

public class SmogonResponse
{
    public Info? Info { get; set; }
    public Dictionary<string, Pokemon> Data { get; set; }
}
