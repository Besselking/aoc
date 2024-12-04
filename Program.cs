using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 18);
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
        for (int y = 0; y < height; y++)
        {
            Array.Fill(debugView, ' ', 0, debugView.Length);
            int x = 0;
            var line = input[y].AsSpan();
            do
            {
                var index = line[x..].IndexOf('X');
                if (index == -1) break;
                x += index;
                int count = FindMas(input, x, y);
                if (count > 0)
                {
                    sum += count;
                    debugView[x] = (char)('0' + count);
                }

                x++;
            } while (!line[x..].IsEmpty);

            Console.WriteLine(new string(debugView));
        }

        return sum;
    }

    private static int FindMas(ReadOnlySpan<string> input, int x, int y)
    {
        int count = 0;

        var line = input[y].AsSpan();
        // XMAS
        if (line[x..].StartsWith("XMAS", StringComparison.Ordinal)) count++;
        // SAMX
        if (line[..x].EndsWith("SAM", StringComparison.Ordinal)) count++;

        // X
        // M
        // A
        // S
        if (input[y..].Length > 3
            && input[y + 1][x] == 'M'
            && input[y + 2][x] == 'A'
            && input[y + 3][x] == 'S') count++;

        // S
        // A
        // M
        // X
        if (y >= 3
            && input[y - 1][x] == 'M'
            && input[y - 2][x] == 'A'
            && input[y - 3][x] == 'S') count++;

        // X
        //  M
        //   A
        //    S
        if (y + 3 < input.Length && x + 3 < input[y].Length
                                 && input[y + 1][x + 1] == 'M'
                                 && input[y + 2][x + 2] == 'A'
                                 && input[y + 3][x + 3] == 'S') count++;

        // S
        //  A
        //   M
        //    X
        if (y >= 3 && x >= 3
                   && input[y - 1][x - 1] == 'M'
                   && input[y - 2][x - 2] == 'A'
                   && input[y - 3][x - 3] == 'S') count++;

        //     X
        //    M
        //   A
        //  S
        if (y + 3 < input.Length && x >= 3
                                 && input[y + 1][x - 1] == 'M'
                                 && input[y + 2][x - 2] == 'A'
                                 && input[y + 3][x - 3] == 'S') count++;

        //     S
        //    A
        //   M
        //  X
        if (y >= 3 && x + 3 < input[y].Length
                   && input[y - 1][x + 1] == 'M'
                   && input[y - 2][x + 2] == 'A'
                   && input[y - 3][x + 3] == 'S') count++;

        return count;
    }
}