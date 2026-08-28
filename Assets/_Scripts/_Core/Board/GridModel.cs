using System;

public class GridModel<T>
{
    public int Width { get; }
    public int Height { get; }

    private T[,] cells;

    private bool[,] validCells;

    // =========================================================
    // FLEX BOARD
    // =========================================================

    public GridModel(int width, int height, bool[,] validCells)
    {
        if (validCells.GetLength(0) != width || validCells.GetLength(1) != height)
        {
            throw new Exception(
                "validCells size does not match Grid size."
            );
        }

        this.Width = width;
        this.Height = height;

        this.cells = new T[width, height];

        this.validCells = validCells;
    }

    public T Get(int x, int y)
    {
        if (!IsInBounds(x, y))
            throw new Exception($"Out of bounds: {x},{y}");

        return cells[x, y];
    }

    public void Set(int x, int y, T value)
    {
        if (!IsInBounds(x, y))
            throw new Exception($"Out of bounds: {x},{y}");

        cells[x, y] = value;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 &&
               y >= 0 &&
               x < Width &&
               y < Height;
    }

    public bool HasCell(int x, int y)
    {
        if (!IsInBounds(x, y))
            return false;

        return this.validCells[x, y];
    }

    public void Swap((int x, int y) a, (int x, int y) b)
    {
        if (!IsInBounds(a.x, a.y) || !IsInBounds(b.x, b.y))
            throw new Exception("Swap out of bounds");

        T temp = cells[a.x, a.y];
        cells[a.x, a.y] = cells[b.x, b.y];
        cells[b.x, b.y] = temp;
    }
}