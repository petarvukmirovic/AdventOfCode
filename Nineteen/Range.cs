namespace Nineteen
{
    internal record Range(int start, int end)
    {
        public const int LowerLimit = 1;
        public const int UpperLimit = 4000;
        public Range[] Split(Range other)
        {
            (var first, var second) =
                start > other.start ? (other, this) : (this, other);
            var result = new List<Range>();
            if (second.start <= first.end)
            {
                // part before second starts
                if (second.start != first.start)
                {
                    result.Add(new Range(first.start, second.start - 1));
                }
                // common part
                result.Add(new Range(Math.Max(first.start, second.start), Math.Min(first.end, second.end)));
                // part that belongs to the rest of the longer one
                if (second.end != first.end)
                {
                    result.Add(new Range(Math.Min(first.end, second.end) + 1, Math.Max(first.end, second.end)));
                }
                return result.ToArray();
            }
            else
            {
                return new Range[0];
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

        public Range Flip()
        {
            if(start == LowerLimit)
            {
                return new Range(end + 1, UpperLimit);
            }
            else
            {
                return new Range(0, LowerLimit - 1);
            }
        }
        
        public static Range OfDescription(string description)
        {
            char sign = description.First();
            int argument = int.Parse(description[1..]);
            if (sign == '<') 
            {
                return new Range(LowerLimit, argument - 1);
            }
            else
            {
                return new Range(argument + 1, UpperLimit);
            }
        }
    }

    internal record RangeQuadruple(Range? x, Range? m, Range? a, Range? s)
    {
        private class NotIntersectable : Exception { }
        public static RangeQuadruple Empty => new RangeQuadruple(null, null, null, null);

        //private Range?[] ToArray => new Range?[] { x, m, a, s };
        //private RangeQuadruple OfArray(Range?[] array) => new RangeQuadruple(array[0], array[1], array[2], array[3]);

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

        public RangeQuadruple Flip() =>
            new RangeQuadruple(x?.Flip(), m?.Flip(), a?.Flip(), s?.Flip());
    }
}
