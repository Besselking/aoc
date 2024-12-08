namespace aoc;

public static class Utils
{
    public static byte ParseByte(string line, int offset = 0)
    {
        return (byte)((line[offset + 0] - '0') * 10 + (line[offset + 1] - '0'));
    }

    private static long Concat(long a, long b)
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
}