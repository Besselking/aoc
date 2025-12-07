using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 21);
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

        for (var row = 0; row < grid.Rows - 1; row++)
        {
            for (var col = 0; col < grid.Cols; col++)
            {
                var cell = grid[row, col];
                if (cell == '.') continue;
                if (cell == 'S')
                {
                    grid[row + 1, col] = '|';
                }
                else if (cell == '^' && grid[row - 1, col] == '|')
                {
                    sum++;
                    grid[row, col - 1] = '|';
                    grid[row, col + 1] = '|';
                    grid[row + 1, col - 1] = '|';
                    grid[row + 1, col + 1] = '|';
                }
                else if (cell == '|' && grid[row + 1, col] == '.')
                {
                    grid[row + 1, col] = '|';
                }
            }

            Console.WriteLine(grid.ToString());
        }

        return sum;
    }
}