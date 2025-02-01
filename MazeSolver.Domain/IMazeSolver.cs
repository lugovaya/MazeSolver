namespace MazeSolver.Domain
{
    public interface IMazeSolver<TParam, TReturn>
    {
        /// <summary>
        /// Solve the maze and return the solution
        /// </summary>
        /// <param name="maze">The maze to solve</param>
        /// <returns>The solution to the maze</returns>
        TReturn Solve(TParam maze);
    }

    public interface IStringBasedMazeSolver : IMazeSolver<string, string?>
    {
    }
}
