
public class GridModel
{
    public int Width { get; }
    public int Height { get; }

    private Piece[,] cells;

    public GridModel(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.cells = new Piece[width, height];
    }

    public Piece Get(int x, int y)
    {
        if (!IsInBounds(x, y))
            throw new System.Exception($"Out of bounds: {x},{y}");

        return this.cells[x, y];
    }

    public void Set(int x, int y, Piece piece)
    {
        if (!IsInBounds(x, y))
            throw new System.Exception($"Out of bounds: {x},{y}");

        this.cells[x, y] = piece;
    }

    public bool IsInBounds(int x, int y)
    {

        return x >= 0 && y >= 0 && x < Width && y < Height;
    }

    public void Swap((int x, int y) a, (int x, int y) b)
    {
        if (!IsInBounds(a.x, a.y) || !IsInBounds(b.x, b.y))
            throw new System.Exception("Swap out of bounds");

        // Nếu không cạnh nhau return
        

        var temp = this.cells[a.x, a.y];
        this.cells[a.x, a.y] = this.cells[b.x, b.y];
        this.cells[b.x, b.y] = temp;
    }
}
