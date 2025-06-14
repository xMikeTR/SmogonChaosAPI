//Interface for getting data on a specific pokemon
using PokemonChaosStatsApi.Models;
using Microsoft.AspNetCore.Mvc;

public interface IPokemonSelector
{
    Task<SmogonResponse?>  GetAllPokemonData(string date, string format,string selected);
}