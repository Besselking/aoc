using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public partial class Program
{
    private record struct Poi(int Xh, int Xt, int Y, int? Val);

    private static List<Poi> GetPoi(string line, int y)
    {
        List<Poi> pois = new();

        foreach (var match in PoiRegex().EnumerateMatches(line))
        {
            pois.Add(new Poi(
                Xh: match.Index,
                Xt: match.Index + match.Length - 1,
                Y: y,
                Val: int.TryParse(line.AsSpan(match.Index, match.Length), out int val) ? val : null
            ));
        }

        return pois;
    }

    public static void Main()
    {
        // Run("testp1", 142);
        Run("test", 467835);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        string[] input = File.ReadAllLines($"{type}-d3.txt");

        var pois = input
            .SelectMany(GetPoi)
            .ToLookup(p => p.Y);

        int output = 0;
        foreach (var row in pois)
        {
            foreach (var symbol in row.Where(p => !p.Val.HasValue))
            {
                var parts = GetParts(pois, symbol, 0)
                            .Concat(GetParts(pois, symbol, -1))
                            .Concat(GetParts(pois, symbol, 1))
                            .Select(p => p.Val!.Value)
                            .ToList();

                if (parts.Count == 2) {
                    output += parts[0] * parts[1];
                }

                Console.WriteLine(String.Join(", ", parts));
            }
        }

        Console.Write($"{type}:\t{output}");

        if (expected.HasValue)
        {
            Console.Write($"\texpected:\t{expected}");
            Debug.Assert(expected == output);
        }
        Console.WriteLine();
    }

    private static IEnumerable<Poi> GetParts(ILookup<int, Poi> output, Poi symbol, int lineOffset)
    {
        return output[symbol.Y + lineOffset]
            .Where(p => p.Val.HasValue 
                && symbol.Xh <= p.Xt + 1
                && symbol.Xt >= p.Xh - 1);
    }

    [GeneratedRegex(@"\d+|\*")]
    private static partial Regex PoiRegex();
}
