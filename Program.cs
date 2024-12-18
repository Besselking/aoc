using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Bounds = new(7, 7);
        AfterNBytes = 12;
        Run("test", 21);
        // Run("test2", 64);
        Bounds = new(71, 71);
        AfterNBytes = 1024;
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d18.txt");
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

    private static Vec2 Bounds = new Vec2(0, 0);
    private static int AfterNBytes = 0;

    private static long GetOutput(string[] input)
    {
        long sum = 0;

        int[][] bytes = input.Select(s => s.Split(',').Select(int.Parse).ToArray()).ToArray();

        char[] gridData = new char[(Bounds.X) * (Bounds.Y)];
        Array.Fill(gridData, '.');
        Grid grid = new(gridData, Bounds.X, Bounds.Y);

        int count = AfterNBytes;

        do
        {
            foreach (var b in bytes.AsSpan(0, count))
            {
                grid[b[1], b[0]] = '#';
            }

            sum = GetCost(grid);

            if (sum != 0)
            {
                count++;
            }
            else
            {
                Console.WriteLine(count);
                Console.Write(bytes[count - 1][0]);
                Console.Write(',');
                Console.WriteLine(bytes[count - 1][1]);
                break;
            }
        } while (count < bytes.Length);

        return count;
    }

    private static long GetCost(Grid grid)
    {
        long sum = 0;
        var endPos = (Bounds.X - 1, Bounds.Y - 1);
        PriorityQueue<((int row, int column) pos, Grid.NeighborType direction), int> frontier = new();
        frontier.Enqueue(((0, 0), Grid.NeighborType.East), 0);
        HashSet<(int row, int column)> expanded = [];

        while (frontier.Count > 0)
        {
            frontier.TryDequeue(out var node, out var cost);
            if (node.pos == endPos)
            {
                sum = cost;
                break;
            }

            expanded.Add(node.pos);

            // forward
            var forward = grid.GetNeighborPos(node.pos, node.direction);
            if (!grid.InBounds(forward) || grid[forward] is '#')
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
            var leftCost = cost + 1;
            if (!grid.InBounds(left) || grid[left] is '#')
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
            var rightCost = cost + 1;
            if (!grid.InBounds(right) || grid[right] is '#')
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