using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private GridModel grid;
    private MatchFinder matchFinder;

    private void Start()
    {
        grid = new GridModel(8, 8);
        matchFinder = new MatchFinder();

        FillRandom();
        TestSwap();
    }

    void FillRandom()
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var type = (PieceType)Random.Range(1, 6);
                grid.Set(x, y, new Piece(type));
            }
        }
    }

    void TestSwap()
    {
        bool result = TrySwap((0, 0), (1, 0));
        Debug.Log("Swap result: " + result);
    }

    public bool TrySwap((int x, int y) a, (int x, int y) b)
    {
        grid.Swap(a, b);

        var matches = matchFinder.FindMatches(grid);

        if (matches.Count == 0)
        {
            grid.Swap(a, b); // swap lại
            return false;
        }

        Debug.Log("Valid swap!");
        return true;
    }
}