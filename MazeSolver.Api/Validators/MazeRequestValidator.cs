using FluentValidation;
using MazeSolver.Api.Models;
using System.Text.RegularExpressions;

namespace MazeSolver.Api.Validators
{
    public class MazeRequestValidator : AbstractValidator<MazeRequestModel>
    {
        private const int MaxMazeHeight = 20;
        private const int MaxMazeWidth = 20;

        public MazeRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Maze cannot be empty.")
                .Must(ContainValidDemensions).WithMessage("Maze format is invalid: dimensions must not exceed 20x20.")
                .Must(ContainAllowedCharacters).WithMessage("Maze format is invalid: only allowed characters are 'S', 'G', 'X', '_', and '\n'.")
                .Must(ContainExactlyOneStartAndGoal).WithMessage("Maze format is invalid: allowed exactly one start (S) and goal (G).");
        }

        private bool ContainValidDemensions(string? maze)
        {
            var rows = maze!.Split('\n');
            return rows.Length <= MaxMazeHeight && rows.All(row => row.Length <= MaxMazeWidth);
        }

        private bool ContainAllowedCharacters(string? maze)
        {
            return Regex.IsMatch(maze!, "^[SGX_\n]*$");
        }

        private bool ContainExactlyOneStartAndGoal(string? maze)
        {
            return Regex.IsMatch(maze!, "^(?=[^S]*S[^S]*$)(?=[^G]*G[^G]*$)[^SG]*S[^SG]*G[^SG]*$");
        }
    }
}
