using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

char[] digits = "123456789".ToCharArray();

string[] numbers = new[]
{
    "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"
};

Regex numlike = MyRegex();
Regex numlikeLast = MyRegex1();

int getNum(string line)
{
    var firstMatch = numlike.Match(line);
    var lastMatch = numlikeLast.Match(line);

    return convertNum(firstMatch.Value) * 10
        + convertNum(lastMatch.Value);
}

int convertNum(string word) {
    int num = Array.IndexOf(numbers, word);
    if (num < 0) {
        int digit = (int)Char.GetNumericValue(word[0]);
        Debug.Assert(digit >= 0);
        return digit;
    }
    return num;
}

Run("testp1", 142);
Run("test", 360);
Run("input");

void Run(string type, int? expected = null)
{
    string[] input = File.ReadAllLines($"{type}-d1.txt");

    var output = input.Select(getNum).Sum();

    Console.Write($"{type}:\t{output}");

    if (expected.HasValue) {
        Console.Write($"\texpected:\t{expected}");
        Debug.Assert(expected == output);
    }
    Console.WriteLine();
}

partial class Program
{
    [GeneratedRegex(@"(one|two|three|four|five|six|seven|eight|nine|\d)")]
    private static partial Regex MyRegex();
}

partial class Program
{
    [GeneratedRegex(@"(one|two|three|four|five|six|seven|eight|nine|\d)", RegexOptions.RightToLeft)]
    private static partial Regex MyRegex1();
}