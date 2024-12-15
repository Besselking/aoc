using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test2", 618);
        Run("test", 9021);
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

        string[] gridMap = input[..gridEnd];

        string[] gridData = new string[gridEnd];
        for (int i = 0; i < gridMap.Length; i++)
        {
            gridData[i] = String.Create(gridMap[i].Length * 2, gridMap[i], (span, map) =>
            {
                for (int j = 0; j < map.Length; j++)
                {
                    string replacement = map[j] switch
                    {
                        '@' => "@.",
                        '#' => "##",
                        'O' => "[]",
                        '.' => ".."
                    };

                    replacement.CopyTo(span.Slice(j * 2, 2));
                }
            });
        }

        Grid grid = new Grid(gridData);
        // Console.WriteLine(grid.ToString());

        string moveData = String.Join("", input[(gridEnd + 1)..]);

        var startPos = grid.IndexOf('@');

        var currentPos = startPos;
        foreach (var move in moveData)
        {
            // Console.WriteLine(grid.ToString());
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
            else if (nextTile is '[' or ']')
            {
                if (neighbour is Grid.NeighborType.East or Grid.NeighborType.West)
                {
                    var pushingPos = grid.GetNeighborPos(grid.GetNeighborPos(nextPos, neighbour), neighbour);
                    bool canPush = true;
                    while (grid[pushingPos] is not '.')
                    {
                        if (grid[pushingPos] is '#')
                        {
                            canPush = false;
                            break;
                        }

                        pushingPos = grid.GetNeighborPos(grid.GetNeighborPos(pushingPos, neighbour), neighbour);
                    }

                    if (!canPush) continue;

                    int globalPushPos = grid.ToGlobalIndex(pushingPos);
                    int globalCurrentPos = grid.ToGlobalIndex(currentPos);

                    Range range = Utils.MinMaxRange(globalPushPos, globalCurrentPos);
                    var span = grid[range];
                    span.Replace('[', '}');
                    span.Replace(']', '[');
                    span.Replace('}', ']');

                    if (grid[nextPos] is '[')
                    {
                        grid[globalPushPos] = '[';
                    }
                    else
                    {
                        grid[globalPushPos] = ']';
                    }

                    grid[currentPos] = '.';
                    currentPos = nextPos;
                    grid[currentPos] = '@';
                }
                else
                {
                    bool canPush = StartCheckObstacles(grid, nextPos, neighbour);

                    if (!canPush) continue;

                    DoPush(grid, nextPos, neighbour);
                    currentPos = nextPos;
                }
            }
        }

        Console.WriteLine(grid.ToString());

        var boxPositions = grid.IndexesOf('[');

        sum = boxPositions.Sum(pos => pos.row * 100 + pos.col);

        return sum;
    }

    private static void DoPush(Grid grid, (int row, int col) nextPos, Grid.NeighborType neighbour)
    {
        char boxTile = grid[nextPos];

        switch (boxTile)
        {
            case '[':
            {
                // pushing left side
                DoPush(grid, grid.GetNeighborPos(nextPos, neighbour), neighbour);
                DoPush(grid, grid.GetNeighborPos(nextPos, neighbour.Right()), neighbour);
                DoPush(grid, nextPos, neighbour);
                break;
            }
            case ']':
            {
                // pushing right side
                DoPush(grid, grid.GetNeighborPos(nextPos, neighbour), neighbour);
                DoPush(grid, grid.GetNeighborPos(nextPos, neighbour.Left()), neighbour);
                DoPush(grid, nextPos, neighbour);
                break;
            }
            case '.':
            {
                (int row, int col) lastPos = grid.GetNeighborPos(nextPos, neighbour.Flip());
                grid[nextPos] = grid[lastPos];
                grid[lastPos] = '.';
                break;
            }
        }
    }

    private static bool StartCheckObstacles(Grid grid, (int row, int col) nextPos, Grid.NeighborType neighbour)
    {
        char boxTile = grid[nextPos];
        bool canPush = true;

        switch (boxTile)
        {
            case '#':
                return false;
            case '[':
                // pushing left side
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour), neighbour);
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour.Right()), neighbour);
                break;
            case ']':
                // pushing right side
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour), neighbour);
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour.Left()), neighbour);
                break;
        }

        return canPush;
    }

    private static bool CheckObstacles(Grid grid, (int row, int col) nextPos, Grid.NeighborType neighbour)
    {
        char boxTile = grid[nextPos];
        bool canPush = true;

        switch (boxTile)
        {
            case '#':
                return false;
            case '.':
                return true;
            case '[':
                // pushing left side

                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour), neighbour);
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour.Right()), neighbour);
                break;
            case ']':
                // pushing right side
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour), neighbour);
                canPush = canPush && CheckObstacles(grid, grid.GetNeighborPos(nextPos, neighbour.Left()), neighbour);
                break;
        }

        return canPush;
    }
}