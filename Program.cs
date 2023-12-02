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

Func<(int id, int red, int green, int blue), bool> possibleGame(int red, int green, int blue) {
    return input => {
        return input.red <= red 
            && input.green <= green
            && input.blue <= blue;
    };
}

// Run("testp1", 142);
Run("test", 8);
Run("input");

void Run(string type, int? expected = null)
{
    string[] input = File.ReadAllLines($"{type}-d2.txt");

    var output = input.Select(toGame).Where(possibleGame(red: 12, green: 13, blue: 14)).Sum(game => game.id);

    Console.Write($"{type}:\t{output}");

    if (expected.HasValue) {
        Console.Write($"\texpected:\t{expected}");
        Debug.Assert(expected == output);
    }
    Console.WriteLine();
}
