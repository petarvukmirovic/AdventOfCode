using Common;

namespace Sixteen
{
    internal class Program
    {
        internal record Position(int i, int j)
        {
            public static Position operator +(Position a, Position b) => new Position(a.i+b.i, a.j+b.j);
            public static Position operator -(Position p) => new Position(-p.i, -p.j);
            public Position Swap() => new Position(j, i);
        }
        
        static readonly Position Up = new(-1, 0);
        static readonly Position Down = new(1, 0);
        static readonly Position Left = new(0, -1);
        static readonly Position Right = new(0, 1);

        static void PartOne()
        {
            var map = Io.AllInputLines().Select(line => line.ToCharArray()).ToArray();
            var nrOfVisitedFields = GetNrOfVisitedFields(map, start:(new Position(0, 0), Right));
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
                                                    GetNrOfVisitedFields(map, start: (new Position(0, colIdx), Down)));
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields,
                                                   GetNrOfVisitedFields(map, start: (new Position(numRows-1, colIdx), Up)));

            }
            for (int rowIdx = 0; rowIdx < numRows; rowIdx++)
            {
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields,
                                                    GetNrOfVisitedFields(map, start: (new Position(rowIdx, 0), Right)));
                maxNumberOfVisitedFields = Math.Max(maxNumberOfVisitedFields,
                                                   GetNrOfVisitedFields(map, start: (new Position(rowIdx, numCols-1), Left)));

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
                if (PositionIsValid(position, map) && !visitedFields.Contains(pair))
                {
                    visitedFields.Add(pair);
                    if (map[position.i][position.j] == '.')
                    {
                        beamQueue.Enqueue((position + direction, direction));
                    }
                    else if (map[position.i][position.j] == '-')
                    {
                        if(direction == Up || direction == Down)
                        {
                            beamQueue.Enqueue((position + Left, Left));
                            beamQueue.Enqueue((position + Right, Right));
                        }
                        else
                        {
                            beamQueue.Enqueue((position + direction, direction));
                        }
                    }
                    else if (map[position.i][position.j] == '|')
                    {
                        if (direction == Left || direction == Right)
                        {
                            beamQueue.Enqueue((position + Up, Up));
                            beamQueue.Enqueue((position + Down, Down));
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

        private static bool PositionIsValid(Position nextPosition, char[][] map) =>
            nextPosition.i >= 0 && nextPosition.i < map.Length &&
            nextPosition.j >= 0 && nextPosition.j < map[0].Length;

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
