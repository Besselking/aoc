using System.Diagnostics;
using System.Text;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test2", 60);
        Run("test", 1928);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d9.txt");
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

        int[] blockSizes = input[0].ToCharArray().Select(c => c - '0').ToArray();

        int[] diskBlocks = new int[blockSizes.Sum()];

        int index = 0;
        int fileIndex = 0;
        bool file = true;
        Span<int> diskBlocksSpan = diskBlocks.AsSpan();
        foreach (int blockSize in blockSizes)
        {
            int fill;
            if (file)
            {
                fill = fileIndex;
                fileIndex++;
                file = false;
            }
            else
            {
                fill = -1;
                file = true;
            }

            diskBlocksSpan.Slice(index, blockSize).Fill(fill);
            index += blockSize;
        }

        var result = FillFileIds(diskBlocks);
        StringBuilder sb = new StringBuilder(diskBlocksSpan.Length * 2);
        int blockPos = 0;
        foreach (var item in result)
        {
            sum += blockPos * item;
            blockPos++;
            sb.Append(item).Append(',');
        }

        Console.WriteLine(sb);

        return sum;
    }

    static IEnumerable<int> FillFileIds(int[] diskBlocks)
    {
        int left = 0;
        int right = diskBlocks.Length;
        while (left < right)
        {
            int leftBlock = diskBlocks[left];
            if (leftBlock is -1)
            {
                Span<int> span = diskBlocks.AsSpan();
                int nextFill = span[..right].LastIndexOfAnyExcept(-1);
                if (nextFill <= left) yield break;
                yield return diskBlocks[nextFill];
                right = nextFill;
            }
            else
            {
                yield return diskBlocks[left];
            }

            left++;
        }
    }
}