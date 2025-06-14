//Main pokemon model containing all fields listed on each JSON

using System.Text.Json.Serialization;
namespace PokemonChaosStatsApi.Models;

public class Pokemon
{
    public string? Name {get;set;}
    public int Battles {get;set;}
    public Dictionary<string,float> Moves {get;set;}
    [JsonPropertyName("Checks and Counters")]
    public Dictionary<string, List<float>> ChecksAndCounters { get; set; } = new();
    public Dictionary<string,float> Abilities {get;set;}
    public Dictionary<string,float> Teammates {get;set;}
    public float Usage {get;set;}
    public Dictionary<string,float> Items {get;set;}
    public int RawCount {get;set;}
    public Dictionary<string,float> Spreads {get;set;}
    public Dictionary<string,float> ViabilityCeiling {get;set;}
}

