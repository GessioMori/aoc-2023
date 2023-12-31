using Main.Tools;

namespace Main.Solutions
{
    internal class Day21Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            char[,] grid = Funcs.ConvertStringArrayToCharGrid(inputData);

            (int startX, int startY) = FindStartingPoint(grid);

            long numOfSteps = 64;

            List<(int, int)> visited = [];

            List<(int, int)> result = [];

            Queue<(int, int, long)> queue = new Queue<(int, int, long)>();

            queue.Enqueue((startX, startY, numOfSteps));

            visited.Add((startX, startY));

            (int, int)[] movements = [(1, 0), (-1, 0), (0, 1), (0, -1)];

            while (queue.Count > 0)
            {
                (int x, int y, long steps) = queue.Dequeue();

                if (steps % 2 == 0)
                {
                    result.Add((x, y));
                }
                if (steps == 0)
                {
                    continue;
                }

                foreach ((int, int) movement in movements)
                {
                    (int, int) newPosition = (x + movement.Item1, y + movement.Item2);

                    if (newPosition.Item1 >= 0 && newPosition.Item2 >= 0 &&
                        newPosition.Item1 < grid.GetLength(0) && newPosition.Item2 < grid.GetLength(1) &&
                        grid[newPosition.Item1, newPosition.Item2] != '#' &&
                        !visited.Any(c => c.Item1 == newPosition.Item1 && c.Item2 == newPosition.Item2))
                    {
                        visited.Add((newPosition.Item1, newPosition.Item2));
                        queue.Enqueue((newPosition.Item1, newPosition.Item2, steps - 1));
                    }
                }
            }

            return result.Count.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            throw new NotImplementedException();
        }

        public static (int, int) FindStartingPoint(char[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == 'S')
                    {
                        return (i, j);
                    }
                }
            }

            throw new Exception("Starting cell not found.");
        }
    }
}
