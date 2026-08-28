using UnityEngine;

[CreateAssetMenu(fileName = "BoardShape", menuName = "SO/BoardShape")]
public class BoardShapeSO : ScriptableObject
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private bool[] validCells;

    public int Width => width;
    public int Height => height;
    public bool IsValid(int x, int y)
    {
        if (this.validCells == null) return false;
        return this.validCells[y * this.width + x];
    }
    public bool[,] GetValidCells2D()
    {
        bool[,] result = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                result[x, y] = validCells[y * width + x];
            }
        }

        return result;
    }
    public bool[] GetValidCells()
    {
        return this.validCells;
    }

    public void SetSize(int newWidth, int newHeight)
    {
        this.width = Mathf.Max(1, newWidth);
        this.height = Mathf.Max(1, newHeight);
        this.validCells = new bool[this.width * this.height];

        for (int i = 0; i < this.validCells.Length; i++)
            this.validCells[i] = true;
    }
}