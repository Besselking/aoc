using System.Collections.Frozen;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("testp1", 142);
        Run("test", 35);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        string[] input = File.ReadAllLines($"{type}-d5.txt");
        var output = GetOutput(input);

        Console.Write($"{type}:\t{output}");

        if (expected.HasValue)
        {
            Console.Write($"\texpected:\t{expected}");
            Debug.Assert(expected == output);
        }

        Console.WriteLine();
    }

    // Implementation

    private static long GetOutput(ReadOnlySpan<string> input)
    {
        // seeds: 79 14 55 13
        List<long> seeds = [];
        var seedsLine = input[0].AsSpan("seeds: ".Length);
        foreach (Range seedRange in seedsLine.Split(' '))
        {
            seeds.Add(long.Parse(seedsLine[seedRange]));
        }

        input = input[2..];
        Debug.Assert(input[0] == "seed-to-soil map:");
        input = input[1..];

        List<Mapping> seedToSoilMap = [];

        int i;
        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            seedToSoilMap.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "soil-to-fertilizer map:");
        input = input[1..];

        List<Mapping> soilToFertilizerMap = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            soilToFertilizerMap.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "fertilizer-to-water map:");
        input = input[1..];

        List<Mapping> fertilizerToWaterMap = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            fertilizerToWaterMap.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "water-to-light map:");
        input = input[1..];

        List<Mapping> waterToLightMap = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            waterToLightMap.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "light-to-temperature map:");
        input = input[1..];

        List<Mapping> lightToTempMap = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            lightToTempMap.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "temperature-to-humidity map:");
        input = input[1..];

        List<Mapping> tempToHumidMap = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            tempToHumidMap.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "humidity-to-location map:");
        input = input[1..];

        List<Mapping> humidToLocMap = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            humidToLocMap.Add(item);
        }

        seedToSoilMap.Sort();
        soilToFertilizerMap.Sort();
        fertilizerToWaterMap.Sort();
        waterToLightMap.Sort();
        lightToTempMap.Sort();
        tempToHumidMap.Sort();
        humidToLocMap.Sort();

        long minLoc = long.MaxValue;
        foreach (long seed in seeds)
        {
            long soil = GetDest(seedToSoilMap, seed);
            long fert = GetDest(soilToFertilizerMap, soil);
            long water = GetDest(fertilizerToWaterMap, fert);
            long light = GetDest(waterToLightMap, water);
            long temp = GetDest(lightToTempMap, light);
            long humid = GetDest(tempToHumidMap, temp);
            long loc = GetDest(humidToLocMap, humid);

            Console.WriteLine($"Seed {seed}, soil {soil}, fertilizer {fert}, water {water}, light {light}, temperature {temp}, humidity {humid}, location {loc}.");

            minLoc = long.Min(minLoc, loc);
        }

        return minLoc;
    }

    private static long GetDest(List<Mapping> mapping, long src)
    {
        long dest = src;
        Mapping? map = mapping.FirstOrDefault(
            map => map.SrcStart <= src
                && map.SrcStart + map.RangeLength > src);

        if (map is not null)
        {
            dest = map.DestOffset + (src - map.SrcStart);
        }
        return dest;
    }

    private static Mapping GetMapping(ReadOnlySpan<char> mapLine, int i)
    {
        int nextSplit = mapLine.IndexOf(' ');

        long destStart = long.Parse(mapLine[..nextSplit]);
        mapLine = mapLine[(nextSplit + 1)..];

        nextSplit = mapLine.IndexOf(' ');
        long srcStart = long.Parse(mapLine[..nextSplit]);

        long rangeLength = long.Parse(mapLine[(nextSplit + 1)..]);

        Mapping item = new Mapping(srcStart, rangeLength, destStart);
        return item;
    }

    private record Mapping(long SrcStart, long RangeLength, long DestOffset) 
        : IComparable<Mapping>
        , IComparable<long>
    {
        public int CompareTo(Mapping? other)
        {
            if (other is null) return -1;

            return Comparer<long>.Default.Compare(SrcStart, other.SrcStart);
        }

        public int CompareTo(long other)
        {
            if (other < SrcStart) return -1;
            if (other > SrcStart + RangeLength - 1) return 1;
            return 0;
        }
    }
}