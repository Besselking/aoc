namespace aoc;

public record Vector2Long(long X, long Y)
{
    public static Vector2Long operator +(Vector2Long a, Vector2Long b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Vector2Long operator *(Vector2Long a, long scalar)
        => new(a.X * scalar, a.Y * scalar);

    public static Vector2Long operator %(Vector2Long a, Vector2Long b)
        => new(a.X % b.X, a.Y % b.Y);

    public static Vector2Long operator /(Vector2Long a, long scalar)
        => new(a.X / scalar, a.Y / scalar);

    public Vector2Long Wrap(Vector2Long bounds)
    {
        long x = X >= 0 ? X % bounds.X : (X % bounds.X + bounds.X) % bounds.X;
        long y = Y >= 0 ? Y % bounds.Y : (Y % bounds.Y + bounds.Y) % bounds.Y;

        return new Vector2Long(x, y);
    }
}