namespace aoc;

public static class Utils
{
    public static byte ParseByte(string line, int offset = 0)
    {
        return (byte)((line[offset + 0] - '0') * 10 + (line[offset + 1] - '0'));
    }
}