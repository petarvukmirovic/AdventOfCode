using Common;

namespace Two
{
    internal class Configuration
    {
        internal required int Red { get; set; }
        internal required int Blue { get; set; }
        internal required int Green { get; set; }

        public static Configuration Empty() => new Configuration() { Blue = 0, Green = 0, Red = 0 };

        public long Power => Red * Blue * Green;

        public Configuration Combine(Configuration other) =>
            new Configuration
            {
                Red = Math.Max(Red, other.Red),
                Blue = Math.Max(Blue, other.Blue),
                Green = Math.Max(Green, other.Green)
            };

        public static Configuration FromString(string configStr)
        {
            var newConfiguration = Empty();
            var configElements = configStr.Trim().Split(',');
            foreach(var configElement in configElements)
            {
                var numberAndColor = configElement.Trim().Split(' ', 2);
                if(numberAndColor.Length == 2)
                {
                    var numberOfBalls = int.Parse(numberAndColor[0]);
                    var colorOfBall = numberAndColor[1];
                    switch(colorOfBall)
                    {
                        case "red":
                            newConfiguration.Red = numberOfBalls;
                            break;
                        case "blue":
                            newConfiguration.Blue = numberOfBalls;
                            break;
                        case "green":
                            newConfiguration.Green = numberOfBalls;
                            break;
                    }
                }
                else
                {
                    throw new InvalidDataException("Could not parse input");
                }
            }
            return newConfiguration;
        }
    }

    internal class Program
    {
        private static Configuration TargetConfiguration = new()
        {
            Red = 12,
            Green = 13,
            Blue = 14,
        };

        private static bool ConfigurationIsPossible(Configuration gameConfiguration) =>
            gameConfiguration.Red <= TargetConfiguration.Red &&
            gameConfiguration.Green <= TargetConfiguration.Green &&
            gameConfiguration.Blue <= TargetConfiguration.Blue;

        private static void PartTwo()
        {
            var solution = Io.AllInputLines().Select((line, index) =>
            {
                var separator = line.IndexOf(':');
                var gamePower =
                    line.Substring(separator + 1).Trim()
                        .Split(';')
                        .Aggregate(Configuration.Empty(), (acc, confStr) => acc.Combine(Configuration.FromString(confStr)));
                return gamePower!.Power;
            }).Sum();
            Console.WriteLine(solution);
        }

        private static void PartOne()
        {
            var solution = Io.AllInputLines().Select((line, index) =>
            {
                var separator = line.IndexOf(':');
                var gameIsPossible =
                    line.Substring(separator + 1).Trim()
                        .Split(';')
                        .All(config => ConfigurationIsPossible(Configuration.FromString(config)));
                return gameIsPossible ? index + 1 : 0;
            }).Sum();
            Console.WriteLine(solution);
        }

        static void Main(string[] args)
        {
            PartTwo();   
        }
    }
}
