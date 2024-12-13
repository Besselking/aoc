using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test3", 80);
        // Run("test2", 436);
        Run("test", 480);
        Run("input", 37901);

        // 6415163624282
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d13.txt");
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

        Machine[] machines = new Machine[input.Length / 4 + 1];

        for (int i = 0; i < input.Length; i += 4)
        {
            Match a = Button().Match(input[i]);
            Match b = Button().Match(input[i + 1]);
            Match prize = Prize().Match(input[i + 2]);

            int ax = a.Groups["x"].ValueSpan.ParseAsInt();
            int ay = a.Groups["y"].ValueSpan.ParseAsInt();

            int bx = b.Groups["x"].ValueSpan.ParseAsInt();
            int by = b.Groups["y"].ValueSpan.ParseAsInt();

            int prizex = prize.Groups["x"].ValueSpan.ParseAsInt();
            int prizey = prize.Groups["y"].ValueSpan.ParseAsInt();

            machines[i / 4] = new Machine(
                new(ax, ay),
                new(bx, by),
                new(prizex, prizey));
        }

        foreach (var machine in machines)
        {
            float delta = machine.A.X * machine.B.Y - machine.A.Y * machine.B.X;

            if (delta == 0)
                continue;

            float aPresses = (machine.B.Y * machine.Prize.X - machine.B.X * machine.Prize.Y) / delta;
            float bPresses = (machine.A.X * machine.Prize.Y - machine.A.Y * machine.Prize.X) / delta;

            if (aPresses % 1 > float.Epsilon
                || bPresses % 1 > float.Epsilon) continue;

            sum += (int)(aPresses * 3 + bPresses);
        }

        return sum;
    }

    private record class Machine(Vector2 A, Vector2 B, Vector2 Prize);
    // private record struct Vector2I(int X, int Y);

    [GeneratedRegex(@"Button [AB]: X\+(?<x>\d{2}), Y\+(?<y>\d{2})")]
    public static partial Regex Button();

    [GeneratedRegex(@"Prize: X=(?<x>\d+), Y=(?<y>\d+)")]
    public static partial Regex Prize();
}