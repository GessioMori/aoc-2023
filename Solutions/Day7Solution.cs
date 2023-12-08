using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day7Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            int total = 0;

            Dictionary<char, int> cardValues = new Dictionary<char, int>()
            {
                {'2', 1 },
                {'3', 2 },
                {'4', 3 },
                {'5', 4 },
                {'6', 5 },
                {'7', 6 },
                {'8', 7 },
                {'9', 8 },
                {'T', 9 },
                {'J', 10 },
                {'Q', 11 },
                {'K', 12 },
                {'A', 13 },
            };

            List<Hand> hands = ProcessHands(inputData, cardValues);
            hands.Sort();

            for (int i = 0; i < hands.Count; i++)
            {
                total += hands[i].Bid * (i + 1);
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            int total = 0;

            Dictionary<char, int> cardValues = new Dictionary<char, int>()
            {
                {'J', 1 },
                {'2', 2 },
                {'3', 3 },
                {'4', 4 },
                {'5', 5 },
                {'6', 6 },
                {'7', 7 },
                {'8', 8 },
                {'9', 9 },
                {'T', 10 },
                {'Q', 11 },
                {'K', 12 },
                {'A', 13 },
            };

            List<Hand> hands = ProcessHands(inputData, cardValues, true);
            hands.Sort();

            for (int i = 0; i < hands.Count; i++)
            {
                total += hands[i].Bid * (i + 1);
            }

            return total.ToString();
        }

        public List<Hand> ProcessHands(string[] inputData, Dictionary<char, int> cardValues, bool considerJokers = false)
        {
            List<Hand> hands = new();

            foreach (string line in inputData)
            {
                Match match = Regex.Match(line, @"(\w{5}) (\d+)");

                if (match.Success)
                {
                    hands.Add(new Hand(match.Groups[1].ToString().ToCharArray(),
                        int.Parse(match.Groups[2].Value),
                        cardValues,
                        considerJokers));
                }
            }

            return hands;
        }
    }

    public enum HandStrength
    {
        FiveOfKind = 7,
        FourOfKind = 6,
        FullHouse = 5,
        ThreeOfKind = 4,
        TwoPair = 3,
        OnePair = 2,
        HighCard = 1
    }

    internal class Hand : IComparable<Hand>
    {
        public char[] Cards;
        public int Bid;
        public HandStrength HandStrength;
        public Dictionary<char, int> CardValues;

        public Hand(char[] cards, int bid, Dictionary<char, int> cardValues, bool considerJokes = false)
        {
            this.Cards = cards;
            this.Bid = bid;
            this.CardValues = cardValues;

            if (considerJokes)
            {
                this.HandStrength = CalculateHandStrenghtWithJokers(cards);

            }
            else
            {
                this.HandStrength = CalculateHandStrenght(cards);

            }
        }

        public HandStrength CalculateHandStrenght(char[] cards)
        {
            Dictionary<char, int> charCountDictionary = new Dictionary<char, int>();

            foreach (char c in cards)
            {
                if (charCountDictionary.ContainsKey(c))
                {
                    charCountDictionary[c]++;
                }
                else
                {
                    charCountDictionary.Add(c, 1);
                }
            }

            int distinctCount = charCountDictionary.Count;
            int maxCount = charCountDictionary.Max(pair => pair.Value);

            switch (distinctCount)
            {
                case 1:
                    return HandStrength.FiveOfKind;
                case 2:
                    return maxCount == 4 ? HandStrength.FourOfKind :
                           maxCount == 3 ? HandStrength.FullHouse :
                           HandStrength.TwoPair;
                case 3:
                    return maxCount == 3 ? HandStrength.ThreeOfKind :
                           HandStrength.TwoPair;
                case 4:
                    return HandStrength.OnePair;
                default:
                    return HandStrength.HighCard;
            }
        }

        public HandStrength CalculateHandStrenghtWithJokers(char[] cards)
        {
            HandStrength handStrength = CalculateHandStrenght(cards);

            List<char[]> combinations = GenerateAllCombinations(cards, 0, new List<char[]>());

            foreach (char[] combination in combinations)
            {
                HandStrength newHandStrength = CalculateHandStrenght(combination);
                if (newHandStrength > handStrength)
                {
                    handStrength = newHandStrength;
                }
            }

            return handStrength;
        }

        public List<char[]> GenerateAllCombinations(char[] cards, int currentIndex, List<char[]> combinations)
        {
            if (currentIndex == cards.Length)
            {
                combinations.Add(cards.ToArray());
                return combinations;
            }

            if (cards[currentIndex] == 'J')
            {
                foreach (KeyValuePair<char, int> card in this.CardValues)
                {
                    cards[currentIndex] = card.Key;
                    GenerateAllCombinations(cards, currentIndex + 1, combinations);
                }

                cards[currentIndex] = 'J';
            }
            else
            {
                GenerateAllCombinations(cards, currentIndex + 1, combinations);
            }

            return combinations;
        }

        public int CompareTo(Hand? other)
        {
            int handStrengthComparison = HandStrength.CompareTo(other?.HandStrength);

            if (handStrengthComparison != 0)
            {
                return handStrengthComparison;
            }

            for (int i = 0; i < Cards.Length; i++)
            {
                if (other != null)
                {
                    int thisCardValue = this.CardValues[Cards[i]];
                    int otherCardValue = this.CardValues[other.Cards[i]];

                    int cardComparison = thisCardValue.CompareTo(otherCardValue);

                    if (cardComparison != 0)
                    {
                        return cardComparison;
                    }
                }
            }

            return Bid.CompareTo(other?.Bid);
        }
    }
}
