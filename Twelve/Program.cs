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

        static void PartOne()
        {
            Solve(CountArrangementsDP);
        }

        static void PartTwo()
        {
            Solve((configuration, segments) =>
            {
                var repeatedConfiugration = string.Join("?", Enumerable.Repeat(configuration, 5));
                var repeatedSegments = Enumerable.Repeat(segments, 5).SelectMany(x => x).ToArray();
                return CountArrangementsDP(repeatedConfiugration, repeatedSegments);
            });
        }

        static List<(string configuration, int[] segments)> ParseInput() =>
            Io.AllInputLines().Select(line =>
            {
                var splitLine = line.Split(' ', Io.IgnoreEmptyElements);
                return (splitLine[0], splitLine[1].Split(',', Io.IgnoreEmptyElements).Select(int.Parse).ToArray());
            }).ToList();    

        private static long CountArrangementsDP(string configuration, int[] segments) =>
             CountArrangementsDPInternal(configuration, segments, positionInString: 0, positionInSegments: 0, memoizationTable: new(), canPlaceDot:true, dbgVal: new char[configuration.Length]);

        private static string SegmentsToString(int[] segments) => string.Join(",", segments);
        private static int[] ReduceSegmentsAt(int[] segments, int indexAt)
        {
            var segmentsCopy = (int[])segments.Clone();
            segmentsCopy[indexAt]--;
            return segmentsCopy;
        }
        private static long CountArrangementsDPInternal(string configuration, int[] segments, int positionInString, int positionInSegments, Dictionary<(int, string), long> memoizationTable, bool canPlaceDot, char[] dbgVal)
        {
            long result;
            if(positionInString >= configuration.Length)
            {
                //Console.WriteLine($"{string.Join("", dbgVal)} : {IsValidEndConfiguration(segments, positionInSegments)}" );
                result = IsValidEndConfiguration(segments, positionInSegments) ? 1 : 0;
            }
            else if(!memoizationTable.TryGetValue((positionInString, SegmentsToString(segments)), out result))
            {
                result = 0;
                if ((configuration[positionInString] == '.' || configuration[positionInString] == '?') && canPlaceDot)
                {
                    dbgVal[positionInString] = '.';
                    result += CountArrangementsDPInternal(configuration, segments, positionInString + 1, positionInSegments, memoizationTable, canPlaceDot, dbgVal);
                }
                
                if (configuration[positionInString] == '#' || configuration[positionInString] == '?')
                {
                    if(positionInSegments < segments.Length)
                    {
                        dbgVal[positionInString] = '#';
                        var newSegments = ReduceSegmentsAt(segments, positionInSegments);
                        if (newSegments[positionInSegments] == 0 && NextPositionTerminatesSegment(configuration, positionInString))
                        {
                            if(positionInString < configuration.Length-1)
                            {
                                dbgVal[positionInString + 1] = '.';
                            }
                            result += CountArrangementsDPInternal(configuration, newSegments, positionInString + 2, positionInSegments+1, memoizationTable, canPlaceDot:true, dbgVal);
                        }
                        else if (newSegments[positionInSegments] > 0)
                        {
                            result += CountArrangementsDPInternal(configuration, newSegments, positionInString + 1, positionInSegments, memoizationTable, canPlaceDot:false, dbgVal);
                        }
                    }
                }
            }
            //else
            //{
            //    string stringSoFar = string.Join("", dbgVal.Take(positionInString+1));
            //    Console.WriteLine($"MEMO: {stringSoFar}({configuration.Substring(positionInString)},{SegmentsToString(segments)}): {result} ");
            //}
            memoizationTable[(positionInString, SegmentsToString(segments))] = result;
            return result;
        }

        private static bool IsValidEndConfiguration(int[] segments, int positionInSegments)
        {
            return positionInSegments >= segments.Length ||
                   (positionInSegments == segments.Length - 1 && segments[positionInSegments] == 0);
        }

        private static bool NextPositionTerminatesSegment(string configuration, int positionInString) =>
            positionInString >= configuration.Length - 1 || configuration[positionInString + 1] == '?' ||
            configuration[positionInString + 1] == '.';

        private static long CountArrangementsExponential(string configuration, int[] segments) =>
             CountArrangementsInternal(configuration, segments, positionInString: 0, positionInSegments: 0, previousWasDot: false, dbgConfig: new char[configuration.Length]);

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
            PartTwo();
        }
    }
}
