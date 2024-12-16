using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 45);
        Run("test2", 64);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d16.txt");
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

        var start = grid.IndexOf('S');
        var endTile = grid.IndexOf('E');
        Grid.NeighborType startDir = Grid.NeighborType.East;

        PriorityQueue<((int row, int column) pos, Grid.NeighborType direction, List<(int row, int col)> path), int>
            frontier = new();
        frontier.Enqueue((start, startDir, [start]), 0);
        Dictionary<((int row, int column) pos, Grid.NeighborType direction), int> minScores = new();
        long bestScore = long.MaxValue;
        List<List<(int row, int column)>> bestPaths = new();

        while (frontier.Count > 0)
        {
            frontier.TryDequeue(out var node, out var cost);
            if (cost > bestScore)
            {
                continue;
            }

            if (node.pos == endTile)
            {
                if (cost == bestScore)
                {
                    bestPaths.Add(node.path);
                }
                else
                {
                    bestPaths = [node.path];
                    bestScore = cost;
                }

                continue;
            }

            {
                // forward
                var forward = grid.GetNeighborPos(node.pos, node.direction);
                if (grid[forward] is '#')
                {
                    //skip
                }
                else
                {
                    var forwardCost = cost + 1;
                    if (!minScores.TryGetValue((forward, node.direction), out var score)
                        || score >= forwardCost)
                    {
                        minScores[(forward, node.direction)] = forwardCost;
                        frontier.Enqueue((forward, node.direction, [..node.path, forward]), forwardCost);
                    }
                }
            }

            {
                // left turn
                Grid.NeighborType leftDir = node.direction.TurnLeft().TurnLeft();
                var left = grid.GetNeighborPos(node.pos, leftDir);
                var leftCost = cost + 1001;
                if (grid[left] is '#')
                {
                    //skip
                }
                else if (!minScores.TryGetValue((left, leftDir), out var score)
                         || score >= leftCost
                        )
                {
                    minScores[(left, leftDir)] = leftCost;
                    frontier.Enqueue((left, leftDir, [..node.path, left]), leftCost);
                }
            }
            {
                // right turn
                Grid.NeighborType rightDir = node.direction.TurnRight().TurnRight();
                var right = grid.GetNeighborPos(node.pos, rightDir);
                var rightCost = cost + 1001;
                if (grid[right] is '#')
                {
                    //skip
                }
                else if (!minScores.TryGetValue((right, rightDir), out var score)
                         || score >= rightCost)
                {
                    minScores[(right, rightDir)] = rightCost;
                    frontier.Enqueue((right, rightDir, [..node.path, right]), rightCost);
                }
            }
        }

        sum = 0;

        foreach (var path in bestPaths.SelectMany(n => n).Distinct())
        {
            grid[path] = 'O';
            sum++;
        }

        Console.WriteLine(grid.ToString());

        return sum;
    }
}