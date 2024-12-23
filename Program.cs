using System.Diagnostics;
using QuikGraph;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        Run("test", 7);
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

        HashSet<HashSet<string>> visited = new(new SetComparer<string>());

        foreach (var edge in graph.Edges.Where(e
                     => e.Source.StartsWith('t')
                        || e.Target.StartsWith('t')))
        {
            var source = edge.Source;
            var target = edge.Target;

            foreach (var adjacent in graph.AdjacentVertices(source).Except([target])
                         .Intersect(graph.AdjacentVertices(target).Except([source])))
            {
                if (visited.Add([source, target, adjacent]))
                {
                    // Console.WriteLine($"{source}, {target}, {adjacent}");
                }
            }
        }

        return visited.Count;
    }

    public class SetComparer<T> : IEqualityComparer<ISet<T>>
    {
        public bool Equals(ISet<T>? x, ISet<T>? y)
        {
            return x.SetEquals(y);
        }

        public int GetHashCode(ISet<T> obj)
        {
            return obj.OrderBy(o => o)
                .Select(o => o.GetHashCode())
                .Aggregate(0, (current, o) => current ^ o);
        }
    }
}