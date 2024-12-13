using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test3", 80);
        // Run("test2", 436);
        // Run("test", 480);
        Run("test", 875318608908);
        Run("input", 77407675412647);
        // 687194767040 low
        // 294205259639
        // 654982512335
        // 148541582979594
        // 77407675412647
    }

    private static void Run(string type, long? expected = null)
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

            long prizex = prize.Groups["x"].ValueSpan.ParseAsInt();
            long prizey = prize.Groups["y"].ValueSpan.ParseAsInt();

            machines[i / 4] = new Machine(
                new(ax, ay),
                new(bx, by),
                new(
                    prizex
                    + 10000000000000L
                    , prizey
                      + 10000000000000L
                ));
        }

        foreach (var machine in machines)
        {
            long delta = machine.A.X * machine.B.Y - machine.A.Y * machine.B.X;
            long aPresses = (machine.B.Y * machine.Prize.X - machine.B.X * machine.Prize.Y) / delta;
            long bPresses = (machine.A.X * machine.Prize.Y - machine.A.Y * machine.Prize.X) / delta;

            if (aPresses * machine.A.X + bPresses * machine.B.X == machine.Prize.X
                && aPresses * machine.A.Y + bPresses * machine.B.Y == machine.Prize.Y)
            {
                sum += (long)(aPresses * 3 + bPresses);
            }
        }

        return sum;
    }

    private record class Machine(Vector2Long A, Vector2Long B, Vector2Long Prize);

    private record struct Vector2Double(double X, double Y);

    private record struct Vector2Long(long X, long Y);

    [GeneratedRegex(@"Button [AB]: X\+(?<x>\d{2}), Y\+(?<y>\d{2})")]
    public static partial Regex Button();

    [GeneratedRegex(@"Prize: X=(?<x>\d+), Y=(?<y>\d+)")]
    public static partial Regex Prize();
}