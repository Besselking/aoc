using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test3", 140);
        Run("test2", 772);
        Run("test", 1930);
        Run("input");

        // 6415163624282
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d12.txt");
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

        HashSet<(int row, int col)> plotted = [];
        Queue<(int row, int col)> queue = [];
        Span<(int row, int col)> nbs = stackalloc (int, int)[4];

        for (int row = 0; row < grid.Rows; row++)
        {
            for (int col = 0; col < grid.Cols; col++)
            {
                (int row, int col) pos = (row, col);
                if (plotted.Contains(pos)) continue;

                queue.Clear();
                queue.Enqueue(pos);
                plotted.Add(pos);

                char plant = grid[row, col];

                int area = 1;
                int perimeter = 4;

                while (queue.Count > 0)
                {
                    var nextPos = queue.Dequeue();
                    int count = grid.NeighborsOf(nbs, nextPos, plant, Grid.NeighborType.Orthogonal);
                    foreach (var nb in nbs[..count])
                    {
                        perimeter--;
                        if (!plotted.Contains(nb))
                        {
                            area++;
                            perimeter += 4;
                            queue.Enqueue(nb);
                            plotted.Add(nb);
                        }
                    }
                }

                sum += area * perimeter;
            }
        }

        return sum;
    }
}