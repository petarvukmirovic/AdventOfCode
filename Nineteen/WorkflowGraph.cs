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
        private record Node(string name, (RangeQuadruple, string)[] neighbours) { }
        private Dictionary<string, Node> nodes = new();

        private static Range IntersectNullable(Range? a, Range? b) => 
            (a ?? Range.MaxRange).Intersect(b ?? Range.MaxRange) ?? throw new InvalidDataException("Unparseable input file");
        public static WorkflowGraph OfDescription(IEnumerable<string> lines)
        {
            var graph = new WorkflowGraph();
            foreach(string line in lines)
            {
                var indexOfBrace = line.IndexOf('{');
                var name = line[..indexOfBrace];
                var rules = line[(indexOfBrace + 1)..(line.Length - 1)].Split(',', Io.IgnoreEmptyElements);
                var neighbours = new List<(RangeQuadruple, string)>();
                var flippedRules = new List<string>();
                foreach(var rule in rules.Take(rules.Length-1))
                {
                    var ruleSplit = rule.Split(':');
                    var property = ruleSplit[0].First();
                    var flippedRange = ruleSplit[0][1] == '<' ? ">=" + ruleSplit[0][2..] : "<=" + ruleSplit[0][2..];
                    var nodeName = ruleSplit[1];
                    neighbours.Add((RangeQuadruple.OfRuleDescriptions(flippedRules.Append(ruleSplit[0])), nodeName));
                    flippedRules.Add(property + flippedRange);
                }
                neighbours.Add((RangeQuadruple.OfRuleDescriptions(flippedRules), rules.Last()));
                graph.nodes[name] = new Node(name, neighbours.ToArray());
            }
            return graph;
        }

       public RangeQuadruple[] AllQuadruplesToAcceptingState()
       {
            IEnumerable<RangeQuadruple> FindQuadruplesToAcceptingState(string nodeName, RangeQuadruple runningQuadruple)
            {
                if (nodeName=="A")
                {
                    return [runningQuadruple];
                }
                else if (nodeName == "R")
                {
                    return [];
                }


                var node = nodes[nodeName];
                return node.neighbours
                            .SelectMany(neighbour =>
                            {
                                var newQuadruple = runningQuadruple.Intersect(neighbour.Item1);
                                return newQuadruple == null ? 
                                        [] : FindQuadruplesToAcceptingState(neighbour.Item2, newQuadruple);
                            });
            }
            return FindQuadruplesToAcceptingState("in", RangeQuadruple.Empty).ToArray();
       }
    }
}
