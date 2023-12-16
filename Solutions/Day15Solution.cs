using Main.Tools;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

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
                total += RunHashAlgorithm(word);
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            Dictionary<int, List<Lens>> boxes = Enumerable.Range(0, 256).ToDictionary(k => k, _ => new List<Lens>());

            foreach (string word in GetListOfWords(inputData[0]))
            {
                Match match = Regex.Match(word, @"([a-zA-Z]+)([=-])(\d*)");

                if (match.Success)
                {
                    string label = match.Groups[1].Value;
                    int boxNumber = RunHashAlgorithm(label);
                    int index = boxes[boxNumber].FindIndex(l => l.Label == label);

                    if (match.Groups[2].Value == "-")
                    {
                        if (index != -1)
                        {
                            boxes[boxNumber].RemoveAt(index);
                        }
                    }
                    else
                    {
                        int focalLength = int.Parse(match.Groups[3].Value);
                        if (index == -1)
                        {
                            boxes[boxNumber].Add(new Lens()
                            {
                                FocalLength = focalLength,
                                Label = label
                            });
                        }
                        else
                        {
                            boxes[boxNumber][index].FocalLength = focalLength;
                        }
                    }
                }
            }

            long total = 0L;

            foreach (var box in boxes)
            {
                for (int i = 0; i < box.Value.Count; i++)
                {
                    total += (box.Key + 1) * (i + 1) * box.Value[i].FocalLength;
                }
            }

            return total.ToString();
        }

        public static List<string> GetListOfWords(string inputLine)
        {
            return inputLine.Split(',').ToList();
        }

        public static int RunHashAlgorithm(string word)
        {
            int currentValue = 0;

            foreach (char ch in word)
            {
                currentValue = ((currentValue + ch) * 17) % 256;
            }

            return currentValue;
        }
    }

    internal class Lens
    {
        public required string Label;
        public required int FocalLength;
    }
}
