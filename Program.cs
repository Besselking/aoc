using System.Collections.Immutable;
using System.Diagnostics;
using QuikGraph;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 4);
        Run("input");
    }

    private static void Run(string type, long? expected = null)
    {
        var input = File.ReadAllLines($"inputs/{type}-d23.txt");
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

        var graph = new UndirectedGraph<string, Edge<string>>();

        foreach (var line in input)
        {
            string left = line[..2];
            string right = line[3..];
            graph.AddVerticesAndEdge(new(left, right));
        }

        GetMaxClique(graph, (res) =>
        {
            if (res.Count > sum)
            {
                var password = String.Join(',', res.OrderBy(x => x));
                Console.WriteLine(password);
                sum = res.Count;
            }
        });


        return sum;
    }

    public static void GetMaxClique(UndirectedGraph<string, Edge<string>> graph,
        Action<ImmutableHashSet<string>> report)
    {
        ImmutableHashSet<string> R = [];
        ImmutableHashSet<string> X = [];
        ImmutableHashSet<string> P = [..graph.Vertices];

        GetMaxClique(graph, R, P, X, report);
    }

    public static void GetMaxClique(
        UndirectedGraph<string, Edge<string>> graph,
        ImmutableHashSet<string> R,
        ImmutableHashSet<string> P,
        ImmutableHashSet<string> X,
        Action<ImmutableHashSet<string>> report)
    {
        if (P.Count == 0 && X.Count == 0)
        {
            report(R);
            return;
        }

        foreach (var v in P)
        {
            var Nv = graph.AdjacentVertices(v);
            GetMaxClique(graph, R.Union([v]), P.Intersect(Nv), X.Intersect(Nv), report);
            P = P.Remove(v);
            X = X.Add(v);
        }
    }
}