using System.Collections.Generic;
using UnityEngine;

public class FallMove
{
    public GemCtrl gem;
    public Vector3 currentPos;
    public Vector3 targetPos;
}

public class GravityResolver
{
    public List<FallMove> ApplyGravity(GridModel<GemCtrl> grid)
    {
        List<FallMove> fallMoves = new();
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = grid.Height - 1; y >= 0; y--)
            {
                if (grid.Get(x, y) != null)
                    continue;

                for (int sourceY = y - 1; sourceY >= 0; sourceY--)
                {
                    GemCtrl fallingGem = grid.Get(x, sourceY);

                    if (fallingGem == null)
                        continue;

                    FallMove fallMove = new FallMove()
                    {
                        gem = fallingGem,
                        currentPos = fallingGem.transform.position,
                        targetPos = new Vector3(x, -y),
                    };

                    grid.Set(x, y, fallingGem);
                    grid.Set(x, sourceY, null);

                    fallingGem.SetGridPos(x, y);

                    fallMoves.Add(fallMove);
                    break;
                }
            }
        }
        return fallMoves;
    }
}