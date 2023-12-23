using Main.Tools;
using System.Collections.Generic;
using System.Formats.Tar;

namespace Main.Solutions
{
    internal class Day17Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            int[,] grid = Funcs.ConvertStringArrayToGrid(inputData);

            int result = Dijkstra(grid);

            return result.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            throw new NotImplementedException();
        }

        static int Dijkstra(int[,] grid)
        {
            int[] dx = [1, -1, 0, 0];
            int[] dy = [0, 0, 1, -1];

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int maxStraightLine = Math.Max(rows, cols);

            int[,] parentX = new int[rows, cols];
            int[,] parentY = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    parentX[i, j] = -1;
                    parentY[i, j] = -1;
                }
            }

            int[,,,] distance = new int[rows, cols, maxStraightLine, maxStraightLine];
            bool[,,,] visited = new bool[rows, cols, maxStraightLine, maxStraightLine];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    for (int k = 0; k < maxStraightLine; k++)
                    {
                        for (int l = 0; l < maxStraightLine; l++)
                        {
                            distance[i, j, k, l] = int.MaxValue;
                            visited[i, j, k, l] = false;
                        }
                    }
                }
            }

            PriorityQueue<(int, int, int, int), int> pq = new();

            pq.Enqueue((0, 0, 0, 0), 0);

            while (pq.Count > 0)
            {
                var ((x, y, sameDirectionSteps, direction), dist) = pq.Dequeue();

                if (x == rows - 1 && y == cols - 1)
                {
                    //PrintPath(rows, cols, parentX, parentY);
                    return dist;
                }

                if (visited[x, y, sameDirectionSteps, direction]) continue;

                visited[x, y, sameDirectionSteps, direction] = true;

                if (x == 0 && y == 0)
                {
                    for (int k = 0; k < maxStraightLine; k++)
                    {
                        for (int l = 0; l < maxStraightLine; l++)
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

                    int nextStepsNumber = nextStepDirection == direction ? sameDirectionSteps + 1 : 0;

                    bool isReturning = Math.Abs(direction - nextStepDirection) == 2;

                    if (IsValid(nx, ny, rows, cols) && !visited[nx, ny, nextStepDirection, nextStepsNumber] && !isReturning && nextStepsNumber < 3)
                    {
                        int calcDistance = (dist + grid[nx, ny]);

                        if (distance[nx, ny, nextStepsNumber, nextStepDirection] > calcDistance)
                        {
                            distance[nx, ny, nextStepsNumber, nextStepDirection] = calcDistance;
                            pq.Enqueue((nx, ny, nextStepsNumber, nextStepDirection), calcDistance);
                            parentX[nx, ny] = x;
                            parentY[nx, ny] = y;
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

        static void PrintPath(int Rows, int Cols, int[,] parentX, int[,] parentY)
        {
            List<(int, int)> path = new List<(int, int)>();
            int x = Rows - 1;
            int y = Cols - 1;

            while (x != 0 || y != 0)
            {
                path.Add((x, y));
                int prevX = parentX[x, y];
                int prevY = parentY[x, y];
                x = prevX;
                y = prevY;
            }

            path.Add((0, 0));
            path.Reverse();

            foreach (var (px, py) in path)
            {
                Console.Write($"({px}, {py}) ");
            }

            Console.WriteLine();
        }


    }

    class PriorityQueue<T, U> where U : IComparable<U>
    {
        private List<(T, U)> heap = [];

        public int Count => heap.Count;

        public void Enqueue(T item, U priority)
        {
            heap.Add((item, priority));
            int i = heap.Count - 1;
            while (i > 0)
            {
                int j = (i - 1) / 2;
                if (heap[i].Item2.CompareTo(heap[j].Item2) >= 0)
                    break;
                Swap(i, j);
                i = j;
            }
        }

        public (T, U) Dequeue()
        {
            int lastIndex = heap.Count - 1;
            (T, U) frontItem = heap[0];
            heap[0] = heap[lastIndex];
            heap.RemoveAt(lastIndex);

            lastIndex--;
            int i = 0;
            while (true)
            {
                int leftIndex = 2 * i + 1;
                if (leftIndex > lastIndex)
                    break;
                int rightIndex = leftIndex + 1;
                if (rightIndex <= lastIndex && heap[rightIndex].Item2.CompareTo(heap[leftIndex].Item2) < 0)
                    leftIndex = rightIndex;
                if (heap[leftIndex].Item2.CompareTo(heap[i].Item2) >= 0)
                    break;
                Swap(i, leftIndex);
                i = leftIndex;
            }

            return frontItem;
        }

        private void Swap(int i, int j)
        {
            (T, U) temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }
    }
}
