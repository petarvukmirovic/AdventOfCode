using Common;

namespace Nine
{
    internal class Program
    {
        private enum ExtrapolateDirection
        {
            Right, Left
        }

        private static long Extrapolate(long[] history, ExtrapolateDirection dir = ExtrapolateDirection.Right)
        {
            var diffList = (long[])history.Clone();
            var sumOfDiffElements = history.Last();
            var allSameInDiff = diffList.All(el => el == history[0]);
            var firstElements = new List<long>() {  history.First() };
            var lastIdx = history.Length-1;
            while(!allSameInDiff)
            {
                allSameInDiff = true;
                for(int i=1; i <= lastIdx; i++)
                {
                    diffList[i-1] = diffList[i] - diffList[i-1];
                    allSameInDiff = allSameInDiff && diffList[i - 1] == diffList[0];
                }
                lastIdx--;
                if(dir == ExtrapolateDirection.Right)
                {
                    sumOfDiffElements += diffList[lastIdx];
                }
                else
                {
                    firstElements.Add(diffList[0]);
                }
            }
            
            if(dir == ExtrapolateDirection.Left)
            {
                firstElements.Reverse();
                sumOfDiffElements = firstElements.Skip(1).Aggregate(firstElements.First(), (acc, el) => el - acc);
            }

            return sumOfDiffElements;
        }

        private static void Solve(ExtrapolateDirection dir = ExtrapolateDirection.Right)
        {
            long[][] AllHistories = 
                Io.AllInputLines()
                  .Select(l => l.Split(' ', Io.IgnoreEmptyElements).Select(long.Parse).ToArray())
                  .ToArray();
            var sumOfExtrapolated = AllHistories.Select(h => Extrapolate(h, dir)).Sum();
            Console.WriteLine(sumOfExtrapolated);
        }

        static void Main(string[] args)
        {
            Solve(ExtrapolateDirection.Left);
        }
    }
}
