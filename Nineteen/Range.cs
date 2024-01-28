
namespace Nineteen
{
    internal record Range(int start, int end)
    {
        public const int LowerLimit = 1;
        public const int UpperLimit = 4000;

        public Range[] Split(Range other)
        {
            var intersection = Intersect(other);
            if(intersection != null)
            {
                List<Range> result = new List<Range>() { intersection };
                if(intersection.start > start)
                {
                    result.Add(new Range(start, intersection.start - 1));
                }
                if(intersection.end < end)
                {
                    result.Add(new Range(intersection.end + 1, end));
                }
                return result.ToArray();
            }
            else
            {
                return [];
            }
        }

        public Range? Intersect(Range other)
        {
            if (start <= other.end && end >= other.start)
            {
                return new Range(Math.Max(start, other.start), Math.Min(end, other.end));
            }
            else
            {
                return null;
            }
        }

        public override string ToString() => $"({start}, {end})";

        public static Range OfDescription(string description)
        {
            char sign = description.First();
            int argument;
            int offset = 1;
            if (description[1] == '=')
            {
                offset = 0;
                argument = int.Parse(description[2..]);
            }
            else
            {
                argument = int.Parse(description[1..]);
            }
            if (sign == '<') 
            {
                return new Range(LowerLimit, argument - offset);
            }
            else
            {
                return new Range(argument + offset, UpperLimit);
            }
        }

        public long TotalElements => end - start + 1;

        public static Range MaxRange => new Range(LowerLimit, UpperLimit);
    }

    internal record RangeQuadruple(Range? x, Range? m, Range? a, Range? s)
    {
        private class NotIntersectable : Exception { }
        public static RangeQuadruple Empty => new RangeQuadruple(null, null, null, null);

        public RangeQuadruple? Intersect (RangeQuadruple other)
        {
            Range? IntersectSingle(Range? first, Range? second)
            {
                if(first != null && second != null)
                {
                    var intersection = first.Intersect(second);
                    if(intersection == null)
                    {
                        throw new NotIntersectable();
                    }
                    else
                    {
                        return intersection;
                    }
                }
                else
                {
                    return first ?? second;
                }
            }
            try
            {
                return new RangeQuadruple(IntersectSingle(x, other.x), IntersectSingle(m, other.m),
                                          IntersectSingle(a, other.a), IntersectSingle(s, other.s));
            }
            catch(NotIntersectable)
            {
                return null;
            }
        }

        public long TotalCombinations =>
            (x?.TotalElements ?? 4000) * (m?.TotalElements ?? 4000) * 
            (a?.TotalElements ?? 4000) * (s?.TotalElements ?? 4000);

        public Range?[] ToArray() => [x, m, a, s];
        public static RangeQuadruple OfArray(Range?[] array) => new RangeQuadruple(array[0], array[1], array[2], array[3]);

        public static RangeQuadruple OfRuleDescriptions(IEnumerable<string> descriptions)
        {
            Range[] rangeArray = [Range.MaxRange, Range.MaxRange, Range.MaxRange, Range.MaxRange];
            foreach (var desc in descriptions)
            {
                var propertyIdx = desc.First() switch { 'x' => 0, 'm' => 1, 'a' => 2, _ => 3 };
                rangeArray[propertyIdx] = rangeArray[propertyIdx].Intersect(Range.OfDescription(desc[1..])) ?? throw new InvalidDataException();
            }
            return OfArray(rangeArray);
        }
    }
}
