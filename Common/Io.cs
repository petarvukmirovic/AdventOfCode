namespace Common
{
    public static class Io
    {
        public static IEnumerable<string> AllInputLines()
        {
            string? line;
            while((line = Console.ReadLine()) != null) 
            { 
                yield return line;
            }
        }

        public const StringSplitOptions IgnoreEmptyElements = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries;
    }
}
