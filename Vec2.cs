namespace aoc;

public record struct Vec2(int X, int Y)
{
    public static readonly Vec2 Zero = new(X: 0, Y: 0);
    public static readonly Vec2 One = new(X: 1, Y: 1);
    public static readonly Vec2 Up = new(X: 0, Y: -1);
    public static readonly Vec2 Down = new(X: 0, Y: 1);
    public static readonly Vec2 Left = new(X: -1, Y: 0);
    public static readonly Vec2 Right = new(X: 1, Y: 0);

    public static Vec2 operator +(Vec2 lhs, Vec2 rhs)
    {
        return new Vec2(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static Vec2 operator -(Vec2 lhs, Vec2 rhs)
    {
        return new Vec2(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static Vec2 operator *(int k, Vec2 rhs)
    {
        return new Vec2(k * rhs.X, k * rhs.Y);
    }

    public static Vec2 operator *(Vec2 rhs, int k)
    {
        return new Vec2(k * rhs.X, k * rhs.Y);
    }

    public static Vec2 operator /(Vec2 lhs, int k)
    {
        return new Vec2(lhs.X / k, lhs.Y / k);
    }
}