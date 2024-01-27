using Common;
using static Nineteen.Models;

namespace Nineteen
{
    internal class Program
    {
        static (Dictionary<string, Pipeline> allPipelines, Part[] allParts) ParseInput()
        {
            var inputLines = Io.AllInputLines();
            Dictionary<string, Pipeline> allPipelines = ParsePipelines(inputLines.TakeWhile(line => !string.IsNullOrEmpty(line)));
            Part[] allParts = ParseParts(inputLines);
            return (allPipelines, allParts);
        }

        static Dictionary<string, Pipeline> ParsePipelines(IEnumerable<string> inputLines)
        {
            var pipelines = new Dictionary<string, Pipeline>();
            foreach (var line in inputLines)
            {
                var indexOfBrace = line.IndexOf('{');
                var name = line[..indexOfBrace];
                var rules = line[(indexOfBrace + 1)..(line.Length - 1)].Split(',', Io.IgnoreEmptyElements);
                var nonDefaultRules = rules.Take(rules.Length - 1);
                var defaultRule = rules.Last();
                pipelines[name] = 
                    new Pipeline(name, nonDefaultRules.Select(Rule.OfStringDescription).ToArray(), defaultRule);
            }
            return pipelines;
        }

        static Part[] ParseParts(IEnumerable<string> inputLines) =>
            inputLines.Select(line =>
            {
                var partValues = line.Trim('{', '}').Split(',').Select(part => int.Parse(part[2..])).ToArray();
                return new Part(partValues[0], partValues[1], partValues[2], partValues[3]);
            }).ToArray();

        static void PartOne()
        {
            var parsedInput = ParseInput();
            var totalRating = parsedInput.allParts.Where(part => part.IsAccepted(parsedInput.allPipelines))
                                                  .Sum(part => part.Rating);
            Console.Write(totalRating);
        }

        static void Main(string[] args)
        {
            PartOne();
        }
    }
}
