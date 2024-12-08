using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test2", 9);
        Run("test", 34);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d8.txt");
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

        int rows = input.Length;
        int cols = input[0].Length;
        Grid grid = new(input.SelectMany(line => line).ToArray(), rows, cols);

        var antennas = grid.IndexesOfExcept('.');

        var joins = antennas.Join(antennas,
                antenna => antenna.item,
                antenna => antenna.item,
                (a, b) => (a, b))
            .Where(pair => pair.a.pos != pair.b.pos)
            .ToArray();

        foreach (var join in joins)
        {
            var antinodes = CalcAntinodes(join.a.pos, join.b.pos);

            foreach (var antinodePos in antinodes)
            {
                if (antinodePos is (1, 8)) Debugger.Break();
                if (!grid.InBounds(antinodePos))
                {
                    break;
                }
                // if (grid[antinodePos] is not '.') Debugger.Break();

                grid[antinodePos] = '#';
            }
        }

        Console.WriteLine(grid.ToString());

        return grid.Count('#');
    }

    private static IEnumerable<(int row, int col)> CalcAntinodes((int row, int col) a, (int row, int col) b)
    {
        var distance = (row: (b.row - a.row), col: (b.col - a.col));
        var antinode = b;
        yield return antinode;
        while (true)
        {
            antinode = (row: antinode.row + distance.row, col: antinode.col + distance.col);
            yield return (antinode.row, antinode.col);
        }
    }
}