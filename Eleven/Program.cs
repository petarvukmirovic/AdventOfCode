using Common;

namespace Eleven
{
    internal class Program
    {
        record Position(int i, int j);

        static int[] IndicesOfFalse(IEnumerable<bool> bools) => bools.Select((b, i) => (b, i)).Where(pair => !pair.b).Select(pair => pair.i).ToArray();

        static (int[] emptyRows, int[] emptyCols, Position[] galaxyPositions) ParseInput()
        {
            var allInputLines = Io.AllInputLines().ToArray();
            var rowHasGalaxies = new bool[allInputLines.Length];
            var colHasGalaxies = new bool[allInputLines[0].Length];
            var galaxyPositions = new List<Position>();
            for (int rowIdx = 0; rowIdx < allInputLines.Length; rowIdx++)
            {
                for (int colIdx = 0; colIdx < allInputLines[rowIdx].Length; colIdx++)
                {
                    if (allInputLines[rowIdx][colIdx] == '#')
                    {
                        galaxyPositions.Add(new(rowIdx, colIdx));
                        rowHasGalaxies[rowIdx] = true;
                        colHasGalaxies[colIdx] = true;
                    }
                }
            }
            return (IndicesOfFalse(rowHasGalaxies), IndicesOfFalse(colHasGalaxies), galaxyPositions.ToArray());
        }

        static void Solve(int intersctionMultiplier, (int[] emptyRows, int[] emptyCols, Position[] galaxyPositions) parsedInput)
        {
            Console.WriteLine(GetSumOfPathLengths(parsedInput.emptyRows, parsedInput.emptyCols, parsedInput.galaxyPositions, intersctionMultiplier));
        }

        private static long GetSumOfPathLengths(int[] emptyRows, int[] emptyCols, Position[] galaxyPositions, int intersectionMultiplier = 1)
        {
            long allPathLengths = 0;
            for(int i=0; i<galaxyPositions.Length; i++)
            {
                var firstGalaxy = galaxyPositions[i];
                for(int j=i+1; j<galaxyPositions.Length; j++)
                {
                    var secondGalaxy = galaxyPositions[j];
                    allPathLengths += 
                           Math.Abs(secondGalaxy.i - firstGalaxy.i) + Math.Abs(secondGalaxy.j - firstGalaxy.j) 
                           + FindIntersections(firstGalaxy.i, secondGalaxy.i, emptyRows) * intersectionMultiplier
                           + FindIntersections(firstGalaxy.j, secondGalaxy.j, emptyCols) * intersectionMultiplier;
                }
            }
            return allPathLengths;
        }

        static int FindIntersections(int i, int j, int[] indicesOfEmpty)
        {
            int minIdx = Math.Min(i, j);
            int maxIdx = Math.Max(i, j);
            var nrOfIntersections = 
                indicesOfEmpty.SkipWhile(idx => idx <= minIdx)
                              .TakeWhile(idx => idx < maxIdx)
                              .Count();
            return nrOfIntersections;
        }

        static void Main(string[] args)
        {
            var parsedInput = ParseInput();
            Solve(1, parsedInput);
            Solve(9, parsedInput);
            Solve(99, parsedInput);
            Solve(999_999, parsedInput);
        }
    }
}
