
using Common;

namespace Three
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PartTwo();
        }

        private static void PartTwo()
        {
            var allLines = Io.AllInputLines().Select(l => l.ToCharArray()).ToArray();
            int nrRows = allLines.Length;
            int nrCols = allLines[0].Length;

            IEnumerable<int> FindNumbers(int row, int col)
            {
                string? FindNumber(int start, int offset)
                {
                    List<char> digits = new List<char>();
                    for(int i=start; i>=0 && i<nrCols && char.IsDigit(allLines![row][i]); i += offset)
                    {
                        digits.Add(allLines![row][i]);
                    }
                    if(digits.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        if(offset < 0)
                        {
                            digits.Reverse();
                        }
                        return new(digits.ToArray());
                    }
                }

                if(row < 0 || row >= nrRows)
                {
                    return Enumerable.Empty<int>();
                }

                var left = FindNumber(col-1, -1);
                var right = FindNumber(col+1, 1);

                if (char.IsDigit(allLines[row][col]))
                {
                    // if there is a letter above, then there is at most one number!
                    left = (left ?? "") + allLines[row][col].ToString() + (right ?? "");
                    right = null;
                }
                return new string?[] { left, right }
                        .Where(x => x != null)
                        .Select(x => int.Parse(x!));
            }

            long ratioSum = 0;
            for (int i = 0; i < nrRows; i++)
            {
                for(int j=0; j < nrCols; j++)
                {
                    if (allLines[i][j] == '*')
                    {
                        var allNumbers = FindNumbers(i-1,j).Concat(FindNumbers(i,j)).Concat(FindNumbers(i+1,j)).ToList();
                        if(allNumbers.Count == 2)
                        {
                            ratioSum += allNumbers.First() * allNumbers.Last();
                        }
                    }
                }
            }
            Console.WriteLine(ratioSum);
        }

        private static void PartOne()
        {
            var allLines = Io.AllInputLines().Select(l => l.ToCharArray()).ToArray();
            int nrRows = allLines.Length;
            int nrCols = allLines[0].Length;

            bool ContainsNumbersAndDots(int rowStart, int rowEnd, int colStart, int colEnd)
            {
                for(int i=rowStart; i<=rowEnd; i++)
                {
                    for(int j=colStart; j<=colEnd; j++)
                    {
                        if (!char.IsDigit(allLines[i][j]) && allLines[i][j] != '.')
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            long partSum = 0;
            for(int i=0; i<allLines.Length; i++)
            {
                var row = allLines[i];
                for(int j=0; j < row.Length; j++) 
                {
                    if (char.IsDigit(row[j]))
                    {
                        var digitStart = j;
                        var number = 0;
                        while (j<row.Length && char.IsDigit(row[j]))
                        {
                            number = number*10 + row[j] - '0';
                            j++;
                        }

                        partSum += ContainsNumbersAndDots(Math.Max(i-1,0),  Math.Min(i+1, nrRows-1),
                                                          Math.Max(digitStart-1, 0), Math.Min(j, nrCols-1)) 
                                   ? 0 : number;
                    }
                }
            }
            Console.WriteLine(partSum);
        }
    }
}
