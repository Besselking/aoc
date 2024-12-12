using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test3", 80);
        Run("test2", 436);
        Run("test", 1206);
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
                int corners = CountCorners(grid, pos);

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
                            corners += CountCorners(grid, nb);
                            queue.Enqueue(nb);
                            plotted.Add(nb);
                        }
                    }
                }

                sum += area * corners;
            }
        }

        return sum;
    }

    private static int CountCorners(Grid grid, (int row, int col) pos)
    {
        int count = 0;
        (int row, int col)[] dirs =
        [
            (0, -1),
            (1, 0),
            (0, 1),
            (-1, 0),
        ];

        for (int i = 0; i < dirs.Length; i++)
        {
            var dir1 = dirs[i];
            var dir2 = dirs[(i + 1) % dirs.Length];

            var plant = grid[(pos.row, pos.col)];
            var leftPos = (pos.row + dir1.row, pos.col + dir1.col);
            char? left = grid.InBounds(leftPos) ? grid[leftPos] : null;

            var rightPos = (pos.row + dir2.row, pos.col + dir2.col);
            char? right = grid.InBounds(rightPos) ? grid[rightPos] : null;

            var middlePos = (pos.row + dir1.row + dir2.row, pos.col + dir1.col + dir2.col);
            char? middle = grid.InBounds(middlePos) ? grid[middlePos] : null;

            if ((left != plant && right != plant) || (left == plant && right == plant && middle != plant))
            {
                count++;
            }
        }

        return count;
    }
}