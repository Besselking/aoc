using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 6);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d6.txt");
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

        var chars = input.SelectMany(s => s).ToArray();
        var copy = new char[chars.Length];

        int tries = 0;
        while (tries < chars.Length)
        {
            visited.Clear();
            chars.CopyTo(copy.AsSpan());
            if (copy[tries] is '#' or '^')
            {
                tries++;
                continue;
            }

            copy[tries++] = 'O';
            var grid = new Grid(copy, input.Length, input[0].Length);

            var pos = grid.IndexOf('^');
            var dir = Dir.Up;
            bool loop = false;
            while (true)
            {
                if (loop)
                {
                    sum++;
                    break;
                }

                // Console.WriteLine(grid.ToString());
                var next = GetNextPos(pos, dir);

                if (!grid.InBounds(next))
                {
                    grid[pos] = GetMarker(dir, grid[pos]);
                    break;
                }

                char nextChar = grid[next];

                if (nextChar is '#' or 'O')
                {
                    grid[pos] = GetMarker(dir, grid[pos]);
                    dir = TurnDir(dir);
                }
                else
                {
                    loop = !visited.Add((pos.row, pos.col, dir));
                    grid[pos] = GetMarker(dir, grid[pos]);
                    pos = next;
                }
            }
        }

        // Console.WriteLine(grid.ToString());

        // return grid.CountAll("+-|");
        return sum;
    }

    private static HashSet<(int row, int col, Dir dir)> visited = new();

    private static char GetMarker(Dir dir, char currentTile)
    {
        if (currentTile != '.')
        {
            return '+';
        }

        return dir switch
        {
            Dir.Up or Dir.Down => '|',
            Dir.Right or Dir.Left => '-',
        };
    }

    private static Dir TurnDir(Dir dir)
    {
        return (Dir)((int)(dir + 1) % 4);
    }

    private enum Dir
    {
        Up,
        Right,
        Down,
        Left
    }

    private static (int row, int col) GetNextPos((int row, int col) pos, Dir dir)
    {
        return dir switch
        {
            Dir.Up => (pos.row - 1, pos.col),
            Dir.Right => (pos.row, pos.col + 1),
            Dir.Down => (pos.row + 1, pos.col),
            Dir.Left => (pos.row, pos.col - 1),
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
}