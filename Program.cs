using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 7036);
        Run("test2", 11048);
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

        var walkableTiles = grid.IndexesOf('.');
        var start = grid.IndexOf('S');
        var endTile = grid.IndexOf('E');
        Grid.NeighborType startDir = Grid.NeighborType.East;

        PriorityQueue<((int row, int column) pos, Grid.NeighborType direction), int> frontier = new();
        frontier.Enqueue((start, startDir), 0);
        HashSet<(int row, int column)> expanded = [];

        while (frontier.Count > 0)
        {
            frontier.TryDequeue(out var node, out var cost);
            if (node.pos == endTile)
            {
                sum = cost;
                break;
            }

            expanded.Add(node.pos);

            // forward
            var forward = grid.GetNeighborPos(node.pos, node.direction);
            if (grid[forward] is '#')
            {
                //skip
            }
            else if (!expanded.Contains(forward)
                     && frontier.UnorderedItems.All(n => n.Element.pos != forward))
            {
                frontier.Enqueue((forward, node.direction), cost + 1);
            }
            else if (frontier.UnorderedItems.FirstOrDefault(n => n.Element.pos == forward)
                         is var forwardInFrontier && forwardInFrontier.Priority > cost + 1)
            {
                frontier.Remove(forwardInFrontier.Element, out _, out _);
                frontier.Enqueue((forward, node.direction), cost + 1);
            }

            // left turn
            Grid.NeighborType leftDir = node.direction.TurnLeft().TurnLeft();
            var left = grid.GetNeighborPos(node.pos, leftDir);
            var leftCost = cost + 1001;
            if (grid[left] is '#')
            {
                //skip
            }
            else if (!expanded.Contains(left)
                     && frontier.UnorderedItems.All(n => n.Element.pos != left))
            {
                frontier.Enqueue((left, leftDir), leftCost);
            }
            else if (frontier.UnorderedItems.FirstOrDefault(n => n.Element.pos == left)
                         is var leftInFrontier && leftInFrontier.Priority > leftCost)
            {
                frontier.Remove(leftInFrontier.Element, out _, out _);
                frontier.Enqueue((left, leftDir), leftCost);
            }

            // right turn
            Grid.NeighborType rightDir = node.direction.TurnRight().TurnRight();
            var right = grid.GetNeighborPos(node.pos, rightDir);
            var rightCost = cost + 1001;
            if (grid[right] is '#')
            {
                //skip
            }
            else if (!expanded.Contains(right)
                     && frontier.UnorderedItems.All(n => n.Element.pos != right))
            {
                frontier.Enqueue((right, rightDir), rightCost);
            }
            else if (frontier.UnorderedItems.FirstOrDefault(n => n.Element.pos == right)
                         is var rightInFrontier && rightInFrontier.Priority > rightCost)
            {
                frontier.Remove(rightInFrontier.Element, out _, out _);
                frontier.Enqueue((right, rightDir), rightCost);
            }
        }

        return sum;
    }
}