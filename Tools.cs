using Main.Solutions;
using System.Text.RegularExpressions;

namespace Main.Tools
{
    public static class Tools
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

        public static bool ValidateArgs(string[] args, out int day, out char? testOrInput, out char? challengePart)
        {
            testOrInput = null;
            challengePart = null;
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
    }
}
