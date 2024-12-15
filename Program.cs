using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test3", 80);
        // Run("test2", 436);
        // Run("test", 480);
        // Bounds = new Vector2Long(11, 7);
        Run("test", 10092);
        // Bounds = new Vector2Long(101, 103);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d15.txt");
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

        int gridEnd = Array.IndexOf(input, "");

        string[] gridData = input[..gridEnd];
        Grid grid = new Grid(gridData);

        string moveData = String.Join("", input[(gridEnd + 1)..]);

        var startPos = grid.IndexOf('@');

        var currentPos = startPos;
        foreach (var move in moveData)
        {
            var neighbour = move switch
            {
                '^' => Grid.NeighborType.North,
                '>' => Grid.NeighborType.East,
                'v' => Grid.NeighborType.South,
                '<' => Grid.NeighborType.West,
            };

            var nextPos = grid.GetNeighborPos(currentPos, neighbour);
            var nextTile = grid[nextPos];

            if (nextTile is '#')
            {
                // wall, ignore move
                continue;
            }
            else if (nextTile is '.')
            {
                // free space, move
                grid[currentPos] = '.';
                currentPos = nextPos;
                grid[currentPos] = '@';
            }
            else if (nextTile is 'O')
            {
                // box, attempt push
                // find next free space
                var pushingPos = grid.GetNeighborPos(nextPos, neighbour);
                bool canPush = true;
                while (grid[pushingPos] is not '.')
                {
                    if (grid[pushingPos] is '#')
                    {
                        canPush = false;
                        break;
                    }

                    pushingPos = grid.GetNeighborPos(pushingPos, neighbour);
                }

                if (!canPush) continue;

                grid[pushingPos] = 'O';
                grid[currentPos] = '.';
                currentPos = nextPos;
                grid[currentPos] = '@';
            }
        }

        var boxPositions = grid.IndexesOf('O');

        sum = boxPositions.Sum(pos => pos.row * 100 + pos.col);

        return sum;
    }
}