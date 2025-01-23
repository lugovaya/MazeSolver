using MazeSolver.Domain.DataAccess;
using MazeSolver.Domain.Models;
using System.Text.RegularExpressions;

namespace MazeSolver.Domain.Services
{
    public class MazeService(
        IRepository<MazeConfiguration, Guid> mazeRepository,
        IMazeSolver mazeSolver) : IMazeService<Guid, string?> // TODO: consider to introduce an OperationResult<T> type to return the result and the error message
    {
        private readonly IRepository<MazeConfiguration, Guid> _mazeRepository = mazeRepository;
        private readonly IMazeSolver mazeSolver = mazeSolver;

        public MazeConfigurationBase<string?, Guid> Get(Guid id)
        {
            return _mazeRepository.Get(id);
        }

        public IEnumerable<MazeConfigurationBase<string?, Guid>> GetAll()
        {
            return _mazeRepository.GetAll();
        }

        public string? Submit(MazeConfigurationBase<string?, Guid> mazeConfiguration)
        {
            GuardAgainstInvalidMaze(mazeConfiguration.Maze!);

            var solution = mazeSolver.Solve(mazeConfiguration.Maze!);
            if (solution == null)
            {
                return null;
            }

            _mazeRepository.Add(mazeConfiguration.AsNewMazeEntity(solution));
            return solution;
        }

        // Guard clause for invalid maze configuration
        private static void GuardAgainstInvalidMaze(string mazeData)
        {
            var rows = mazeData.Split('\n');
            var startCount = mazeData.Count(c => c == 'S');
            var goalCount = mazeData.Count(c => c == 'G');

            // Define the validation rules as tuples containing the condition and the error message
            var validations = new List<(bool condition, string errorMessage)>
            {
                (rows.Length > 20 || rows.Any(row => row.Length > 20), "Maze exceeds the allowed size of 20x20."),
                (startCount != 1 || goalCount != 1, "Maze must contain exactly one start point (S) and one goal point (G)."),
                (!Regex.IsMatch(mazeData!, "^[SGX_\n]*$"), "Maze contains invalid characters."),
                (startCount == 0 || goalCount == 0, "Start or goal point is missing.")
            };

            // Collect all error messages for failed validations
            var validationErrors = validations
                .Where(validation => validation.condition)
                .Select(validation => validation.errorMessage)
                .ToList();

            // If there are validation errors, throw an exception with the combined error messages
            if (validationErrors.Any())
            {
                throw new ArgumentException(string.Join(" ", validationErrors));
            }
        }
    }
}
