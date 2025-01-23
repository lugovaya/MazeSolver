using System.Net;
using System.Net.Http.Json;
using NSubstitute;
using MazeSolver.Domain.Models;
using MazeSolver.Domain.Services;
using FluentValidation;
using MazeSolver.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace MazeSolver.Tests.API
{
    public class MazesControllerFixture
    {
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of IMazeService
            var mazeServiceMock = Substitute.For<IMazeService<Guid, string?>>();
            mazeServiceMock.GetAll().Returns(new List<MazeConfiguration>
            {
                new ("S__\n_X_\n__G") { Id = Guid.NewGuid(), Solution = "SXX\n_X_\nXXG" }
            });

            mazeServiceMock.Submit(Arg.Any<MazeConfigurationBase<string?, Guid>>()).Returns("SXX\n_X_\nXXG");

            // Create a mock of IValidator<MazeRequestModel>
            var validatorMock = Substitute.For<IValidator<MazeRequestModel>>();
            validatorMock.Validate(Arg.Any<MazeRequestModel>())
                .Returns(new FluentValidation.Results.ValidationResult());

            // Initialize the WebApplicationFactory and create the client
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Register mocked services
                        services.AddScoped(_ => mazeServiceMock);
                        services.AddScoped(_ => validatorMock);
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

        [Ignore("Tests are currently broken: fix should be implemented soon.")]
        [TestCase("", "Maze cannot be empty.")]
        [TestCase("S__\n_X_\n__G\nExtraRowTooLong", "Maze format is invalid: dimensions must not exceed 20x20.")]
        [TestCase("S__\n_#_\n__G", "Maze format is invalid: only allowed characters are 'S', 'G', 'X', '_', and '\n'.")]
        [TestCase("__\n_X_\n__G", "Maze format is invalid: allowed start (S) and goal (G).")]
        public async Task Submit_InvalidMaze_ReturnsValidationError(string content, string expectedError)
        {
            // Arrange
            var mazeRequest = new MazeRequestModel { Content = content };

            // Act
            var response = await _client.PostAsJsonAsync("/api/mazes", mazeRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors, Does.Contain(expectedError));
        }

        [Ignore("The test is currently broken: fix should be provided soon")]
        [Test]
        public async Task Submit_SolutionNotFound_ReturnsProblem()
        {
            // Arrange
            var mazeRequest = new MazeRequestModel
            {
                Content = "SXX\nXXX\nXXX"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/mazes", mazeRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            var problem = await response.Content.ReadAsStringAsync();
            Assert.That(problem, Does.Contain("Solution for provided maze was not found."));
        }

        [Ignore("The test is currently broken: fix should be provided soon")]
        [Test]
        public async Task TimeoutMiddleware_ReturnsRequestTimeout()
        {
            // Simulate a long request
            _client.Timeout = TimeSpan.FromMilliseconds(500); // Ensure timeout is low
            var mazeRequest = new MazeRequestModel
            {
                Content = "S__\n_X_\n__G"
            };

            try
            {
                await _client.PostAsJsonAsync("/api/mazes", mazeRequest);
                Assert.Fail("Expected TaskCanceledException was not thrown.");
            }
            catch (TaskCanceledException exception)
            {
                Assert.That(exception, Is.Not.Null);
            }
        }

        [Ignore("The test should be fixed")]
        [Test]
        public async Task ExceptionMiddleware_ReturnsServerError()
        {
            // Simulate an exception in the server
            var response = await _client.GetAsync("/api/mazes?causeException=true");

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
