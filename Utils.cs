using System.Text.RegularExpressions;

namespace aoc;

public static class Utils
{
    internal static byte ParseByte(string line, int offset = 0)
    {
        return (byte)((line[offset + 0] - '0') * 10 + (line[offset + 1] - '0'));
    }

    internal static int DigitCount(long value)
    {
        return value switch
        {
            < 10L => 1,
            < 100L => 2,
            < 1000L => 3,
            < 10000L => 4,
            < 100000L => 5,
            < 1000000L => 6,
            < 10000000L => 7,
            < 100000000L => 8,
            < 1000000000L => 9,
            < 10000000000L => 10,
            < 100000000000L => 11,
            < 1000000000000L => 12,
            < 10000000000000L => 13,
            < 100000000000000L => 14,
            < 1000000000000000L => 15,
            < 10000000000000000L => 16,
            < 100000000000000000L => 18,
            _ => 19
        };
    }

    internal static (long left, long right) Split(long value)
    {
        return value switch
        {
            < 100L => (value / 10L, value % 10L),
            < 10000L => (value / 100L, value % 100L),
            < 1000000L => (value / 1000L, value % 1000L),
            < 100000000L => (value / 10000L, value % 10000L),
            < 10000000000L => (value / 100000L, value % 100000L),
            < 1000000000000L => (value / 1000000L, value % 1000000L),
            < 100000000000000L => (value / 1000000L, value % 1000000L),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
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

    internal static int ParseAsInt(this ReadOnlySpan<char> span) => int.Parse(span);
    internal static long ParseAsLong(this ReadOnlySpan<char> span) => long.Parse(span);
    internal static uint ParseAsUint(this ReadOnlySpan<char> span) => uint.Parse(span);
    internal static ulong ParseAsUlong(this ReadOnlySpan<char> span) => ulong.Parse(span);

    internal static byte ParseAsByte(this ReadOnlySpan<char> span) => byte.Parse(span);

    internal static byte[] ParseAsBytes(this MemoryExtensions.SpanSplitEnumerator<char> enumerator,
        ReadOnlySpan<char> span)
    {
        List<byte> result = new List<byte>();
        foreach (Range range in enumerator)
        {
            result.Add(span[range].ParseAsByte());
        }

        return result.ToArray();
    }

    internal static long GetLong(this Match match, string group) => match.Groups[group].ValueSpan.ParseAsLong();

    internal static Range MinMaxRange(int a, int b)
    {
        if (a < b)
        {
            return a..b;
        }
        else
        {
            return b..a;
        }
    }

    internal static Grid.NeighborType RightColumn(this Grid.NeighborType type)
    {
        return type switch
        {
            Grid.NeighborType.North => Grid.NeighborType.NorthEast,
            Grid.NeighborType.South => Grid.NeighborType.SouthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    internal static Grid.NeighborType LeftColumn(this Grid.NeighborType type)
    {
        return type switch
        {
            Grid.NeighborType.North => Grid.NeighborType.NorthWest,
            Grid.NeighborType.South => Grid.NeighborType.SouthWest,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    internal static Grid.NeighborType Flip(this Grid.NeighborType type)
    {
        return type switch
        {
            Grid.NeighborType.North => Grid.NeighborType.South,
            Grid.NeighborType.South => Grid.NeighborType.North,
            Grid.NeighborType.East => Grid.NeighborType.West,
            Grid.NeighborType.West => Grid.NeighborType.East,
            Grid.NeighborType.NorthEast => Grid.NeighborType.SouthWest,
            Grid.NeighborType.NorthWest => Grid.NeighborType.SouthEast,
            Grid.NeighborType.SouthEast => Grid.NeighborType.NorthWest,
            Grid.NeighborType.SouthWest => Grid.NeighborType.NorthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    internal static Grid.NeighborType TurnLeft(this Grid.NeighborType type)
    {
        return type switch
        {
            Grid.NeighborType.North => Grid.NeighborType.NorthWest,
            Grid.NeighborType.NorthWest => Grid.NeighborType.West,
            Grid.NeighborType.West => Grid.NeighborType.SouthWest,
            Grid.NeighborType.SouthWest => Grid.NeighborType.South,
            Grid.NeighborType.South => Grid.NeighborType.SouthEast,
            Grid.NeighborType.SouthEast => Grid.NeighborType.East,
            Grid.NeighborType.East => Grid.NeighborType.NorthEast,
            Grid.NeighborType.NorthEast => Grid.NeighborType.North,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    internal static Grid.NeighborType TurnRight(this Grid.NeighborType type)
    {
        return type switch
        {
            Grid.NeighborType.North => Grid.NeighborType.NorthEast,
            Grid.NeighborType.NorthEast => Grid.NeighborType.East,
            Grid.NeighborType.East => Grid.NeighborType.SouthEast,
            Grid.NeighborType.SouthEast => Grid.NeighborType.South,
            Grid.NeighborType.South => Grid.NeighborType.SouthWest,
            Grid.NeighborType.SouthWest => Grid.NeighborType.West,
            Grid.NeighborType.West => Grid.NeighborType.NorthWest,
            Grid.NeighborType.NorthWest => Grid.NeighborType.North,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}