using PokemonChaosStatsApi.Models;
using Microsoft.AspNetCore.Mvc;

public interface IPokemonSelector
{
    Task<SmogonResponse?>  GetAllPokemonData(string date, string format,string selected);
}