using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 9);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        string[] input = File.ReadAllLines($"{type}-d4.txt");
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

    static long GetOutput(string[] input)
    {
        long sum = 0;

        char[] debugView = new char[input[0].Length];

        int height = input.Length;
        for (int y = 1; y < height - 1; y++)
        {
            Array.Fill(debugView, '.', 0, debugView.Length);
            int x = 1;
            var line = input[y].AsSpan();
            do
            {
                var index = line[x..].IndexOf('A');
                if (index == -1) break;
                x += index;
                if (x == line.Length - 1) break;
                bool isMas = FindMas(input, x, y);
                if (isMas)
                {
                    sum++;
                    debugView[x] = 'X';
                }

                x++;
            } while (!line[(x + 1)..].IsEmpty);

            Console.WriteLine(new string(debugView));
        }

        return sum;
    }

    private static bool FindMas(ReadOnlySpan<string> input, int x, int y)
    {
        char leftTop = input[y - 1][x - 1];
        if (leftTop is not ('M' or 'S')) return false;
        char rightBottom = input[y + 1][x + 1];
        if (rightBottom is not ('M' or 'S')
            || leftTop == rightBottom) return false;

        char rightTop = input[y - 1][x + 1];
        if (rightTop is not ('M' or 'S')) return false;
        char leftBottom = input[y + 1][x - 1];
        if (leftBottom is not ('M' or 'S')
            || rightTop == leftBottom) return false;

        return true;
    }
}