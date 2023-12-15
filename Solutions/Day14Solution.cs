namespace Main.Solutions
{
    internal class Day14Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<List<char>> mapByColumns = RollStones(TransposeMap(GetMap(inputData)));

            long total = 0L;

            foreach (List<char> rolledColumn in mapByColumns)
            {
                for (int i = 0; i < rolledColumn.Count; i++)
                {
                    if (rolledColumn[i] == 'O')
                    {
                        total += rolledColumn.Count - i;
                    }
                }
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            List<List<char>> mapByColumns = RollStonesFullCycle(TransposeMap(GetMap(inputData)));

            for (int i = 0; i < 999; i++)
            {
                mapByColumns = RollStonesFullCycle(mapByColumns);
            }

            List<List<char>> mapAfter1000Cycles = CopyListOfLists(mapByColumns);

            int cycles = 1;

            mapByColumns = RollStonesFullCycle(mapByColumns);

            while (!AreListsEqual(mapAfter1000Cycles, mapByColumns))
            {
                cycles++;
                mapByColumns = RollStonesFullCycle(mapByColumns);
            }

            long total = 0L;

            int cycleMod = (1000000000 - 1000) % cycles;

            mapByColumns = CopyListOfLists(mapAfter1000Cycles);

            for (int i = 1; i < cycleMod; i++)
            {
                mapByColumns = RollStonesFullCycle(mapByColumns);
            }

            foreach (List<char> rolledColumn in mapByColumns)
            {
                for (int i = 0; i < rolledColumn.Count; i++)
                {
                    if (rolledColumn[i] == 'O')
                    {
                        total += rolledColumn.Count - i;
                    }
                }
            }

            return total.ToString();
        }

        public static List<List<char>> GetMap(string[] inputData)
        {
            List<List<char>> map = [];

            foreach (string line in inputData)
            {
                map.Add(line.ToCharArray().ToList());
            }

            return map;
        }
        public static List<List<char>> TransposeMap(List<List<char>> originalList)
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
        public static List<List<char>> RollStones(List<List<char>> mapByColumns)
        {
            List<List<char>> rolledMap = [];

            foreach (List<char> column in mapByColumns)
            {
                List<List<char>> splittedColumn = SplitList(column, '#');

                splittedColumn.ForEach(SortSublist);

                List<char> joinedList = splittedColumn.SelectMany(x => x).ToList();

                rolledMap.Add(joinedList);
            }

            return rolledMap;
        }
        public static List<List<char>> RollStonesFullCycle(List<List<char>> mapByColumns)
        {
            List<List<char>> rolledMap = [];

            foreach (List<char> column in mapByColumns)
            {
                List<List<char>> splittedColumn = SplitList(column, '#');

                splittedColumn.ForEach(SortSublist);

                List<char> joinedList = splittedColumn.SelectMany(x => x).ToList();

                rolledMap.Add(joinedList);
            }

            rolledMap = TransposeMap(rolledMap);

            List<List<char>> newRolledMap = [];

            foreach (List<char> column in rolledMap)
            {
                List<List<char>> splittedColumn = SplitList(column, '#');

                splittedColumn.ForEach(SortSublist);

                List<char> joinedList = splittedColumn.SelectMany(x => x).ToList();

                newRolledMap.Add(joinedList);
            }

            newRolledMap = TransposeMap(newRolledMap);

            rolledMap.Clear();

            foreach (List<char> column in newRolledMap)
            {
                List<List<char>> splittedColumn = SplitList(column, '#');

                splittedColumn.ForEach(SortSublistInverted);

                List<char> joinedList = splittedColumn.SelectMany(x => x).ToList();

                rolledMap.Add(joinedList);
            }

            rolledMap = TransposeMap(rolledMap);

            newRolledMap.Clear();

            foreach (List<char> column in rolledMap)
            {
                List<List<char>> splittedColumn = SplitList(column, '#');

                splittedColumn.ForEach(SortSublistInverted);

                List<char> joinedList = splittedColumn.SelectMany(x => x).ToList();

                newRolledMap.Add(joinedList);
            }

            return TransposeMap(newRolledMap);
        }

        public static List<List<char>> SplitList(List<char> inputList, char separator)
        {
            List<List<char>> resultList = new List<List<char>>();
            List<char> currentList = new List<char>();

            foreach (char c in inputList)
            {
                if (c == separator)
                {
                    if (currentList.Any())
                    {
                        resultList.Add(currentList.ToList());
                        currentList.Clear();
                    }
                    resultList.Add(new List<char> { c });
                }
                else
                {
                    currentList.Add(c);
                }
            }

            if (currentList.Any())
            {
                resultList.Add(currentList.ToList());
            }

            return resultList;
        }
        public static void SortSublist(List<char> sublist)
        {
            sublist.Sort((a, b) =>
            {
                if (a == 'O' && b != 'O') return -1;
                if (a == '#' && b == 'O') return 1;
                if (a == '#' && b == '.') return -1;
                if (a == '.' && b != '.') return 1;
                return 0;
            });
        }

        public static void SortSublistInverted(List<char> sublist)
        {
            sublist.Sort((a, b) =>
            {
                if (a == 'O' && b != 'O') return 1;
                if (a == '#' && b == 'O') return -1;
                if (a == '#' && b == '.') return 1;
                if (a == '.' && b != '.') return -1;
                return 0;
            });
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
    }
}
