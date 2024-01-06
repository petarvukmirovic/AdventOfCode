
using Common;

namespace Eight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Solve(2);
        }

        private static void Solve(int part)
        {
            var allLines = Io.AllInputLines();
            var instructions = NetworkDirectionHelpers.ParseDirections(allLines.First());
            var network = Network.FromStringDescription(allLines.Skip(1));
            if(part == 1)
            {
                Console.WriteLine(network.MinStepsFromAaaToZzz(instructions));
            }
            else
            {
                Console.WriteLine(network.MinStepsFromAToZ(instructions));
            }
            
        }
    }
}
