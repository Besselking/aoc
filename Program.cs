using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 6);
        Run("input");
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

    private static long GetOutput(string[] input)
    {
        long sum = 0;

        string[] towels = input[0].Split(", ");
        string[] patterns = input[2..];

        Regex towelRx = new Regex($"^({String.Join("|", towels)})+$", RegexOptions.Compiled);

        sum = patterns.Count(s => towelRx.IsMatch(s));

        return sum;
    }
}