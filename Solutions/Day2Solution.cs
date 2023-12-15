using Main.Tools;
using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day2Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            int total = 0;

            List<List<List<int>>> games = CreateListOfGames(inputData);

            for (int i = 0; i < games.Count; i++)
            {
                if (CheckPossibleGame(games[i]))
                {
                    total += i + 1;
                }
            }

            return total.ToString();
        }

        public static bool CheckPossibleGame(List<List<int>> game)
        {
            foreach (List<int> handShown in game)
            {
                if (handShown[0] > 12 || handShown[1] > 13 || handShown[2] > 14)
                {
                    return false;
                }
            }
            return true;
        }

        public string RunPartB(string[] inputData)
        {
            int total = 0;

            List<List<List<int>>> games = CreateListOfGames(inputData);

            foreach (List<List<int>> game in games)
            {
                
                int[] mins = [0, 0, 0];

                foreach (List<int> handShown in game)
                {
                    for (int i = 0; i < mins.Length; i++)
                    {
                        if (handShown[i] > mins[i])
                        {
                            mins[i] = handShown[i];
                        }
                    }
                }

                int gamePower = mins.Aggregate((acc, cur) => acc * cur);

                total += gamePower;
            }

            return total.ToString();
        }

        public static List<List<List<int>>> CreateListOfGames(string[] inputData)
        {
            List<List<List<int>>> listOfGames = new List<List<List<int>>>();

            Dictionary<string, int> colorCounts = new()
            {
                { "red", 0},
                { "green", 1},
                { "blue", 2}
            };

            // foreach game
            for (int i = 0; i < inputData.Length; i++)
            {
                string[] handsShown = inputData[i].Split(':')[1].Split(';');

                listOfGames.Add(new List<List<int>>());

                foreach (string hand in handsShown)
                {
                    List<int> arrayOfColors = [0, 0, 0];
                    MatchCollection matches = Regex.Matches(hand, @"(\d+) (\w+)");
                    foreach (Match match in matches)
                    {
                        if (colorCounts.ContainsKey(match.Groups[2].Value))
                        {
                            arrayOfColors[colorCounts[match.Groups[2].Value]] =
                                int.Parse(match.Groups[1].Value);
                        }
                    }
                    listOfGames[i].Add(arrayOfColors);
                }
            }

            return listOfGames;
        }
    }
}
