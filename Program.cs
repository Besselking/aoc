using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test");
        Run("input");
        // 250813 low
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d21.txt");
        var output = GetOutput(input);

        Console.Write($"{type}:\t{output}");

        if (expected.HasValue)
        {
            Console.WriteLine($"\texpected:\t{expected}");
            Debug.Assert(expected == output);
        }

        Console.WriteLine();
    }

    // Implementation

    private static long GetOutput(string[] input)
    {
        long sum = 0;


        foreach (var line in input)
        {
            Console.WriteLine($"{line}: ");

            long length = CountKeypadMoves(line);

            int num = int.Parse(line[..^1]);
            long complexity = length * num;

            Console.WriteLine($"{length} * {num} = {complexity} ");
            sum += complexity;
        }

        return sum;
    }

    private static long CountKeypadMoves(string line)
    {
        long length = 0;

        length = line
            .Prepend('A')
            .Window()
            .DeconstructSelectMany(GetMoves)
            .Prepend('A')
            .Window()
            .Select(tup => GetMoveCount(tup.Item1, tup.Item2, 25))
            .Sum();

        return length;
    }

    private static Dictionary<(char from, char to, int level), long> Cache = new();

    private static long GetMoveCount(char from, char to, int level)
    {
        if (Cache.TryGetValue((from, to, level), out var moveCount)) return moveCount;

        var moves = GetMoves(from, to);
        if (level == 1)
        {
            moveCount = moves.Count();
        }
        else
        {
            moveCount = moves
                .Prepend('A')
                .Window()
                .Select(tup => GetMoveCount(tup.Item1, tup.Item2, level - 1))
                .Sum();
        }

        Cache.Add((from, to, level), moveCount);

        return moveCount;
    }

    private static IEnumerable<char> GetMoves(char from, char to)
    {
        var curPos = from.ToPos();
        var target = to.ToPos();
        var delta = target - curPos;
        var d = 0;

        while (delta != Vec2.Zero)
        {
            const string directions = "<^v>";
            var dirChar = directions[(d++ % directions.Length)];
            var dir = dirChar.ToDir();
            var amount = dir.X == 0 ? delta.Y / dir.Y : delta.X / dir.X;
            if (amount <= 0)
                continue;
            var dest = curPos + (dir * amount);
            if (dest == Hole)
                continue;
            curPos = dest;
            delta -= dir * amount;
            for (int i = 0; i < amount; i++)
            {
                yield return dirChar;
            }
        }

        yield return 'A';
    }

    public static IEnumerable<TResult> DeconstructSelectMany<TSource, TResult>(
        this IEnumerable<(TSource, TSource)> source,
        Func<TSource, TSource, IEnumerable<TResult>> selector)
    {
        return source.SelectMany(tup => selector(tup.Item1, tup.Item2));
    }

    public static IEnumerable<TResult> DeconstructSelect<TSource, TResult>(
        this IEnumerable<(TSource, TSource)> source,
        Func<TSource, TSource, TResult> selector)
    {
        return source.Select(tup => selector(tup.Item1, tup.Item2));
    }

    public static IEnumerable<(T, T)> Window<T>(this IEnumerable<T> input)
    {
        using var enumerator = input.GetEnumerator();
        Debug.Assert(enumerator.MoveNext());
        T last = enumerator.Current;
        while (enumerator.MoveNext())
        {
            T current = enumerator.Current;
            yield return (last, current);
            last = current;
        }
    }

    private static Vec2 Hole = new Vec2(-2, 0);

    private static Vec2 ToDir(this char c)
    {
        return c switch
        {
            '^' => new(0, -1),
            'v' => new(0, 1),
            '<' => new(-1, 0),
            '>' => new(1, 0),
        };
    }

    private static Vec2 ToPos(this char c)
    {
        return c switch
        {
            'A' => Vec2.Zero,
            '0' => new(-1, 0),
            '1' => new(-2, -1),
            '2' => new(-1, -1),
            '3' => new(0, -1),
            '4' => new(-2, -2),
            '5' => new(-1, -2),
            '6' => new(0, -2),
            '7' => new(-2, -3),
            '8' => new(-1, -3),
            '9' => new(0, -3),

            '^' => new(-1, 0),
            'v' => new(-1, 1),
            '<' => new(-2, 1),
            '>' => new(0, 1),
        };
    }
}