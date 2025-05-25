using Microsoft.AspNetCore.Mvc;
using PokemonChaosStatsApi.Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;


public class AllDataFetcher : IAllDataFetcher
{
    private readonly IRequestsService _requestService;
    private readonly IUriService uriService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AllDataFetcher> _logger;

    public AllDataFetcher(
        IRequestsService requestService, 
        IUriService uriService, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<AllDataFetcher> logger)
    {
        _logger = logger;
        _requestService = requestService;
        this.uriService = uriService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<SmogonResponse?> GetAllData(string date, string format, [FromQuery] PaginationFilter filter)
    {
        string url = $"stats/{date}/chaos/{format}";

        var smogonResponse = await _requestService.GetStreamAsync(url);
            if (smogonResponse is null)
            {
                return null;
        
            }
            var dataWithNames = smogonResponse.Data
            .ToDictionary(kvp => kvp.Key, kvp => 
            {
                var pokemon = kvp.Value;
                pokemon.Name = kvp.Key;      
                return pokemon;
            
            });
            var route = _httpContextAccessor.HttpContext?.Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber,filter.PageSize);
            var pagedData = dataWithNames
                .Skip((validFilter.PageNumber - 1)*validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToDictionary(kvp=>kvp.Key,kvp=>kvp.Value);
            var totalRecords =  dataWithNames.Count();
            var pageNumber = 1;
            var pageSize = 5;

        return new SmogonResponse
        {
            Info = smogonResponse.Info,
            Data = pagedData
        };
    }
}