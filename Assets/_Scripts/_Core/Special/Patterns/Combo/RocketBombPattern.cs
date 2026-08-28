using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBombPattern : ISpecialComboPattern
{
    private ISpecialPattern pattern;
    public RocketBombPattern(ISpecialPattern specialPattern)
    {
        this.pattern = specialPattern;
    }

    public IEnumerator Execute(
        GemCtrl gemA,
        GemCtrl gemB,
        GridModel<GemCtrl> grid,
        Action<List<Vector2Int>> onCompleted
    )
    {
        HashSet<Vector2Int> clearCells = new();

        GemCtrl rocket = gemA.GemData.GemSpecialType == GemSpecialType.HorizontalRocket ||
                         gemA.GemData.GemSpecialType == GemSpecialType.VerticalRocket
                         ? gemA
                         : gemB;

        // Spawn 2 additional beams beside the rocket
        Vector2Int rocketPos = rocket.GemData.GridPos;

        if (rocket.GemData.GemSpecialType == GemSpecialType.HorizontalRocket)
        {
            GemCtrl topGem = GetGem(grid, rocketPos.x, rocketPos.y + 1);
            GemCtrl bottomGem = GetGem(grid, rocketPos.x, rocketPos.y - 1);

            if (topGem != null)
            {
                VFXSpawner.Instance.SpawnBeamVFX(
                    topGem,
                    rocket.GemData.GemType,
                    Vector3.right,
                    Vector3.zero
                );

                VFXSpawner.Instance.SpawnBeamVFX(
                    topGem,
                    rocket.GemData.GemType,
                    Vector3.left,
                    new Vector3(0, 0, 180)
                );
            }

            if (bottomGem != null)
            {
                VFXSpawner.Instance.SpawnBeamVFX(
                    bottomGem,
                    rocket.GemData.GemType,
                    Vector3.right,
                    Vector3.zero
                );

                VFXSpawner.Instance.SpawnBeamVFX(
                    bottomGem,
                    rocket.GemData.GemType,
                    Vector3.left,
                    new Vector3(0, 0, 180)
                );
            }
        }
        else
        {
            GemCtrl leftGem = grid.Get(rocketPos.x - 1, rocketPos.y);
            GemCtrl rightGem = grid.Get(rocketPos.x + 1, rocketPos.y);

            if (leftGem != null)
            {
                VFXSpawner.Instance.SpawnBeamVFX(
                    leftGem,
                    rocket.GemData.GemType,
                    Vector3.up,
                    new Vector3(0, 0, 90)
                );

                VFXSpawner.Instance.SpawnBeamVFX(
                    leftGem,
                    rocket.GemData.GemType,
                    Vector3.down,
                    new Vector3(0, 0, -90)
                );
            }

            if (rightGem != null)
            {
                VFXSpawner.Instance.SpawnBeamVFX(
                    rightGem,
                    rocket.GemData.GemType,
                    Vector3.up,
                    new Vector3(0, 0, 90)
                );

                VFXSpawner.Instance.SpawnBeamVFX(
                    rightGem,
                    rocket.GemData.GemType,
                    Vector3.down,
                    new Vector3(0, 0, -90)
                );
            }
        }

        bool isHorizontal = rocket.GemData.GemSpecialType == GemSpecialType.HorizontalRocket;

        for (int offset = -1; offset <= 1; offset++)
        {
            int x = rocketPos.x;
            int y = rocketPos.y;

            if (isHorizontal)
                y += offset;
            else
                x += offset;

            if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                continue;

            GemCtrl gem = grid.Get(x, y);
            if (gem == null) continue;

            clearCells.UnionWith(this.pattern.GetCells(gem, grid));
        }

        onCompleted?.Invoke(new List<Vector2Int>(clearCells));
        yield break;
    }

    private GemCtrl GetGem(
    GridModel<GemCtrl> grid,
    int x,
    int y
)
    {
        return grid.IsInBounds(x, y) ? grid.Get(x, y) : null;
    }
}