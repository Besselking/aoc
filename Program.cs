using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test2", 60);
        Run("test", 2858);
        Run("input");

        // 6415163624282
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
                ToMoveFiles.Push(fileIndex);
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

        Span<int> holeSpan = stackalloc int[9];
        holeSpan.Fill(-1);
        while (ToMoveFiles.TryPop(out int toMoveFile))
        {
            Range fileRange = Utils.RangeOf(diskBlocksSpan, toMoveFile);
            int fileSize = fileRange.GetLength();
            int holeIndex = diskBlocksSpan.IndexOf(holeSpan[..fileSize]);
            if (holeIndex == -1 || holeIndex > fileRange.Start.Value) continue;

            diskBlocksSpan[fileRange].CopyTo(diskBlocksSpan[holeIndex..]);
            diskBlocksSpan[fileRange].Fill(-1);
        }

        // StringBuilder sb = new StringBuilder();
        int blockIndex = 0;
        foreach (var fileId in diskBlocksSpan)
        {
            if (fileId != -1)
            {
                sum += blockIndex * fileId;
                // sb.Append(fileId);
            }
            else
            {
                // sb.Append('.');
            }

            blockIndex++;
        }

        // Console.WriteLine(sb);

        return sum;
    }

    private static readonly Stack<int> ToMoveFiles = [];
}