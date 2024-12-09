namespace aoc;

public static class Utils
{
    internal static byte ParseByte(string line, int offset = 0)
    {
        return (byte)((line[offset + 0] - '0') * 10 + (line[offset + 1] - '0'));
    }

    internal static long Concat(long a, long b)
    {
        return b switch
        {
            < 10L => 10L * a + b,
            < 100L => 100L * a + b,
            < 1000L => 1000L * a + b,
            < 10000L => 10000L * a + b,
            < 100000L => 100000L * a + b,
            < 1000000L => 1000000L * a + b,
            < 10000000L => 10000000L * a + b,
            < 100000000L => 100000000L * a + b,
            _ => 1000000000L * a + b
        };
    }

    internal static Range RangeOf<T>(this ReadOnlySpan<T> span, T item) where T : IEquatable<T>
    {
        int startIndex = span.IndexOf(item);
        int length = span[startIndex..].IndexOfAnyExcept(item);
        if (length == -1) return startIndex..span.Length;
        return startIndex..(startIndex + length);
    }

    internal static Range LastRangeOf<T>(this ReadOnlySpan<T> span, T item) where T : IEquatable<T>
    {
        int endIndex = span.LastIndexOf(item) + 1;
        int startIndex = span[..endIndex].LastIndexOfAnyExcept(item);
        if (startIndex == -1) return 0..endIndex;
        return (startIndex + 1)..endIndex;
    }

    internal static int GetLength(this Range range)
    {
        return range.End.Value - range.Start.Value;
    }
}