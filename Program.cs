using System.Diagnostics;
using System.Runtime.InteropServices;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test2", 4);
        Run("test", 3);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d25.txt");
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

    private static long GetOutput(ReadOnlySpan<string> input)
    {
        long sum = 0;

        List<uint> keys = new List<uint>();
        List<uint> holes = new List<uint>();

        foreach (var range in input.Split(""))
        {
            var keyOrHole = input[range];

            uint bitmap = 0;
            foreach (var line in keyOrHole)
            {
                foreach (var ch in line)
                {
                    if (ch is '#')
                    {
                        bitmap |= 1;
                    }

                    bitmap <<= 1;
                }
            }

            bitmap >>= 1;

            if ((bitmap & 1) == 1)
            {
                keys.Add(bitmap);
            }
            else
            {
                holes.Add(bitmap);
            }
        }

        foreach (var key in CollectionsMarshal.AsSpan(keys))
        {
            foreach (var hole in CollectionsMarshal.AsSpan(holes))
            {
                if ((key & hole) == 0)
                {
                    sum++;
                }
            }
        }


        return sum;
    }
}