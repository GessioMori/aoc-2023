using Main.Tools;
using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day18Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<DiggingStep> steps = inputData.Select(l => ProcessLine(l, 'A')).ToList();

            int maximumLength = steps.Sum(ds => ds.NumOfBlocks * 2);

            bool[,] grid = new bool[maximumLength, maximumLength];

            (int, int) currentBlock = (maximumLength / 2, maximumLength / 2);

            grid[currentBlock.Item1, currentBlock.Item2] = true;

            for (int s = 0; s < steps.Count; s++)
            {
                DiggingStep currentStep = steps[s];
                DiggingStep nextStep = steps[s == steps.Count - 1 ? s : 0];

                if (currentStep.DirectionSymbol == "R")
                {
                    for (int i = currentBlock.Item2 + 1; i <= currentBlock.Item2 + currentStep.NumOfBlocks; i++)
                    {
                        grid[currentBlock.Item1, i] = true;
                    }

                    currentBlock.Item2 += currentStep.NumOfBlocks;
                }
                else if (currentStep.DirectionSymbol == "L")
                {
                    for (int i = currentBlock.Item2 - 1; i >= currentBlock.Item2 - currentStep.NumOfBlocks; i--)
                    {
                        grid[currentBlock.Item1, i] = true;
                    }

                    currentBlock.Item2 -= currentStep.NumOfBlocks;
                }
                else if (currentStep.DirectionSymbol == "U")
                {
                    for (int i = currentBlock.Item1 - 1; i >= currentBlock.Item1 - currentStep.NumOfBlocks; i--)
                    {
                        grid[i, currentBlock.Item2] = true;
                    }

                    currentBlock.Item1 -= currentStep.NumOfBlocks;
                }
                else if (currentStep.DirectionSymbol == "D")
                {
                    for (int i = currentBlock.Item1 + 1; i <= currentBlock.Item1 + currentStep.NumOfBlocks; i++)
                    {
                        grid[i, currentBlock.Item2] = true;
                    }

                    currentBlock.Item1 += currentStep.NumOfBlocks;
                }
            }

            FloodFill(grid, maximumLength / 2 + 1, maximumLength / 2 + 1);

            int total = 0;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j])
                    {
                        total++;
                    }
                }
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            List<DiggingStep> steps = inputData.Select(l => ProcessLine(l, 'B')).ToList();

            List<(double, double)> listOfVertex = CreateListOfVertex(steps);

            double partialArea = 0;

            for (int i = 0; i < listOfVertex.Count; i++)
            {
                partialArea +=
                    (listOfVertex[i].Item1 *
                        (listOfVertex[i == 0 ? ^1 : i - 1].Item2 - listOfVertex[(i + 1) % listOfVertex.Count].Item2)) * 0.5;
            }

            partialArea = Math.Abs(partialArea);

            double numberOfExternalBlocks = steps.Sum(step => step.NumOfBlocks);

            double internalArea = partialArea - (numberOfExternalBlocks * 0.5) + 1;

            double completeArea = internalArea + numberOfExternalBlocks;

            return completeArea.ToString();
        }

        static void FloodFill(bool[,] grid, int row, int col)
        {
            int rows = grid.GetLength(0);
            int columns = grid.GetLength(1);
            bool[,] visited = new bool[rows, columns];

            Stack<(int, int)> stack = new();
            stack.Push((row, col));

            while (stack.Count > 0)
            {
                var (currentRow, currentCol) = stack.Pop();

                if (currentRow < 0 ||
                    currentRow >= rows ||
                    currentCol < 0 ||
                    currentCol >= columns ||
                    visited[currentRow, currentCol] ||
                    grid[currentRow, currentCol])
                {
                    continue;
                }

                visited[currentRow, currentCol] = true;
                grid[currentRow, currentCol] = true;

                stack.Push((currentRow - 1, currentCol));
                stack.Push((currentRow + 1, currentCol));
                stack.Push((currentRow, currentCol - 1));
                stack.Push((currentRow, currentCol + 1));
            }
        }

        public static DiggingStep ProcessLine(string line, char part)
        {
            if (part == 'A')
            {
                Match match = Regex.Match(line, @"^([RLDU])\s(\d+)\s\(#([0-9a-fA-F]{6})\)$");

                if (match.Success)
                {
                    return new DiggingStep(match.Groups[1].Value, int.Parse(match.Groups[2].Value));
                }

                throw new Exception("Error parsing: " + line);
            }
            else
            {
                Match match = Regex.Match(line, @"^([RLDU])\s(\d+)\s\(#([0-9a-fA-F]{5})([0123])\)$");

                if (match.Success)
                {
                    string dirChar = "";

                    switch (match.Groups[4].Value)
                    {
                        case "0":
                            dirChar = "R";
                            break;
                        case "1":
                            dirChar = "D";
                            break;
                        case "2":
                            dirChar = "L";
                            break;
                        case "3":
                            dirChar = "U";
                            break;
                    }

                    return new DiggingStep(dirChar, Convert.ToInt32(match.Groups[3].Value, 16));
                }

                throw new Exception("Error parsing: " + line);
            }
        }

        public static List<(double, double)> CreateListOfVertex(List<DiggingStep> diggingSteps)
        {
            List<(double, double)> listOfVertex = [];

            (double, double) currentVertex = (0, 0);

            foreach (DiggingStep diggingStep in diggingSteps)
            {
                listOfVertex.Add(
                    (currentVertex.Item1 + diggingStep.Direction.Item1 * diggingStep.NumOfBlocks,
                    currentVertex.Item2 + diggingStep.Direction.Item2 * diggingStep.NumOfBlocks));

                currentVertex = listOfVertex[^1];
            }

            return listOfVertex;
        }
    }

    internal class DiggingStep(string dirChar, int numOfBlocks)
    {
        public (int, int) Direction = dirDict[dirChar];
        public int NumOfBlocks = numOfBlocks;
        public string DirectionSymbol = dirChar;

        private static readonly Dictionary<string, (int, int)> dirDict = new()
        {
            {"R", (1,0)  },
            {"L", (-1,0) },
            {"U", (0,-1) },
            {"D", (0,1)  }
        };
    }
}
