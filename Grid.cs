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