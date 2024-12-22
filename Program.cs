using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 37327623);
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

        foreach (var line in input)
        {
            var secretNumber = ulong.Parse(line);
            var startNumber = secretNumber;

            for (int i = 0; i < 2000; i++)
            {
                // mult 64
                var mult64 = secretNumber << 6;
                // mix
                secretNumber = secretNumber ^ mult64;
                // prune
                secretNumber = secretNumber % 16777216;

                // div 32
                var div32 = secretNumber >> 5;
                // mix
                secretNumber = secretNumber ^ div32;
                // prune
                secretNumber = secretNumber % 16777216;

                // mult 2048
                var mult2048 = secretNumber << 11;
                // mix
                secretNumber = secretNumber ^ mult2048;
                // prune
                secretNumber = secretNumber % 16777216;
            }

            Console.WriteLine($"{startNumber}: {secretNumber}");
            sum += secretNumber;
        }

        return sum;
    }
}