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

            IEnumerable<long> seeds = GetSeedsRange(GetSeeds(inputData));

            long lowestLocation = long.MaxValue;

            object lowestLocationLock = new object();

            Parallel.ForEach(seeds, seed =>
            {
                long currentPosition = seed;

                foreach (KeyValuePair<string, List<Map>> map in maps)
                {
                    currentPosition = MapValue(map, currentPosition);
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

        public IEnumerable<long> GetSeedsRange(List<long> seedsInput)
        {
            for (int i = 0; i < seedsInput.Count; i += 2)
            {
                for (long j = seedsInput[i]; j < seedsInput[i] + seedsInput[i + 1]; j++)
                {
                    yield return j;
                }
            }
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

    internal class Map
    {
        public required long destinationRangeStart;
        public required long sourceRangeStart;
        public required long rangeLength;
    }
}
