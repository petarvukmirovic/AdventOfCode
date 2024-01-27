using Common;
using System.Text.RegularExpressions;

namespace Eighteen
{
    internal class Program
    {
        private static readonly Regex digPlanRegex = new Regex(@"(\w) (\d+) \(#([a-z0-9]{6})\)");
        private static DigPlan ParseDigPlanElementPartOne(string line)
        {
            var match = digPlanRegex.Match(line);
            var position =
                match.Groups[1].Value.First() switch
                {
                    'R' => Position.Right,
                    'L' => Position.Left,
                    'U' => Position.Up,
                    _ => Position.Down
                };
            return new DigPlan(position, int.Parse(match.Groups[2].Value));
        }

        private static DigPlan ParseDigPlanElementPartTwo(string line)
        {
            var match = digPlanRegex.Match(line);
            var hashCode = match.Groups[3].Value.TrimStart('#');
            var hexLength = hashCode.Substring(0, 5);
            var direction = hashCode.Last();
            return new DigPlan(
                direction switch
                {
                    '0' => Position.Right,
                    '1' => Position.Down,
                    '2' => Position.Left,
                    _ => Position.Up
                },
                Convert.ToInt32(hexLength, 16));
        }

        private record DigPlan(Position direction, int length);

        static void Solve(Func<string, DigPlan> parser)
        {
            var allPlans = Io.AllInputLines().Select(parser).ToArray();
            var capacity = FindDigPlanCapacity(allPlans);
            Console.WriteLine(capacity);
        }

        private static long FindDigPlanCapacity(DigPlan[] allPlans)
        {
            void AddToMap(Dictionary<long, List<(long, long)>> map, long idx, (long, long) pair)
            {
                if (!map.ContainsKey(idx))
                {
                    map[idx] = new List<(long, long)>();
                }
                map[idx].Add(pair);
            }

            var verticalWallsMap = new Dictionary<long, List<(long,long)>>();
            Position currentPosition = new Position(0, 0);

            foreach(var plan in allPlans) 
            {
                Position endPosition = currentPosition + (plan.direction * plan.length);
                if(plan.direction == Position.Down || plan.direction == Position.Up)
                {
                    for(long i=currentPosition.i+plan.direction.i; i != endPosition.i; i += plan.direction.i)
                    {
                        AddToMap(verticalWallsMap, i, (currentPosition.j, 0));
                    }
                }
                else
                {
                    Position startNewPosition = currentPosition + plan.direction;
                    AddToMap(verticalWallsMap, currentPosition.i, (currentPosition.j, plan.direction.j * plan.length));
                }
                currentPosition = endPosition;
            }

            foreach (var indicesList in verticalWallsMap.Values)
            {
                indicesList.Sort((pair1, pair2) => 
                {
                    var diff = pair1.Item1 + pair1.Item2 - (pair2.Item1 + pair2.Item2);
                    return diff switch
                    {
                        0 => 0,
                        > 0 => 1,
                        _ => -1
                    };
                });
            }

            return CalculateCapacityIncludingEdges(verticalWallsMap);
        }

        static (long start, long end) ToStartEnd((long start, long offset) pair) =>
            (Math.Min(pair.start, pair.start + pair.offset),
                Math.Max(pair.start, pair.start + pair.offset));

        private static long CalculateCapacityIncludingEdges(Dictionary<long, List<(long start, long offset)>> rows)
        {

            long capacity = 0;

            List<(long start, long end)> previousSegmentsInPolygon = new();
            List<(long start, long end)> segmentsInsideThePolygon = new();
            foreach(var pair in rows.OrderBy(r => r.Key))
            {
                previousSegmentsInPolygon = segmentsInsideThePolygon;
                segmentsInsideThePolygon = new();
                var row = pair.Value;
                bool stillInsideThePolygon = false;
                for(int i=0; i < row.Count; i++)
                {
                    var leftPoint = row[i];
                    if (leftPoint.offset == 0 && !stillInsideThePolygon)
                    {
                        var rightPoint = row[i + 1];
                        if (rightPoint.offset == 0)
                        {
                            capacity += rightPoint.start - leftPoint.start + 1;
                            segmentsInsideThePolygon.Add((leftPoint.start, rightPoint.start));
                            i++;
                        }
                        else
                        {
                            var rightStart = ToStartEnd(rightPoint).start;
                            capacity += rightStart - leftPoint.start;
                            segmentsInsideThePolygon.Add((leftPoint.start, rightStart));
                        }
                    }
                    else
                    {
                        stillInsideThePolygon = false;
                        var leftAsStartEnd = ToStartEnd(leftPoint);
                        capacity += leftAsStartEnd.end - leftAsStartEnd.start + 1;
                        if(leftPoint.offset != 0 && !PreviousRowInsidePolygon(previousSegmentsInPolygon, leftAsStartEnd.start))
                        {
                            segmentsInsideThePolygon.Add((leftAsStartEnd.start, leftAsStartEnd.end));
                        }
                        (long start, long offset)? rightPoint = i != row.Count - 1 ? row[i + 1] : null;
                        if(rightPoint != null)
                        {
                            var rightAsStartEnd = ToStartEnd(rightPoint.Value);
                            if(PreviousRowInsidePolygon(previousSegmentsInPolygon, leftAsStartEnd.end+1))
                            {
                                // adding all fields in between
                                capacity += rightAsStartEnd.start - leftAsStartEnd.end - 1; 
                                segmentsInsideThePolygon.Add((leftAsStartEnd.end+1, rightAsStartEnd.start));
                                stillInsideThePolygon = true;
                            }
                        }
                    }
                }
            }
            return capacity;
        }

        private static bool PreviousRowInsidePolygon(List<(long start, long end)> previousSegmentsInPolygon, long point) =>
            previousSegmentsInPolygon.Any(pair => point > pair.start && point < pair.end);

        static void Main(string[] args)
        {
            Solve(ParseDigPlanElementPartTwo);
        }
    }
}
