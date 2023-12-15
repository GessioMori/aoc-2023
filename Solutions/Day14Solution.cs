namespace Main.Solutions
{
    internal class Day14Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<List<char>> mapByColumns = Tools.Funcs.TransposeMatrix(
                RollOneDirection(
                    SortSublist,
                    Tools.Funcs.TransposeMatrix(
                        Tools.Funcs.GetCharMatrix(inputData))));

            return MeasureLoad(mapByColumns);
        }

        public string RunPartB(string[] inputData)
        {
            List<List<char>> mapByColumns = RollStonesFullCycle(Tools.Funcs.TransposeMatrix(Tools.Funcs.GetCharMatrix(inputData)));

            for (int i = 0; i < 999; i++)
            {
                mapByColumns = RollStonesFullCycle(mapByColumns);
            }

            List<List<char>> mapAfter1000Cycles = Tools.Funcs.CopyListOfLists(mapByColumns);

            int cycles = 1;

            mapByColumns = RollStonesFullCycle(mapByColumns);

            while (!Tools.Funcs.AreListsEqual(mapAfter1000Cycles, mapByColumns))
            {
                cycles++;
                mapByColumns = RollStonesFullCycle(mapByColumns);
            }

            int cycleMod = (1000000000 - 1000) % cycles;

            mapByColumns = Tools.Funcs.CopyListOfLists(mapAfter1000Cycles);

            for (int i = 1; i < cycleMod; i++)
            {
                mapByColumns = RollStonesFullCycle(mapByColumns);
            }

            return MeasureLoad(mapByColumns);
        }

        public static string MeasureLoad(List<List<char>> map)
        {
            long total = 0L;

            foreach (List<char> rolledColumn in map)
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

        public static List<List<char>> RollStonesFullCycle(List<List<char>> mapByColumns)
        {
            mapByColumns = RollOneDirection(SortSublist, mapByColumns);
            mapByColumns = RollOneDirection(SortSublist, mapByColumns);
            mapByColumns = RollOneDirection(SortSublistInverted, mapByColumns);
            mapByColumns = RollOneDirection(SortSublistInverted, mapByColumns);

            return mapByColumns;
        }

        public static List<List<char>> RollOneDirection(Action<List<char>> sorterFunc, List<List<char>> mapByColumns)
        {
            List<List<char>> rolledMap = [];

            foreach (List<char> column in mapByColumns)
            {
                List<List<char>> splittedColumn = SplitList(column, '#');

                splittedColumn.ForEach(sorterFunc);

                List<char> joinedList = splittedColumn.SelectMany(x => x).ToList();

                rolledMap.Add(joinedList);
            }

            return Tools.Funcs.TransposeMatrix(rolledMap);
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
    }
}
