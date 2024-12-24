using System.Diagnostics;
using System.Linq.Expressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("test2", 4);
        // Run("test", 2024);
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

        bool[] x = new bool[45];
        bool[] y = new bool[45];

        int ix = 0;
        int iy = 0;

        foreach (var wireInput in inputsSpan)
        {
            var key = wireInput[..3];
            var value = wireInput[5] == '1';

            if (key.StartsWith('x'))
            {
                x[ix++] = value;
            }
            else
            {
                y[iy++] = value;
            }
        }

        Dictionary<string, (string left, Op op, string right)> gates = new Dictionary<string, (string, Op, string)>();

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

        var xP = Expression.Parameter(typeof(bool[]), "x");
        var yP = Expression.Parameter(typeof(bool[]), "y");

        // wcb <-> z34
        var wcb = gates["wcb"];
        var z34 = gates["z34"];
        gates["z34"] = wcb;
        gates["wcb"] = z34;

        // mkk <-> z10
        var mkk = gates["mkk"];
        var z10 = gates["z10"];
        gates["z10"] = mkk;
        gates["mkk"] = z10;

        // qbw <-> z14
        var qbw = gates["qbw"];
        var z14 = gates["z14"];
        gates["z14"] = qbw;
        gates["qbw"] = z14;

        // wjb <-> cvp
        var wjb = gates["wjb"];
        var cvp = gates["cvp"];
        gates["cvp"] = wjb;
        gates["wjb"] = cvp;

        foreach (var register in gates.Keys.Where(key => key.StartsWith('z')).OrderDescending())
        {
            Expression expr = BuildExpression(register, gates, xP, yP).expr;
            var lambda = Expression.Lambda<Func<bool[], bool[], bool>>(expr, xP, yP);
            // Console.WriteLine($"{register}: {expr.ToString("C#")}");
            var compiled = lambda.Compile();
            bool result = compiled(x, y);
            // Console.WriteLine($"{register}:\t{result}");
            if (result)
            {
                sum |= 1;
            }

            sum <<= 1;
        }

        sum >>= 1;

        Console.WriteLine($"s: {sum}: {sum:B}");

        return sum;
    }

    private static Dictionary<string, (Expression, int)> Expressions = new();

    private static (Expression expr, int depth) BuildExpression(string register,
        Dictionary<string, (string left, Op op, string right)> gates,
        ParameterExpression xP,
        ParameterExpression yP)
    {
        if (Expressions.TryGetValue(register, out var expression)) return expression;

        if (gates.TryGetValue(register, out var result))
        {
            var buildExpressionLeft = BuildExpression(result.left, gates, xP, yP);
            var buildExpressionRight = BuildExpression(result.right, gates, xP, yP);
            switch (result.op)
            {
                case Op.And:
                    expression = GetMaxExpression(Expression.And, buildExpressionLeft, buildExpressionRight);
                    break;
                case Op.Or:
                    expression = GetMaxExpression(Expression.Or, buildExpressionLeft, buildExpressionRight);
                    break;
                case Op.Xor:
                    expression = GetMaxExpression(Expression.ExclusiveOr, buildExpressionLeft, buildExpressionRight);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            if (register.StartsWith('x'))
            {
                expression = (Expression.ArrayAccess(xP, Expression.Constant(int.Parse(register.AsSpan(1)))), 1);
            }
            else
            {
                expression = (Expression.ArrayAccess(yP, Expression.Constant(int.Parse(register.AsSpan(1)))), 1);
            }
        }

        Expressions.Add(register, expression);
        return expression;
    }

    private static (Expression, int) GetMaxExpression(
        Func<Expression, Expression, BinaryExpression> func,
        (Expression expr, int depth) buildExpressionLeft,
        (Expression expr, int depth) buildExpressionRight)
    {
        (Expression, int) expression;
        if (buildExpressionLeft.depth > buildExpressionRight.depth)
        {
            expression = (func(buildExpressionRight.expr, buildExpressionLeft.expr),
                buildExpressionLeft.depth + 1);
        }
        else
        {
            expression = (func(buildExpressionLeft.expr, buildExpressionRight.expr),
                buildExpressionRight.depth + 1);
        }

        return expression;
    }

    enum Op
    {
        And,
        Or,
        Xor
    }
}