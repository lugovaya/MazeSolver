namespace MazeSolver.Domain
{
    /// <summary>
    /// A simple BFS implementation for solving the maze 
    /// </summary>
    public class BFSMazeSolver : IStringBasedMazeSolver
    {
        private static readonly (int, int)[] Directions =
        [
            (-1, 0), // Up
            (1, 0),  // Down
            (0, -1), // Left
            (0, 1)   // Right
        ];

        public string? Solve(string maze)
        {
            var rows = maze.Split('\n');
            var start = FindPoint(maze, 'S');
            var goal = FindPoint(maze, 'G');

            if (start == (-1, -1) || goal == (-1, -1))
            {
                return null; // Start or goal is missing
            }

            var queue = new Queue<(int, int)>();
            var visited = new bool[rows.Length, rows[0].Length];
            var parent = new (int, int)[rows.Length, rows[0].Length];

            queue.Enqueue(start);
            visited[start.X, start.Y] = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                if ((x, y) == goal)
                {
                    return ReconstructPath(parent, start, goal);
                }

                foreach (var (dx, dy) in Directions)
                {
                    var nx = x + dx;
                    var ny = y + dy;

                    if (IsValidMove(nx, ny, rows, visited))
                    {
                        visited[nx, ny] = true;
                        parent[nx, ny] = (x, y);
                        queue.Enqueue((nx, ny));
                    }
                }
            }

            return null; // No solution found
        }

        private static (int X, int Y) FindPoint(string maze, char point)
        {
            var rows = maze.Split('\n');
            for (var i = 0; i < rows.Length; i++)
            {
                var index = rows[i].IndexOf(point);
                if (index != -1)
                {
                    return (i, index);
                }
            }
            return (-1, -1);
        }

        private static bool IsValidMove(int x, int y, string[] rows, bool[,] visited)
        {
            return x >= 0 && x < rows.Length && y >= 0 && y < rows[x].Length &&
                   rows[x][y] != 'X' && !visited[x, y];
        }

        private static string ReconstructPath((int, int)[,] parent, (int X, int Y) start, (int X, int Y) goal)
        {
            var path = new List<string>();
            var current = goal;

            while (current != start)
            {
                path.Add($"({current.X},{current.Y})");
                current = parent[current.X, current.Y];
            }
            path.Add($"({start.X},{start.Y})");
            path.Reverse();
            return string.Join(" -> ", path);
        }
    }
}
