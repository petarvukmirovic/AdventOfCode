using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Five
{
    internal record InclusiveRange(long start, long end)
    {
        public InclusiveRange(InclusiveRange other) => new InclusiveRange(other.start, other.end);
        public bool Intersects(InclusiveRange other) => end >= other.start && start <= other.end;
        public long Length => end - start + 1;
    }
}
