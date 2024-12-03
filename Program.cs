using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test", 4);
        Run("input", 84893551);
    }

    // private static void Run(string type, int? expected = null)
    // {
    //     string[] input = File.ReadAllLines($"{type}-d2.txt");
    //     var output = GetOutput(input);
    //
    //     Console.Write($"{type}:\t{output}");
    //
    //     if (expected.HasValue)
    //     {
    //         Console.WriteLine($"\texpected:\t{expected}");
    //         Debug.Assert(expected == output);
    //     }
    //
    //     Console.WriteLine();
    // }

    private static void Run(string type, int? expected = null)
    {
        string input = File.ReadAllText($"{type}-d3.txt");
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

    static long GetOutput(string input)
    {
        Regex mulRegx = MulRegex();

        long sum = 0;

        bool doMul = true;

        foreach (var match in mulRegx.Matches(input).AsEnumerable())
        {
            if (match.ValueSpan.Equals("do()", StringComparison.Ordinal))
            {
                doMul = true;
                continue;
            }
            else if (match.ValueSpan.Equals("don't()", StringComparison.Ordinal))
            {
                doMul = false;
                continue;
            }

            if (!doMul)
            {
                continue;
            }

            var left = int.Parse(match.Groups[1].ValueSpan);
            var right = int.Parse(match.Groups[2].ValueSpan);

            sum += left * right;
        }

        return sum;
    }

    [GeneratedRegex(@"(?:mul\((\d{1,3}),(\d{1,3})\))|do(?:n't)?\(\)")]
    private static partial Regex MulRegex();
}