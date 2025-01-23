namespace MazeSolver.Domain
{
    /// <summary>
    /// A simple DFS implementation for solving the maze
    /// </summary>
    public class DFSMazeSolver : IMazeSolver
    {
        private readonly int[] _rowDirections = [-1, 1, 0, 0]; // Up, Down, Left, Right
        private readonly int[] _colDirections = [0, 0, -1, 1];

        public string? Solve(string maze)
        {
            // Convert maze string to 2D array for easier processing
            var mazeArray = ConvertTo2DArray(maze);
            var rows = mazeArray.GetLength(0);
            var columns = mazeArray.GetLength(1);

            var start = FindStart(mazeArray, rows, columns);
            var goal = FindGoal(mazeArray, rows, columns);

            if (start == null || goal == null)
            {
                return null; // Start or goal is missing
            }

            // Stack for DFS traversal
            var stack = new Stack<(int row, int col, List<(int, int)> path)>();
            stack.Push((start.Value.Item1, start.Value.Item2, new List<(int, int)> { start.Value }));

            var visited = new bool[rows, columns];
            visited[start.Value.Item1, start.Value.Item2] = true;

            // DFS algorithm
            while (stack.Count > 0)
            {
                var (currentRow, currentColumn, currentPath) = stack.Pop();

                // If we've reached the goal, return the path
                if (currentRow == goal.Value.Item1 && currentColumn == goal.Value.Item2)
                {
                    return PathToString(currentPath);
                }

                // Explore all 4 directions
                for (int i = 0; i < 4; i++)
                {
                    var newRow = currentRow + _rowDirections[i];
                    var newColumn = currentColumn + _colDirections[i];

                    if (IsValidMove(mazeArray, newRow, newColumn, visited, rows, columns))
                    {
                        visited[newRow, newColumn] = true;
                        var newPath = new List<(int, int)>(currentPath) { (newRow, newColumn) };
                        stack.Push((newRow, newColumn, newPath));
                    }
                }
            }
            
            return null; // No solution found
        }

        // Convert maze string to a 2D char array for easier manipulation
        private static char[,] ConvertTo2DArray(string mazeData)
        {
            var rows = mazeData.Trim().Split('\n');
            var maxColumns = rows.Max(row => row.Length);
            var maze = new char[rows.Length, maxColumns];

            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    maze[i, j] = rows[i][j];
                }
            }

            return maze;
        }

        // Find the start (S) position in the maze
        private static (int, int)? FindStart(char[,] maze, int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == 'S')
                    {
                        return (i, j);
                    }
                }
            }
            return null;
        }

        // Find the goal (G) position in the maze
        private static (int, int)? FindGoal(char[,] maze, int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == 'G')
                    {
                        return (i, j);
                    }
                }
            }
            return null;
        }

        // Check if the current move is valid
        private static bool IsValidMove(char[,] maze, int rowNumber, int columnNumber, bool[,] visited, int rowsCount, int columnsCount)
        {
            // Check bounds first to avoid IndexOutOfRangeException
            if (rowNumber < 0 || columnNumber < 0 || rowNumber >= rowsCount || columnNumber >= columnsCount)
            {
                return false;
            }

            // Check if the cell is open (empty space) and not visited
            return maze[rowNumber, columnNumber] != 'X' && !visited[rowNumber, columnNumber];
        }

        // Convert the path (list of coordinates) to a string
        private static string PathToString(List<(int, int)> path)
        {
            var pathString = string.Empty;

            foreach (var (row, col) in path)
            {
                pathString += $"({row},{col}) ";
            }

            return pathString.Trim();
        }
    }
}
