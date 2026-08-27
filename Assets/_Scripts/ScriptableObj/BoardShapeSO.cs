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
        if (validCells == null) return false;
        return validCells[y * width + x];
    }

    public bool[] GetValidCells()
    {
        return validCells;
    }

    public void SetSize(int newWidth, int newHeight)
    {
        width = Mathf.Max(1, newWidth);
        height = Mathf.Max(1, newHeight);
        validCells = new bool[width * height];

        for (int i = 0; i < validCells.Length; i++)
            validCells[i] = true;
    }
}