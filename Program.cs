using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

Regex gameID = new Regex(@"^Game (\d+):");
Regex redAmt = new Regex(@"(\d+) red");
Regex greenAmt = new Regex(@"(\d+) green");
Regex blueAmt = new Regex(@"(\d+) blue");


(int id, int red, int green, int blue) toGame(string line)
{
    var gameIDMatch = gameID.Match(line);
    int redMax = getMax(line, redAmt);
    int greenMax = getMax(line, greenAmt);
    int blueMax = getMax(line, blueAmt);

    return (
        id: int.Parse(gameIDMatch.Groups[1].ValueSpan),
        red: redMax,
        green: greenMax,
        blue: blueMax
    );

    static int getMax(string line, Regex regex)
    {
        return regex.Matches(line).Select(m => int.Parse(m.Groups[1].ValueSpan)).Max();
    }
}

int power((int id, int red, int green, int blue) game) {
    return game.red * game.green * game.blue;
}

// Run("testp1", 142);
Run("test", 2286);
Run("input");

void Run(string type, int? expected = null)
{
    string[] input = File.ReadAllLines($"{type}-d2.txt");

    var output = input.Select(toGame).Select(power).Sum();

    Console.Write($"{type}:\t{output}");

    if (expected.HasValue) {
        Console.Write($"\texpected:\t{expected}");
        Debug.Assert(expected == output);
    }
    Console.WriteLine();
}
