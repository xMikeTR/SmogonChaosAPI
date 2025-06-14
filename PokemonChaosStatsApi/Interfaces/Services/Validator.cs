//Validation service to check for proper date format and whitespaces
//No Regex forcing on format nor Pokemon name as that was causing too much restriction

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
using System.Text.RegularExpressions;


public class InputValidators : IInputValidators
{
    private readonly ILogger<InputValidators> _logger;

    public  InputValidators(ILogger<InputValidators> logger)
    {
        _logger = logger;
    }

    public bool IsValidDate(string date)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            _logger.LogWarning("Invalid date received: {date}. Date should not be empty.", date);
            return false;
        }

        if (!Regex.IsMatch(date, @"^\d{4}-\d{2}$"))
        {
            _logger.LogWarning("Invalid date format received: {date}. Expected format is yyyy-mm.", date);
            return false;
        }
        return true;
    }

    public bool IsValidFormat(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            _logger.LogWarning("Invalid format received: {format}. Format should not be empty.", format);
            return false;
        }
        return true;
    }

    public bool IsValidPokemon(string selected)
    {
        if (string.IsNullOrWhiteSpace(selected))
        {
            _logger.LogWarning("Invalid Pokemon name received: {selected}. Selected should not be empty.", selected);
            return false;
        }
        return true;
    }
}