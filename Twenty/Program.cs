using Common;

namespace Twenty
{
    internal class Program
    {
        private static ModuleGraph ParseGraph() => ModuleGraph.OfDescription(Io.AllInputLines());
        
        public static void PartOne()
        {
            var graph = ParseGraph();
            var numberOfSignals = graph.GetNumberOfLowAndHighSignals(1000);
            Console.WriteLine(numberOfSignals.numLowSignals * numberOfSignals.numHighSignals);
        }

        static void Main(string[] args) => PartOne();
    }
}
