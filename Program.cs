using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("testp1", 142);
        Run("test", 11);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        string[] input = File.ReadAllLines($"{type}-d1.txt");
        var output = GetOutput(input);

        Console.Write($"{type}:\t{output}");

        if (expected.HasValue)
        {
            Console.Write($"\texpected:\t{expected}");
            Debug.Assert(expected == output);
        }

        Console.WriteLine();
    }

    // Implementation

    private static long GetOutput(ReadOnlySpan<string> input)
    {
        List<long> left = new List<long>(input.Length);
        List<long> right = new List<long>(input.Length);

        foreach (ReadOnlySpan<char> line in input)
        {
            var splits = line.Split("   ");
            splits.MoveNext();
            long leftNum = long.Parse(line[splits.Current]);
            splits.MoveNext();
            long rightNum = long.Parse(line[splits.Current]);

            left.Add(leftNum);
            right.Add(rightNum);
        }

        left.Sort();
        right.Sort();

        return left.Zip(right, (l, r) => Math.Abs(r - l)).Sum();
    }
}