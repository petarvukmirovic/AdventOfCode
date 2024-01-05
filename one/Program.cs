using Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace One
{
    internal static class Helpers
    {
        private static readonly Dictionary<string, string> spelledOutDigits = new()
        {
            { "one", "one1one" }, { "two", "two2two" }, { "three", "three3three" }, { "four", "four4four" }, { "five", "five5five" },
            { "six", "six6six"}, { "seven", "seven7seven" }, { "eight", "eight8eight"}, {"nine", "nine9nine"}
        };
        public static string ReplaceSpelledOutDigits(this string line) =>
            spelledOutDigits.Keys.Aggregate( line, (acc, key) => acc.Replace(key, spelledOutDigits[key]));
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = Io.AllInputLines();
            Console.WriteLine(
                lines.Select(line => CalibrationValue(line.ReplaceSpelledOutDigits()))
                     .Sum());
        }

        private static long CalibrationValue(string line)
        {
            var digits = line.Where(char.IsDigit).Select(c => c - '0');
            var firstDigit = digits.First();
            var lastDigit = digits.Last();
            return firstDigit*10 + lastDigit;
        }

    }
}
