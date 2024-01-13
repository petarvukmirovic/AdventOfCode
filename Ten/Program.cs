using Common;

namespace Ten
{
    internal class Program
    {
        private static char[][] ParsePipeMatrix() => 
            Io.AllInputLines().Select(line => line.Trim().ToCharArray())
              .Where(row => row.Length != 0)
              .ToArray();

        private static void PartOne()
        {
            var pipeMatrix = ParsePipeMatrix();
            Position? startPos = 
                pipeMatrix.Select((row, i) =>
                    row.Select((pipe, j) => pipe == 'S' ? new Position(i,j) : null).SingleOrDefault(x => x != null))
                .SingleOrDefault(x => x != null);
            if(startPos != null)
            {
                var initialNodes = Traversal.GetInitialNodes(pipeMatrix, startPos);
                var maxDistance = Traversal.FindMaxDistance(initialNodes.First(), pipeMatrix);
                Console.WriteLine(maxDistance);
            }
            else
            {
                Console.WriteLine("Start not found!");
            }
        }

        static void Main(string[] args)
        {
            PartOne();
        }
    }
}
