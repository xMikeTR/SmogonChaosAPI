using Microsoft.AspNetCore.Mvc;
using PokemonChaosStatsApi.Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using PokemonChaosStatsApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace PokemonChaosStatsApi.Services;

public class AllDataFetcher : IAllDataFetcher
{
    private readonly IRequestsService _requestService;

    public AllDataFetcher(IRequestsService requestService)
    {
        _requestService = requestService;
    }
    public async Task<ActionResult<SmogonResponse>> GetAllData(string date, string format, int page = 1, int pageSize = 20)
    {
        string url = $"stats/{date}/chaos/{format}.json";

        var smogonResponse = await _requestService.GetStreamAsync(url);
            if (smogonResponse is null)
            {
                return new NotFoundResult();
        
            }
            var dataWithNames = smogonResponse.Data
            .ToDictionary(kvp => kvp.Key, kvp => 
            {
                var pokemon = kvp.Value;
                pokemon.Name = kvp.Key;      
                return pokemon;
            
            });

            int totalItems = dataWithNames.Count;
            var pagedResults = dataWithNames
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .ToDictionary(kvp=>kvp.Key, kvp=>kvp.Value);

            


        return new OkObjectResult(new {
                smogonResponse.Info,
                //Meta = meta,
                Data = pagedResults
            });
    }
}