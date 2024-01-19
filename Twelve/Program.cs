using Common;

namespace Twelve
{
    internal class Program
    {
        static void Solve(Func<string, int[], long> Counter)
        {
            var allConfiguartionsAndSegments = ParseInput();
            var sumOfArrangementCounts = 
                allConfiguartionsAndSegments.Select(pair =>
                {
                    var nrOfArrangements = Counter(pair.configuration, pair.segments);
                    Console.WriteLine($"({pair.configuration}, {string.Join(',', pair.segments)}): {nrOfArrangements}");
                    return nrOfArrangements;
                }).Sum();
            Console.WriteLine(sumOfArrangementCounts);
        }

        static void PartTwo()
        {
            Solve(CountMultipliedArrangements);
        }

        static void PartOne()
        {
            Solve(CountArrangements);
        }

        static List<(string configuration, int[] segments)> ParseInput() =>
            Io.AllInputLines().Select(line =>
            {
                var splitLine = line.Split(' ', Io.IgnoreEmptyElements);
                return (splitLine[0], splitLine[1].Split(',', Io.IgnoreEmptyElements).Select(int.Parse).ToArray());
            }).ToList();

        public static long CountMultipliedArrangements(string configuration, int[] segments)
        {
            var precomputedHashSegments = PrecomputeHashSegments(configuration, segments);
            return CountMultipliedArrangementsInternal(configuration, segments, segmentsBoundByHash: 0, remainingSeparators: 4, precomputedHashSegments);
        }

        private static Dictionary<string, long> PrecomputeHashSegments(string configuration, int[] segments)
        {
            var initialValues = 
                new string[] { configuration, configuration + "#", "#" + configuration, "#" + configuration + "#" };
            var memoizationTable = new Dictionary<string, long>();
            foreach (var initConfiguration in initialValues)
            {
                memoizationTable.Add(initConfiguration, CountArrangements(initConfiguration, segments));
            }

            for(int i=1; i<=4; i++)
            {
                var joinedConfiguration = string.Join('#', Enumerable.Repeat(configuration, i));
                memoizationTable[joinedConfiguration] = CountJoinedArrangements(configuration, memoizationTable, numberOfSeparators: i, startWithHash: false);
            }

            return memoizationTable;
        }

        private static long CountJoinedArrangements(string configuration, Dictionary<string, long> memoizationTable, int numberOfSeparators, bool startWithHash)
        {
            string configurationBase = startWithHash ? "#" + configuration : configuration;
            if (numberOfSeparators == 0)
            {
                return memoizationTable[configurationBase];
            }

            long withoutHashAtEnd = memoizationTable[configurationBase] * CountJoinedArrangements(configuration, memoizationTable, numberOfSeparators - 1, startWithHash: true);
            long withHashAtEnd = memoizationTable[configurationBase+"#"] * CountJoinedArrangements(configuration, memoizationTable, numberOfSeparators - 1, startWithHash: false);
            return withoutHashAtEnd + withHashAtEnd;
        }

        private static long CountMultipliedArrangementsInternal(string configuration, int[] segments, int segmentsBoundByHash, int remainingSeparators, Dictionary<string, long> segmentMemoization)
        {
            if (remainingSeparators == 0)
            {
                return SolveMemoized(configuration, segmentMemoization, segmentsBoundByHash, segments);
            }

            // first try with dot
            long solutionWithDot = SolveMemoized(configuration, segmentMemoization, segmentsBoundByHash, segments);
            solutionWithDot *= CountMultipliedArrangementsInternal(configuration, segments, segmentsBoundByHash:0, remainingSeparators - 1, segmentMemoization);

            long solutionWithHash = CountMultipliedArrangementsInternal(configuration, segments, segmentsBoundByHash:segmentsBoundByHash+1, remainingSeparators-1, segmentMemoization);
            return solutionWithDot + solutionWithHash;
        }

        private static long SolveMemoized(string configuration, Dictionary<string, long> segmentMemoization, int segmentsBoundByHash, int[] segments) 
        {
            long solution;
            var toSolve = ConstructSegmentsBoundByHash(configuration, segmentsBoundByHash, segments);
            if(!segmentMemoization.TryGetValue(toSolve.configuration, out solution))
            {
                solution = CountArrangements(toSolve.configuration, toSolve.segments);
                segmentMemoization[toSolve.configuration] = solution;
            }
            return solution;
        }

        private static (string configuration, int[] segments) ConstructSegmentsBoundByHash(string configuration, int segmentsBoundByHash, int[] segments)
        {
            if(segmentsBoundByHash == 0)
            {
                return (configuration, segments);
            }
            else
            {
                var configurationJoined = string.Join("#", Enumerable.Repeat(configuration, segmentsBoundByHash + 1));
                var segmentsJoined = Enumerable.Repeat(segments, segmentsBoundByHash + 1).SelectMany(x => x).ToArray();
                return (configurationJoined, segmentsJoined);
            }
        }

        private static long CountArrangements(string configuration, int[] segments) =>
             CountArrangementsInternal(configuration, segments, positionInString: 0, positionInSegments: 0, previousWasDot:false, dbgConfig: new char[configuration.Length]);

        private static long CountArrangementsInternal(string configuration, int[] segments, int positionInString, int positionInSegments, bool previousWasDot, char[] dbgConfig)
        {
            if (positionInString == configuration.Length)
            {
                return IsValidEndConfiguration(segments, positionInSegments) ? 1 : 0;
            }

            if (configuration[positionInString] == '#')
            {
                return HandleHash(configuration, segments, positionInString, positionInSegments, previousWasDot, dbgConfig);
            }
            else if (configuration[positionInString] == '.')
            {
                return HandleDot(configuration, segments, positionInString, positionInSegments, previousWasDot, dbgConfig);
            }
            else
            {
                return HandleHash(configuration, segments, positionInString, positionInSegments, previousWasDot, dbgConfig) + 
                       HandleDot(configuration, segments, positionInString, positionInSegments, previousWasDot, dbgConfig);
            }
        }

        private static bool IsValidEndConfiguration(int[] segments, int positionInSegments)
        {
            return positionInSegments >= segments.Length || 
                   (positionInSegments == segments.Length-1 && segments[positionInSegments] == 0);
        }

        private static long HandleHash(string configuration, int[] segments, int positionInString, int positionInSegments, bool previousWasDot, char[] dbgConfig)
        {
            long result = 0;
            if (positionInSegments < segments.Length && segments[positionInSegments] > 0)
            {
                dbgConfig[positionInString] = '#';
                segments[positionInSegments]--;
                result = CountArrangementsInternal(configuration, segments, positionInString + 1, positionInSegments, previousWasDot: false, dbgConfig);
                segments[positionInSegments]++;
            }
            return result;
        }

        private static long HandleDot(string configuration, int[] segments, int positionInString, int positionInSegments, bool previousWasDot, char[] dbgConfig)
        {
            long result = 0;
            if (previousWasDot || positionInString == 0)
            {
                dbgConfig[positionInString] = '.';
                result = CountArrangementsInternal(configuration, segments, positionInString + 1, positionInSegments, previousWasDot: true, dbgConfig);
            }
            else if (positionInSegments >= segments.Length || segments[positionInSegments] == 0)
            {
                dbgConfig[positionInString] = '.';
                result = CountArrangementsInternal(configuration, segments, positionInString + 1, positionInSegments + 1, previousWasDot: true, dbgConfig);
            }
            return result;
        }

        static void Main(string[] args)
        {
            PartOne();
        }
    }
}
