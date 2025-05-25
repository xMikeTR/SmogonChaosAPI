using PokemonChaosStatsApi.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAllDataFetcher
{
    Task<SmogonResponse?>  GetAllData(string date, string format,[FromQuery] PaginationFilter filter);
}