using Main.Tools;
public static class Program
{
    static void Main(string[] args)
    {
        if (Tools.ValidateArgs(args, out int dayToRun, out char? exampleOrInputChoice, out char? solutionPartChoice))
        {
            if (exampleOrInputChoice == 't')
            {
                string examplePath = $"Examples/day{dayToRun}_example{solutionPartChoice.ToString()?.ToUpper()}.txt";
                string expectedAnswerPath = $"Answers/day{dayToRun}_answer.txt";

                Tools.RunExampleAndCheck(Tools.ReadFileToArray(examplePath),
                    solutionPartChoice == 'a' ? Tools.ReadFileToArray(expectedAnswerPath)[0] : Tools.ReadFileToArray(expectedAnswerPath)[1],
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