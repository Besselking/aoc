using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test2", 60);
        Run("test", 55312);
        Run("input");

        // 6415163624282
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d11.txt");
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

        long[] startStones = input[0].Split(' ').Select(long.Parse).ToArray();

        LinkedList<long> stones = new LinkedList<long>(startStones);

        const int targetBlinks = 25;

        for (int i = 0; i < targetBlinks; i++)
        {
            for (LinkedListNode<long>? node = stones.First; node != null; node = node.Next)
            {
                switch (node.Value)
                {
                    case 0:
                    {
                        node.ValueRef++;
                        break;
                    }
                    case var x when Utils.DigitCount(x) % 2 == 0:
                    {
                        var split = Utils.Split(x);
                        node.ValueRef = split.left;
                        node = stones.AddAfter(node, split.right);
                        break;
                    }
                    default:
                    {
                        node.ValueRef *= 2024;
                        break;
                    }
                }
            }
        }

        sum = stones.Count();

        return sum;
    }
}