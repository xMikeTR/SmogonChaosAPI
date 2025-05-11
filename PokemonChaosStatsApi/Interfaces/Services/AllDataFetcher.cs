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
    private readonly IUriService uriService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AllDataFetcher(IRequestsService requestService, IUriService uriService, IHttpContextAccessor httpContextAccessor)
    {
        _requestService = requestService;
        this.uriService = uriService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<ActionResult<SmogonResponse>> GetAllData(string date, string format, [FromQuery] PaginationFilter filter)
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
            var route = _httpContextAccessor.HttpContext?.Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber,filter.PageSize);
            var pagedData = dataWithNames
                .Skip((validFilter.PageNumber - 1)*validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToDictionary(kvp=>kvp.Key,kvp=>kvp.Value);
                //.ToListAsync();
            var totalRecords =  dataWithNames.Count();

            //int totalItems = dataWithNames.Count;
            //var pagedResults = dataWithNames
              //  .Skip((page -1) * pageSize)
                //.Take(pageSize)
                //.ToDictionary(kvp=>kvp.Key, kvp=>kvp.Value);

        //return new OkObjectResult(new {
          //      smogonResponse.Info,
                //Meta = meta,
            //    Data = pagedResults
            //});
        //return new OkObjectResult(new PagedResponse<Dictionary<string,Pokemon>>(pagedData,validFilter.PageNumber,validFilter.PageSize));
        //we just need to pass in our main dict and we're done
        List<SmogonResponse> spokemon = new List<SmogonResponse>();
        var pagedResponse = PaginationHelper.CreatePagedReponse<spokemon>(pagedData,validFilter,totalRecords,uriService,route);
        return new OkObjectResult(pagedResponse);
    }
}