using System.Text.RegularExpressions;

namespace Main.Tools
{
    public static class Funcs
    {
        public static string[] ReadFileToArray(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return File.ReadAllLines(filePath);
        }

        public static ISolution CreateSolutionInstance(int day)
        {
            string typeName = $"Main.Solutions.Day{day}Solution";
            Type? solutionType = Type.GetType(typeName);
            if (solutionType != null)
            {
                object? solution = Activator.CreateInstance(solutionType);
                if (solution != null)
                {
                    return (ISolution)solution;
                }
            }

            throw new InvalidOperationException($"Solution type '{typeName}' not found.");
        }

        public static void RunExampleAndCheck(string[] exampleData, char partChoice, int day)
        {
            ISolution solution = CreateSolutionInstance(day);

            List<string[]> splitExampleData = new List<string[]>();
            List<string> currentStrings = new List<string>();

            foreach (string line in exampleData)
            {
                if (line.Equals("==="))
                {
                    splitExampleData.Add(currentStrings.ToArray());
                    currentStrings.Clear();
                }
                else
                {
                    currentStrings.Add(line);
                }
            }

            if (currentStrings.Count > 0)
            {
                splitExampleData.Add(currentStrings.ToArray());
            }

            string expectedAnswer = partChoice == 'a' ? splitExampleData[1][0] : splitExampleData[3][0];
            string[] exampleDataInput = partChoice == 'a' ? splitExampleData[0] : splitExampleData[2];
            string result = partChoice == 'a' ? solution.RunPartA(exampleDataInput) : solution.RunPartB(exampleDataInput);

            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Expected: {expectedAnswer}");
            Console.WriteLine($"Test passed: {result == expectedAnswer}");
        }

        public static void RunSolution(string[] inputData, char partChoice, int day)
        {
            ISolution solution = CreateSolutionInstance(day);
            string result = partChoice == 'a' ? solution.RunPartA(inputData) : solution.RunPartB(inputData);

            Console.WriteLine($"Result: {result}");
        }

        public static bool ValidateArgs(string[] args, out int day, out char testOrInput, out char challengePart)
        {
            testOrInput = 'i';
            challengePart = 'a';
            day = 0;
            if (args.Length > 0)
            {
                Match match = Regex.Match(args[0], @"(\d+)(t|i)(a|b)");

                if (match.Success)
                {
                    day = int.Parse(match.Groups[1].Value);
                    testOrInput = char.Parse(match.Groups[2].Value);
                    challengePart = char.Parse(match.Groups[3].Value);

                    return true;
                }
            }

            return false;
        }

        public static List<List<char>> CopyListOfLists(List<List<char>> originalList)
        {
            List<List<char>> newList = new List<List<char>>();

            foreach (List<char> innerList in originalList)
            {
                List<char> newInnerList = new List<char>(innerList);
                newList.Add(newInnerList);
            }

            return newList;
        }

        public static bool AreListsEqual(List<List<char>> list1, List<List<char>> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].SequenceEqual(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static List<List<char>> TransposeMatrix(List<List<char>> originalList)
        {
            int rows = originalList.Count;
            int cols = originalList[0].Count;

            List<List<char>> transposedList = new List<List<char>>();
            for (int col = 0; col < cols; col++)
            {
                List<char> newRow = new List<char>();
                for (int row = 0; row < rows; row++)
                {
                    char c = originalList[row][col];
                    newRow.Add(c);
                }
                transposedList.Add(newRow);
            }

            return transposedList;
        }

        public static List<List<char>> GetCharMatrix(string[] inputData)
        {
            List<List<char>> matrix = [];

            foreach (string line in inputData)
            {
                matrix.Add(line.ToCharArray().ToList());
            }

            return matrix;
        }

        public static int[,] ConvertStringArrayToGrid(string[] gridStrings)
        {
            int rows = gridStrings.Length;
            int cols = gridStrings[0].Length;
            int[,] grid = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!int.TryParse(gridStrings[i][j].ToString(), out grid[i, j]))
                    {
                        Console.WriteLine($"Error parsing value at row {i}, column {j}");
                    }
                }
            }

            return grid;
        }
    }

    public interface ISolution
    {
        string RunPartA(string[] inputData);
        string RunPartB(string[] inputData);
    }
}
