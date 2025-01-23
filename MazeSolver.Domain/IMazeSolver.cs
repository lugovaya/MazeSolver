namespace MazeSolver.Domain
{
    public interface IMazeSolver // TODO: consider making this generic, e.g. <TParam, TResult>
    {
        /// <summary>
        /// Solve the maze and return the solution
        /// </summary>
        /// <param name="maze">The maze to solve</param>
        /// <returns>The solution to the maze</returns>
        string? Solve(string maze);
    }
}
