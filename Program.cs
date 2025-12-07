using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 40);
        // Run("test", 2024);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"Inputs/2025/{type}-07.txt");
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

    private static long GetOutput(ReadOnlySpan<string> input)
    {
        long sum = 0;

        Grid grid = new(input.ToArray());

        var (row, col) = grid.IndexOf('S');
        Dictionary<(int row, int col), long> results = new();
        sum += Beam(results, grid, row + 1, col);

        return sum;
    }

    private static long Beam(Dictionary<(int row, int col), long> results, Grid grid, int row, int col)
    {
        while (true)
        {
            if (row == grid.Rows)
            {
                return 1;
            }

            var cell = grid[row, col];
            switch (cell)
            {
                case '.':
                    row += 1;
                    continue;
                case '^':
                    if (results.TryGetValue((row, col), out var result))
                    {
                        return result;
                    }

                    var newResult = Beam(results, grid, row, col + 1) + Beam(results, grid, row, col - 1);
                    results.Add((row, col), newResult);
                    return newResult;
                default:
                    throw new InvalidOperationException($"unkown cell: {cell}");
            }

            break;
        }
    }
}