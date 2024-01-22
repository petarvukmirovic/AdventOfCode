using Common;

namespace Seventeen
{
    internal class Program
    {


        static void PartOne()
        {
            var map = Io.AllInputLines().Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            var minimalHeatLoss = FindMinimalHeatLoss(map);
            Console.WriteLine(minimalHeatLoss);
        }

        static void PartTwo()
        {
            var map = Io.AllInputLines().Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            var minimalHeatLoss = FindMinimalHeatLossUltra(map);
            Console.WriteLine(minimalHeatLoss);
        }

        private record BfsNode(Position pos, Position prevDir, int remainingSteps, int heat);
        private static long FindMinimalHeatLoss(int[][] map)
        {
            const int maxSteps = 3;
            var bfsQueue = new PriorityQueue<BfsNode, int>();
            var visitedNodes = new HashSet<(Position, Position, int)>();
            bfsQueue.Enqueue(new BfsNode(new Position(0, 0), Position.Up, maxSteps, 0), 0);

            while(bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();
                if(visitedNodes.Contains((node.pos, node.prevDir, node.remainingSteps)))
                {
                    continue;
                }

                visitedNodes.Add((node.pos, node.prevDir, node.remainingSteps));
                if (node.pos.i == map.Length-1 && node.pos.j == map[0].Length-1)
                {
                    return node.heat;
                }

                Position[] allDirections = [Position.Up, Position.Down, Position.Left, Position.Right];
                foreach (var direction in allDirections.Where(dir => node.prevDir != -dir)) 
                {
                    Position newPosition = node.pos + direction;
                    if (newPosition.IsValidForMatrix(map))
                    {
                        var newNode = node with
                        {
                            pos = newPosition,
                            heat = node.heat + map[newPosition.i][newPosition.j],
                            prevDir = direction,
                            remainingSteps = node.prevDir == direction ? node.remainingSteps - 1 : maxSteps
                        };

                        if (newNode.remainingSteps > 0 && !visitedNodes.Contains((newNode.pos, newNode.prevDir, newNode.remainingSteps)))
                        {
                            bfsQueue.Enqueue(newNode, newNode.heat);
                        }
                    }
                }
            }

            return -1;
        }

        // TODO: this can be refactored and only the function CanMakeUltraStep
        // can be made a parameter -- too sleepy for that
        private record BfsNodeUltra(Position pos, Position prevDir, int stepsMadeInDirection, int heat);
        private static long FindMinimalHeatLossUltra(int[][] map)
        {
            var bfsQueue = new PriorityQueue<BfsNodeUltra, int>();
            var visitedNodes = new HashSet<(Position, Position, int)>();
            bfsQueue.Enqueue(new BfsNodeUltra(Position.Right, Position.Right, 1, map[0][1]), map[0][1]);
            bfsQueue.Enqueue(new BfsNodeUltra(Position.Down, Position.Down, 1, map[1][0]), map[0][1]);

            while (bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();
                if (visitedNodes.Contains((node.pos, node.prevDir, node.stepsMadeInDirection)))
                {
                    continue;
                }

                visitedNodes.Add((node.pos, node.prevDir, node.stepsMadeInDirection));
                if (node.pos.i == map.Length - 1 && node.pos.j == map[0].Length - 1 && node.stepsMadeInDirection >= 4)
                {
                    return node.heat;
                }

                Position[] allDirections = [Position.Up, Position.Down, Position.Left, Position.Right];
                foreach (var direction in allDirections.Where(dir => node.prevDir != -dir))
                {
                    Position newPosition = node.pos + direction;
                    if (newPosition.IsValidForMatrix(map))
                    {
                        var newNode = node with
                        {
                            pos = newPosition,
                            heat = node.heat + map[newPosition.i][newPosition.j],
                            prevDir = direction,
                            stepsMadeInDirection = direction == node.prevDir ? node.stepsMadeInDirection+1 : 1,
                        };

                        if (CanMakeUltraStep(node, newNode) && !visitedNodes.Contains((newNode.pos, newNode.prevDir, newNode.stepsMadeInDirection)))
                        {
                            bfsQueue.Enqueue(newNode, newNode.heat);
                        }
                    }
                }
            }

            return -1;
        }

        private static bool CanMakeUltraStep(BfsNodeUltra current, BfsNodeUltra next)
        {
            const int minConsecutiveSteps = 4;
            const int maxConsecutiveSteps = 10;
            if (current.prevDir == next.prevDir)
            {
                return next.stepsMadeInDirection <= maxConsecutiveSteps;
            }
            else
            {
                return current.stepsMadeInDirection >= minConsecutiveSteps;
            }
        }

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
