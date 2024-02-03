using Common;

namespace Twenty
{
    internal class ModuleGraph
    {
        private abstract class Node
        {
            public required  string Name { get; set; }
            public required List<string> Neighbours { get; set; }
            public abstract bool? ProcessInputSignal(bool signal, string inputNode);
        }

        private class FlipFlopNode : Node
        {
            private bool State { get; set; } = false;

            public override bool? ProcessInputSignal(bool signal, string inputNode)
            {
                if(signal)
                {
                    return null;
                }
                else
                {
                    return State = !State;
                }
            }

            public override string ToString() => $"FF({Name}):{State}";
        }

        private class ConjunctionNode : Node
        {
            private Dictionary<string, bool> inputNodes = new();

            public override bool? ProcessInputSignal(bool signal, string inputNode)
            {
                inputNodes[inputNode] = signal;
                var allHigh = inputNodes.Values.All(signal => signal);
                return !allHigh;
            }

            internal void RegisterInputNode(string inputNode)
            {
                inputNodes[inputNode] = false;
            }
            
            public override string ToString() => $"Conjunction({Name}):{string.Join(',', inputNodes.Select(pair => pair.Key + ":" + pair.Value))}";
        }

        private class BroadcasterNode : Node
        {
            public override bool? ProcessInputSignal(bool signal, string inputNode) => false;
        }

        private ModuleGraph() { }

        private Dictionary<string, Node> Nodes = new();

        public static ModuleGraph OfDescription(IEnumerable<string> inputLines)
        {
            var graph = new ModuleGraph();
            foreach(var line in inputLines)
            {
                var nodeAndNeighbours = line.Split(" -> ", Io.IgnoreEmptyElements);
                var nodeName = nodeAndNeighbours[0];
                var neighbours = nodeAndNeighbours[1].Split(',', Io.IgnoreEmptyElements).ToList();

                Node node = nodeName[0] switch
                {
                    '%' => new FlipFlopNode() { Name = nodeName[1..], Neighbours = neighbours },
                    '&' => new ConjunctionNode() { Name = nodeName[1..], Neighbours = neighbours },
                    _ => new BroadcasterNode() { Name = nodeName, Neighbours = neighbours }
                };

                graph.Nodes[node.Name] = node;
            }

            foreach(var nameAndNode in graph.Nodes)
            {
                foreach(var neighbourName in nameAndNode.Value.Neighbours.Where(graph.Nodes.ContainsKey))
                {
                    var neighbourNode = graph.Nodes[neighbourName] as ConjunctionNode;
                    neighbourNode?.RegisterInputNode(nameAndNode.Key);
                }
            }

            return graph;
        }

        private BroadcasterNode StartNode => (Nodes["broadcaster"] as BroadcasterNode)!;

        public (int numLowSignals, int numHighSignals) GetNumberOfLowAndHighSignals(int buttonPresses)
        {
            (int numLowSignals, int numHighSignals) = (0, 0);
            for (int i = 0; i < buttonPresses; i++)
            {
                var numberOfSignalsInPress = GetNumberOfLowAndHighSignalsForSinglePress();
                numLowSignals += numberOfSignalsInPress.numLowSignals;
                numHighSignals += numberOfSignalsInPress.numHighSignals;
            }
            return (numLowSignals, numHighSignals);
        }

        private (int numLowSignals, int numHighSignals) GetNumberOfLowAndHighSignalsForSinglePress()
        {
            var signalProcessingQueue = new Queue<(bool signal, string previousName, Node node)>();
            (int numLowSignals, int numHighSignals) = (1, 0);
            foreach (var node in StartNode.Neighbours)
            {
                numLowSignals++;
                signalProcessingQueue.Enqueue((false, StartNode.Name, Nodes[node]));
            }
            
            while(signalProcessingQueue.TryDequeue(out var signalInfo))
            {
                var outSignal = signalInfo.node.ProcessInputSignal(signalInfo.signal, signalInfo.previousName);
                if(outSignal.HasValue)
                {
                    foreach (var node in signalInfo.node.Neighbours)
                    {
                        if (outSignal.Value)
                        {
                            numHighSignals++;
                        }
                        else
                        {
                            numLowSignals++;
                        }

                        if(Nodes.TryGetValue(node, out var neighbourNode))
                        {
                            signalProcessingQueue.Enqueue((outSignal.Value, signalInfo.node.Name, neighbourNode));
                        }
                    }
                }
            }

            return (numLowSignals, numHighSignals);
        }
    }
}
