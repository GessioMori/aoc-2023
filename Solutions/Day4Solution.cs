using Main.Tools;

namespace Main.Solutions
{
    internal class Day4Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            List<Card> cards = CreateCards(inputData);

            int total = 0;

            foreach (Card card in cards)
            {
                int correctNumbers = 0;

                foreach (int number in card.winningNumbers)
                {
                    if (card.cardNumbers.Contains(number))
                    {
                        correctNumbers++;
                    }
                }

                if (correctNumbers > 0)
                {
                    total += (int)Math.Pow(2, correctNumbers - 1);
                }
            }

            return total.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            List<Card> cards = CreateCards(inputData);

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];

                int correctNumbers = 0;

                foreach (int number in card.winningNumbers)
                {
                    if (card.cardNumbers.Contains(number))
                    {
                        correctNumbers++;
                    }
                }

                for (int j = i + 1; (j < i + correctNumbers + 1) && (j < cards.Count); j++)
                {
                    cards[j].numberOfInstances += card.numberOfInstances;
                }
            }

            int totalCards = 0;

            foreach (Card card in cards)
            {
                totalCards += card.numberOfInstances;
            }

            return totalCards.ToString();
        }

        public List<Card> CreateCards(string[] inputData)
        {
            List<Card> cards = new List<Card>();

            foreach (string line in inputData)
            {
                string[] parts = line.Split([':', '|']);

                cards.Add(new Card()
                {
                    winningNumbers = GetNumbers(parts[1]),
                    cardNumbers = GetNumbers(parts[2])
                });
            }

            return cards;
        }

        public List<int> GetNumbers(string cardPart)
        {
            return cardPart.Split([' '])
                    .Select(str => str.Trim())
                    .Where(str => str.Length > 0)
                    .Select(int.Parse)
                    .ToList();
        }
    }

    internal class Card
    {
        public required List<int> winningNumbers = new List<int>();
        public required List<int> cardNumbers = new List<int>();
        public int numberOfInstances = 1;
    }
}
