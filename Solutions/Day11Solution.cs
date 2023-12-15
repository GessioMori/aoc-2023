using Main.Tools;

namespace Main.Solutions
{
    internal class Day11Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<List<char>> map = MapInitialUniverse(inputData);

            (List<int> emptyRows, List<int> emptyCols) = GetEmptySpaces(map);

            List<List<char>> expandedMap = ExpandUniverse(map, emptyRows, emptyCols);

            List<int[]> listOfGalaxies = FindGalaxies(expandedMap);

            long totalDistance = 0;

            for (int i = 0; i < listOfGalaxies.Count; i++)
            {
                for (int j = i + 1; j < listOfGalaxies.Count; j++)
                {
                    totalDistance += Math.Abs(listOfGalaxies[i][0] - listOfGalaxies[j][0])
                        + Math.Abs(listOfGalaxies[i][1] - listOfGalaxies[j][1]);
                }
            }

            return totalDistance.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            List<List<char>> map = MapInitialUniverse(inputData);

            (List<int> emptyRows, List<int> emptyCols) = GetEmptySpaces(map);

            List<int[]> listOfGalaxies = FindGalaxies(map);

            long totalDistance = 0;

            for (int i = 0; i < listOfGalaxies.Count; i++)
            {
                for (int j = i + 1; j < listOfGalaxies.Count; j++)
                {
                    totalDistance += Math.Abs(listOfGalaxies[i][0] - listOfGalaxies[j][0])
                        + Math.Abs(listOfGalaxies[i][1] - listOfGalaxies[j][1]);

                    int emptyRowsCrossed = emptyRows.Count(
                        r => r < Math.Max(listOfGalaxies[i][1], listOfGalaxies[j][1]) &&
                        r > Math.Min(listOfGalaxies[i][1], listOfGalaxies[j][1]));

                    int emptyColsCrossed = emptyCols.Count(
                        c => c < Math.Max(listOfGalaxies[i][0], listOfGalaxies[j][0]) &&
                        c > Math.Min(listOfGalaxies[i][0], listOfGalaxies[j][0]));

                    totalDistance += (emptyRowsCrossed + emptyColsCrossed) * 999999;
                }
            }

            return totalDistance.ToString();
        }
        public List<int[]> FindGalaxies(List<List<char>> map)
        {
            List<int[]> listOfGalaxies = [];

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == '#')
                    {
                        listOfGalaxies.Add([i, j]);
                    }
                }
            }

            return listOfGalaxies;
        }
        public List<List<char>> ExpandUniverse(List<List<char>> map, List<int> emptyRows, List<int> emptyCols)
        {
            List<List<char>> expandedMap = new List<List<char>>();

            for (int i = 0; i < map.Count; i++)
            {
                expandedMap.Add(map[i]);

                if (emptyCols.Contains(i))
                {
                    expandedMap.Add(Enumerable.Repeat('.', map[i].Count).ToList());
                }
            }

            for (int i = 0; i < emptyRows.Count; i++)
            {
                for (int j = 0; j < expandedMap.Count; j++)
                {
                    expandedMap[j].Insert(emptyRows[i] + i, '.');
                }
            }

            return expandedMap;
        }
        public static void PrintUniverse(List<List<char>> map)
        {
            for (int j = 0; j < map[0].Count; j++)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    Console.Write(map[i][j]);
                }
                Console.WriteLine();
            }
        }
        public List<List<char>> MapInitialUniverse(string[] inputData)
        {
            List<List<char>> map = [];

            foreach (string line in inputData)
            {
                map.Add([.. line.ToCharArray()]);
            }

            int rows = map.Count;
            int cols = map[0].Count;

            List<List<char>> transposedMap = [];

            for (int col = 0; col < cols; col++)
            {
                transposedMap.Add(Enumerable.Repeat('.', cols).ToList());

                for (int row = 0; row < rows; row++)
                {
                    transposedMap[col][row] = map[row][col];
                }
            }

            return transposedMap;
        }
        public (List<int>, List<int>) GetEmptySpaces(List<List<char>> map)
        {
            List<int> emptyRows = [];
            List<int> emptyCols = [];

            for (int i = 0; i < map.Count; i++)
            {
                if (map[i].All(c => c == '.'))
                {
                    emptyCols.Add(i);
                }
            }
            for (int j = 0; j < map[0].Count; j++)
            {
                bool isEmpty = true;

                for (int i = 0; i < map.Count; i++)
                {
                    if (map[i][j] == '#')
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                {
                    emptyRows.Add(j);
                }
            }

            return (emptyRows, emptyCols);
        }
    }
}
