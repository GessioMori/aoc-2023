using Main.Tools;
public static class Program
{
    static void Main(string[] args)
    {
        if (Tools.ValidateArgs(args, out int dayToRun, out char? exampleOrInputChoice, out char? solutionPartChoice))
        {
            if (exampleOrInputChoice == 't')
            {
                string examplePath = $"Examples/day{dayToRun}_example.txt";

                Tools.RunExampleAndCheck(Tools.ReadFileToArray(examplePath),
                    solutionPartChoice ?? 'a',
                    dayToRun);
            }
            else
            {
                string inputPath = $"Inputs/day{dayToRun}.txt";
                string[] inputData = Tools.ReadFileToArray(inputPath);

                Tools.RunSolution(inputData, solutionPartChoice ?? 'a', dayToRun);
            }
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}