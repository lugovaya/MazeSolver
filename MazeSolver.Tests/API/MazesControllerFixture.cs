using System.Net;
using System.Net.Http.Json;
using NSubstitute;
using MazeSolver.Domain.Models;
using MazeSolver.Domain.Services;
using FluentValidation;
using MazeSolver.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;

namespace MazeSolver.Tests.API
{
    public class MazesControllerFixture
    {
        private HttpClient _client;
        private IMazeService<Guid, string?> _service;
        private IValidator<MazeRequestModel> _validator;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of IMazeService
            _service = Substitute.For<IMazeService<Guid, string?>>();
            _service.GetAll().Returns(new List<MazeConfiguration>
            {
                new ("S__\n_X_\n__G") { Id = Guid.NewGuid(), Solution = "SXX\n_X_\nXXG" }
            });

            _service.Submit(Arg.Any<MazeConfigurationBase<string?, Guid>>()).Returns("SXX\n_X_\nXXG");

            // Create a mock of IValidator<MazeRequestModel>
            _validator = Substitute.For<IValidator<MazeRequestModel>>();
            _validator.Validate(Arg.Any<MazeRequestModel>())
                .Returns(new FluentValidation.Results.ValidationResult());

            // Initialize the WebApplicationFactory and create the client
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Register mocked services
                        services.AddScoped(_ => _service);
                        services.AddScoped(_ => _validator);
                    });
                });

            _client = factory.CreateClient();
        }

        [Test]
        public async Task Get_ReturnsAllMazes()
        {
            // Arrange
            var response = await _client.GetAsync("/api/mazes");

            // Act
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var mazes = (await response.Content.ReadFromJsonAsync<IEnumerable<MazeConfiguration>>())?.ToList() ?? [];

            // Assert
            Assert.That(mazes, Is.Not.Null);
            Assert.That(mazes.Count, Is.GreaterThan(0));
            foreach (var maze in mazes)
            {
                Assert.That(maze.Maze, Is.Not.Null);
                Assert.That(maze.Maze, Is.Not.Empty);
            }
        }

        [Test]
        public async Task Submit_ValidMaze_ReturnsSolution()
        {
            // Arrange
            var mazeRequest = new MazeRequestModel
            {
                Content = "S__\n_X_\n__G"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/mazes", mazeRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var solution = await response.Content.ReadAsStringAsync();
            Assert.That(solution, Is.Not.Null);
            Assert.That(solution, Is.Not.Empty);
        }

        [Test]
        public async Task Submit_InvalidMaze_ReturnsValidationError()
        {
            // Arrange
            var mazeRequest = new MazeRequestModel { Content = "" };

            _validator.Validate(Arg.Any<MazeRequestModel>())
                .Returns(new FluentValidation.Results.ValidationResult { Errors = [new ()] });

            // Act
            var response = await _client.PostAsJsonAsync("/api/mazes", mazeRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
                
        [Test]
        public async Task Submit_SolutionNotFound_ReturnsProblem()
        {
            // Arrange
            var mazeRequest = new MazeRequestModel
            {
                Content = "S__X\n_X_X\n_X_G"
            };
            _service.Submit(Arg.Any<MazeConfigurationBase<string?, Guid>>())
                .Returns((string?)null);

            // Act
            var response = await _client.PostAsJsonAsync("/api/mazes", mazeRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var problem = await response.Content.ReadAsStringAsync();
            Assert.That(problem, Does.Contain("Solution for provided maze was not found."));
        }

        [Ignore("The test is currently broken: fix should be provided soon")]
        [Test]
        public async Task TimeoutMiddleware_ReturnsRequestTimeout()
        {
            // Arrange
            // Simulate a long-running request by adding a delay to the mocked service
            _service
                .When(s => s.GetAll())
                .Do(async _ => await Task.Delay(TimeSpan.FromSeconds(11)));

            // Act
            var response = await _client.GetAsync("/api/mazes");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            var error = await response.Content.ReadAsStringAsync();
            Assert.That(error, Does.Contain("An unexpected error occurred."));
        }

        [Test]
        public async Task ExceptionMiddleware_ReturnsServerError()
        {
            // Arrange
            _service.GetAll().ThrowsForAnyArgs(new Exception("An unexpected error occurred."));

            // Act
            var response = await _client.GetAsync("/api/mazes");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            var error = await response.Content.ReadAsStringAsync();
            Assert.That(error, Does.Contain("An unexpected error occurred."));
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up or dispose resources if needed
            _client.Dispose();
        }
    }
}
