using Common;

namespace Seven
{
    internal class Program
    {
        static void PartOne()
        {
            var cards = Io.AllInputLines().Select(l => CamelCardHand.FromLine(l, jokersAreWeakest:false))
                                          .ToArray();
            Array.Sort(cards);
            var totalWinnings = 
                cards.Select((c, i) => (card: c, index: i + 1))
                     .Aggregate(0L, (sum, pair) => sum + pair.card.Bid * pair.index);
            Console.WriteLine(totalWinnings);
        }

        static void PartTwo()
        {
            var cards = Io.AllInputLines().Select(l => CamelCardHand.FromLine(l, jokersAreWeakest:true))
                                          .ToArray();

            Array.Sort(cards, (a, b) =>
            {
                (var aOptimized, var bOptimized) = (a.WithOptimizedJokers, b.WithOptimizedJokers);
                var optimizedComparison = ComparisonCriteria.CompareByOrderedComparisonCriteria(aOptimized, bOptimized);
                return optimizedComparison != 0 ? optimizedComparison : ComparisonCriteria.LexicographicComparison(a, b);
            });
            var totalWinnings =
               cards.Select((c, i) => (card: c, index: i + 1))
                    .Aggregate(0L, (sum, pair) => sum + pair.card.Bid * pair.index);
            Console.WriteLine(totalWinnings);
        }

        static void Main(string[] args)
        {
            //PartOne();
            PartTwo();
        }
    }
}
