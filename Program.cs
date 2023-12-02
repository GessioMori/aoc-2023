using Main.Tools;
public static class Program
{
    static void Main()
    {
        Console.Write("Enter the day for which you want to run the solution: ");
        if (int.TryParse(Console.ReadLine(), out int dayToRun))
        {
            string inputPath = $"Inputs/day{dayToRun}.txt";
            string[] inputData = Tools.ReadFileToArray(inputPath);

            string examplePathA = $"Examples/day{dayToRun}_exampleA.txt";
            string examplePathB = $"Examples/day{dayToRun}_exampleB.txt";
            string expectedAnswerPath = $"Answers/day{dayToRun}_answer.txt";
            string[] exampleDataA = Tools.ReadFileToArray(examplePathA);
            string[] exampleDataB = Tools.ReadFileToArray(examplePathB);
            string[] expectedAnswer = Tools.ReadFileToArray(expectedAnswerPath);

            Console.Write("Do you want to run the test or the input? (t/I): ");
            char? exampleOrInputChoice = Console.ReadLine()?.ToUpper()[0];
            if (exampleOrInputChoice == 'T')
            {
                Console.Write("Do you want to run Part A or Part B for the example? (A/B): ");
                char examplePartChoice = Console.ReadLine()?.ToUpper()[0] ?? 'B';
                if (examplePartChoice == 'A' || examplePartChoice == 'B')
                {
                    Tools.RunExampleAndCheck(examplePartChoice == 'A' ? exampleDataA : exampleDataB, 
                        examplePartChoice == 'A' ? expectedAnswer[0] : expectedAnswer[1],
                        examplePartChoice, 
                        dayToRun);
                }
            }
            else
            {
                Console.Write("Do you want to run Part A or Part B for the solution? (A/B): ");
                char solutionPartChoice = Console.ReadLine()?.ToUpper()[0] ?? 'B';

                if (solutionPartChoice == 'A' || solutionPartChoice == 'B')
                {
                    Tools.RunSolution(inputData, solutionPartChoice, dayToRun);
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid day number.");
        }
    }
}