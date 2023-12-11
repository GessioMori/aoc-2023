namespace Main.Solutions
{
    internal class Day10Solution : ISolution
    {
        public Dictionary<char, (int[], int[])> PipeDirections = new()
            {
                {'|', ([0,1],[0,-1])},
                {'-', ([1,0],[-1,0])},
                {'L', ([1,0],[0,-1])},
                {'J', ([-1,0],[0,-1])},
                {'7', ([0,1],[-1,0])},
                {'F', ([0,1],[1,0])},
            };

        public string RunPartA(string[] inputData)
        {
            char[][] map = CreateMap(inputData);

            int[] startPosition = FindStart(map);

            int[] nextPosition = FindNextPositionFromStart(startPosition, map);

            int[] currentPosition = startPosition;

            int countMoves = 1;

            while (!nextPosition.SequenceEqual(startPosition))
            {
                countMoves++;

                int[] tmpPos = nextPosition;

                nextPosition = GetNextPosition(currentPosition,
                    nextPosition,
                    map[nextPosition[0]][nextPosition[1]]);

                currentPosition = tmpPos;
            }

            return (countMoves / 2).ToString();
        }

        public string RunPartB(string[] inputData)
        {
            char[][] map = CreateMap(inputData);

            int[] startPosition = FindStart(map);

            int[] nextPosition = FindNextPositionFromStart(startPosition, map);

            List<int[]> listOfPositions = GetListOfPostions(startPosition, nextPosition, map);

            List<(int, int)> listOfTuples = listOfPositions.Select(arr => (arr[0], arr[1])).ToList();

            map[startPosition[0]][startPosition[1]] = SubstituteSPipe(map, startPosition, listOfPositions);

            List<char> listOfLeftPipes = ['-', 'J', '7'];

            int countInside = 0;

            for (int i = 0; i < map.Length; i++)
            {
                bool isCounting = false;

                for (int j = 0; j < map[0].Length; j++)
                {
                    char currentPipe = map[i][j];

                    if (listOfTuples.Contains((i, j)) && listOfLeftPipes.Contains(currentPipe))
                    {
                        isCounting = !isCounting;
                    }
                    else if (isCounting && !listOfTuples.Contains((i, j)))
                    {
                        countInside++;
                    }
                }
            }

            return countInside.ToString();
        }

        public char SubstituteSPipe(char[][] originalMap, int[] startPosition, List<int[]> listOfPositions)
        {
            int[] movementVectorStart = [listOfPositions[1][0] - startPosition[0], listOfPositions[1][1] - startPosition[1]];
            int[] movementVectorEnd = [listOfPositions[^1][0] - startPosition[0], listOfPositions[^1][1] - startPosition[1]];

            foreach (var pipeDirection in PipeDirections)
            {
                if ((pipeDirection.Value.Item1.ToList().SequenceEqual(movementVectorStart) &&
                    pipeDirection.Value.Item2.ToList().SequenceEqual(movementVectorEnd)) ||
                    (pipeDirection.Value.Item2.ToList().SequenceEqual(movementVectorStart) &&
                    pipeDirection.Value.Item1.ToList().SequenceEqual(movementVectorEnd)))
                {
                    return pipeDirection.Key;
                }
            }

            throw new Exception("Could not find substitute for S");
        }
        public List<int[]> GetListOfPostions(int[] startPosition, int[] nextPosition, char[][] map)
        {
            List<int[]> listOfPositions = [startPosition, nextPosition];

            int[] currentPosition = startPosition;

            while (!nextPosition.SequenceEqual(startPosition))
            {
                int[] tmpPos = nextPosition;

                nextPosition = GetNextPosition(currentPosition,
                    nextPosition,
                    map[nextPosition[0]][nextPosition[1]]);

                if (nextPosition.SequenceEqual(startPosition))
                {
                    break;
                }

                listOfPositions.Add(nextPosition);

                currentPosition = tmpPos;
            }

            return listOfPositions;
        }
        public static char[][] CreateMap(string[] inputData)
        {
            char[][] originalMap = inputData.Select(line => line.ToCharArray()).ToArray();

            int rows = originalMap.Length;
            int cols = originalMap[0].Length;

            char[][] transposedMap = new char[cols][];

            for (int col = 0; col < cols; col++)
            {
                transposedMap[col] = new char[rows];

                for (int row = 0; row < rows; row++)
                {
                    transposedMap[col][row] = originalMap[row][col];
                }
            }

            return transposedMap;
        }
        public static int[] FindStart(char[][] map)
        {
            for (int x = 0; x < map.Length; x++)
            {
                for (int y = 0; y < map[0].Length; y++)
                {
                    if (map[x][y] == 'S')
                    {
                        return [x, y];
                    }
                }
            }

            throw new Exception("Could not find the start.");
        }
        public int[] GetNextPosition(int[] previousPosition, int[] currentPosition, char currentPipe)
        {
            int[] lastMovementVector = [(currentPosition[0] - previousPosition[0]) * -1,
                (currentPosition[1] - previousPosition[1]) * -1];

            int[] nextMovementVector = FindOtherArray(PipeDirections[currentPipe], lastMovementVector);

            return [currentPosition[0] + nextMovementVector[0],
                currentPosition[1] + nextMovementVector[1]];
        }
        static int[] FindOtherArray((int[], int[]) tuple, int[] givenArray)
        {
            if (tuple.Item1.SequenceEqual(givenArray))
            {
                return tuple.Item2;
            }
            else if (tuple.Item2.SequenceEqual(givenArray))
            {
                return tuple.Item1;
            }
            else
            {
                throw new ArgumentException("Given array not found in the tuple.");
            }
        }
        public int[] FindNextPositionFromStart(int[] startPosition, char[][] map)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i == 0 || j == 0) && (i != 0 || j != 0))
                    {
                        int possibleX = startPosition[0] + i;
                        int possibleY = startPosition[1] + j;

                        if (possibleX >= 0 && possibleY >= 0)
                        {
                            char possiblePipe = map[possibleX][possibleY];

                            if (PipeDirections.ContainsKey(possiblePipe))
                            {
                                int[] possibleMovement = [-i, -j];

                                if (possibleMovement.SequenceEqual(PipeDirections[possiblePipe].Item1) ||
                                    possibleMovement.SequenceEqual(PipeDirections[possiblePipe].Item2))

                                    return [possibleX, possibleY];
                            }
                        }
                    }
                }
            }

            throw new Exception("Position not found.");
        }
    }
}
