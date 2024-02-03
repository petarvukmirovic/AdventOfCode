using Common;

namespace TwentyOne
{
    internal class Program
    {
        static char[][] ParseInputMatrix() => 
            Io.AllInputLines().Select(line => line.ToCharArray()).ToArray();

        static void PartOne(int numberOfSteps)
        {
            var matrix = ParseInputMatrix();
            var startPosition = FindStartPosition(matrix);
            var result = FindNumberOfPossiblePositionsAfterSteps(startPosition, matrix, numberOfSteps);
            Console.WriteLine(result);
        }

        private static int FindNumberOfPossiblePositionsAfterSteps(Position startPosition, char[][] matrix, int maxSteps)
        {
            HashSet<(Position pos, int steps)> processedPositions = new();
            Queue<(Position position, int steps)> bfsQueue = new();
            bfsQueue.Enqueue((startPosition, 0));
            int foundEndPositions = 0;

            IEnumerable<Position> directions = [Position.Up, Position.Down, Position.Left, Position.Right];
            while(bfsQueue.TryDequeue(out var positionAndSteps))
            {
                if(!processedPositions.Contains(positionAndSteps))
                {
                    (var position, var steps) = positionAndSteps;

                    if(steps == maxSteps)
                    {
                        foundEndPositions++;
                    }               
                    else
                    {
                        var newPositions =
                            directions.Select(d => d + position)
                                      .Where(newPos => newPos.IsValidForMatrix(matrix) &&
                                                       matrix[newPos.i][newPos.j] != '#'  &&
                                                       !processedPositions.Contains((newPos, steps + 1)));
                        foreach(var newPos in newPositions)
                        {
                            bfsQueue.Enqueue((newPos, steps + 1));
                        }
                    }
                    processedPositions.Add(positionAndSteps);
                }
            }

            return foundEndPositions;
        }

        private static Position FindStartPosition(char[][] matrix)
        {
            for(int rowIdx = 0; rowIdx < matrix.Length; rowIdx++)
            {
                for(int colIdx = 0; colIdx < matrix[rowIdx].Length; colIdx++)
                {
                    if (matrix[rowIdx][colIdx] == 'S')
                    {
                        return new(rowIdx, colIdx);
                    }
                }
            }
            throw new InvalidDataException("Input must contain S");
        }

        static void Main(string[] args)
        {
            int numberOfSteps = 64;
            if(args.Any())
            {
                numberOfSteps = int.Parse(args[0]);
            }
            PartOne(numberOfSteps);
        }
    }
}
