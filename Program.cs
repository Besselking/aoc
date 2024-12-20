using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 0);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d20.txt");
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

        Grid grid = new Grid(input);

        int threshhold = 100;

        var nPathtiles = grid.Count('.');
        var pathTiles = new (int row, int col)[nPathtiles + 2];
        var startTile = grid.IndexOf('S');
        var endTile = grid.IndexOf('E');

        pathTiles[0] = startTile;
        pathTiles[^1] = endTile;

        var prevTile = startTile;
        var currentTile = startTile;
        Span<(int row, int col)> neighbors = stackalloc (int row, int col)[2];
        for (int i = 1; i < pathTiles.Length - 1; i++)
        {
            int nbCount = grid.NeighborsOf(neighbors, currentTile, '.', Grid.NeighborType.Orthogonal);

            foreach (var nb in neighbors[..nbCount])
            {
                if (nb == prevTile)
                {
                    continue;
                }

                pathTiles[i] = nb;
                prevTile = currentTile;
                currentTile = nb;
                break;
            }
        }

        for (int i = 0; i < pathTiles.Length; i++)
        {
            for (int j = pathTiles.Length - 1; j > i; j--)
            {
                var left = pathTiles[i];
                var right = pathTiles[j];

                if (Math.Abs(left.row - right.row)
                    + Math.Abs(left.col - right.col) == 2)
                {
                    var middle = ((left.row + right.row) / 2, (left.col + right.col) / 2);
                    if (grid[middle] is '#')
                    {
                        var diff = Math.Abs(i - j) - 2;

                        if (diff >= threshhold)
                        {
                            sum++;
                        }
                        // Console.WriteLine($"left {i} {left}, right {j} {right}, middle {middle}, diff {diff}");
                    }
                }
            }
        }

        return sum;
    }
}