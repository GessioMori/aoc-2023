using Main.Tools;

namespace Main.Solutions
{
    internal class Day15Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<string> listOfWords = GetListOfWords(inputData[0]);

            long total = 0L;

            foreach (string word in listOfWords)
            {
                int currentValue = 0;

                foreach (char ch in word)
                {
                    currentValue = ((currentValue + (int)ch) * 17) % 256;
                }

                total += currentValue;
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            throw new NotImplementedException();
        }

        public static List<string> GetListOfWords(string inputLine)
        {
            return inputLine.Split(',').ToList();
        }
    }
}
