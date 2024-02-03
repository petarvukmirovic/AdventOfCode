using Common;

namespace Twenty
{
    internal class Program
    {
        private static ModuleGraph ParseGraph() => ModuleGraph.OfDescription(Io.AllInputLines());

        private class FoundLowSignalForRxException : Exception;
        
        public static void PartOne()
        {
            var graph = ParseGraph();
            var numberOfSignals = graph.GetNumberOfLowAndHighSignals(1000);
            Console.WriteLine(numberOfSignals.numLowSignals * numberOfSignals.numHighSignals);
        }

        public static void PartTwo() 
        {
            var graph = ParseGraph();
            var steps = 0;
            Dictionary<string, int> previouslySeen = new();
            Action<string, bool, string> stoppingCondition = (prev, signal, node) =>
            {
                // solved by seeing what is the frequency with which these nodes
                // produce high signal, and then did LCM on that
                string[] nodesToCheck = ["rz", "lf", "br", "fk"];

                foreach(var nodeToCheck in nodesToCheck)
                {
                    if(prev == nodeToCheck && signal) 
                    {
                        if(previouslySeen.TryGetValue(nodeToCheck, out var prevSteps))
                        {
                            var diff = steps - prevSteps;
                            Console.WriteLine($"{nodeToCheck}: {diff}");
                        }
                        else
                        {
                            Console.WriteLine($"{nodeToCheck}: {steps} (first)");
                        }
                        previouslySeen[nodeToCheck] = steps;
                    }
                }

                if (!signal && node == "rx")
                {
                    throw new FoundLowSignalForRxException();
                }
            };
            try
            {
                while(true)
                {
                    steps++;
                    graph.GetNumberOfLowAndHighSignalsForSinglePress(stoppingCondition);
                    if(steps % 10000000 == 0)
                    {
                        Console.WriteLine($"Made steps:{steps}");
                    }
                }
            }
            catch(FoundLowSignalForRxException)
            {
                Console.WriteLine(steps);
            }
        }

        static void Main(string[] args) => PartTwo();
    }
}
