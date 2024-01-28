using Common;
using System.Diagnostics;
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
            Console.WriteLine(conditionsToA.Sum(c => c.TotalCombinations));
            Console.WriteLine($"Without repetitions: {CalculateNrOfAcceptingQuadruples(conditionsToA)}");
        }

        private static long CalculateNrOfAcceptingQuadruples(RangeQuadruple[] conditionsToA)
        {
            List<RangeQuadruple> processedConditions = new();
            Queue<RangeQuadruple> conditionsToProcess = 
                new(
                    conditionsToA.OrderByDescending(q => 
                        (q.x?.TotalElements ?? 4000, q.m?.TotalElements ?? 4000, q.a?.TotalElements ?? 4000, q.s?.TotalElements ?? 4000)));
            while(conditionsToProcess.TryDequeue(out var condition))
            {
                var conditionsToAdd = FindConditionsToAdd(processedConditions, condition);
                if (conditionsToAdd != null && conditionsToAdd.Length != 0)
                {
                    foreach(var cond in conditionsToAdd)
                    {
                        conditionsToProcess.Enqueue(cond);
                    }
                }
                else if (conditionsToAdd != null)
                {
                    processedConditions.Add(condition);
                }
            }
            return processedConditions.Sum(quadruple => quadruple.TotalCombinations);
        }

        static bool IsSubsumed(IEnumerable<RangeQuadruple> processedSoFar, RangeQuadruple newCondition)
        {
            return processedSoFar.Any(processed => IsSubsumed(processed, newCondition));
        }

        static bool IsSubsumed(RangeQuadruple processed, RangeQuadruple newCondition) =>
            IsSubsumed(processed.x, newCondition.x) && IsSubsumed(processed.m, newCondition.m) &&
            IsSubsumed(processed.a, newCondition.a) && IsSubsumed(processed.s, newCondition.s);

        static bool IsSubsumed(Range? outerCandidate, Range? innerCandidate) =>
            (outerCandidate == null) ||
            (outerCandidate != null && innerCandidate != null && 
                innerCandidate.start >= outerCandidate.start && innerCandidate.end <= outerCandidate.end);

        static bool NoIntersection(Range? a, Range? b) => a != null && b != null && a.Intersect(b) == null;

        private static RangeQuadruple[]? FindConditionsToAdd(List<RangeQuadruple> processedConditions, RangeQuadruple condition)
        {
            var conditionsToAdd = new List<RangeQuadruple>();

            foreach(var processedCondition in 
                        processedConditions.Where(p => !NoIntersection(p.x, condition.x) && !NoIntersection(p.m, condition.m) 
                                                       && !NoIntersection(p.a, condition.a) && !NoIntersection(p.s, condition.s)))
            {
                var processedArray = processedCondition.ToArray();
                var currentArray = condition.ToArray();
                bool foundDifference = false;
                for(int i=0; i<processedArray.Length && !foundDifference; i++)
                {
                    if (!IsSubsumed(processedArray[i], currentArray[i]))
                    {
                        foundDifference = true;
                        var split = (currentArray[i] ?? Range.MaxRange).Split(processedArray[i] ?? Range.MaxRange);
                        if (split.Length != 0)
                        {
                            foreach (var splitEl in split)
                            {
                                currentArray[i] = splitEl;
                                var newQuadruple = RangeQuadruple.OfArray(currentArray);
                                if(!IsSubsumed(processedConditions, newQuadruple))
                                {
                                    conditionsToAdd.Add(newQuadruple);
                                }
                            }
                        }
                    }
                }
                if (!foundDifference)
                {
                    return null;
                }
                else if (conditionsToAdd.Any())
                {
                    return conditionsToAdd.ToArray();
                }
            }
            return [];
        }

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
