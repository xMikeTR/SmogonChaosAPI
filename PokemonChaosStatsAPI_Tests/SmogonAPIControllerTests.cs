using Moq;
using Microsoft.AspNetCore.Mvc;
using PokemonChaosStatsApi.Controllers;
using Microsoft.Extensions.Logging;
using PokemonChaosStatsApi.Models;


namespace PokemonChaosStatsAPI_Tests;

public class SmogonControllerTests
{
    private readonly Mock<ILogger<SmogonPokemonAPIController>> _mockLogger;
    private readonly Mock<IInputValidators> _mockValidators;
    private readonly Mock<IDateFetcherService> _mockDateService;
    private readonly Mock<IFormatFetcherService> _mockFormatService;
    private readonly Mock<IAllDataFetcher> _mockDataFetcher;
    private readonly Mock<IPokemonSelector> _mockSelector;

    private readonly SmogonPokemonAPIController _controller;

    public SmogonControllerTests()
    {
        _mockLogger = new Mock<ILogger<SmogonPokemonAPIController>>();
        _mockValidators = new Mock<IInputValidators>();
        _mockDateService = new Mock<IDateFetcherService>();
        _mockFormatService = new Mock<IFormatFetcherService>();
        _mockDataFetcher = new Mock<IAllDataFetcher>();
        _mockSelector = new Mock<IPokemonSelector>();

        _controller = new SmogonPokemonAPIController(
            _mockLogger.Object,
            _mockValidators.Object,
            _mockDateService.Object,
            _mockFormatService.Object,
            _mockDataFetcher.Object,
            _mockSelector.Object
        );


    }

    [Fact]
    public void Test_Date_Fetcher()
    {
        //Arrange
        var expectedDates = new List<string> {"2024-01","2024-02","2024-03"};
        _mockDateService.Setup(s => s.GetAvailableDates())
                        .Returns(expectedDates);

        //Act
        var result = _controller.GetAvailableDates();


        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDates = Assert.IsType<List<string>>(okResult.Value);
        Assert.Equal(3,returnedDates.Count);
        
    }

    [Fact]
    public void GetAvailableFormats_WhenServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var testDate = "2024-01";
        _mockFormatService.Setup(s => s.GetFormats(testDate))
                      .Throws(new ArgumentException("Invalid date format"));
    
        // Act
        var result = _controller.GetAvailableFormats(testDate);
    
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }  

    [Fact]
    public void Test_Format_Fetcher_ReturnOk()
    {
        //Arrange
        var testDate = "2024-01";
        var expectedFormats = new List<string> {"gen1moderngen1-0.json","gen1uu-1760.json","gen3ou-0.json"};
        _mockFormatService.Setup(s => s.GetFormats(testDate))
                        .Returns(expectedFormats);

        _mockValidators.Setup(v => v.IsValidDate(testDate))
                        .Returns(true);

        //Act
        var formatResult = _controller.GetAvailableFormats(testDate);


        //Assert
        var okResult = Assert.IsType<OkObjectResult>(formatResult);
        var returnedFormats = Assert.IsType<List<string>>(okResult.Value);
        Assert.Equal(3,returnedFormats.Count);
    }
    
    public class ControllerTestBuilder
    {
        private Mock<IInputValidators> _mockValidator = new();
        private Mock<IAllDataFetcher> _mockDataFetcher = new();
        private Mock<IPokemonSelector> _mockSelector = new();
    
        public ControllerTestBuilder WithValidInput()
        {
            _mockValidator.Setup(v => v.IsValidDate(It.IsAny<string>())).Returns(true);
            _mockValidator.Setup(v => v.IsValidFormat(It.IsAny<string>())).Returns(true);
            _mockValidator.Setup(v => v.IsValidPokemon(It.IsAny<string>())).Returns(true);
            return this;
        }
    
        public ControllerTestBuilder WithInvalidDate()
        {
            _mockValidator.Setup(v => v.IsValidDate(It.IsAny<string>())).Returns(false);
            return this;
        }
    
        public ControllerTestBuilder WithDataReturned()
        {
            _mockDataFetcher.Setup(f => f.GetAllData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PaginationFilter>()))
                   .ReturnsAsync(new SmogonResponse { Data = new Dictionary<string, Pokemon>() });
            return this;
        }

        public ControllerTestBuilder WithPokemonSelected()
        {
            _mockSelector.Setup(s => s.GetAllPokemonData(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()))
                    .ReturnsAsync(new SmogonResponse{Data = new Dictionary<string, Pokemon>()});
            return this;
        }
    
        public SmogonPokemonAPIController Build()
        {
            return new SmogonPokemonAPIController(
                Mock.Of<ILogger<SmogonPokemonAPIController>>(),
                _mockValidator.Object,
                Mock.Of<IDateFetcherService>(),
                Mock.Of<IFormatFetcherService>(),
                _mockDataFetcher.Object,
                _mockSelector.Object);
        }
    }

    // Usage:
    [Fact]
    public async Task GetFromJsonAsync_ValidInput_ReturnsOkResult()
    {
        var controller = new ControllerTestBuilder()
            .WithValidInput()
            .WithDataReturned()
            .Build();

        var result = await controller.GetFromJsonAsync("2024-01", "gen1moderngen1-0.json", new PaginationFilter());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetFromAsync_Selected_Pokemon_ReturnsOk()
    {
        var controller = new ControllerTestBuilder()
            .WithValidInput()
            .WithPokemonSelected()
            .Build();

        var result = await controller.GetPokemonFromJsonAsync("2024-01", "gen1moderngen1-0.json", "Archeops");
        Assert.IsType<OkObjectResult>(result);
    }

}
