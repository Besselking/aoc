using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 3749);
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
        bool? valid = null;
        valid ??= TestSlow(ans, nums);

        // Console.WriteLine();

        return valid ?? throw new InvalidOperationException("oof");
    }

    private static bool? TestSlow(long ans, int[] nums)
    {
        Span<char> ops = stackalloc char[nums.Length - 1];
        ops.Fill('+');
        int tries = 0;
        while (ops.Contains('+'))
        {
            if (tries > 0)
            {
                InsertMul(ops);
            }

            int i = 1;
            long sum = nums[0];
            foreach (var op in ops)
            {
                switch (op)
                {
                    case '+':
                    {
                        sum += nums[i];
                        break;
                    }
                    case '*':
                    {
                        sum *= nums[i];
                        break;
                    }
                }

                i++;
            }

            if (sum == ans) return true;
            tries++;
        }

        return false;
    }

    private static void InsertMul(Span<char> ops)
    {
        for (int i = 0; i < ops.Length; i++)
        {
            if (ops[i] == '*')
            {
                ops[i] = '+';
                continue;
            }

            if (ops[i] == '+')
            {
                ops[i] = '*';
                break;
            }
        }
        // Console.WriteLine(new string(ops));
    }
}