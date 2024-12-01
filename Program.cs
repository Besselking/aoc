using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("testp1", 142);
        // Run("test", 46);
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
        List<long> seedRanges = [];
        var seedsLine = input[0].AsSpan("seeds: ".Length);
        foreach (Range seedRange in seedsLine.Split(' '))
        {
            seedRanges.Add(long.Parse(seedsLine[seedRange]));
        }

        input = input[2..];
        Debug.Assert(input[0] == "seed-to-soil map:");
        input = input[1..];

        List<Mapping> seedToSoil = [];

        int i;
        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            seedToSoil.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "soil-to-fertilizer map:");
        input = input[1..];

        List<Mapping> soilToFertilizer = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            soilToFertilizer.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "fertilizer-to-water map:");
        input = input[1..];

        List<Mapping> fertilizerToWater = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            fertilizerToWater.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "water-to-light map:");
        input = input[1..];

        List<Mapping> waterToLight = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            waterToLight.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "light-to-temperature map:");
        input = input[1..];

        List<Mapping> lightToTemp = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            lightToTemp.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "temperature-to-humidity map:");
        input = input[1..];

        List<Mapping> tempToHumid = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            tempToHumid.Add(item);
        }

        input = input[(i + 1)..];
        Debug.Assert(input[0] == "humidity-to-location map:");
        input = input[1..];

        List<Mapping> humidToLoc = [];

        for (i = 0; i < input.Length && input[i] != ""; i++)
        {
            var mapLine = input[i].AsSpan();
            Mapping item = GetMapping(mapLine, i);

            humidToLoc.Add(item);
        }

        Mapping[] seedToSoilMap = [.. seedToSoil.Order()];
        Mapping[] soilToFertilizerMap = [.. soilToFertilizer.Order()];
        Mapping[] fertilizerToWaterMap = [.. fertilizerToWater.Order()];
        Mapping[] waterToLightMap = [.. waterToLight.Order()];
        Mapping[] lightToTempMap = [.. lightToTemp.Order()];
        Mapping[] tempToHumidMap = [.. tempToHumid.Order()];
        Mapping[] humidToLocMap = [.. humidToLoc.Order()];

        Mapping[][] maps = 
            [ seedToSoilMap
            , soilToFertilizerMap
            , fertilizerToWaterMap
            , waterToLightMap
            , lightToTempMap
            , tempToHumidMap
            , humidToLocMap
            ];


        long minLoc = long.MaxValue;

        var seeds = seedRanges.Chunk(2).Select(range => (range[0], range[0] + range[1] - 1)).ToArray();

        var lowestlocdest = humidToLocMap.OrderBy(map => map.DestOffset).First();
        Console.WriteLine("locs");
        Console.WriteLine(lowestlocdest);

        var pothumid = tempToHumidMap.Where(map => map.DestOffset < lowestlocdest.SrcEnd
                                                && map.DestOffset >= lowestlocdest.SrcStart).ToList();
        
        Console.WriteLine("humids");
        pothumid.ForEach(Console.WriteLine);

        var pottemp = lightToTemp.Where(map => pothumid.Any(
                                            humid => map.DestOffset < humid.SrcEnd
                                                  && map.DestOffset >= humid.SrcStart)).ToList();
        
        Console.WriteLine("temp");
        pottemp.ForEach(Console.WriteLine);

        var potlight = waterToLight.Where(map => pothumid.Any(
                                            humid => map.DestOffset < humid.SrcEnd
                                                  && map.DestOffset >= humid.SrcStart)).ToList();
        
        Console.WriteLine("light");
        potlight.ForEach(Console.WriteLine);

        var potwater = fertilizerToWater.Where(map => potlight.Any(
                                            humid => map.DestOffset < humid.SrcEnd
                                                  && map.DestOffset >= humid.SrcStart)).ToList();
        
        Console.WriteLine("water");
        potwater.ForEach(Console.WriteLine);

        var potfert = soilToFertilizer.Where(map => potwater.Any(
                                            humid => map.DestOffset < humid.SrcEnd
                                                  && map.DestOffset >= humid.SrcStart)).ToList();
        
        Console.WriteLine("fert");
        potfert.ForEach(Console.WriteLine);

        var potsoil = seedToSoilMap.Where(map => potfert.Any(
                                            humid => map.DestOffset < humid.SrcEnd
                                                  && map.DestOffset >= humid.SrcStart)).ToList();
        
        Console.WriteLine("soil");
        potsoil.ForEach(Console.WriteLine);

        var potseeds = seeds.Where(seed => pottemp.Any(
            soil => soil.SrcEnd < seed.Item1 && soil.SrcStart > seed.Item2
        )).ToList();

        Console.WriteLine("seeds");
        potseeds.ForEach(seed => Console.WriteLine($"seed: {seed.Item1}, {seed.Item2}"));

        return minLoc;
    }

    private static long GetDest(Mapping[] mapping, long src)
    {
        long dest = src;
        Mapping? map = GetMap(mapping, src);

        if (map is not null)
        {
            dest = map.DestOffset + (src - map.SrcStart);
        }
        return dest;
    }

    private static Mapping? GetMap(ReadOnlySpan<Mapping> mapping, long src)
    {
        if (mapping.IsEmpty) return null;

        int idx = mapping.Length / 2;
        Mapping map = mapping[idx];

        if (src < map.SrcStart) return GetMap(mapping[..idx], src);
        if (src > map.SrcEnd) return GetMap(mapping[(idx + 1)..], src);

        return map;
    }

    private static Mapping GetMapping(ReadOnlySpan<char> mapLine, int i)
    {
        int nextSplit = mapLine.IndexOf(' ');

        long destStart = long.Parse(mapLine[..nextSplit]);
        mapLine = mapLine[(nextSplit + 1)..];

        nextSplit = mapLine.IndexOf(' ');
        long srcStart = long.Parse(mapLine[..nextSplit]);

        long rangeLength = long.Parse(mapLine[(nextSplit + 1)..]);

        Mapping item = new Mapping(srcStart, srcStart + rangeLength - 1, destStart);
        return item;
    }

    private record Mapping(
            long SrcStart,
            long SrcEnd,
            long DestOffset)
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
            if (other > SrcEnd) return 1;
            return 0;
        }
    }
}