namespace Main.Solutions
{
    internal class Day13Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<List<List<char>>> patterns = GetListOfPatterns(inputData);

            long total = 0;

            foreach (var pattern in patterns)
            {
                total += FindRowMirror(pattern) * 100;

                List<List<char>> transposedPattern = TransposePattern(pattern);

                total += FindRowMirror(transposedPattern);
            }

            return total.ToString();
        }
        public string RunPartB(string[] inputData)
        {
            List<List<List<char>>> patterns = GetListOfPatterns(inputData);

            long total = 0;

            foreach (var pattern in patterns)
            {
                total += FindWithSmudge(pattern) * 100;

                List<List<char>> transposedPattern = TransposePattern(pattern);

                total += FindWithSmudge(transposedPattern);
            }

            return total.ToString();
        }
        public List<List<List<char>>> GetListOfPatterns(string[] inputData)
        {
            List<List<List<char>>> patterns = [];

            List<List<char>> pattern = [];

            List<string> input = inputData.ToList();

            input.Add("");

            foreach (string line in input)
            {
                if (line != string.Empty)
                {
                    pattern.Add(line.ToCharArray().ToList());

                }
                else
                {
                    patterns.Add(new List<List<char>>(pattern));
                    pattern.Clear();
                }
            }

            return patterns;
        }
        public static int FindRowMirror(List<List<char>> pattern, int originalResult = int.MaxValue)
        {
            int result = 0;

            for (int i = 1; i < pattern.Count; i++)
            {
                List<List<char>> firstPart = new(pattern.GetRange(0, i));
                List<List<char>> secondPart = new(pattern.GetRange(i, pattern.Count - i));

                if (firstPart.Count < secondPart.Count)
                {
                    secondPart.RemoveRange(firstPart.Count, secondPart.Count - firstPart.Count);
                }
                else
                {
                    firstPart.RemoveRange(0, firstPart.Count - secondPart.Count);
                }

                secondPart.Reverse();

                if (AreListsEqual(firstPart, secondPart) && i != originalResult)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
        public static int FindWithSmudge(List<List<char>> pattern)
        {
            int result = 0;

            bool resultFound = false;

            int originalResult = FindRowMirror(pattern);

            for (int i = 0; i < pattern.Count && !resultFound; i++)
            {
                for (int j = 0; j < pattern[0].Count && !resultFound; j++)
                {
                    List<List<char>> modifiedPattern = CopyListOfLists(pattern);

                    if (modifiedPattern[i][j] == '.')
                    {
                        modifiedPattern[i][j] = '#';
                    }
                    else
                    {
                        modifiedPattern[i][j] = '.';
                    }

                    int partialResult = FindRowMirror(modifiedPattern, originalResult);

                    if (partialResult != 0)
                    {
                        result = partialResult;
                        resultFound = true;
                    }
                }
            }

            return result;
        }
        public static List<List<char>> CopyListOfLists(List<List<char>> originalList)
        {
            List<List<char>> newList = new List<List<char>>();

            foreach (List<char> innerList in originalList)
            {
                List<char> newInnerList = new List<char>(innerList);
                newList.Add(newInnerList);
            }

            return newList;
        }
        public static bool AreListsEqual(List<List<char>> list1, List<List<char>> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].SequenceEqual(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }
        public static List<List<char>> TransposePattern(List<List<char>> originalList)
        {
            int rows = originalList.Count;
            int cols = originalList[0].Count;

            List<List<char>> transposedList = new List<List<char>>();
            for (int col = 0; col < cols; col++)
            {
                List<char> newRow = new List<char>();
                for (int row = 0; row < rows; row++)
                {
                    char c = originalList[row][col];
                    newRow.Add(c);
                }
                transposedList.Add(newRow);
            }

            return transposedList;
        }
    }


}
