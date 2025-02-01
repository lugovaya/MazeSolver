namespace MazeSolver.Domain
{
    public class DFSRecursiveMazeSolver : IStringBasedMazeSolver
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
            var mazeArray = ConvertTo2DArray(maze);
            var rows = mazeArray.GetLength(0);
            var columns = mazeArray.GetLength(1);

            var start = FindPoint(mazeArray, rows, columns, 'S');
            var goal = FindPoint(mazeArray, rows, columns, 'G');

            if (start == null || goal == null)
            {
                return null; // Start or goal is missing
            }

            var visited = new bool[rows, columns];
            var path = new List<(int, int)>();

            if (DFSImplementation(mazeArray, visited, goal.Value, path, start.Value.X, start.Value.Y, rows, columns))
            {
                return PathToString(path);
            }

            return null; // No solution found
        }

        private static bool DFSImplementation(
            char[,] maze,
            bool[,] visited, 
            (int X, int Y) goal,
            List<(int, int)> path,
            int rowIndex,
            int columnIndex, int rowsCount,
            int columnsCount,
            int recursionDepth = 0)
        {
            if (recursionDepth > rowsCount * columnsCount)
            {
                return false;
            }

            if (rowIndex < 0 || columnIndex < 0 || 
                rowIndex >= rowsCount || columnIndex >= columnsCount || 
                visited[rowIndex, columnIndex] || maze[rowIndex, columnIndex] == 'X')
            {
                return false;
            }

            // Mark the cell as visited
            visited[rowIndex, columnIndex] = true;
            path.Add((rowIndex, columnIndex));

            // If we reached the goal, return true
            if (rowIndex == goal.X && columnIndex == goal.Y)
            {
                return true;
            }

            // Explore all 4 directions recursively
            for (int i = 0; i < 4; i++)
            {
                var (x, y) = Directions[i];
                if (DFSImplementation(maze,
                    visited, goal,
                    path,
                    rowIndex + x,
                    columnIndex + y, rowsCount,
                    columnsCount,
                    recursionDepth++))
                {
                    return true;
                }
            }

            // Backtrack if no path is found
            path.RemoveAt(path.Count - 1);
            return false;
        }

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

        private static string PathToString(List<(int, int)> path)
        {
            return string.Join("->", path.Select(p => $"({p.Item1},{p.Item2})"));
        }
    }
}   
