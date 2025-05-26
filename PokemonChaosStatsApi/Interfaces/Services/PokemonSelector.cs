using Microsoft.AspNetCore.Mvc;
using PokemonChaosStatsApi.Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;


public class PokemonSelector : IPokemonSelector
{
    private readonly IRequestsService _requestService;
    private readonly IUriService uriService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PokemonSelector> _logger;

    public PokemonSelector(
        IRequestsService requestService, 
        IUriService uriService, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<PokemonSelector> logger)
    {
        _logger = logger;
        _requestService = requestService;
        this.uriService = uriService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<SmogonResponse?> GetAllPokemonData(string date, string format,string selected)
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

            
        var filteredNames = dataWithNames
            .Where(kvp=>kvp.Value.Name.Equals(selected,StringComparison.OrdinalIgnoreCase))
            .ToDictionary(kvp=> kvp.Key, kvp=>kvp.Value);
        
            if (!dataWithNames.ContainsKey(selected))
            {
                _logger.LogWarning("Invalid Pokémon: {selected}", selected);
                return null;
            }

        return new SmogonResponse
        {
            Info = smogonResponse.Info,
            Data = filteredNames
        };
    }
}