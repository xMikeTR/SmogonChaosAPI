using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonChaosStatsApi.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Diagnostics;


namespace PokemonChaosStatsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmogonPokemonAPIController : ControllerBase
{
    private readonly ILogger<SmogonPokemonAPIController> _logger;
    private readonly InputValidators _inputValidators;
    private readonly IDateFetcherService _dateService;
    private readonly IFormatFetcherService _formatService;
    private readonly IAllDataFetcher _dataFetcher;

    public SmogonPokemonAPIController(
        ILogger<SmogonPokemonAPIController> logger, 
        InputValidators inputValidators,
        IDateFetcherService dateService,
        IFormatFetcherService formatService,
        IAllDataFetcher dataFetcher)
        {
            _logger = logger;
            _inputValidators = inputValidators;
            _dateService = dateService;
            _formatService = formatService;
            _dataFetcher = dataFetcher;
        }
    
    private static  HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://www.smogon.com/"),
        Timeout = TimeSpan.FromSeconds(15)
    };
    //Returning available dates within Smogon stats, so users know how to filter later. Separation from main filtering endpoint for modularity
    [HttpGet("dates")]
    [ResponseCache(Duration = 60)]
    public IActionResult GetAvailableDates()
    {
        var dates = _dateService.GetAvailableDates();
        return Ok(dates);
    }
        

    

    //Pass in a date in format yyyy-mm
    [HttpGet("formats")]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] {"date"})]
    public IActionResult GetAvailableFormats(string date)
    {
        if (!_inputValidators.IsValidDate(date))
        {
            return BadRequest(new{Success = false, Error="Invalid date format. Use yyyy-MM."});
        }
        var formats = _formatService.GetFormats(date);
        return Ok(formats);
    }


    [HttpGet("alldata")]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] {"date","format"})]
    public async Task<IActionResult> GetFromJsonAsync(string date, string format, [FromQuery] PaginationFilter filter) //, int page = 1, int pageSize = 20)
    {
        if (!_inputValidators.IsValidDate(date))
        {
            return BadRequest(new{Success = false, Error="Invalid date format. Use yyyy-MM."});
        }

        if (!_inputValidators.IsValidFormat(format))
        {
            return BadRequest(new{Success = false, Error="Invalid battle format, please check available formats."});
        }


       var allpokemon = await _dataFetcher.GetAllData(date,format,filter);
       return Ok(allpokemon);
        
    }

    [HttpGet("pokemondata")]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] {"date","format","selected"})]
    public async Task<IActionResult> GetFromJsonAsync(string date, string format,string selected)
    {
        if (!_inputValidators.IsValidDate(date))
        {
            return BadRequest(new{Success = false, Error="Invalid date format. Use yyyy-MM."});
        }

        if (!_inputValidators.IsValidFormat(format))
        {
            return BadRequest(new{Success = false, Error="Invalid battle format, please use something like gen5ou-0."});
        }

        if (string.IsNullOrWhiteSpace(selected))
        {
            _logger.LogWarning("Invalid Pokemon name received: {selected}. Selected should not be empty.", selected);
            return BadRequest(new{Success = false, Error="Invalid Pokemon Name"});
        }

        using Stream stream = await sharedClient.GetStreamAsync($"stats/{date}/chaos/{format}.json");

        SmogonResponse? smogonResponse = await JsonSerializer.DeserializeAsync<SmogonResponse>(
            stream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (smogonResponse is null)
        {
            return NotFound("failure");
        
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
            return NotFound(new {Success = false, Error = $"Pokémon '{selected}' not found for format '{format}' and date '{date}'."});
        }

        return Ok(new {smogonResponse.Info, Data = filteredNames});
        
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error-development")]
    public IActionResult HandleErrorDevelopment(
        [FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }

        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        return Problem(
            detail: exceptionHandlerFeature.Error.StackTrace,
            title: exceptionHandlerFeature.Error.Message);
        
    }
    

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError() =>
        Problem();
}