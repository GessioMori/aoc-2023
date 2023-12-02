using System.Text.RegularExpressions;

namespace Main.Solutions
{
    public class Day1Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            int total = 0;

            foreach (string line in inputData)
            {
                total += int.Parse($"{line.First(char.IsDigit)}{line.Last(char.IsDigit)}");
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            int total = 0;

            string pattern = @"one|two|three|four|five|six|seven|eight|nine|\d";

            foreach (string line in inputData)
            {
                string firstPart = Regex.Match(line, pattern).Value;
                string lastPart = Regex.Match(line, pattern, RegexOptions.RightToLeft).Value;
                total += ConvertToNumber(firstPart) * 10 + ConvertToNumber(lastPart);
            }

            return total.ToString();
        }

        private static int ConvertToNumber(string number)
        {
            switch (number)
            {
                case "one": return 1;
                case "two": return 2;
                case "three": return 3;
                case "four": return 4;
                case "five": return 5;
                case "six": return 6;
                case "seven": return 7;
                case "eight": return 8;
                case "nine": return 9;
                default: return int.Parse(number);
            }
        }
    }
}
