using UnityEngine;

[System.Serializable]
public class GemPair
{
    public Vector2Int a;
    public Vector2Int b;
    public GemPair(Vector2Int a, Vector2Int b)
    {
        this.a = a;
        this.b = b;
    }
}
