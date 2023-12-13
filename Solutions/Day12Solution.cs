using System;

namespace Main.Solutions
{
    internal class Day12Solution : ISolution
    {
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
            int total = 0;

            Parallel.ForEach(inputData, line =>
            {
                (string sequence, int[] numbers) = ProcessLineAndRepeat(line);

                List<string> listOfSequences = [];

                List<string> listOfInvalids = [];

                CreateListOfPossibleSequences(sequence, listOfSequences);

                foreach (string testSequence in listOfSequences)
                {
                    if (IsSequenceValidWithMemo(testSequence, numbers, listOfInvalids))
                    {
                        Interlocked.Increment(ref total);
                    }
                }
            });

            return total.ToString();
        }

        public static (string, int[]) ProcessLine(string line)
        {
            string sequence = line.Split(' ')[0];
            int[] numbers = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();

            return (sequence, numbers);
        }

        public static (string, int[]) ProcessLineAndRepeat(string line)
        {
            string sequence = line.Split(' ')[0];
            int[] numbers = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();


            sequence = string.Join("?", Enumerable.Repeat(sequence, 5));
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

        public static bool IsSequenceValidWithMemo(string sequence, int[] springs, List<string> listOfInvalids)
        {
            int count = 0;

            int currentGroup = 0;

            bool isCounting = false;

            string currentPart = "";

            foreach (char c in sequence + '.')
            {
                currentPart += c;

                if (listOfInvalids.Contains(currentPart))
                {
                    return false;
                }

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
                        listOfInvalids.Add(currentPart);
                        return false;
                    }
                }
            }

            if (currentGroup == springs.Length)
            {
                return true;
            }

            listOfInvalids.Add(currentPart);
            return false;
        }
    }
}
