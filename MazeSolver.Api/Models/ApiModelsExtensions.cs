using MazeSolver.Domain.Models;

namespace MazeSolver.Api.Models
{
    internal static class ApiModelsExtensions
    {
        public static MazeConfigurationBase<string?, Guid> ToDomainModel(this MazeRequestModel mazeRequest)
        {
            return new MazeConfiguration(mazeRequest.Content);
        }
    }
}
