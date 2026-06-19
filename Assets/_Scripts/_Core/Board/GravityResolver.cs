using System.Collections.Generic;
using UnityEngine;

public class FallMove
{
    public GemCtrl Gem;
    public Vector2Int CurrentPos;
    public Vector2Int TargetPos;
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
                        Gem = fallingGem,
                        CurrentPos = new Vector2Int(x, sourceY),
                        TargetPos = new Vector2Int(x, y),
                    };

                    grid.Set(x, y, fallingGem);
                    grid.Set(x, sourceY, null);

                    fallingGem.GemData.SetGridPos(x, y);

                    fallMoves.Add(fallMove);
                    break;
                }
            }
        }
        return fallMoves;
    }
}