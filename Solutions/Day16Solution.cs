using Main.Tools;
using System.Collections.Generic;

namespace Main.Solutions
{
    internal class Day16Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<List<char>> charMap = Funcs.TransposeMatrix(Funcs.GetCharMatrix(inputData));

            long total = GetEnergizedCellsByStart(charMap, (0, 0), (-1, 0));

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            long maxEnergized = 0L;

            List<List<char>> charMap = Funcs.TransposeMatrix(Funcs.GetCharMatrix(inputData));

            for (int i = 0; i < charMap[0].Count; i++)
            {
                long possibleMax = GetEnergizedCellsByStart(charMap, (0, i), (-1, 0));

                if (possibleMax > maxEnergized)
                {
                    maxEnergized = possibleMax;
                }
                possibleMax = GetEnergizedCellsByStart(charMap, (charMap[0].Count - 1, i), (1, 0));

                if (possibleMax > maxEnergized)
                {
                    maxEnergized = possibleMax;
                }
            }

            for (int i = 0; i < charMap.Count; i++)
            {
                long possibleMax = GetEnergizedCellsByStart(charMap, (i, 0), (0, -1));

                if (possibleMax > maxEnergized)
                {
                    maxEnergized = possibleMax;
                }
                possibleMax = GetEnergizedCellsByStart(charMap, (i, charMap.Count - 1), (0, 1));

                if (possibleMax > maxEnergized)
                {
                    maxEnergized = possibleMax;
                }
            }

            return maxEnergized.ToString();
        }

        public long GetEnergizedCellsByStart(List<List<char>> charMap, (int, int) startCoord, (int, int) side)
        {
            List<List<MapCell>> map = CreateMapFromChars(charMap);

            LightBeam firstLightBeam = new LightBeam();

            firstLightBeam.Path.AddRange(
                [(startCoord.Item1 + side.Item1, startCoord.Item2 + side.Item2),
                    startCoord]);

            List<LightBeam> lightBeamList = [firstLightBeam];

            int mapHeight = map[0].Count;
            int mapWidth = map.Count;

            while (lightBeamList.Count != 0)
            {
                LightBeam lightBeam = lightBeamList[0];

                (int, int) currentCoord = lightBeam.Path[^1];
                (int, int) prevCoord = lightBeam.Path[^2];
                (int, int) lastMovement = (currentCoord.Item1 - prevCoord.Item1,
                        currentCoord.Item2 - prevCoord.Item2);

                if (!MapCell.IsValidCoord(currentCoord, mapHeight, mapWidth) ||
                    map[currentCoord.Item1][currentCoord.Item2].IsAlreadyEnergizedOnThatDirection(lastMovement))
                {
                    lightBeamList.RemoveAt(0);
                }
                else
                {
                    (int, int) nextCoord = map[currentCoord.Item1][currentCoord.Item2]
                        .GetNextPositionAndDuplicate(lastMovement, lightBeamList);

                    lightBeam.Path.Add(nextCoord);
                }
            }

            long total = 0L;

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j].IsEnergized())
                    {
                        total++;
                    }
                }
            }

            return total;
        }

        public List<List<MapCell>> CreateMapFromChars(List<List<char>> charMap)
        {
            List<List<MapCell>> map = new();

            for (int i = 0; i < charMap.Count; i++)
            {
                List<MapCell> column = [];

                for (int j = 0; j < charMap[0].Count; j++)
                {
                    column.Add(new MapCell((i, j), charMap[i][j]));
                }

                map.Add(column);
            }

            return map;
        }
    }

    internal class LightBeam()
    {
        public List<(int, int)> Path = new List<(int, int)>();
    }

    enum Cell
    {
        None,
        ForwardMirror,
        BackwardMirror,
        HorizontalSplitter,
        VerticalSplitter,
    }

    internal class MapCell((int, int) coords, char cellChar)
    {
        public static Dictionary<char, Cell> cellsDict = new()
        {
            {'.', Cell.None},
            {'/', Cell.ForwardMirror},
            {'\\', Cell.BackwardMirror},
            {'-', Cell.HorizontalSplitter},
            {'|', Cell.VerticalSplitter},
        };

        public HashSet<(int, int)> EnergizedDirections = new();

        public (int, int) Coords = coords;

        public Cell CellType = cellsDict[cellChar];

        public (int, int) GetNextPositionAndDuplicate((int, int) lastMovement, List<LightBeam> lightBeamList)
        {
            EnergizedDirections.Add(lastMovement);

            if (this.CellType == Cell.None)
            {
                return (Coords.Item1 + lastMovement.Item1, Coords.Item2 + lastMovement.Item2);
            }
            else if (this.CellType == Cell.ForwardMirror)
            {
                (int, int) nextMovement = (lastMovement.Item2 * -1, lastMovement.Item1 * -1);
                return (Coords.Item1 + nextMovement.Item1, Coords.Item2 + nextMovement.Item2);
            }
            else if (this.CellType == Cell.BackwardMirror)
            {
                (int, int) nextMovement = (lastMovement.Item2, lastMovement.Item1);
                return (Coords.Item1 + nextMovement.Item1, Coords.Item2 + nextMovement.Item2);
            }
            else if (this.CellType == Cell.HorizontalSplitter)
            {
                if (lastMovement.Item2 == 0)
                {
                    return (Coords.Item1 + lastMovement.Item1, Coords.Item2 + lastMovement.Item2);
                }

                LightBeam newLightBeam = new();

                newLightBeam.Path.AddRange([(Coords.Item1, Coords.Item2),
                    (Coords.Item1 - 1, Coords.Item2)]);

                lightBeamList.Add(newLightBeam);

                return (Coords.Item1 + 1, Coords.Item2);

            }
            else if (this.CellType == Cell.VerticalSplitter)
            {
                if (lastMovement.Item1 == 0)
                {
                    return (Coords.Item1 + lastMovement.Item1, Coords.Item2 + lastMovement.Item2);
                }

                LightBeam newLightBeam = new();

                newLightBeam.Path.AddRange([(Coords.Item1, Coords.Item2),
                    (Coords.Item1, Coords.Item2 - 1)]);

                lightBeamList.Add(newLightBeam);

                return (Coords.Item1, Coords.Item2 + 1);
            }

            throw new Exception("Invalid mirror type");
        }

        public static bool IsValidCoord((int, int) coord, int mapHeight, int mapWidth)
        {
            return coord.Item1 >= 0 && coord.Item2 >= 0 && coord.Item1 < mapWidth && coord.Item2 < mapHeight;
        }

        public bool IsAlreadyEnergizedOnThatDirection((int, int) lastMovement)
        {
            return this.EnergizedDirections.Contains(lastMovement);
        }

        public bool IsEnergized()
        {
            return this.EnergizedDirections.Count > 0;
        }
    }
}
