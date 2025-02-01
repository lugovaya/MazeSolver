using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MazeSolver.Domain
{
    /// <summary>
    /// A simple DFS implementation for solving the maze
    /// </summary>
    public class DFSMazeSolver : IStringBasedMazeSolver
    {
        private static readonly (int X, int Y)[] Directions =
        [
            (-1, 0), // Up
            (1, 0),  // Down
            (0, -1), // Left
            (0, 1)   // Right
        ];

        public string? Solve(string maze)
        {
            // Convert maze string to 2D array for easier processing
            var mazeArray = ConvertTo2DArray(maze);
            var rows = mazeArray.GetLength(0);
            var columns = mazeArray.GetLength(1);

            var start = FindPoint(mazeArray, rows, columns, 'S');
            var goal = FindPoint(mazeArray, rows, columns, 'G');

            if (start == null || goal == null)
            {
                return null; // Start or goal is missing
            }

            // Stack for DFS traversal
            var stack = new Stack<(int row, int column, List<(int, int)> path)>();
            stack.Push((start.Value.X, start.Value.Y, new List<(int, int)> { start.Value }));

            var visited = new bool[rows, columns];
            visited[start.Value.X, start.Value.Y] = true;

            // DFS algorithm
            while (stack.Count > 0)
            {
                var (currentRow, currentColumn, currentPath) = stack.Pop();

                // If we've reached the goal, return the path
                if (currentRow == goal.Value.X && currentColumn == goal.Value.Y)
                {
                    return PathToString(currentPath);
                }

                // Explore all 4 directions
                for (int i = 0; i < 4; i++)
                {
                    var (x, y) = Directions[i];
                    var newRow = currentRow + x;
                    var newColumn = currentColumn + y;

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

        private static (int X, int Y)? FindPoint(char[,] maze, int rows, int columns, char point)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == point)
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
        private static string PathToString(List<(int X, int Y)> path)
        {
            return string.Join("->", path.Select(point => $"({point.X}, {point.Y})")).Trim();
        }
    }
}
