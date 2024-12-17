using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 4635635210);
        // Run("test2", 64);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d17.txt");
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
        uint ra = input[0].AsSpan("Register A: ".Length).ParseAsUint();
        uint rb = input[1].AsSpan("Register B: ".Length).ParseAsUint();
        uint rc = input[2].AsSpan("Register C: ".Length).ParseAsUint();

        var programSpan = input[4].AsSpan("Program: ".Length);
        uint[] instructions = programSpan.Split(',').ParseAsUints(programSpan);

        List<uint> output = new List<uint>();

        for (uint i = 0; i + 1 < instructions.Length;)
        {
            uint opcode = instructions[i];
            uint operand = instructions[i + 1];

            switch (opcode)
            {
                case 0: // adv, A division
                {
                    uint numerator = ra;
                    uint combo = GetComboOperand(operand);
                    uint denominator = (uint)Math.Pow(2, combo);
                    ra = numerator / denominator;
                    break;
                }
                case 1: // bxl, B xor
                {
                    rb ^= operand;
                    break;
                }
                case 2: // bst, B modulo
                {
                    uint combo = GetComboOperand(operand);
                    rb = combo % 8;
                    break;
                }
                case 3: // jnz, jump not zero
                {
                    if (ra == 0) break;
                    i = operand;
                    continue;
                }
                case 4: // bxc, B xor C
                {
                    rb ^= rc;
                    break;
                }
                case 5: //out
                {
                    uint combo = GetComboOperand(operand);
                    Output(combo % 8);
                    break;
                }
                case 6: // bdv, B division
                {
                    uint numerator = ra;
                    uint combo = GetComboOperand(operand);
                    uint denominator = (uint)Math.Pow(2, combo);
                    rb = numerator / denominator;
                    break;
                }
                case 7: // cdv, C division
                {
                    uint numerator = ra;
                    uint combo = GetComboOperand(operand);
                    uint denominator = (uint)Math.Pow(2, combo);
                    rc = numerator / denominator;
                    break;
                }
            }

            i += 2;
        }

        sum = String.Join("", output.Select(x => x.ToString())).AsSpan().ParseAsLong();

        return sum;

        uint GetComboOperand(uint combo)
        {
            return combo switch
            {
                <= 3 => combo,
                4 => ra,
                5 => rb,
                6 => rc,
                7 => throw new InvalidOperationException("invalid program 7 is reserved"),
                _ => throw new InvalidOperationException($"invalid program, not an operator '{combo}'"),
            };
        }

        void Output(uint value)
        {
            if (output.Count != 0) Console.Write(',');
            Console.Write(value);
            output.Add(value);
        }
    }
}