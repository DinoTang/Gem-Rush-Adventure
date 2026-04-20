public class GridModel<T>
{
    public int Width { get; }
    public int Height { get; }

    private T[,] cells;

    public GridModel(int width, int height)
    {
        Width = width;
        Height = height;
        cells = new T[width, height];
    }

    public T Get(int x, int y)
    {
        if (!IsInBounds(x, y))
            throw new System.Exception($"Out of bounds: {x},{y}");

        return cells[x, y];
    }

    public void Set(int x, int y, T value)
    {
        if (!IsInBounds(x, y))
            throw new System.Exception($"Out of bounds: {x},{y}");

        cells[x, y] = value;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 &&
               y >= 0 &&
               x < Width &&
               y < Height;
    }

    public void Swap((int x, int y) a, (int x, int y) b)
    {
        if (!IsInBounds(a.x, a.y) || !IsInBounds(b.x, b.y))
            throw new System.Exception("Swap out of bounds");

        T temp = cells[a.x, a.y];
        cells[a.x, a.y] = cells[b.x, b.y];
        cells[b.x, b.y] = temp;
    }
}