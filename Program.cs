using Main.Tools;
using System.Diagnostics;
public static class Program
{
    static void Main(string[] args)
    {
        if (Funcs.ValidateArgs(args, out int dayToRun, out char exampleOrInputChoice, out char solutionPartChoice))
        {
            string filePath = (exampleOrInputChoice == 't')
                ? $"Examples/day{dayToRun}_example.txt"
                : $"Inputs/day{dayToRun}.txt";

            Stopwatch sw = Stopwatch.StartNew();

            if (exampleOrInputChoice == 't')
            {
                Funcs.RunExampleAndCheck(Funcs.ReadFileToArray(filePath), solutionPartChoice, dayToRun);
            }
            else
            {
                Funcs.RunSolution(Funcs.ReadFileToArray(filePath), solutionPartChoice, dayToRun);
            }

            sw.Stop();

            Console.WriteLine($"Time taken: {sw.Elapsed.TotalMilliseconds} ms");
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}