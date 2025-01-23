namespace MazeSolver.Domain.Models
{
    internal static class ModelsExtensions
    {
        public static MazeConfiguration AsNewMazeEntity(this MazeConfigurationBase<string?, Guid> mazeConfiguration, string? solution)
        {
            return new MazeConfiguration(mazeConfiguration.Maze)
            {
                Id = Guid.NewGuid(),
                Solution = solution
            };
        }
    }
}
