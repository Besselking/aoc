using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test3", 80);
        // Run("test2", 436);
        // Run("test", 480);
        Bounds = new Vector2Long(11, 7);
        Run("test", 12);
        Bounds = new Vector2Long(101, 103);
        Run("input");
        // 223432704 low
        // 228421332
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d14.txt");
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

        Robot[] robots = new Robot[input.Length];
        for (int i = 0; i < robots.Length; i++)
        {
            Match match = RobotRx().Match(input[i]);

            Debug.Assert(match.Success, input[i]);

            robots[i] = new Robot(
                new(
                    match.GetLong("px"),
                    match.GetLong("py")
                ),
                new(
                    match.GetLong("vx"),
                    match.GetLong("vy")
                )
            );
        }

        const long seconds = 100;

        var chars = new char[Bounds.X * Bounds.Y];
        Array.Fill(chars, '.');
        Grid grid = new Grid(chars, (int)Bounds.Y, (int)Bounds.X);

        for (int i = 0; i < robots.Length; i++)
        {
            var robot = robots[i];
            var newPos = robot.Pos + (robot.Vel * seconds);
            var wrappedPos = newPos.Wrap(Bounds);

            robots[i] = robot with { Pos = wrappedPos };
            if (grid[(int)wrappedPos.Y, (int)wrappedPos.X] != '.')
            {
                grid[(int)wrappedPos.Y, (int)wrappedPos.X]++;
            }
            else
            {
                grid[(int)wrappedPos.Y, (int)wrappedPos.X] = '1';
            }
        }

        Console.WriteLine(grid.ToString());

        var middlepoint = Bounds / 2;

        int topleft = robots.Count(r => r.Pos.Y < middlepoint.Y && r.Pos.X < middlepoint.X);
        int topRight = robots.Count(r => r.Pos.Y < middlepoint.Y && r.Pos.X > middlepoint.X);
        int bottomLeft = robots.Count(r => r.Pos.Y > middlepoint.Y && r.Pos.X < middlepoint.X);
        int bottomRight = robots.Count(r => r.Pos.Y > middlepoint.Y && r.Pos.X > middlepoint.X);

        return topleft * topRight * bottomLeft * bottomRight;
    }

    private static Vector2Long Bounds = new(0, 0);

    public record Robot(Vector2Long Pos, Vector2Long Vel);

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

    [GeneratedRegex(@"p=(?<px>\d+),(?<py>\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)")]
    public static partial Regex RobotRx();
}