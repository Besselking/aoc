using System.Diagnostics;
using System.Linq.Expressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test2", 4);
        Run("test", 2024);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d24.txt");
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

    private static long GetOutput(ReadOnlySpan<string> input)
    {
        long sum = 0;

        int endOfInputs = input.IndexOf("");
        var inputsSpan = input[..endOfInputs];
        var gatesSpan = input[(endOfInputs + 1)..];

        Dictionary<string, bool> inputs = new Dictionary<string, bool>(inputsSpan.Length);

        foreach (var wireInput in inputsSpan)
        {
            var key = wireInput[..3];
            var value = wireInput[5] == '1';
            inputs.Add(key, value);
        }

        Dictionary<string, (string, Op, string)> gates = new Dictionary<string, (string, Op, string)>();

        foreach (var gate in gatesSpan)
        {
            if (gate.Length == 17)
            {
                // or
                string left = gate[..3];
                string right = gate[7..10];
                string result = gate[14..];

                Op op = Op.Or;

                gates.Add(result, (left, op, right));
            }
            else
            {
                // and / xor
                string left = gate[..3];
                string right = gate[8..11];
                string result = gate[15..];

                Op op = gate[4] == 'A' ? Op.And : Op.Xor;
                gates.Add(result, (left, op, right));
            }
        }

        Expressions.Clear();

        foreach (var register in gates.Keys.Where(key => key.StartsWith('z')).OrderDescending())
        {
            Expression expr = BuildExpression(register, gates, inputs);
            bool result = Expression.Lambda<Func<bool>>(expr).Compile()();
            Console.WriteLine($"{register}:\t{result}");
            if (result)
            {
                sum |= 1;
            }

            sum <<= 1;
        }

        sum >>= 1;

        return sum;
    }

    private static Dictionary<string, Expression> Expressions = new Dictionary<string, Expression>();

    private static Expression BuildExpression(string register,
        Dictionary<string, (string left, Op op, string right)> gates, Dictionary<string, bool> inputs)
    {
        if (Expressions.TryGetValue(register, out var expression)) return expression;

        if (gates.TryGetValue(register, out var result))
        {
            expression = result.op switch
            {
                Op.And => Expression.And(
                    BuildExpression(result.left, gates, inputs),
                    BuildExpression(result.right, gates, inputs)),
                Op.Or => Expression.Or(
                    BuildExpression(result.left, gates, inputs),
                    BuildExpression(result.right, gates, inputs)),
                Op.Xor => Expression.ExclusiveOr(
                    BuildExpression(result.left, gates, inputs),
                    BuildExpression(result.right, gates, inputs)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else if (inputs.TryGetValue(register, out var input))
        {
            expression = Expression.Constant(input);
        }
        else
        {
            throw new InvalidOperationException($"Unknown register '{register}'");
        }

        Expressions.Add(register, expression);
        return expression;
    }

    enum Op
    {
        And,
        Or,
        Xor
    }
}