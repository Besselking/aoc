using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test2", 60);
        // Run("test", 55312);
        Run("input");

        // 6415163624282
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d11.txt");
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

        Histogram<long> stones = new Histogram<long>(input[0].Split(' ').Select(long.Parse)
            .GroupBy(stone => stone)
            .ToDictionary(stone => stone.Key, stone => stone.LongCount()));


        const int targetBlinks = 75;

        for (int i = 0; i < targetBlinks; i++)
        {
            var keyvalues = stones.ToArray();
            foreach (var stoneGroup in keyvalues)
            {
                long stone = stoneGroup.Key;
                long count = stoneGroup.Value;

                switch (stone)
                {
                    case 0:
                    {
                        stones.DecrementCount(0, count);
                        stones.IncrementCount(1, count);
                        break;
                    }
                    case var x when Utils.DigitCount(x) % 2 == 0:
                    {
                        var split = Utils.Split(x);
                        stones.DecrementCount(x, count);

                        stones.IncrementCount(split.left, count);
                        stones.IncrementCount(split.right, count);
                        break;
                    }
                    default:
                    {
                        stones.DecrementCount(stone, count);
                        stones.IncrementCount(stone * 2024, count);
                        break;
                    }
                }
            }
        }

        return stones.Values.Sum();
    }
}