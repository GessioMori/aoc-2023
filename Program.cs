using Main.Tools;
public static class Program
{
    static void Main(string[] args)
    {
        if (Tools.ValidateArgs(args, out int dayToRun, out char exampleOrInputChoice, out char solutionPartChoice))
        {
            string filePath = (exampleOrInputChoice == 't')
                ? $"Examples/day{dayToRun}_example.txt"
                : $"Inputs/day{dayToRun}.txt";

            if (exampleOrInputChoice == 't')
            {
                Tools.RunExampleAndCheck(Tools.ReadFileToArray(filePath), solutionPartChoice, dayToRun);
            }
            else
            {
                Tools.RunSolution(Tools.ReadFileToArray(filePath), solutionPartChoice, dayToRun);
            }
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}