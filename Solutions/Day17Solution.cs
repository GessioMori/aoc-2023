using Main.Tools;

namespace Main.Solutions
{
    internal class Day17Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            int[,] grid = Funcs.ConvertStringArrayToGrid(inputData);

            int result = Dijkstra(grid, 3, false);

            return result.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            int[,] grid = Funcs.ConvertStringArrayToGrid(inputData);

            int result = Dijkstra(grid, 10, true);

            return result.ToString();
        }

        static int Dijkstra(int[,] grid, int maxStepsNumber, bool hasMinSteps)
        {
            int[] dx = [1, -1, 0, 0];
            int[] dy = [0, 0, 1, -1];

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int maxStraightLine = Math.Max(rows, cols);

            int[,,,] distance = new int[rows, cols, maxStraightLine, 4];
            bool[,,,] visited = new bool[rows, cols, maxStraightLine, 4];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    for (int k = 0; k < maxStraightLine; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            distance[i, j, k, l] = int.MaxValue;
                            visited[i, j, k, l] = false;
                        }
                    }
                }
            }

            Tools.PriorityQueue<(int, int, int, int), int> pq = new();

            pq.Enqueue((0, 0, 0, 0), 0);

            while (pq.Count > 0)
            {
                var ((x, y, sameDirectionSteps, direction), dist) = pq.Dequeue();

                if (x == rows - 1 && y == cols - 1 && (!hasMinSteps || sameDirectionSteps >= 4))
                {
                    return dist;
                }

                if (visited[x, y, sameDirectionSteps, direction]) continue;

                visited[x, y, sameDirectionSteps, direction] = true;

                if (x == 0 && y == 0)
                {
                    for (int k = 0; k < maxStraightLine; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            distance[0, 0, k, l] = 0;
                            visited[0, 0, k, l] = true;
                        }
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    int nextStepDirection;

                    if (dx[i] == 1) nextStepDirection = 0;
                    else if (dy[i] == 1) nextStepDirection = 1;
                    else if (dx[i] == -1) nextStepDirection = 2;
                    else nextStepDirection = 3;

                    int nextStepsNumber = nextStepDirection == direction ? sameDirectionSteps + 1 : 1;

                    bool isReturning = Math.Abs(direction - nextStepDirection) == 2;

                    if (IsValid(nx, ny, rows, cols) &&
                        !visited[nx, ny, nextStepsNumber, nextStepDirection] &&
                        !isReturning &&
                        nextStepsNumber <= maxStepsNumber &&
                        (!hasMinSteps || (nextStepDirection == direction || sameDirectionSteps >= 4 || (x == 0 && y == 0))))
                    {
                        int calcDistance = (dist + grid[nx, ny]);

                        if (distance[nx, ny, nextStepsNumber, nextStepDirection] > calcDistance)
                        {
                            distance[nx, ny, nextStepsNumber, nextStepDirection] = calcDistance;
                            pq.Enqueue((nx, ny, nextStepsNumber, nextStepDirection), calcDistance);
                        }
                    }
                }
            }

            return -1;
        }

        static bool IsValid(int x, int y, int Rows, int Cols)
        {
            return x >= 0 && x < Rows && y >= 0 && y < Cols;
        }
    }


}
