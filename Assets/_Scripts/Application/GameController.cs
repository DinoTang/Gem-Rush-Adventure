using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private GridModel grid;
    private MatchFinder matchFinder;
    
    private void Start()
    {
        this.grid = new GridModel(8, 8);
        this.matchFinder = new MatchFinder();

        this.FillRandom();
        this.TestSwap();
    }

    public void FillRandom()
    {
        for (int x = 0; x < this.grid.Width; x++)
        {
            for (int y = 0; y < this.grid.Height; y++)
            {
                var type = (PieceType)Random.Range(1, 6);
                this.grid.Set(x, y, new Piece(type));
            }
        }
    }

    public void TestSwap()
    {
        bool result = TrySwap((0, 0), (1, 0));
        Debug.Log("Swap result: " + result);
    }

    public bool TrySwap((int x, int y) a, (int x, int y) b)
    {
        this.grid.Swap(a, b);

        var matches = matchFinder.FindMatches(grid);

        if (matches.Count == 0)
        {
            this.grid.Swap(a, b); // swap lại
            return false;
        }

        Debug.Log("Valid swap!");
        return true;
    }
}