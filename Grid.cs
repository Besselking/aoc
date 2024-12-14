namespace aoc;

public readonly ref struct Grid(char[] grid, int rows, int cols)
{
    private readonly Span<char> _grid = grid;

    public int Rows => rows;
    public int Cols => cols;

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
        get => _grid[row * cols + col];
        set => _grid[row * cols + col] = value;
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

    public char? GetNeighbor((int row, int col) pos, NeighborType neighbor)
    {
        var neighborPos = GetNeighborPos(pos, neighbor);
        if (InBounds(neighborPos))
        {
            return this[GetNeighborPos(pos, neighbor)];
        }
        else
        {
            return null;
        }
    }

    public (int row, int col) GetNeighborPos((int row, int col) pos, NeighborType neighborType)
    {
        return neighborType switch
        {
            NeighborType.North => (pos.row - 1, pos.col),
            NeighborType.East => (pos.row, pos.col + 1),
            NeighborType.South => (pos.row + 1, pos.col),
            NeighborType.West => (pos.row, pos.col - 1),
            NeighborType.NorthEast => (pos.row - 1, pos.col + 1),
            NeighborType.SouthEast => (pos.row + 1, pos.col + 1),
            NeighborType.SouthWest => (pos.row + 1, pos.col - 1),
            NeighborType.NorthWest => (pos.row - 1, pos.col - 1),
            _ => throw new ArgumentOutOfRangeException(nameof(neighborType), neighborType, null)
        };
    }

    public int GetEdgeCount((int row, int col) pos)
    {
        int count = 0;
        if (pos.row == 0) count++;
        if (pos.col == 0) count++;
        if (pos.row == Rows - 1) count++;
        if (pos.col == Cols - 1) count++;
        return count;
    }

    public (int row, int col)[] NeighborsOf((int row, int col) pos, char? equals, NeighborType neighborType)
    {
        Span<(int row, int col)> neighbors = stackalloc (int row, int col)[8];
        int count = NeighborsOf(neighbors, pos, equals, neighborType);
        return neighbors[..count].ToArray();
    }

    public int NeighborsOf(Span<(int row, int col)> neighbors, (int row, int col) pos, char? equals,
        NeighborType neighborType)
    {
        int index = 0;
        if (neighborType.HasFlag(NeighborType.North)
            && TryGetValue((pos.row - 1, pos.col), out char neighborNorth)
            && (equals == null || neighborNorth == equals))
        {
            neighbors[index++] = (pos.row - 1, pos.col);
        }

        if (neighborType.HasFlag(NeighborType.East)
            && TryGetValue((pos.row, pos.col + 1), out char neighborEast)
            && (equals == null || neighborEast == equals))
        {
            neighbors[index++] = (pos.row, pos.col + 1);
        }

        if (neighborType.HasFlag(NeighborType.South)
            && TryGetValue((pos.row + 1, pos.col), out char neighborSouth)
            && (equals == null || neighborSouth == equals))
        {
            neighbors[index++] = (pos.row + 1, pos.col);
        }

        if (neighborType.HasFlag(NeighborType.West)
            && TryGetValue((pos.row, pos.col - 1), out char neighborWest)
            && (equals == null || neighborWest == equals))
        {
            neighbors[index++] = (pos.row, pos.col - 1);
        }

        if (neighborType.HasFlag(NeighborType.NorthEast)
            && TryGetValue((pos.row - 1, pos.col + 1), out char neighborNorthEast)
            && (equals == null || neighborNorthEast == equals))
        {
            neighbors[index++] = (pos.row - 1, pos.col + 1);
        }

        if (neighborType.HasFlag(NeighborType.SouthEast)
            && TryGetValue((pos.row + 1, pos.col + 1), out char neighborSouthEast)
            && (equals == null || neighborSouthEast == equals))
        {
            neighbors[index++] = (pos.row + 1, pos.col + 1);
        }

        if (neighborType.HasFlag(NeighborType.SouthWest)
            && TryGetValue((pos.row + 1, pos.col - 1), out char neighborSouthWest)
            && (equals == null || neighborSouthWest == equals))
        {
            neighbors[index++] = (pos.row + 1, pos.col - 1);
        }

        if (neighborType.HasFlag(NeighborType.NorthWest)
            && TryGetValue((pos.row - 1, pos.col - 1), out char neighborNorthWest)
            && (equals == null || neighborNorthWest == equals))
        {
            neighbors[index++] = (pos.row - 1, pos.col - 1);
        }

        return index;
    }

    public int NeighborsOf(Span<(NeighborType, char)> neighbors, (int row, int col) pos,
        NeighborType neighborType)
    {
        int index = 0;
        if (neighborType.HasFlag(NeighborType.North)
            && TryGetValue((pos.row - 1, pos.col), out char neighborNorth))
        {
            neighbors[index++] = (NeighborType.North, neighborNorth);
        }

        if (neighborType.HasFlag(NeighborType.East)
            && TryGetValue((pos.row, pos.col + 1), out char neighborEast))
        {
            neighbors[index++] = (NeighborType.East, neighborEast);
        }

        if (neighborType.HasFlag(NeighborType.South)
            && TryGetValue((pos.row + 1, pos.col), out char neighborSouth))
        {
            neighbors[index++] = (NeighborType.South, neighborSouth);
        }

        if (neighborType.HasFlag(NeighborType.West)
            && TryGetValue((pos.row, pos.col - 1), out char neighborWest))
        {
            neighbors[index++] = (NeighborType.West, neighborWest);
        }

        if (neighborType.HasFlag(NeighborType.NorthEast)
            && TryGetValue((pos.row - 1, pos.col + 1), out char neighborNorthEast))
        {
            neighbors[index++] = (NeighborType.NorthEast, neighborNorthEast);
        }

        if (neighborType.HasFlag(NeighborType.SouthEast)
            && TryGetValue((pos.row + 1, pos.col + 1), out char neighborSouthEast))
        {
            neighbors[index++] = (NeighborType.SouthEast, neighborSouthEast);
        }

        if (neighborType.HasFlag(NeighborType.SouthWest)
            && TryGetValue((pos.row + 1, pos.col - 1), out char neighborSouthWest))
        {
            neighbors[index++] = (NeighborType.SouthWest, neighborSouthWest);
        }

        if (neighborType.HasFlag(NeighborType.NorthWest)
            && TryGetValue((pos.row - 1, pos.col - 1), out char neighborNorthWest))
        {
            neighbors[index++] = (NeighborType.NorthWest, neighborNorthWest);
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
            int offset = 0;
            for (int row = 0; row < state.rows; row++)
            {
                for (int col = 0; col < state.cols; col++)
                {
                    str[row * state.cols + col + offset] = state.grid[row * state.cols + col];
                }

                str[row * state.cols + state.cols + offset] = '\n';
                offset++;
            }
        });
    }
}