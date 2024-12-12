using System.Runtime.CompilerServices;

namespace aoc;

//https://stackoverflow.com/a/926589
[CollectionBuilder(typeof(HistogramBuilder), "Create")]
public class Histogram<T> : Dictionary<T, long>
    where T : notnull
{
    public Histogram(ReadOnlySpan<T> source)
    {
        foreach (var item in source)
        {
            Add(item, 0);
        }
    }

    public Histogram(ReadOnlySpan<KeyValuePair<T, long>> source)
    {
        foreach (var item in source)
        {
            Add(item.Key, item.Value);
        }
    }

    public Histogram(Dictionary<T, long> source) : base(source)
    {
    }

    public void IncrementCount(T key)
    {
        if (ContainsKey(key))
        {
            this[key]++;
        }
        else
        {
            Add(key, 1);
        }
    }

    public void IncrementCount(T key, long count)
    {
        if (ContainsKey(key))
        {
            this[key] += count;
        }
        else
        {
            Add(key, count);
        }
    }

    public void DecrementCount(T key, long count)
    {
        if (ContainsKey(key))
        {
            this[key] -= count;
        }
        else
        {
            Add(key, -count);
        }
    }
}

public static class HistogramBuilder
{
    internal static Histogram<T> Create<T>(ReadOnlySpan<T> values) where T : notnull
        => new Histogram<T>(values);

    internal static Histogram<T> Create<T>(ReadOnlySpan<KeyValuePair<T, long>> values) where T : notnull
        => new Histogram<T>(values);
}