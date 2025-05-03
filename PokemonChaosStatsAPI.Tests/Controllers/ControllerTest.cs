using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using PokemonChaosStatsApi.Controllers;
using Microsoft.Extensions.Logging;
using PokemonChaosStatsApi.Models;
using System.Collections.Generic;
using Microsoft.CSharp;
using PokemonChaosStatsAPI.Tests.ErrorModel;

namespace PokemonChaosStatsAPI.Tests.Controllers
{
    public class SmogonPokemonAPIControllerTest
    {

        private readonly SmogonPokemonAPIController _controller;
        
        public SmogonPokemonAPIControllerTest()
        {
            var mockLogger = new Mock<ILogger<SmogonPokemonAPIController>>();
            var mockValidatorLogger = new Mock<ILogger<InputValidators>>();
            var realValidator = new InputValidators(mockValidatorLogger.Object);

            _controller = new SmogonPokemonAPIController(mockLogger.Object, realValidator);
        }

        [Fact]
        public void GetAvailableDates_ReturnsListOfDates()
        {
            var result = _controller.GetAvailableDates();

            Assert.NotNull(result);
            Assert.IsType<List<string>>(result);
            Assert.True(result.Count >0, "The list of dates should not be empty.");
            Assert.All(result, item => Assert.False(string.IsNullOrWhiteSpace(item),"Date entries should not be null or whitespace"));
        }

        [Fact]
        public void GetAvailableFormats_ReturnsBadRequest_OnInvalidDate()
        {
            string invalidDate = "202x-1";

            var result = _controller.GetAvailableFormats(invalidDate);

            //var resultObject = Assert.IsType<BadRequestObjectResult>(result);
            //Assert.Contains("Invalid date format", resultObject.Value.ToString());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);

            Assert.Equal("Invalid date format. Use yyyy-MM.",response.Error);
            Assert.False(response.Success);

        }


        //[Fact]
        //public void GetAllPokemon_ReturnsBadRequest_OnInvalidDate_OnInvalidFormat();

        //string invalidDate = "202x-2";

        //var result = _controller.
    }
}
