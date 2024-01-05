
namespace Five
{
    internal class SourceDestinationMap
    {
        private record MapRangeDefinition(long SourceStart, long DestinationStart, long RangeLength)
        {
            public long MapValue(long val) => 
                val >= SourceStart && SourceStart + RangeLength > val ? 
                    DestinationStart + val - SourceStart 
                    : val;

            public InclusiveRange MapRangeWithOverlappingStart(InclusiveRange range)
            {
                if(range.start >= SourceStart && range.start < SourceStart + RangeLength)
                {
                    var offset = range.start - SourceStart;
                    var remainingElements = Math.Min(RangeLength-offset, range.Length);
                    var mappedStart = DestinationStart + offset;
                    return new InclusiveRange(mappedStart, mappedStart + remainingElements-1);
                }
                else
                {
                    return new InclusiveRange(range);
                }
            }

            public InclusiveRange AsSourceRange => new InclusiveRange(SourceStart, SourceStart + RangeLength - 1);
        }

        private SourceDestinationMap() { }

        private MapRangeDefinition[] MappedRanges { get; set; } = new MapRangeDefinition[0];
        public static SourceDestinationMap FromStringList(IEnumerable<string> lines) =>
            new SourceDestinationMap()
            {
                MappedRanges = lines.Select(line =>
                {
                    var definitionElements =
                        line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                    return new MapRangeDefinition(
                        SourceStart: definitionElements[1],
                        DestinationStart: definitionElements[0],
                        RangeLength: definitionElements[2]);
                })
                .OrderBy(mr => mr.SourceStart)
                .ToArray()
            };

        private long MapValue(long i)
        {
            var mappingCandidate = MappedRanges.TakeWhile(mr => mr.SourceStart <= i).LastOrDefault();
            return mappingCandidate == null ? i : mappingCandidate.MapValue(i);
        }

        private InclusiveRange[] MapValue(InclusiveRange range)
        {
            var valuesMappedForRange = MappedRanges.Where(mr => mr.AsSourceRange.Intersects(range)).ToArray();
            List<InclusiveRange> resultingRanges = new List<InclusiveRange>();
            var rangeToMapStart = range.start;

            foreach(var mr in valuesMappedForRange)
            {
                if(mr.SourceStart > rangeToMapStart)
                {
                    // unpammed part
                    resultingRanges.Add(new InclusiveRange(rangeToMapStart, mr.SourceStart - 1));
                    rangeToMapStart = mr.SourceStart;
                }

                var mappedRange = mr.MapRangeWithOverlappingStart(new(rangeToMapStart, range.end));
                rangeToMapStart = rangeToMapStart + mappedRange.Length;
                resultingRanges.Add(mappedRange);
            }

            if(rangeToMapStart < range.end)
            {
                resultingRanges.Add(new InclusiveRange(rangeToMapStart, range.end));
            }

            return resultingRanges.ToArray();
        }

        public long this[long i] { get => MapValue(i); }
        public InclusiveRange[] this[InclusiveRange range] { get => MapValue(range); }

    }
}
