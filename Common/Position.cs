namespace Common
{

    public record Position(long i, long j)
    {
        public static Position operator +(Position a, Position b) => new Position(a.i + b.i, a.j + b.j);
        public static Position operator -(Position p) => new Position(-p.i, -p.j);
        public static Position operator *(Position p, int c) => new Position(p.i*c, p.j*c);
        public Position Swap() => new Position(j, i);

        public bool IsValidForMatrix<T>(T[][] map) =>
            i >= 0 && i < map.Length &&
            j >= 0 && j < map[0].Length;

        public static readonly Position Up = new(-1, 0);
        public static readonly Position Down = new(1, 0);
        public static readonly Position Left = new(0, -1);
        public static readonly Position Right = new(0, 1);
    }

}
