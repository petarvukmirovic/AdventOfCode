using Common;

namespace Four
{
    internal class Program
    {
        static int[] ParseNumbers(string linePart) =>
            linePart.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray();

        static int GetNumberOfMatches(string line)
        {
            var separatedLine = line.Substring(line.IndexOf(':') + 1).Split('|');
            var winningNumbers = new HashSet<int>(ParseNumbers(separatedLine[0]));
            return ParseNumbers(separatedLine[1]).Count(winningNumbers.Contains);
        }

        static long GetPointsPartOne(string line)
        {
            var numberOfMatches = GetNumberOfMatches(line);
            return numberOfMatches != 0 ? 1 << (numberOfMatches - 1) : 0; 
        }

        static void PartOne(IEnumerable<string> lines) 
        {
            Console.WriteLine(lines.Select(GetPointsPartOne).Sum());
        }

        static void PartTwo(IEnumerable<string> lines)
        {
            string[] linesArray = lines.ToArray();
            long[] numberOfCopies = Enumerable.Repeat(1L, linesArray.Length).ToArray();
            for(int i=0; i<numberOfCopies.Length; i++)
            {
                UpdateNumberOfCopies(numberOfCopies, linesArray.ElementAt(i), i);
            }

            Console.WriteLine(numberOfCopies.Sum());
        }

        private static void UpdateNumberOfCopies(long[] numberOfCopies, string line, int lineIdx)
        {
            var numberOfMatches = GetNumberOfMatches(line);
            var numberOfLineCopies = numberOfCopies[lineIdx];
            var lastLine = Math.Min(lineIdx + numberOfMatches, numberOfCopies.Length - 1);
            for(int i=lineIdx+1; i<=lastLine; i++)
            {
                numberOfCopies[i] += numberOfLineCopies;
            }
        }

        static void Main(string[] args)
        {
            var allLines = Io.AllInputLines();
            PartTwo(allLines);
        }
    }
}
