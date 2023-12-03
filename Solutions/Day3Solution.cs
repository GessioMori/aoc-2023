namespace Main.Solutions
{
    internal class Day3Solution : ISolution
    {
        List<int> availableParts = new List<int>();
        List<string> availablePartsIds = new List<string>();

        public string RunPartA(string[] inputData)
        {
            int lineLength = inputData[0].Length;

            Position[][] positions = MapPositions(inputData);

            for (int i = 0; i < inputData.Length; i++)
            {
                for (int j = 0; j < lineLength; j++)
                {
                    Position pos = positions[i][j];

                    if (pos.positionType == PositionType.symbol)
                    {
                        for (int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                int curX = pos.positionNumber.Item1 - k;
                                int curY = pos.positionNumber.Item2 - l;

                                if (curX >= 0 && curY >= 0)
                                {
                                    int? currentValue = positions[curX][curY].associatedPart;
                                    string? currentId = positions[curX][curY].partId;

                                    if (currentValue != null && currentId != null)
                                    {
                                        AddValueToAvailableParts(currentId, currentValue);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int total = 0;

            foreach (int i in availableParts)
            {
                total += i;
            }

            return total.ToString();
        }

        public void AddValueToAvailableParts(string? id, int? value)
        {
            if (id != null && !this.availablePartsIds.Contains(id ?? ""))
            {
                this.availableParts.Add(value ?? 0);
                this.availablePartsIds.Add(id ?? "");
            }
        }

        public string RunPartB(string[] inputData)
        {
            int lineLength = inputData[0].Length;
            int total = 0;

            Position[][] positions = MapPositions(inputData);

            for (int i = 0; i < inputData.Length; i++)
            {
                for (int j = 0; j < lineLength; j++)
                {
                    Position pos = positions[i][j];

                    if (pos.positionType == PositionType.symbol && pos.positionValue == '*')
                    {
                        for (int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                int curX = pos.positionNumber.Item1 - k;
                                int curY = pos.positionNumber.Item2 - l;

                                if (curX >= 0 && curY >= 0)
                                {
                                    int? currentValue = positions[curX][curY].associatedPart;
                                    string? currentId = positions[curX][curY].partId;

                                    if (currentValue != null && currentId != null)
                                    {
                                        AddValueToAvailableParts(currentId, currentValue);
                                    }
                                }
                            }
                        }

                        int currentResult = 1;

                        foreach (int num in availableParts)
                        {
                            currentResult *= num;
                        }

                        if (availableParts.Count == 2)
                        {
                            total += currentResult;
                        }

                        this.availablePartsIds.Clear();
                        this.availableParts.Clear();
                    }
                }
            }

            return total.ToString();
        }


        private static Position[][] MapPositions(string[] inputData)
        {
            int lineLength = inputData[0].Length;
            Position[][] positions = new Position[inputData.Length][];

            for (int i = 0; i < inputData.Length; i++)
            {
                positions[i] = new Position[lineLength];

                char[] line = inputData[i].ToCharArray();

                List<Position> currentPositions = new List<Position>();
                string curPartId = "";

                for (int j = 0; j < lineLength; j++)
                {
                    PositionType positionType = Position.CheckPositionType(line[j]);
                    Position position = new Position(i, j, line[j], null, null);

                    if (positionType == PositionType.digit)
                    {
                        currentPositions.Add(position);
                        string currentAssociatedPart = "";
                        foreach (Position pos in currentPositions)
                        {
                            currentAssociatedPart += pos.positionValue.ToString();
                        }
                        foreach (Position pos in currentPositions)
                        {
                            pos.associatedPart = int.Parse(currentAssociatedPart);
                            pos.partId = curPartId.Length > 0 ? curPartId : Guid.NewGuid().ToString();
                            curPartId = pos.partId;
                        }
                    }
                    if (positionType == PositionType.dot || positionType == PositionType.symbol)
                    {
                        currentPositions.Clear();
                        curPartId = "";
                    }
                    positions[i][j] = position;
                }
            }
            return positions;
        }
    }

    internal enum PositionType
    {
        dot,
        digit,
        symbol
    }

    internal class Position
    {
        public (int, int) positionNumber;
        public PositionType positionType;
        public int? associatedPart;
        public char positionValue;
        public string? partId;
        public Position(int x, int y, char value, int? associatedPart, string? partId)
        {
            this.positionNumber = (x, y);
            this.positionType = CheckPositionType(value);
            this.associatedPart = associatedPart;
            this.positionValue = value;
            this.partId = partId;
        }

        public static PositionType CheckPositionType(char value)
        {
            if (value == '.')
            {
                return PositionType.dot;
            }
            else if (int.TryParse(value.ToString(), out _))
            {
                return PositionType.digit;
            }
            else
            {
                return PositionType.symbol;
            }
        }
    }
}
