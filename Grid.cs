namespace aoc;

public readonly ref struct Grid(char[] grid, int rows, int cols)
{
    private readonly Span<char> _grid = grid;

    public char this[int globalIndex]
    {
        get => _grid[globalIndex];
        set => _grid[globalIndex] = value;
    }

    public char this[int row, int col]
    {
        get => _grid[row * rows + col];
        set => _grid[row * rows + col] = value;
    }

    public char this[(int row, int col) pos]
    {
        get => this[pos.row, pos.col];
        set => this[pos.row, pos.col] = value;
    }

    public int GlobalIndexOf(char item)
    {
        return _grid.IndexOf(item);
    }

    public (int row, int col) IndexOf(char item)
    {
        int globalIndex = GlobalIndexOf(item);
        return ToIndex(globalIndex);
    }

    private (int, int) ToIndex(int globalIndex)
    {
        return (globalIndex / rows, globalIndex % cols);
    }

    public int Count(char item) => _grid.Count(item);

    public int CountAll(ReadOnlySpan<char> items)
    {
        int sum = 0;
        foreach (var item in items)
        {
            sum += _grid.Count(item);
        }

        return sum;
    }

    public (int row, int col)[] IndexesOf(char item)
    {
        List<(int row, int col)> indexes = new List<(int row, int col)>();
        int last = 0;
        int index;
        while ((index = _grid[last..].IndexOf(item)) != -1)
        {
            index += last;
            indexes.Add(ToIndex(index));
            last = index + 1;
        }

        return indexes.ToArray();
    }

    public (char item, (int row, int col) pos)[] IndexesOfExcept(char item)
    {
        List<(char item, (int row, int col))> indexes = [];
        int last = 0;
        int index;
        while ((index = _grid[last..].IndexOfAnyExcept(item)) != -1)
        {
            index += last;
            indexes.Add((_grid[index], ToIndex(index)));
            last = index + 1;
        }

        return indexes.ToArray();
    }

    public bool InBounds((int row, int col) next)
    {
        return next.row >= 0 && next.row < rows
                             && next.col >= 0 && next.col < cols;
    }

    public string ToString()
    {
        return String.Create(rows * cols + rows, (grid, rows, cols), (str, state) =>
        {
            for (int row = 0; row < state.rows; row++)
            {
                for (int col = 0; col < state.cols; col++)
                {
                    str[row * state.rows + col + row] = state.grid[row * state.rows + col];
                }

                str[row * state.rows + state.cols + row] = '\n';
            }
        });
    }
}