using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 23);
        Run("input");
    }

    private static void Run(string type, ulong? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d22.txt");
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

    private static ulong GetOutput(string[] input)
    {
        ulong sum = 0;

        Dictionary<ulong, Dictionary<(int, int, int, int), int>> output = new();

        foreach (var line in input)
        {
            var secretNumber = ulong.Parse(line);
            var startNumber = secretNumber;
            var currentDict = output[startNumber] = new();

            var price = (int)(secretNumber % 10);

            (int a, int b, int c, int d) sequence = (0, 0, 0, 0);

            for (int i = 0; i < 2000; i++)
            {
                // mult 64
                var mult64 = secretNumber << 6;
                // mix
                secretNumber ^= mult64;
                // prune
                secretNumber %= 16777216;

                // div 32
                var div32 = secretNumber >> 5;
                // mix
                secretNumber ^= div32;
                // prune
                secretNumber %= 16777216;

                // mult 2048
                var mult2048 = secretNumber << 11;
                // mix
                secretNumber ^= mult2048;
                // prune
                secretNumber %= 16777216;

                int newPrice = (int)(secretNumber % 10);
                var change = newPrice - price;
                price = newPrice;

                sequence = (sequence.b, sequence.c, sequence.d, change);
                if (i > 3)
                {
                    currentDict.TryAdd(sequence, newPrice);
                }
            }

            Console.WriteLine($"{startNumber}: {secretNumber}");
            sum += secretNumber;
        }

        var maxGroup = output
            .SelectMany(g => g.Value)
            .GroupBy(g => g.Key, g => g.Value)
            .MaxBy(g => g.Sum());

        sum = (ulong)maxGroup.Sum();
        Console.WriteLine($"{maxGroup.Key}: {sum}");

        foreach (var group in output)
        {
            group.Value.TryGetValue(maxGroup.Key, out var price);
            Console.WriteLine($"{group.Key}: {price}");
        }

        return sum;
    }
}