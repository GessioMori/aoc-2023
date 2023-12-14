using System.Collections.Concurrent;

namespace Main.Solutions
{
    internal class Day12Solution : ISolution
    {
        ConcurrentDictionary<string, long> cache = [];
        public string RunPartA(string[] inputData)
        {
            int total = 0;

            foreach (string line in inputData)
            {
                (string sequence, int[] numbers) = ProcessLine(line);

                List<string> listOfSequences = [];

                CreateListOfPossibleSequences(sequence, listOfSequences);

                foreach (string testSequence in listOfSequences)
                {
                    if (IsSequenceValid(testSequence, numbers))
                    {
                        total++;
                    }
                }
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            long total = 0;

            Parallel.ForEach(inputData, line =>
            {
                (string sequence, int[] springs) = ProcessLineAndRepeat(line);

                long matches = CountMatches(sequence, springs);

                Interlocked.Add(ref total, matches);
            });

            return total.ToString();
        }

        private long CountMatches(string pattern, int[] springs)
        {
            if (pattern == "")
            {
                return springs.Length == 0 ? 1 : 0;
            }

            if (springs.Length == 0)
            {
                return pattern.Contains('#') ? 0 : 1;
            }

            string key = $"{pattern}_{string.Join(",", springs)}";

            if (cache.TryGetValue(key, out long cVal))
            {
                return cVal;
            }

            long result = 0;

            if (pattern[0] == '.' || pattern[0] == '?')
            {
                result += CountMatches(pattern[1..], springs);
            }

            if (pattern[0] == '#' || pattern[0] == '?')
            {
                if (springs[0] <= pattern.Length && !pattern[..springs[0]].Contains('.') &&
                    (springs[0] == pattern.Length || pattern[springs[0]] != '#'))
                {
                    int[] newSprings = new int[springs.Length - 1];
                    Array.Copy(springs, 1, newSprings, 0, newSprings.Length);

                    string newPattern = springs[0] + 1 > pattern.Length
                        ? ""
                        : pattern[(springs[0] + 1)..];

                    result += CountMatches(newPattern, newSprings);
                }
            }

            cache.TryAdd(key, result);
            return result;
        }

        public static (string, int[]) ProcessLine(string line)
        {
            string sequence = line.Split(' ')[0];
            int[] numbers = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();

            return (sequence, numbers);
        }

        public static (string, int[]) ProcessLineAndRepeat(string line)
        {
            string sequence = string.Join("?", Enumerable.Repeat(line.Split(' ')[0], 5));
            int[] numbers = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();

            int[] repeatedNumbers = Enumerable.Repeat(numbers, 5).SelectMany(x => x).ToArray();

            return (sequence, repeatedNumbers);
        }

        public static void CreateListOfPossibleSequences(string currentSequence, List<string> listOfSequences)
        {
            int indexOfQuestionMark = currentSequence.IndexOf('?');

            if (indexOfQuestionMark == -1)
            {
                listOfSequences.Add(currentSequence);
            }
            else
            {
                CreateListOfPossibleSequences(
                    string.Concat(currentSequence.AsSpan(0, indexOfQuestionMark), ".", currentSequence.AsSpan(indexOfQuestionMark + 1)),
                    listOfSequences);
                CreateListOfPossibleSequences(
                    string.Concat(currentSequence.AsSpan(0, indexOfQuestionMark), "#", currentSequence.AsSpan(indexOfQuestionMark + 1)),
                    listOfSequences);
            }
        }

        public static bool IsSequenceValid(string sequence, int[] springs)
        {
            int count = 0;

            int currentGroup = 0;

            bool isCounting = false;

            foreach (char c in sequence + '.')
            {
                if (c == '#')
                {
                    isCounting = true;
                    count++;
                }
                else if (c == '.' && isCounting)
                {
                    isCounting = false;
                    if (currentGroup < springs.Length && count == springs[currentGroup])
                    {
                        currentGroup++;
                        count = 0;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (currentGroup == springs.Length)
            {
                return true;
            }

            return false;
        }
    }
}
