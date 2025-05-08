using PokemonChaosStatsApi.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAllDataFetcher
{
    Task<ActionResult<SmogonResponse>>  GetAllData(string date, string format,int pageIndex,int pageSize);
}