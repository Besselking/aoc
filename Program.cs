using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 11387);
        Run("input");
        // 6383609732401 too low
        // 7574285277337 not
        // 7574285277337
        // 7579994664753
    }

    private static void Run(string type, int? expected = null)
    {
        var input = File.ReadAllLines($"{type}-d7.txt");
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

        var equations = input
            .Select(line => line.Split(':'))
            .Select(split => (
                ans: long.Parse(split[0]),
                nums: split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()))
            .ToArray();

        Parallel.ForEach(equations, equation =>
        {
            bool isValid = TestValid(equation.ans, 0, equation.nums);
            if (isValid)
            {
                Interlocked.Add(ref sum, equation.ans);
            }
        });

        Console.WriteLine($"total:\t{equations.Sum(eq => eq.ans)}");

        return sum;
    }

    private static bool TestValid(long ans, long acc, ReadOnlySpan<int> nums)
    {
        if (nums.IsEmpty) return ans == acc;
        if (acc > ans) return false;
        return TestValid(ans, acc + nums[0], nums[1..])
               || TestValid(ans, acc * nums[0], nums[1..])
               || TestValid(ans, Concat(acc, nums[0]), nums[1..]);
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