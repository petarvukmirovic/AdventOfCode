using Common;

namespace Five
{
    internal class Program
    {
        static void PartOne()
        {
            (long[] seeds, SourceDestinationMap[] maps) = ParseInput(Io.AllInputLines());
            var minimumLocation = seeds.Select(seed => maps.Aggregate(seed, (seed, map) => map[seed])).Min();
            Console.WriteLine(minimumLocation);
        }

        static void PartTwo()
        {
            (long[] seeds, SourceDestinationMap[] maps) = ParseInput(Io.AllInputLines());
            var seedsRanges = new List<InclusiveRange>();
            for(int i=0; i<seeds.Length; i+=2) 
            {
                seedsRanges.Add(new(seeds[i], seeds[i] + seeds[i + 1] - 1));
            }

            var minimumLocation =
                    seedsRanges.Select(sr => maps.Aggregate(
                        Enumerable.Empty<InclusiveRange>().Append(sr),
                        (acc, map) => acc.SelectMany(range => map[range])))
                    .SelectMany(range => range.Select(r => r.start))
                    .Min();
            Console.WriteLine(minimumLocation);
        }

        private static (long[] seeds, SourceDestinationMap[] maps) ParseInput(IEnumerable<string> allLines)
        {
            var linesIterator = allLines.GetEnumerator();
            linesIterator.MoveNext();

            var firstLine = linesIterator.Current;
            var seeds = firstLine.Substring(firstLine.IndexOf(':') + 1).Split(' ', Io.IgnoreEmptyElements).Select(long.Parse).ToArray();

            linesIterator.MoveNext();
            linesIterator.MoveNext();

            var maps = new List<SourceDestinationMap>();

            while(linesIterator.MoveNext())
            {
                var mapLines = new List<string>();
                var endReached = false;
                while(!string.IsNullOrEmpty(linesIterator.Current) && !endReached)
                {
                    mapLines.Add(linesIterator.Current);
                    endReached = !linesIterator.MoveNext();
                }
                maps.Add(SourceDestinationMap.FromStringList(mapLines));

                linesIterator.MoveNext();
            }

            return (seeds, maps.ToArray());
        }

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
