//Interface to grab all available data for selected date and format
using PokemonChaosStatsApi.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAllDataFetcher
{
    Task<SmogonResponse?>  GetAllData(string date, string format,[FromQuery] PaginationFilter filter);
}