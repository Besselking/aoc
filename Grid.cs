namespace aoc;

public readonly ref struct Grid(char[] grid, int rows, int cols)
{
    private readonly Span<char> _grid = grid;

    public Grid(string[] input) : this(input.SelectMany(s => s).ToArray(), input.Length, input[0].Length)
    {
    }

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

    [Flags]
    public enum NeighborType
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,

        NorthEast = 16,
        SouthEast = 32,
        SouthWest = 64,
        NorthWest = 128,

        Orthogonal = North | East | South | West,
        Diagonal = NorthEast | SouthEast | NorthWest | SouthWest,
        All = Orthogonal | Diagonal,
    }

    public (int row, int col)[] NeighborsOf((int row, int col) pos, char equals, NeighborType neighborType)
    {
        Span<(int row, int col)> neighbors = stackalloc (int row, int col)[8];
        int count = NeighborsOf(neighbors, pos, equals, neighborType);
        return neighbors[..count].ToArray();
    }

    public int NeighborsOf(Span<(int row, int col)> neighbors, (int row, int col) pos, char equals,
        NeighborType neighborType)
    {
        int index = 0;
        if (neighborType.HasFlag(NeighborType.North)
            && TryGetValue((pos.row - 1, pos.col), out char neighborNorth)
            && neighborNorth == equals)
        {
            neighbors[index++] = (pos.row - 1, pos.col);
        }

        if (neighborType.HasFlag(NeighborType.East)
            && TryGetValue((pos.row, pos.col + 1), out char neighborEast)
            && neighborEast == equals)
        {
            neighbors[index++] = (pos.row, pos.col + 1);
        }

        if (neighborType.HasFlag(NeighborType.South)
            && TryGetValue((pos.row + 1, pos.col), out char neighborSouth)
            && neighborSouth == equals)
        {
            neighbors[index++] = (pos.row + 1, pos.col);
        }

        if (neighborType.HasFlag(NeighborType.West)
            && TryGetValue((pos.row, pos.col - 1), out char neighborWest)
            && neighborWest == equals)
        {
            neighbors[index++] = (pos.row, pos.col - 1);
        }

        if (neighborType.HasFlag(NeighborType.NorthEast)
            && TryGetValue((pos.row - 1, pos.col + 1), out char neighborNorthEast)
            && neighborNorthEast == equals)
        {
            neighbors[index++] = (pos.row - 1, pos.col + 1);
        }

        if (neighborType.HasFlag(NeighborType.SouthEast)
            && TryGetValue((pos.row + 1, pos.col + 1), out char neighborSouthEast)
            && neighborSouthEast == equals)
        {
            neighbors[index++] = (pos.row + 1, pos.col + 1);
        }

        if (neighborType.HasFlag(NeighborType.SouthWest)
            && TryGetValue((pos.row + 1, pos.col - 1), out char neighborSouthWest)
            && neighborSouthWest == equals)
        {
            neighbors[index++] = (pos.row + 1, pos.col - 1);
        }

        if (neighborType.HasFlag(NeighborType.NorthWest)
            && TryGetValue((pos.row - 1, pos.col - 1), out char neighborNorthWest)
            && neighborNorthWest == equals)
        {
            neighbors[index++] = (pos.row - 1, pos.col - 1);
        }

        return index;
    }

    private bool TryGetValue((int, int col) pos, out char item)
    {
        if (InBounds(pos))
        {
            item = this[pos];
            return true;
        }
        else
        {
            item = default;
            return false;
        }
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