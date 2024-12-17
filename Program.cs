using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 117440);
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
        long sum = 1;
        ulong ra = 0; //input[0].AsSpan("Register A: ".Length).ParseAsUint();
        ulong rb = 0; //input[1].AsSpan("Register B: ".Length).ParseAsUint();
        ulong rc = 0; //input[2].AsSpan("Register C: ".Length).ParseAsUint();

        var programSpan = input[4].AsSpan("Program: ".Length);
        Span<byte> instructions = programSpan.Split(',').ParseAsBytes(programSpan);

        byte[] output = new byte[instructions.Length];
        int outputCount = 0;
        ulong initRa;
        for (initRa = 1; initRa < (ulong)Math.Pow(8, instructions.Length); initRa++)
        {
            outputCount = 0;
            ra = initRa;
            rb = 0;
            rc = 0;

            for (int i = 0; i + 1 < instructions.Length;)
            {
                byte opcode = instructions[i];
                if (output.Length > instructions.Length) break;
                byte operand = instructions[i + 1];

                switch (opcode)
                {
                    case 0: // adv, A division
                    {
                        var numerator = ra;
                        var combo = GetComboOperand(operand);
                        double denominator = Math.Pow(2, combo);
                        ra = (ulong)(numerator / denominator);
                        break;
                    }
                    case 1: // bxl, B xor
                    {
                        rb ^= operand;
                        break;
                    }
                    case 2: // bst, B modulo
                    {
                        var combo = GetComboOperand(operand);
                        rb = (ulong)combo % 8;
                        break;
                    }
                    case 3: // jnz, jump not zero
                    {
                        if (ra == 0) break;
                        i = (int)operand;
                        continue;
                    }
                    case 4: // bxc, B xor C
                    {
                        rb ^= rc;
                        break;
                    }
                    case 5: //out
                    {
                        var combo = GetComboOperand(operand);
                        var outval = combo % 8;
                        bool overflow = Output((byte)outval);
                        if (overflow) goto reset;
                        break;
                    }
                    case 6: // bdv, B division
                    {
                        var numerator = ra;
                        var combo = GetComboOperand(operand);
                        var denominator = Math.Pow(2, combo);
                        rb = (ulong)(numerator / denominator);
                        break;
                    }
                    case 7: // cdv, C division
                    {
                        var numerator = ra;
                        var combo = GetComboOperand(operand);
                        double denominator = Math.Pow(2, combo);
                        rc = (ulong)(numerator / denominator);
                        break;
                    }
                }

                i += 2;
            }

            reset:
            if (outputCount > 0 && instructions.EndsWith(output.AsSpan(0, outputCount)))
            {
                if (outputCount == instructions.Length) break;
                initRa = (initRa * 8) - 1;
            }
        }

        // sum = String.Join("", output.Select(x => x.ToString())).AsSpan().ParseAsLong();
        Console.WriteLine(initRa);
        return (long)initRa;

        ulong GetComboOperand(byte combo)
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

        bool Output(byte value)
        {
            // if (output.Count != 0) Console.Write(',');
            // Console.Write(value);
            if (outputCount == output.Length) return true;
            output[outputCount++] = value;
            return false;
        }
    }
}