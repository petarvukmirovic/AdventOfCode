using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen
{
    internal class WorkflowGraph
    {
        private WorkflowGraph() { }
        private record Node(string name, (RangeQuadruple, string)[] neighbours);
        private Dictionary<string, Node> nodes = new();

        public static WorkflowGraph OfDescription(IEnumerable<string> lines)
        {
            var graph = new WorkflowGraph();
            foreach(string line in lines)
            {
                var indexOfBrace = line.IndexOf('{');
                var name = line[..indexOfBrace];
                var rules = line[(indexOfBrace + 1)..(line.Length - 1)].Split(',', Io.IgnoreEmptyElements);
                var neighbours = new List<(RangeQuadruple, string)>();
                RangeQuadruple previousQuadruple = RangeQuadruple.Empty;
                foreach(var rule in rules.Take(rules.Length-1))
                {
                    var ruleSplit = rule.Split(':');
                    var property = ruleSplit[0].First();
                    var newRange = Range.OfDescription(ruleSplit[0][1..]);
                    var conditionQuadruple = property switch
                    {
                        'x' => previousQuadruple.Flip() with { x = newRange },
                        'm' => previousQuadruple.Flip() with { m = newRange },
                        'a' => previousQuadruple.Flip() with { a = newRange },
                        _ => previousQuadruple.Flip() with { s = newRange },
                    };
                    previousQuadruple = conditionQuadruple;
                    neighbours.Add((previousQuadruple, ruleSplit[1]));
                }
                neighbours.Add((previousQuadruple.Flip(), rules.Last()));
                graph.nodes[name] = new Node(name, neighbours.ToArray());
            }
            return graph;
        }

       public RangeQuadruple[] AllQuadruplesToAcceptingState()
       {
            IEnumerable<RangeQuadruple> FindQuadruplesToAcceptingState(string nodeName, RangeQuadruple runningQuadruple, HashSet<string> visited)
            {
                if (nodeName=="A")
                {
                    return [runningQuadruple];
                }
                else if (nodeName == "R")
                {
                    return [];
                }

                if (!visited.Contains(nodeName))
                {
                    visited.Add(nodeName);
                    var node = nodes[nodeName];
                    return node.neighbours
                               .Where(neighbour => !visited.Contains(neighbour.Item2))
                               .SelectMany(neighbour =>
                               {
                                   var newQuadruple = runningQuadruple.Intersect(neighbour.Item1);
                                   return newQuadruple == null ? 
                                            [] : FindQuadruplesToAcceptingState(neighbour.Item2, newQuadruple, visited);
                               });
                }
                return [];
            }
            
            var startNode = "in";
            return FindQuadruplesToAcceptingState(startNode, RangeQuadruple.Empty, new()).ToArray();
       }
    }
}
