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

        foreach (var equation in equations)
        {
            bool isValid = TestValid(equation.ans, equation.nums);
            if (isValid)
            {
                sum += equation.ans;
            }
        }

        Console.WriteLine($"total:\t{equations.Sum(eq => eq.ans)}");

        return sum;
    }

    private static bool TestValid(long ans, int[] nums)
    {
        return TestAdd(ans, 0, nums)
               || TestMul(ans, 0, nums)
               || TestConcat(ans, 0, nums);
    }

    private static bool TestConcat(long ans, long acc, Span<int> nums)
    {
        if (nums.IsEmpty) return ans == acc;
        long conc = Concat(acc, nums[0]);
        if (conc > ans) return false;

        return TestAdd(ans, conc, nums[1..])
               || TestMul(ans, conc, nums[1..])
               || TestConcat(ans, conc, nums[1..]);
    }

    private static bool TestMul(long ans, long acc, Span<int> nums)
    {
        if (nums.IsEmpty) return ans == acc;

        var mult = acc * nums[0];
        if (mult > ans) return false;

        return TestAdd(ans, mult, nums[1..])
               || TestMul(ans, mult, nums[1..])
               || TestConcat(ans, mult, nums[1..]);
    }

    private static bool TestAdd(long ans, long acc, Span<int> nums)
    {
        if (nums.IsEmpty) return ans == acc;
        var sum = acc + nums[0];
        if (sum > ans) return false;

        return TestAdd(ans, sum, nums[1..])
               || TestMul(ans, sum, nums[1..])
               || TestConcat(ans, sum, nums[1..]);
    }

    private static long Concat(long a, long b)
    {
        if (b < 10L) return 10L * a + b;
        if (b < 100L) return 100L * a + b;
        if (b < 1000L) return 1000L * a + b;
        if (b < 10000L) return 10000L * a + b;
        if (b < 100000L) return 100000L * a + b;
        if (b < 1000000L) return 1000000L * a + b;
        if (b < 10000000L) return 10000000L * a + b;
        if (b < 100000000L) return 100000000L * a + b;
        return 1000000000L * a + b;
    }
}