using Common;

namespace Thirteen
{
    internal class Program
    {
        static IEnumerable<char[][]> GetPatterns()
        {
            var allInputLines = Io.AllInputLines();
            var lineIterator = allInputLines.GetEnumerator();
            List<char[]> patternList = new();
            List<char[][]> allPatterns = new();
            while(lineIterator.MoveNext())
            {
                var line = lineIterator.Current;
                if(string.IsNullOrEmpty(line))
                {
                    allPatterns.Add(patternList.ToArray());
                    patternList.Clear();
                }
                else
                {
                    patternList.Add(line.ToCharArray());
                }
            }
            allPatterns.Add(patternList.ToArray());
            return allPatterns;
        }

        static void Solve(Func<char[][], long> summarizer)
        {
            var result =
                GetPatterns()
                .Select(summarizer)
                .Sum();
            Console.WriteLine(result);
        }

        private static int? GetRowSmudgeSummary(char[][] pattern)
        {
            for(int mirrorIdx=0; mirrorIdx < pattern.Length-1; mirrorIdx++)
            {
                var upperPart = mirrorIdx;
                var lowerPart = mirrorIdx+1;
                long foundDifferences = 0;
                while(upperPart >= 0 && lowerPart < pattern.Length && foundDifferences <= 1)
                {
                    foundDifferences += NumberOfDifferences(pattern[upperPart--], pattern[lowerPart++]);
                }
                if(foundDifferences == 1)
                {
                    return mirrorIdx + 1;
                }
            }
            return null;
        }

        private static int? GetColumnSmudgeSummary(char[][] pattern) =>
            GetRowSmudgeSummary(TranslateMatrix(pattern));

        private static long NumberOfDifferences(char[] upperRow, char[] lowerRow) =>
            upperRow.Zip(lowerRow).Count(pair => pair.First != pair.Second);

        private static long GetPatternSmudgeSummary(char[][] pattern)
        {
            var rowSummary = GetRowSmudgeSummary(pattern);
            if(rowSummary != null)
            {
                return 100 * rowSummary.Value;
            }
            else
            {
                return GetColumnSmudgeSummary(pattern)!.Value;
            }
        }

        private static long GetPatternSummary(char[][] pattern)
        {
            var colSummary = GetColumnSummary(pattern);
            if(colSummary != null)
            {
                return colSummary.Value;
            }
            else
            {
                return 100 * GetRowSummary(pattern)!.Value;
            }
        }

        private static int? GetRowSummary(char[][] pattern) => GetColumnSummary(TranslateMatrix(pattern));

        private static char[][] TranslateMatrix(char[][] pattern)
        {
            var numRows = pattern.Length;
            var numCols = pattern[0].Length; 
            var translated = new char[numCols][];
            for(int i=0; i< numCols; i++)
            {
                translated[i] = new char[numRows];
                for(int j=0; j<numRows; j++)
                {
                    translated[i][j] = pattern[j][i];
                }
            }
            return translated;
        }

        private static int? GetColumnSummary(char[][] pattern)
        {
            var numCols = pattern[0].Length;
            var candidates = Enumerable.Repeat(true, numCols-1).ToArray();
            for(int rowIdx =0; rowIdx < pattern.Length; rowIdx++)
            {
                var row = pattern[rowIdx];
                for(int colIdx =0; colIdx < numCols-1; colIdx++)
                {
                    if (candidates[colIdx])
                    {
                        var leftPart = colIdx;
                        var rightPart = colIdx+1;
                        while(leftPart >= 0 && rightPart < numCols && row[leftPart] == row[rightPart])
                        {
                            leftPart--;
                            rightPart++;
                        }
                        candidates[colIdx] = leftPart < 0 || rightPart >= numCols;
                    }
                }
            }
            return IndexOfSingleTrue(candidates);
        }

        private static int? IndexOfSingleTrue(bool[] candidateArray)
        {
            for(int i=0; i<candidateArray.Length; i++)
            {
                if (candidateArray[i])
                {
                    return i+1;
                }
            }
            return null;
        }

        private static void PartOne() => Solve(GetPatternSummary);
        private static void PartTwo() => Solve(GetPatternSmudgeSummary);

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
