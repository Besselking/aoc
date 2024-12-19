using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 16);
        Run("input", 584553405070389);
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d19.txt");
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

    private static string[] towels;
    private static Dictionary<string, long> cache = new Dictionary<string, long>();

    private static Dictionary<string, long>.AlternateLookup<ReadOnlySpan<char>> cacheLookup =
        cache.GetAlternateLookup<ReadOnlySpan<char>>();

    private static long GetOutput(string[] input)
    {
        long sum = 0;

        towels = input[0].Split(", ");
        string[] patterns = input[2..];

        cache.Clear();
        sum = 0;
        foreach (var s in patterns)
        {
            sum += GetMatchCount(s);
        }

        return sum;
    }

    private static long GetMatchCount(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty) return 1;
        if (cacheLookup.TryGetValue(pattern, out long count)) return count;

        foreach (var towel in towels)
        {
            if (pattern.StartsWith(towel))
            {
                count += GetMatchCount(pattern[towel.Length..]);
            }
        }

        return cacheLookup[pattern] = count;
    }
}