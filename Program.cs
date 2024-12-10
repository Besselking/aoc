using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test2", 60);
        Run("test", 81);
        Run("input");

        // 6415163624282
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d10.txt");
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

        Grid grid = new(input);

        var trailStarts = grid.IndexesOf('0');

        foreach (var trailStart in trailStarts)
        {
            long score = GetTrailScore(grid, trailStart, trailStart);
            sum += score;
        }

        return sum;
    }

    private static readonly HashSet<((int row, int col) start, (int row, int col) end)> CountedPeaks = [];

    private static long GetTrailScore(Grid grid, (int row, int col) start, (int row, int col) pos)
    {
        char elevation = grid[pos];
        if (elevation is '9')
        {
            return 1;
            // return CountedPeaks.Add((start, pos)) ? 1 : 0;
        }

        var neighbors = grid.NeighborsOf(pos, (char)(elevation + 1), Grid.NeighborType.Orthogonal);

        long score = 0;

        foreach (var neighbor in neighbors)
        {
            score += GetTrailScore(grid, start, neighbor);
        }

        return score;
    }
}