using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 143);
        Run("input");
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
            .ToLookup(rule => rule.right, rule => rule.left);

        var updates = updatesSection
            .Select(line => line.Split(',').Select(num => ParseByte(num)).ToArray());

        foreach (var update in updates)
        {
            var span = update.AsSpan();
            if (IsCorrect(span, rules))
            {
                sum += span[span.Length / 2];
            }
        }

        return sum;
    }

    private static bool IsCorrect(Span<byte> span, ILookup<byte, byte> rules)
    {
        for (var index = 0; index < span.Length - 1; index++)
        {
            var pagenum = span[index];
            var deps = rules[pagenum].ToArray();
            if (span[(index + 1)..].ContainsAny(deps))
            {
                return false;
            }
        }

        return true;
    }

    private static (byte left, byte right) ParseRule(string line)
    {
        if (line.Length < 5) return (0, 0);
        byte left = ParseByte(line);
        byte right = ParseByte(line, 3);
        return (left, right);
    }

    private static byte ParseByte(string line, int offset = 0)
    {
        return (byte)((line[offset + 0] - '0') * 10 + (line[offset + 1] - '0'));
    }
}