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

        static void PartTwo()
        {
            var graphLines = Io.AllInputLines().TakeWhile(line => !string.IsNullOrEmpty(line));
            var graph = WorkflowGraph.OfDescription(graphLines);
            var conditionsToA = graph.AllQuadruplesToAcceptingState();
            var nrOfAcceptingQuadruples = CalculateNrOfAcceptingQuadruples(conditionsToA);
            Console.WriteLine(nrOfAcceptingQuadruples);
        }

        private static long CalculateNrOfAcceptingQuadruples(RangeQuadruple[] conditionsToA)
        {
            List<RangeQuadruple> processedConditions = new();
            Stack<RangeQuadruple> conditionsToProcess = new(conditionsToA);
            while(conditionsToProcess.TryPop(out var condition))
            {
                var conditionsToAdd = FindConditionsToAdd(processedConditions, condition);
                if (conditionsToAdd.Length == 0)
                {
                    foreach(var cond in conditionsToAdd)
                    {
                        conditionsToProcess.Push(cond);
                    }
                }
            }
            return processedConditions.Sum(quadruple => quadruple.TotalCombinations);
        }

        private static RangeQuadruple[] FindConditionsToAdd(List<RangeQuadruple> processedConditions, RangeQuadruple condition)
        {
            bool IsFullyContained(Range? outerCandidate, Range? innerCandidate) =>
                (outerCandidate == null) ||
                (outerCandidate != null && innerCandidate != null && 
                  innerCandidate.start >= outerCandidate.start && innerCandidate.end <= outerCandidate.end);

            var conditionsToAdd = new List<RangeQuadruple>();

            foreach(var processedCondition in processedConditions)
            {
                var processedArray = processedCondition.ToArray();
                var currentArray = condition.ToArray();
                for(int i=0; i<processedArray.Length; i++)
                {
                    if (!IsFullyContained(processedArray[i], currentArray[i]))
                    {
                        var split = (processedArray[i] ?? Range.MaxRange).Split(currentArray[i] ?? Range.MaxRange);
                        if(split.Length != 0)
                        {
                            foreach(var splitEl in split)
                            {
                                currentArray[i] = splitEl;
                                conditionsToAdd.Add(RangeQuadruple.OfArray(split));
                            }
                            break;
                        }
                    }
                }
            }
            return conditionsToAdd.ToArray();
        }

        static void Main(string[] args)
        {
            PartOne();
        }
    }
}
