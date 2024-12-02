using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 4);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        string[] input = File.ReadAllLines($"{type}-d2.txt");
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
        return input
            .Select(line => line.Split(' '))
            .Select(report => report.Select(int.Parse).ToArray())
            .Count(IsSafe);

        static bool IsSafe(int[] reports)
        {
            return GetMutations(reports).Any(static reports =>
            {
                int incline = 0;
                int last = reports[0];

                foreach (var report in reports.AsSpan(1))
                {
                    int cmp = last - report;
                    if (incline != 0 && Math.Sign(cmp) != Math.Sign(incline)
                        || Math.Abs(cmp) is > 3 or < 1)
                    {
                        return false;
                    }

                    incline = cmp;
                    last = report;
                }

                return true;
            });
        }
    }

    private static IEnumerable<int[]> GetMutations(int[] reports)
    {
        yield return reports;


        for (int i = 0; i < reports.Length; i++)
        {
            var span = reports.AsSpan();
            var left = span[..i];
            var right = span[(i + 1)..];

            yield return [..left, ..right];
        }
    }
}