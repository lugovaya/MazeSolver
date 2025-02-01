using FluentValidation;
using MazeSolver.Api.Models;
using MazeSolver.Api.Validators;
using System.Net;

namespace MazeSolver.Tests;

public class MazeRequestValidatorFixture
{
    private MazeRequestValidator _validator = new();

    [SetUp]
    public void Setup()
    {
    }

    [TestCase("", "Maze cannot be empty.")]
    [TestCase("S_____X_____XXXX_____G\n", "Maze format is invalid: dimensions must not exceed 20x20.")]
    [TestCase("S__\n_#_\n__G", "Maze format is invalid: only allowed characters are 'S', 'G', 'X', '_', and '\n'.")]
    [TestCase("__\n_X_\n__G", "Maze format is invalid: allowed exactly one start (S) and goal (G).")]
    public async Task ShouldReturnValidationError(string content, string expectedError)
    {
        // Arrange
        var mazeRequest = new MazeRequestModel { Content = content };

        // Act
        var result = await _validator.ValidateAsync(mazeRequest);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Is.Not.Null);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(expectedError));
    }
}
