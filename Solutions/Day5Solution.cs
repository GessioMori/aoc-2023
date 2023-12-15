using Main.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day5Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            Dictionary<string, List<Map>> maps = GetMaps(inputData);

            List<long> seeds = GetSeeds(inputData);

            long lowestLocation = long.MaxValue;

            long currentPosition;

            foreach (long seed in seeds)
            {
                currentPosition = seed;

                foreach (KeyValuePair<string, List<Map>> map in maps)
                {
                    currentPosition = MapValue(map, currentPosition);
                }

                if (currentPosition < lowestLocation)
                {
                    lowestLocation = currentPosition;
                }
            }

            return lowestLocation.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            Dictionary<string, List<Map>> maps = GetMaps(inputData);

            //return RunPartBOnBruteForce(maps, GetListOfSeedsFromRange(GetSeeds(inputData)));

            return RunPartBFast(maps, GetListOfSeedsRange(GetSeeds(inputData)));
        }

        public string RunPartBFast(Dictionary<string, List<Map>> dicOfLevels, List<SeedRange> listOfSeedRanges)
        {
            foreach (KeyValuePair<string, List<Map>> level in dicOfLevels)
            {
                List<SeedRange> listOfSeedRangesForNextLevel = new List<SeedRange>();

                while (listOfSeedRanges.Count > 0)
                {
                    SeedRange currentSeedRange = new SeedRange()
                    {
                        seedStart = listOfSeedRanges[listOfSeedRanges.Count - 1].seedStart,
                        seedEnd = listOfSeedRanges[listOfSeedRanges.Count - 1].seedEnd
                    };

                    listOfSeedRanges.RemoveAt(listOfSeedRanges.Count - 1);

                    bool rangeHasCollided = false;

                    foreach (Map map in level.Value)
                    {
                        long newSeedRangeStart = long.Max(currentSeedRange.seedStart, map.sourceRangeStart);
                        long newSeedRangeEnd = long.Min(currentSeedRange.seedEnd, map.sourceRangeStart + map.rangeLength);

                        if (newSeedRangeStart < newSeedRangeEnd)
                        {
                            listOfSeedRangesForNextLevel.Add(
                                new SeedRange()
                                {
                                    seedStart = newSeedRangeStart + map.destinationRangeStart - map.sourceRangeStart,
                                    seedEnd = newSeedRangeEnd + map.destinationRangeStart - map.sourceRangeStart
                                });

                            if (currentSeedRange.seedStart < map.sourceRangeStart)
                            {
                                listOfSeedRanges.Add(
                                    new SeedRange()
                                    {
                                        seedStart = currentSeedRange.seedStart,
                                        seedEnd = map.sourceRangeStart
                                    });
                            }
                            if (currentSeedRange.seedEnd > map.sourceRangeStart + map.rangeLength)
                            {
                                listOfSeedRanges.Add(
                                    new SeedRange()
                                    {
                                        seedStart = map.sourceRangeStart + map.rangeLength,
                                        seedEnd = currentSeedRange.seedEnd
                                    });
                            }

                            rangeHasCollided = true;
                        }
                    }

                    if (!rangeHasCollided)
                    {
                        listOfSeedRangesForNextLevel.Add(
                            new SeedRange()
                            {
                                seedStart = currentSeedRange.seedStart,
                                seedEnd = currentSeedRange.seedEnd
                            });
                    }
                }

                listOfSeedRanges = MergeSeedRanges(new List<SeedRange>(listOfSeedRangesForNextLevel));
                //listOfSeedRanges = new List<SeedRange>(listOfSeedRangesForNextLevel);
            }
            
            //listOfSeedRanges = listOfSeedRanges.OrderBy(seedRange => seedRange.seedStart).ToList();

            return listOfSeedRanges[0].seedStart.ToString();
        }

        public List<SeedRange> MergeSeedRanges(List<SeedRange> listOfSeedRanges)
        {
            if (listOfSeedRanges.Count <= 1)
            {
                return listOfSeedRanges;
            }

            listOfSeedRanges = listOfSeedRanges.OrderBy(seedRange => seedRange.seedStart).ToList();

            List<SeedRange> mergedRanges = new List<SeedRange>();

            SeedRange currentSeedRange = listOfSeedRanges[0];

            for (int i = 1; i < listOfSeedRanges.Count; i++)
            {
                SeedRange nextSeedRange = listOfSeedRanges[i];

                if (currentSeedRange.seedEnd >= nextSeedRange.seedStart)
                {
                    currentSeedRange.seedEnd = long.Max(currentSeedRange.seedEnd, nextSeedRange.seedEnd);
                }
                else
                {
                    mergedRanges.Add(currentSeedRange);
                    currentSeedRange = nextSeedRange;
                }
            }

            mergedRanges.Add(currentSeedRange);

            return mergedRanges;
        }
        public string RunPartBOnBruteForce(Dictionary<string, List<Map>> dicOfLevels, IEnumerable<long> listOfSeedRanges)
        {
            long lowestLocation = long.MaxValue;

            object lowestLocationLock = new object();

            Parallel.ForEach(listOfSeedRanges, seed =>
            {
                long currentPosition = seed;

                foreach (KeyValuePair<string, List<Map>> level in dicOfLevels)
                {
                    currentPosition = MapValue(level, currentPosition);
                }

                lock (lowestLocationLock)
                {
                    if (currentPosition < lowestLocation)
                    {
                        lowestLocation = currentPosition;
                    }
                }
            });

            return lowestLocation.ToString();
        }

        public long MapValue(KeyValuePair<string, List<Map>> maps, long currentPosition)
        {
            long newPosition = currentPosition;

            foreach (Map map in maps.Value)
            {
                if (currentPosition >= map.sourceRangeStart &&
                    currentPosition < map.sourceRangeStart + map.rangeLength)
                {
                    newPosition = map.destinationRangeStart + (currentPosition - map.sourceRangeStart);
                }
            }

            return newPosition;
        }

        public List<long> GetSeeds(string[] inputData)
        {
            return inputData[0]
                .Split(":")[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse)
                .ToList();
        }

        public IEnumerable<long> GetListOfSeedsFromRange(List<long> seedsInput)
        {
            for (int i = 0; i < seedsInput.Count; i += 2)
            {
                for (long j = seedsInput[i]; j < seedsInput[i] + seedsInput[i + 1]; j++)
                {
                    yield return j;
                }
            }
        }

        public List<SeedRange> GetListOfSeedsRange(List<long> seedsInput)
        {
            List<SeedRange> listOfSeedsRange = new List<SeedRange>();

            for (int i = 0; i < seedsInput.Count; i += 2)
            {
                listOfSeedsRange.Add(new SeedRange()
                {
                    seedStart = seedsInput[i],
                    seedEnd = seedsInput[i] + seedsInput[i + 1]
                });
            }

            return listOfSeedsRange;
        }

        public Dictionary<string, List<Map>> GetMaps(string[] inputData)
        {
            Dictionary<string, List<Map>> maps = new Dictionary<string, List<Map>>();

            string currentMapName = string.Empty;

            foreach (string line in inputData)
            {
                if (!string.IsNullOrEmpty(currentMapName) && !string.IsNullOrEmpty(line))
                {
                    if (!maps.TryGetValue(currentMapName, out _))
                    {
                        maps.Add(currentMapName, new List<Map>());
                    }

                    string[] numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    maps[currentMapName].Add(new Map()
                    {
                        destinationRangeStart = long.Parse(numbers[0]),
                        sourceRangeStart = long.Parse(numbers[1]),
                        rangeLength = long.Parse(numbers[2])
                    });
                }
                else
                {
                    Match match = Regex.Match(line, @"([\w-]+)\s*map:");

                    if (match.Success)
                    {
                        currentMapName = match.Groups[1].Value;
                    }
                    else if (line == string.Empty)
                    {
                        currentMapName = string.Empty;
                    }
                }
            }

            return maps;
        }
    }

    internal class SeedRange
    {
        public required long seedStart;
        public required long seedEnd;
    }

    internal class Map
    {
        public required long destinationRangeStart;
        public required long sourceRangeStart;
        public required long rangeLength;
    }
}
