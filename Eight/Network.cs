using System.Text.RegularExpressions;

namespace Eight
{
    internal enum NetworkDirection
    {
        Left,
        Right
    }

    static class NetworkDirectionHelpers
    {
        public static NetworkDirection[] ParseDirections(string line) => 
            line.Select(c => c == 'L' ? NetworkDirection.Left : NetworkDirection.Right).ToArray();
    }

    internal class Network
    {
        private record Neighbors(string left, string right)
        {
            public string ChooseNeighbor(NetworkDirection direction) =>
                direction switch
                {
                    NetworkDirection.Left => left,
                    _ => right
                };

        }

        private Network() { }

        public long MinStepsFromAaaToZzz(NetworkDirection[] instructions)
        {
            long step = 0;
            var currentNode = "AAA";
            var targetNode = "ZZZ";
            
            while(currentNode != targetNode) 
            {
                var neighbors = NeighborsMap[currentNode];
                currentNode = neighbors.ChooseNeighbor(instructions[step % instructions.Length]);
                step++;
            }
            
            return step;
        }

        // this works only because the length of the loop is always equal to the position where
        // ** THE ONLY ** Z node exists in the loop!!!
        // this is an assumption that is simply not in the input and in the case of multiple (or no)
        // Z nodes existing in the loop it will not work.
        public long MinStepsFromAToZ(NetworkDirection[] instructions)
        {
            long LeastCommonMultiple(long a, long b) => a * b / GreatestCommonDivisor(a, b);
            long GreatestCommonDivisor(long a, long b)
            {
                while(b!=0)
                {
                     (a, b) = (b, a % b);
                }
                return a;
            }

            var loopSizes =
                NeighborsMap.Keys.Where(k => k.EndsWith('A'))
                            .Select(startNode =>
                            {
                                var currentNode = startNode;
                                long step = 0;
                                while(!currentNode.EndsWith('Z'))
                                {
                                    var neighbors = NeighborsMap[currentNode];
                                    currentNode = neighbors.ChooseNeighbor(instructions[step % instructions.Length]);
                                    step++;
                                }
                                return step;
                            });
            return loopSizes.Aggregate(1L, LeastCommonMultiple);
        }


        private static Regex NetworkNodeRegex = new Regex(@"(\w{3}) = \((\w{3}), (\w{3})\)");
        public static Network FromStringDescription(IEnumerable<string> strDescirption)
        {
            var network = new Network();
            foreach (var line in strDescirption)
            {
                var match = NetworkNodeRegex.Match(line);
                network.NeighborsMap[match.Groups[1].Value] =
                    new Neighbors(match.Groups[2].Value, match.Groups[3].Value);
            }
            return network;
        }

        private Dictionary<string, Neighbors> NeighborsMap = new();
    }
}
