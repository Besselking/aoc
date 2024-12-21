using System.Diagnostics;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 126384);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d21.txt");
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


        foreach (var line in input)
        {
            var radiatedRobot = new Robot(null);
            var depressedRobot = new Robot(radiatedRobot);
            var numRobot = new Robot(depressedRobot);
            var keypad = new Robot(numRobot);

            Console.WriteLine($"{line}: ");

            foreach (var c in line)
            {
                keypad.Instruct(c);
            }

            Console.WriteLine(String.Join("", keypad.Instructions));
            Console.WriteLine(String.Join("", numRobot.Instructions));
            Console.WriteLine(String.Join("", depressedRobot.Instructions));
            Console.WriteLine(String.Join("", radiatedRobot.Instructions));

            int length = radiatedRobot.Instructions.Count;
            int num = int.Parse(line[..^1]);
            int complexity = length * num;

            Console.WriteLine($"{length} * {num} = {complexity} ");
            sum += complexity;
        }

        return sum;
    }

    private static Vec2 Hole = new Vec2(-2, 0);

    private class Robot(
        Robot? programmer)
    {
        public Vec2 Pos => Current.ToPos();
        public char Current { get; private set; } = 'A';
        public List<char> Instructions { get; init; } = [];
        public Robot? Programmer { get; set; } = programmer;

        public void Instruct(char instruction)
        {
            int currentIndex = Programmer?.Instructions.Count ?? 0;
            Programmer?.Program(instruction);
            int padding = Programmer?.Instructions.Count ?? 0;
            for (int i = 1; i < padding - currentIndex; i++)
            {
                Instructions.Add(' ');
            }

            Instructions.Add(instruction);
        }

        public void Program(char instruction)
        {
            var currentPos = Pos;
            var targetPos = instruction.ToPos();
            var path = targetPos - currentPos;

            while (path != Vec2.Zero)
            {
                int xdir = Math.Sign(path.X);
                int ydir = Math.Sign(path.Y);
                if (path.X != 0 && xdir == -1)
                {
                    var amount = -path.X;
                    Vec2 deltaPath = (Vec2.Left * amount);
                    var potPos = currentPos + deltaPath;
                    if (potPos != Hole)
                    {
                        path -= deltaPath;
                        currentPos = potPos;
                        for (int i = 0; i < amount; i++)
                        {
                            Instruct('<');
                        }

                        continue;
                    }
                }

                if (path.Y != 0 && ydir == 1)
                {
                    var amount = path.Y;
                    Vec2 deltaPath = (Vec2.Down * amount);
                    var potPos = currentPos + deltaPath;
                    if (potPos != Hole)
                    {
                        path -= deltaPath;
                        currentPos = potPos;
                        for (int i = 0; i < amount; i++)
                        {
                            Instruct('v');
                        }

                        continue;
                    }
                }

                if (path.Y != 0 && ydir == -1)
                {
                    var amount = -path.Y;
                    Vec2 deltaPath = (Vec2.Up * amount);
                    var potPos = currentPos + deltaPath;
                    if (potPos != Hole)
                    {
                        path -= deltaPath;
                        currentPos = potPos;
                        for (int i = 0; i < amount; i++)
                        {
                            Instruct('^');
                        }

                        continue;
                    }
                }

                if (path.X != 0 && xdir == 1)
                {
                    var amount = path.X;
                    Vec2 deltaPath = (Vec2.Right * amount);
                    var potPos = currentPos + deltaPath;
                    if (potPos != Hole)
                    {
                        path -= deltaPath;
                        currentPos = potPos;
                        for (int i = 0; i < amount; i++)
                        {
                            Instruct('>');
                        }

                        continue;
                    }
                }
            }

            Instruct('A');
            Current = instruction;
        }
    }

    private static Vec2 ToPos(this char c)
    {
        return c switch
        {
            'A' => Vec2.Zero,
            '0' => new(-1, 0),
            '1' => new(-2, -1),
            '2' => new(-1, -1),
            '3' => new(0, -1),
            '4' => new(-2, -2),
            '5' => new(-1, -2),
            '6' => new(0, -2),
            '7' => new(-2, -3),
            '8' => new(-1, -3),
            '9' => new(0, -3),

            '^' => new(-1, 0),
            'v' => new(-1, 1),
            '<' => new(-2, 1),
            '>' => new(0, 1),
        };
    }
}