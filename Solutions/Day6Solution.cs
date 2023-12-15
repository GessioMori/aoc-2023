using Main.Tools;
using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day6Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<(int, int)> listOfTimeDistances = GetTimeDistances(inputData);

            List<double> numberOfPossibilities = [];

            foreach ((int, int) timeDistance in listOfTimeDistances)
            {
                numberOfPossibilities.Add(GetNumberOfPossibilities(timeDistance));
            }

            return numberOfPossibilities.Aggregate((cur, next) => cur * next).ToString();
        }

        public string RunPartB(string[] inputData)
        {
            (long, long) timeDistance = GetSingleTimeDistance(inputData);

            return GetNumberOfPossibilities(timeDistance).ToString();
        }

        public double GetNumberOfPossibilities((long, long) timeDistance)
        {
            double maximum, minimum;

            double delta = Math.Sqrt(Math.Pow(timeDistance.Item1, 2) - 4 * timeDistance.Item2);

            minimum = Math.Floor((timeDistance.Item1 - delta) * 0.5 + 1);

            maximum = Math.Ceiling((timeDistance.Item1 + delta) * 0.5 - 1);

            return maximum - minimum + 1;
        }

        public List<(int, int)> GetTimeDistances(string[] inputData)
        {
            MatchCollection timeMatches = Regex.Matches(inputData[0], @"(\d+)");
            MatchCollection distanceMatches = Regex.Matches(inputData[1], @"(\d+)");

            List<(int, int)> listOfTimeDistances = [];

            for (int i = 0; i < timeMatches.Count; i++)
            {
                listOfTimeDistances.Add((int.Parse(timeMatches[i].Value), int.Parse(distanceMatches[i].Value)));
            }

            return listOfTimeDistances;
        }

        public (long, long) GetSingleTimeDistance(string[] inputData)
        {
            Match timeMatch = Regex.Match(inputData[0].Replace(" ", ""), @"(\d+)");
            Match distanceMatch = Regex.Match(inputData[1].Replace(" ", ""), @"(\d+)");

            if (timeMatch.Success && distanceMatch.Success)
            {
                return (long.Parse(timeMatch.Value), long.Parse(distanceMatch.Value));
            }

            throw new Exception("Could not get values.");
        }
    }
}
