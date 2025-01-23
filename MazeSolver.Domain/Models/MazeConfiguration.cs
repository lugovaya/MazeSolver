namespace MazeSolver.Domain.Models
{
    public abstract class MazeConfigurationBase<T, TKey>
    {
        public abstract TKey Id { get; set; }
        public abstract T Maze { get; }
        public abstract T Solution { get; set; }
    }

    public class MazeConfiguration(string? maze) : MazeConfigurationBase<string?, Guid>
    {
        public override Guid Id { get; set; }
        public override string? Maze { get; } = maze;
        public override string? Solution { get; set; }
    }
}
