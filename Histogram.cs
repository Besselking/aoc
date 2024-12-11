namespace aoc;

//https://stackoverflow.com/a/926589
public class Histogram<T>(Dictionary<T, long> source) : Dictionary<T, long>(source)
    where T : notnull
{
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