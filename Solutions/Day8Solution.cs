using Main.Tools;
using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day8Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            char[] directions = MapDirections(inputData[0]);
            Dictionary<string, (string, string)> positions = MapPositions(inputData);

            string firstPosition = "AAA";

            string currentPosition = firstPosition;

            int moves = 0;

            while (currentPosition != "ZZZ")
            {
                moves++;

                currentPosition = directions[(moves - 1) % directions.Length] == 'L' ?
                    positions[currentPosition].Item1 :
                    positions[currentPosition].Item2;
            }

            return moves.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            char[] directions = MapDirections(inputData[0]);

            Dictionary<string, (string, string)> positions = MapPositions(inputData);

            List<string> firstPositions = positions.Where(p => p.Key.EndsWith('A'))
                .Select(p => p.Key)
                .ToList();

            List<string> currentPositions = new List<string>(firstPositions);

            List<long> moves = Enumerable.Repeat(0L, firstPositions.Count).ToList();

            for (int i = 0; i < firstPositions.Count; i++)
            {
                while (!currentPositions[i].EndsWith('Z'))
                {
                    moves[i]++;

                    currentPositions[i] = directions[(moves[i] - 1) % directions.Length] == 'L' ?
                        positions[currentPositions[i]].Item1 :
                        positions[currentPositions[i]].Item2;
                }
            }

            return moves.Aggregate(1L, Lcm).ToString();
        }

        long Lcm(long a, long b) => a * b / Gcd(a, b);
        long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

        public char[] MapDirections(string directions)
        {
            return directions.ToCharArray();
        }

        public Dictionary<string, (string, string)> MapPositions(string[] inputData)
        {
            Dictionary<string, (string, string)> positions = new Dictionary<string, (string, string)>();

            foreach (string positionString in inputData)
            {
                Match match = Regex.Match(positionString, @"^([A-Z]{3}) = \(([A-Z]{3}), ([A-Z]{3})\)$");

                if (match.Success)
                {
                    positions.Add(match.Groups[1].Value,
                        (match.Groups[2].Value, match.Groups[3].Value));
                }

            }

            return positions;
        }
    }
}
