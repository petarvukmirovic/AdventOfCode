using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen
{
    public class WorkflowGraph
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
                foreach(var rule in rules)
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
                graph.nodes[name] = new Node(name, neighbours.ToArray());
            }
            return graph;
        }
    }
}
