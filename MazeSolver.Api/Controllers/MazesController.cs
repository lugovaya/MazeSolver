using FluentValidation;
using MazeSolver.Api.Models;
using MazeSolver.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MazeSolver.Api.Controllers
{
    [ApiController]
    [Route("api/mazes")]
    public class MazesController(
        IMazeService<Guid, string?> mazeService,
        IValidator<MazeRequestModel> mazeValidator) : ControllerBase
    {
        private readonly IMazeService<Guid, string?> _mazeService = mazeService;
        private readonly IValidator<MazeRequestModel> _mazeValidator = mazeValidator;

        /// <summary>
        /// Get a list of all previously submitted mazes and their solutions
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetMazes")]
        public IResult Get()
        {
            var mazes = _mazeService.GetAll();
            return Results.Ok(mazes);
        }

        /// <summary>
        /// Submit a new maze configuration, returning one possible solution
        /// </summary>
        /// <returns></returns>
        [HttpPost(Name = "SubmitMaze")]
        public IResult Submit(MazeRequestModel model)
        {
            var validationResult = _mazeValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var solution = _mazeService.Submit(model.ToDomainModel());

            return solution != null
                ? Results.Created("api/mazes", new { solution })
                : Results.NotFound("Solution for provided maze was not found.");
        }

        [HttpGet("random", Name = "GenerateRandomMaze")]
        public IResult GenerateRandomMaze(int width = 10, int height = 10)
        {
            var random = new Random();

            // Ensure the start and goal positions are within bounds
            var maze = new char[height, width];

            // Fill the maze with random values: 'X' for walls and '_' for empty spaces
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    maze[i, j] = random.Next(0, 2) == 0 ? 'X' : '_'; // 50% chance for wall or empty space
                }
            }

            // Ensure there's a start 'S' and a goal 'G'
            var startX = random.Next(0, height);
            var startY = random.Next(0, width);
            var goalX = random.Next(0, height);
            var goalY = random.Next(0, width);

            // Ensure start and goal are not in the same position
            while (startX == goalX && startY == goalY)
            {
                goalX = random.Next(0, height);
                goalY = random.Next(0, width);
            }

            maze[startX, startY] = 'S'; // Set the start position
            maze[goalX, goalY] = 'G';   // Set the goal position

            // Convert the maze to a string for easy handling
            var mazeString = string.Empty;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mazeString += maze[i, j];
                }
                mazeString += '\n'; // Add a new line after each row
            }

            return Results.Ok(new MazeRequestModel
            {
                Content = mazeString.TrimEnd()
            });
        }
    }
}
