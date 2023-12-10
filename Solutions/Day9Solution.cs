namespace Main.Solutions
{
    internal class Day9Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<List<int>> originalLines = MapLines(inputData);

            List<int> lineResults = [];

            foreach (List<int> line in originalLines)
            {
                List<List<int>> currentSolution = GetSolution(line);

                int currentLastNumber = 0;

                for (int i = currentSolution.Count - 1; i > 0; i--)
                {
                    currentLastNumber += currentSolution[i - 1][^1];
                }

                lineResults.Add(currentLastNumber);
            }

            return lineResults.Sum().ToString();
        }

        public string RunPartB(string[] inputData)
        {
            List<List<int>> originalLines = MapLines(inputData);

            List<int> lineResults = [];

            foreach (List<int> line in originalLines)
            {
                List<List<int>> currentSolution = GetSolution(line);

                int currentLastNumber = 0;

                for (int i = currentSolution.Count - 1; i > 0; i--)
                {
                    currentLastNumber = currentSolution[i - 1][0] - currentLastNumber;
                }

                lineResults.Add(currentLastNumber);
            }

            return lineResults.Sum().ToString();
        }

        public List<List<int>> GetSolution(List<int> line)
        {
            List<List<int>> currentSolution = [line];

            while (!currentSolution[^1].All(n => n == 0))
            {
                currentSolution.Add([]);

                for (int i = 1; i < currentSolution[^2].Count; i++)
                {
                    currentSolution[^1].Add(
                        currentSolution[^2][i] -
                        currentSolution[^2][i - 1]
                        );
                }
            }

            return currentSolution;
        }

        public List<List<int>> MapLines(string[] inputData)
        {
            List<List<int>> lines = [];

            foreach (string line in inputData)
            {
                lines.Add(line.Split(" ").Select(int.Parse).ToList());
            }

            return lines;
        }
    }
}
