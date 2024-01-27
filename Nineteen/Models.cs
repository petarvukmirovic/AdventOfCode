namespace Nineteen
{
    internal class Models
    {
        public record Part(int x, int m, int a, int s)
        {
            public int Rating => x + m + a + s;

            public bool IsAccepted(Dictionary<string, Pipeline> pipelines)
            {
                var currentPipelineName = "in";
                while (currentPipelineName != "A" && currentPipelineName != "R")
                {
                    var currentPipeline = pipelines[currentPipelineName];
                    currentPipelineName = currentPipeline.FindResultingRule(this);
                }
                return currentPipelineName == "A";
            }
        }

        public record Rule(Func<Part, int> propertySelector, Func<int, int, bool> comparator, int comparisonValue, string resultRule)
        {
            public bool IsSatisfied(Part part) => comparator(propertySelector(part), comparisonValue);

            public static Rule OfStringDescription(string description)
            {
                var propertySelector = GetPropertySelector(description[0]);
                Func<int, int, bool> comparator =
                    description[1] == '<' ? (int a, int b) => a < b : (int a, int b) => a > b;
                var comparisonValueAndResult = description[2..].Split(':');
                return new Rule(propertySelector, comparator, int.Parse(comparisonValueAndResult[0]), comparisonValueAndResult[1]);
            }

            private static Func<Part, int> GetPropertySelector(char property) =>
                property switch
                {
                    'x' => (Part p) => p.x,
                    'm' => (Part p) => p.m,
                    'a' => (Part p) => p.a,
                    _ => (Part p) => p.s
                };
        }

        public record Pipeline(string name, Rule[] rulesInPipline, string defaultRule)
        {
            public string FindResultingRule(Part part) =>
                rulesInPipline.FirstOrDefault(rule => rule.IsSatisfied(part))?.resultRule
                ?? defaultRule;
        }
    }
}
