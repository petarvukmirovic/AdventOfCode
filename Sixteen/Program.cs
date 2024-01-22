using Common;

namespace Sixteen
{
    internal class Program
    {
        static void PartOne()
        {
            var map = Io.AllInputLines().Select(line => line.ToCharArray()).ToArray();
            var nrOfVisitedFields = GetNrOfVisitedFields(map, start:(new Position(0, 0), Position.Right));
            Console.WriteLine(nrOfVisitedFields);
        }

        static void PartTwo()
        {
            var map = Io.AllInputLines().Select(line => line.ToCharArray()).ToArray();
            var maxNumberOfVisitedFields = 0;
            var numRows = map.Length;
            var numCols = map[0].Length;
            
            for(int colIdx = 0; colIdx < numCols; colIdx++)
            {
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields, 
                                                    GetNrOfVisitedFields(map, start: (new Position(0, colIdx), Position.Down)));
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields,
                                                   GetNrOfVisitedFields(map, start: (new Position(numRows-1, colIdx), Position.Up)));

            }
            for (int rowIdx = 0; rowIdx < numRows; rowIdx++)
            {
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields,
                                                    GetNrOfVisitedFields(map, start: (new Position(rowIdx, 0), Position.Right)));
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields,
                                                   GetNrOfVisitedFields(map, start: (new Position(rowIdx, numCols-1), Position.Left)));

            }

            Console.WriteLine(maxNumberOfVisitedFields);
        }

        private static int GetNrOfVisitedFields(char[][] map, (Position, Position) start)
        {
            HashSet<(Position,Position)> visitedFields = new();
            Queue<(Position position, Position direction)> beamQueue = new();
            beamQueue.Enqueue(start);

            while(beamQueue.TryDequeue(out var pair)) 
            {
                (var position, var direction) = pair;
                if (position.IsValidForMatrix(map) && !visitedFields.Contains(pair))
                {
                    visitedFields.Add(pair);
                    if (map[position.i][position.j] == '.')
                    {
                        beamQueue.Enqueue((position + direction, direction));
                    }
                    else if (map[position.i][position.j] == '-')
                    {
                        if(direction == Position.Up || direction == Position.Down)
                        {
                            beamQueue.Enqueue((position + Position.Left, Position.Left));
                            beamQueue.Enqueue((position + Position.Right, Position.Right));
                        }
                        else
                        {
                            beamQueue.Enqueue((position + direction, direction));
                        }
                    }
                    else if (map[position.i][position.j] == '|')
                    {
                        if (direction == Position.Left || direction == Position.Right)
                        {
                            beamQueue.Enqueue((position + Position.Up, Position.Up));
                            beamQueue.Enqueue((position + Position.Down, Position.Down));
                        }
                        else
                        {
                            beamQueue.Enqueue((position + direction, direction));
                        }
                    }
                    else if (map[position.i][position.j] == '/')
                    {
                        var newDirection = (-direction).Swap();
                        beamQueue.Enqueue((position + newDirection, newDirection));
                    }
                    else // case '\'
                    {
                        var newDirection = direction.Swap();
                        beamQueue.Enqueue((position + newDirection, newDirection));
                    }
                }
            }

            return visitedFields.Select(pair => pair.Item1).Distinct().Count();
        }

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
