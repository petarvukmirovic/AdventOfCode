
using Common;

namespace Six
{
    internal class Program
    {
        private record Race(long time, long distance)
        {
            // can be computed faster if we stop at the first hit
            public long NumberOfWaysToBeat
            {
                get
                {
                    var waysToBeat = 0L;
                    for(long i=1; i<time; i++)
                    {
                        var score = i * (time - i);
                        waysToBeat += score > distance ? 1 : 0;
                    }
                    return waysToBeat;
                }
            }
        }

        static void Main(string[] args)
        {
            //PartOne();
            PartTwo();
        }

        private static IEnumerable<Race> ParseInput(IEnumerable<string> lines)
        {
            IEnumerable<long> ParseNumbers(string line) =>
                line.Substring(line.IndexOf(':') + 1).Split(' ', Io.IgnoreEmptyElements).Select(long.Parse);

            return ParseNumbers(lines.First())
                   .Zip(ParseNumbers(lines.First()))
                   .Select((pair) => new Race(time:pair.First, distance: pair.Second));
        }

        private static void PartOne()
        {
            var races = ParseInput(Io.AllInputLines());
            var product = races.Select(r => r.NumberOfWaysToBeat)
                               .Aggregate(1L, (acc, numOfWays) => acc*numOfWays);
            Console.WriteLine(product);
        }

        private static void PartTwo()
        {
            var race = ParseInput(Io.AllInputLines().Select(line => line.Replace(" ", ""))).Single();
            Console.WriteLine(race.NumberOfWaysToBeat);
        }
    }
}
