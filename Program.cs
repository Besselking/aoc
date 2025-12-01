using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 3);
        // Run("test", 2024);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"Inputs/2025/{type}-01.txt");
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

    private static long GetOutput(ReadOnlySpan<string> input)
    {
        long sum = 0;
        int dial = 50;

        foreach (var line in input)
        {
            if (dial == 0) sum++;
            char dir = line[0];
            int rot = line.AsSpan()[1..].ParseAsInt();

            dial += dir switch
            {
                'L' => -rot,
                'R' => rot,
            };

            dial %= 100;
        }

        return sum;
    }
}