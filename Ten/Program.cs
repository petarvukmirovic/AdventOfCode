using Common;

namespace Ten
{
    internal class Program
    {
        private static char[][] ParsePipeMatrix() => 
            Io.AllInputLines().Select(line => line.Trim().ToCharArray())
              .Where(row => row.Length != 0)
              .ToArray();

        private static void Solve(int part = 1)
        {
            var pipeMatrix = ParsePipeMatrix();
            Position? startPos = 
                pipeMatrix.Select((row, i) =>
                    row.Select((pipe, j) => pipe == 'S' ? new Position(i,j) : null).SingleOrDefault(x => x != null))
                .SingleOrDefault(x => x != null);
            if(startPos != null)
            {
                var initialNodes = Traversal.GetInitialNodes(pipeMatrix, startPos);
                var solution = part == 1 ? Traversal.FindMaxDistance(initialNodes.First(), pipeMatrix) : Traversal.FindNumberOfTilesInsideTheLoop(startPos, initialNodes.First(), pipeMatrix);
                Console.WriteLine(solution);
            }
            else
            {
                Console.WriteLine("Start not found!");
            }
        }


        static void Main(string[] args)
        {
            Solve(part:2);
        }
    }
}
