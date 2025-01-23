using MazeSolver.Domain;

namespace MazeSolver.Tests
{
    [TestFixture]
    public class MazeSolverFixture
    {
        private static readonly List<TestCaseData> MazeTestCases =
        [
            // Valid maze with a solution
            new TestCaseData(new BFSMazeSolver(), @"
S_________
_XXXXXXXX_
_X______X_
_X_XXXX_X_
_X_X__X_X_
_X_X__X_X_
_X_X____X_
_X_XXXXXX_
_X________
XXXXXXXXG_", true)
            .SetName("BFS_Solves_ValidMaze"),
            new TestCaseData(new DFSMazeSolver(), @"
S_________
_XXXXXXXX_
_X______X_
_X_XXXX_X_
_X_X__X_X_
_X_X__X_X_
_X_X____X_
_X_XXXXXX_
_X________
XXXXXXXXG_", true)
            .SetName("DFS_Solves_ValidMaze"),

            // Unsolvable maze
            new TestCaseData(new BFSMazeSolver(), @"
SXXXXXXXX_
_XXXXXXXX_
_XXXXXXX__
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
XXXXXXXXG_", false)
            .SetName("BFS_Solver_Fails_UnsolvableMaze"),
            new TestCaseData(new DFSMazeSolver(), @"
SXXXXXXXX_
_XXXXXXXX_
_XXXXXXX__
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
_XXXXXXXX_
XXXXXXXXG_", false)
            .SetName("DFS_Solver_Fails_UnsolvableMaze"),

            // Maze missing the start point
            new TestCaseData(new BFSMazeSolver(), @"
__________
_XXXXXXXX_
_X______X_
_X_XXXX_X_
_X_X__X_X_
_X_X__X_X_
_X_X____X_
_X_XXXXXX_
_X________
XXXXXXXXG_", false)
            .SetName("BFS_Solver_Fails_MazeWithoutStart"),
            new TestCaseData(new DFSMazeSolver(), @"
__________
_XXXXXXXX_
_X______X_
_X_XXXX_X_
_X_X__X_X_
_X_X__X_X_
_X_X____X_
_X_XXXXXX_
_X________
XXXXXXXXG_", false)
            .SetName("DFS_Solver_Fails_MazeWithoutStart"),

            // Maze missing the goal point
            new TestCaseData(new BFSMazeSolver(), @"
S_________
_XXXXXXXX_
_X______X_
_X_XXXX_X_
_X_X__X_X_
_X_X__X_X_
_X_X____X_
_X_XXXXXX_
_X________
XXXXXXXX__", false)
            .SetName("BFS_Solver_Fails_MazeWithoutGoal"),
            new TestCaseData(new DFSMazeSolver(), @"
S_________
_XXXXXXXX_
_X______X_
_X_XXXX_X_
_X_X__X_X_
_X_X__X_X_
_X_X____X_
_X_XXXXXX_
_X________
XXXXXXXX__", false)
            .SetName("DFS_Solver_Fails_MazeWithoutGoal")
        ];

        [TestCaseSource(nameof(MazeTestCases))]
        public void SolveMazeTests(IMazeSolver solver, string maze, bool shouldSolve)
        {
            string cleanedMaze = CleanMazeString(maze);

            var result = solver.Solve(cleanedMaze);

            if (!shouldSolve)
            {
                Assert.That(result, Is.Null, $"{solver.GetType().Name} should fail for invalid maze input.");
            }
            else
            {                
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Not.Empty);
            }
        }

        private static string CleanMazeString(string maze)
        {
            // Remove leading/trailing whitespace from maze string for uniformity
            return maze.Trim('\r', '\n');
        }
    }
}
