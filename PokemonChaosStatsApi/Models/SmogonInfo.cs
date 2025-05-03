namespace PokemonChaosStatsApi.Models;

public class Info
{
    public string? Metagame { get; set; }
    public double Cutoff { get; set; }
    public double CutoffDeviation { get; set; }
    public string? TeamType { get; set; }
    public int NumberOfBattles { get; set; }
}
