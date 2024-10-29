using System.Collections.Frozen;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc;

public static partial class Program
{
    public static void Main()
    {
        // Run("testp1", 142);
        Run("test", 30);
        Run("input");
    }

    private static void Run(string type, int? expected = null)
    {
        string[] input = File.ReadAllLines($"{type}-d4.txt");
        int output = GetOutput(input);

        Console.Write($"{type}:\t{output}");

        if (expected.HasValue)
        {
            Console.Write($"\texpected:\t{expected}");
            Debug.Assert(expected == output);
        }

        Console.WriteLine();
    }

    // Implementation

    record Metrics(
        Range IdRange,
        Range WinnerRange,
        Range NumberRange);

    record Card(int Id, SortedSet<int> Winners, int[] Numbers)
    {
        public readonly Lazy<int> Score = new Lazy<int>(() => CountScore(Winners, Numbers));
    }

    static Card ParseCard(Metrics metrics, ReadOnlySpan<char> line)
    {
        // Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        int id = int.Parse(line[metrics.IdRange]);
        List<int> winners = [];
        ReadOnlySpan<char> winnerSpan = line[metrics.WinnerRange];
        foreach (Range winner in winnerSpan.Split(' '))
        {
            ReadOnlySpan<char> s = winnerSpan[winner];
            if (s.IsEmpty) continue;
            winners.Add(int.Parse(s));
        }

        List<int> numbers = [];
        ReadOnlySpan<char> numberSpan = line[metrics.NumberRange];
        foreach (Range number in numberSpan.Split(' '))
        {
            ReadOnlySpan<char> s = numberSpan[number];
            if (s.IsEmpty) continue;
            numbers.Add(int.Parse(s));
        }

        return new Card(id, [.. winners], [.. numbers]);
    }

    private static int GetOutput(string[] input)
    {
        Metrics metrics = GetMetrics(input[0]);

        var cardset = input.Select(line => ParseCard(metrics, line)).ToFrozenDictionary(card => card.Id);

        int cardCount = 0;
        Queue<Card> cardQueue = new Queue<Card>(cardset.Values);

        while (cardQueue.Count > 0)
        {
            Card card = cardQueue.Dequeue();
            cardCount++;
            int score = card.Score.Value;
            if (score == 0) continue;

            for (int i = card.Id + 1; i < score + card.Id + 1; i++)
            {
                if (cardset.TryGetValue(i, out Card? wonCard))
                {
                    cardQueue.Enqueue(wonCard);
                }
            }
        }

        return cardCount;
    }

    private static int CountScore(SortedSet<int> winners, int[] numbers)
    {
        int count = 0;

        foreach (var i in numbers)
        {
            if (winners.Contains(i)) count++;
        }

        return count;
    }

    private static Metrics GetMetrics(string line)
    {
        Match match = MetricRegex().Match(line);

        Range idRange = match.Groups[1].Index..(match.Groups[1].Index + match.Groups[1].Length);
        Range winnersRange = match.Groups[2].Index..(match.Groups[2].Index + match.Groups[2].Length);
        Range numbersRange = match.Groups[3].Index..(match.Groups[3].Index + match.Groups[3].Length);

        return new Metrics(idRange, winnersRange, numbersRange);
    }

    // Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
    [GeneratedRegex(@"^Card(\s+\d+): ((?:\s*\d{1,2})+) \| ((?:\s*\d{1,2})+)$")]
    private static partial Regex MetricRegex();

    private static IEnumerable<TValue> PickFrom<TKey, TValue>(this IEnumerable<TKey> source,
        IDictionary<TKey, TValue> dict)
    {
        foreach (TKey key in source)
        {
            if (dict.TryGetValue(key, out TValue? value) && value != null)
            {
                yield return value;
            }
        }
    }
}