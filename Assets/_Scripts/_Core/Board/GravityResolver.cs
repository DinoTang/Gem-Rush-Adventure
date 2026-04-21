using UnityEngine;

public class GravityResolver
{
    public void ApplyGravity(GridModel<GemCtrl> grid)
    {
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

                    grid.Set(x, y, fallingGem);
                    grid.Set(x, sourceY, null);

                    fallingGem.SetGridPos(x, y);
                    fallingGem.transform.position =
                        new Vector2(x, -y);

                    break;
                }
            }
        }
    }
}