using Common;

namespace Fourteen
{
    internal enum Direction 
    {
        Up = -1,
        Down = 1,
        Left = -1,
        Right = 1,
    }

    internal class Program
    {
        static char[][] GetPlatform() => Io.AllInputLines().Select(line => line.ToCharArray()).ToArray();

        static void PartOne()
        {
            var platform = GetPlatform();
            MoveVertically(platform, Direction.Up);
            Console.WriteLine(CalculateTotalLoad(platform));
        }

        static int CalculateTotalLoad(char[][] platform)
        {
            int[] loadPerRow = GetLoadOnRows(platform);
            var numRows = platform.Length;
            return loadPerRow.Select((load, idx) => (load, idx))
                             .Aggregate(0, (acc, pair) => acc + pair.load * (numRows - pair.idx));
        }

        static void PartTwo()
        {
            var platform = GetPlatform();
            var seenPlatforms = new Dictionary<string, int>();
            var seenPlatformsInverse = new Dictionary<int, string>();
            int cycleNr = 0;
            const int maxCycles = 1000000000;
            char[][] resultPlatform = Array.Empty<char[]>();
            do
            {
                var platformAsString = PlatformAsString(platform);
                if(seenPlatforms.TryGetValue(platformAsString, out var originalCycleNr))
                {

                    int cycleSize = cycleNr - originalCycleNr;
                    int indexInCycle = (maxCycles - originalCycleNr) % cycleSize;
                    resultPlatform = 
                        seenPlatformsInverse[originalCycleNr + indexInCycle]
                        .Split('\n')
                        .Select(line => line.ToCharArray())
                        .ToArray();
                    break;
                }
                seenPlatforms[platformAsString] = cycleNr;
                seenPlatformsInverse[cycleNr] = platformAsString;
                PerformOneCycle(platform);
                cycleNr++;
            } while (cycleNr < maxCycles);

            Console.WriteLine(CalculateTotalLoad(resultPlatform));
        }

        private static void PerformOneCycle(char[][] platform)
        {
            MoveVertically(platform, Direction.Up);
            MoveHorizontally(platform, Direction.Left);
            MoveVertically(platform, Direction.Down);
            MoveHorizontally(platform, Direction.Right);
        }
        private static void MoveVertically(char[][] platform, Direction dir)
        {
            var numCols = platform[0].Length;
            
            for(int colIdx = 0; colIdx < numCols; colIdx++)
            {
                int offset = 0;
                var rowIdx = dir == Direction.Up ? 0 : platform.Length - 1;
                while(rowIdx >= 0 && rowIdx < platform.Length)
                {
                    if (platform[rowIdx][colIdx] == '.')
                    {
                        offset++;
                    }
                    else if (platform[rowIdx][colIdx] == '#')
                    {
                        offset = 0;
                    }
                    else
                    {
                        platform[rowIdx + (int)dir * offset][colIdx] = 'O';
                        if(offset != 0)
                        {
                            platform[rowIdx][colIdx] = '.';
                        }
                    }
                    rowIdx -= (int)dir;
                }
            }
        }

        private static void MoveHorizontally(char[][] platform, Direction dir)
        {
            var numCols = platform[0].Length;
            for(int rowIdx = 0; rowIdx < platform.Length; rowIdx++)
            {
                var colIdx = dir == Direction.Right ? numCols-1 : 0;
                int offset = 0;
                while(colIdx >= 0 && colIdx < numCols)
                {
                    if (platform[rowIdx][colIdx] == '.')
                    {
                        offset++;
                    }
                    else if (platform[rowIdx][colIdx] == '#')
                    {
                        offset = 0;
                    }
                    else
                    {
                        platform[rowIdx][colIdx + (int)dir * offset] = 'O';
                        if(offset != 0)
                        {
                            platform[rowIdx][colIdx] = '.';
                        }
                    }
                    colIdx -= (int)dir;
                }
            }
        }


        private static string PlatformAsString(char[][] platform) => 
            string.Join('\n', platform.Select(row => string.Join("", row)));

        private static int[] GetLoadOnRows(char[][] platform) => platform.Select(row => row.Count(r => r == 'O')).ToArray();

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
