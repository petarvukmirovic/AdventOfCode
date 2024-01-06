namespace Seven
{
    internal interface IComparisonCriterion
    {
        bool SatifiesCriterion(CamelCardHand card);
    }

    internal static class ComparisonCriteria
    {
        public static int LexicographicComparison(CamelCardHand a, CamelCardHand b) =>
            a.Hand.Zip(b.Hand).Select(pair => pair.First - pair.Second).FirstOrDefault(x => x!=0);

        public static int CompareByOrderedComparisonCriteria(CamelCardHand a, CamelCardHand b) =>
            OrderedCriteria
            .Select(crit =>
            {
                int thisSatisfiesCriterion = crit.SatifiesCriterion(a) ? 1 : -1;
                int otherSatisfiesCriterion = crit.SatifiesCriterion(b) ? 1 : -1;
                return thisSatisfiesCriterion - otherSatisfiesCriterion;
            })
            .FirstOrDefault(x => x != 0);

        private static IEnumerable<IComparisonCriterion> OrderedCriteria =
           new IComparisonCriterion[]
           {
                new FiveOfAKind(), new FourOfAKind(), new FullHouse(), new ThreeOfAKind(),
                new TwoPair(), new OnePair(), new HighCard()
           };

        private class FiveOfAKind : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card)
            {
                var firstCard = card.Hand.First();
                var otherCards = card.Hand.Skip(1);
                return otherCards.All(c => firstCard == c);
            }
        }

        private class FourOfAKind : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card) => card.OccurenceMap.Any(occ => occ == 4 && occ != 0);
        }

        private class FullHouse : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card) => card.OccurenceMap.All(occ => occ == 3 || occ == 2 || occ == 0);
        }

        private class ThreeOfAKind : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card) => card.OccurenceMap.Count(occ => occ == 3) == 1 && card.OccurenceMap.Count(occ => occ == 1) == 2;
        }

        private class TwoPair : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card) => card.OccurenceMap.Count(occ => occ == 2) == 2;
        }

        private class OnePair : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card) => card.OccurenceMap.Count(occ => occ == 1) == 3 &&
                                                                 card.OccurenceMap.Count(occ => occ == 2) == 1;
        }

        private class HighCard : IComparisonCriterion
        {
            public bool SatifiesCriterion(CamelCardHand card) => card.OccurenceMap.All(occ => occ == 1);
        }
    }
}
