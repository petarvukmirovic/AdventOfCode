using Common;

namespace Seven
{
    internal class CamelCardHand : IComparable<CamelCardHand>
    {
        private CamelCardHand() { }

        public required int[] Hand { get; init; }
        public required int Bid { get; init; }

        public int[] OccurenceMap => OccurenceMapOfHand(Hand);
            
        private static int[] OccurenceMapOfHand(int[] hand) =>
            hand.Aggregate(new int[14], (acc, card) => { acc[card - 1]++; return acc; });

        public int CompareTo(CamelCardHand? other)
        {
            if (other == null)
            {
                return 1;
            }

            var comparisonRes = ComparisonCriteria.CompareByOrderedComparisonCriteria(this, other);
            return comparisonRes != 0 ? comparisonRes : ComparisonCriteria.LexicographicComparison(this, other);
        }

        private CamelCardHand? _withOptimizedJokers = null;
        public CamelCardHand WithOptimizedJokers
        {
            get => _withOptimizedJokers = _withOptimizedJokers ?? OptimizeJokers();
        }

        private CamelCardHand OptimizeJokers()
        {
            var nonJokerCards = Hand.Where(c => c != 1).ToArray();
            if(nonJokerCards.Any())
            {
                var nonJokerOccMap = OccurenceMapOfHand(nonJokerCards);
                var maxOcc = nonJokerOccMap.Max();
                var cardWithMaxOcc = nonJokerOccMap.Select((occ, idx) => (occ, idx:idx+1)).First(pair => pair.occ == maxOcc).idx;
                return new CamelCardHand
                {
                    Hand = Hand.Select(c => c == 1 ? cardWithMaxOcc : c).ToArray(),
                    Bid = Bid
                };
            }
            else
            {
                return new CamelCardHand
                {
                    Hand = (int[])Hand.Clone(),
                    Bid = Bid
                };
            }


        }

        public override string ToString() => 
            $"{string.Join(", ", Hand)} | {string.Join(", ", Hand.Order())}";

        public static CamelCardHand FromLine(string line, bool jokersAreWeakest = false)
        {
            var handAndBid = line.Split(' ', Io.IgnoreEmptyElements);
            return new CamelCardHand()
            {
                Hand = ParseCards(handAndBid[0], jokersAreWeakest),
                Bid = int.Parse(handAndBid[1]),
            };
        }

        private static int[] ParseCards(string cards, bool jokersAreWeakest) =>
            cards.Select(card => char.IsDigit(card) ? card - '0' : LetterCardToIntCard(card, jokersAreWeakest)).ToArray();

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        private static int LetterCardToIntCard(char card, bool jokersAreWeakest) =>
            card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => jokersAreWeakest ? 1 : 11,
                'T' => 10
            };
#pragma warning restore CS8509
    }
}
