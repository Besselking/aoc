using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 123);
        Run("input", 6897);
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d5.txt");
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

        int endOfRules = Array.IndexOf(input, "");

        string[] rulesSection = input[..endOfRules];
        string[] updatesSection = input[(endOfRules + 1)..];

        var rules = rulesSection
            .Select(ParseRule)
            .ToHashSet();

        var updates = updatesSection
            .Select(line => line.Split(',').Select(num => ParseByte(num)).ToArray());

        var comparer = Comparer<byte>.Create((x, y)
            => rules.Contains((x, y)) ? -1 : 1);

        foreach (var update in updates)
        {
            var span = update.AsSpan();
            if (IsFixed(span, comparer))
            {
                sum += span[span.Length / 2];
            }
        }

        return sum;
    }

    private static bool IsFixed(Span<byte> span, Comparer<byte> comparer)
    {
        var orig = span.ToArray();
        // part 1
        // Array.Sort(orig, comparer);
        // return span.SequenceEqual(orig);

        // part 2
        span.Sort(comparer);
        return !span.SequenceEqual(orig);
    }

    private static (byte left, byte right) ParseRule(string line)
    {
        byte left = ParseByte(line);
        byte right = ParseByte(line, 3);
        return (left, right);
    }

    private static byte ParseByte(string line, int offset = 0)
    {
        return (byte)((line[offset + 0] - '0') * 10 + (line[offset + 1] - '0'));
    }
}