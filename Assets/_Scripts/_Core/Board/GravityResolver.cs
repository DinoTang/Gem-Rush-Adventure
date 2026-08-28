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
        GravityMap gravityMap = new(grid);

        Dictionary<GemCtrl, Vector2Int> startPositions = new();

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (!grid.HasCell(x, y))
                    continue;

                GemCtrl gem = grid.Get(x, y);

                if (gem == null)
                    continue;

                startPositions[gem] = new Vector2Int(x, y);
            }
        }

        bool moved;

        do
        {
            moved = false;

            for (int y = grid.Height - 2; y >= 0; y--)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    Vector2Int currentPos = new(x, y);

                    if (TryMoveStraightDown(
                        currentPos,
                        grid,
                        gravityMap))
                    {
                        moved = true;
                    }
                }
            }
        }
        while (moved);

        do
        {
            moved = false;

            for (int y = grid.Height - 2; y >= 0; y--)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    Vector2Int currentPos = new(x, y);

                    if (TryMoveDiagonal(
                        currentPos,
                        grid,
                        gravityMap))
                    {
                        moved = true;
                    }
                }
            }
        }
        while (moved);

        foreach (var pair in startPositions)
        {
            GemCtrl gem = pair.Key;
            Vector2Int startPos = pair.Value;

            Vector2Int targetPos = gem.GemData.GridPos;

            if (startPos == targetPos)
                continue;

            fallMoves.Add(new FallMove
            {
                Gem = gem,
                CurrentPos = startPos,
                TargetPos = targetPos
            });
        }

        return fallMoves;
    }

    private bool TryMoveStraightDown(
        Vector2Int currentPos,
        GridModel<GemCtrl> grid,
        GravityMap gravityMap)
    {
        if (!gravityMap.IsValid(currentPos))
            return false;

        GemCtrl gem = grid.Get(currentPos.x, currentPos.y);

        if (gem == null)
            return false;

        Vector2Int targetPos = new(
            currentPos.x,
            currentPos.y + 1
        );

        if (!gravityMap.IsEmpty(targetPos))
            return false;

        MoveGem(
            gem,
            currentPos,
            targetPos,
            grid
        );

        return true;
    }

    private bool TryMoveDiagonal(
        Vector2Int currentPos,
        GridModel<GemCtrl> grid,
        GravityMap gravityMap)
    {
        if (!gravityMap.IsValid(currentPos))
            return false;

        GemCtrl gem = grid.Get(currentPos.x, currentPos.y);

        if (gem == null)
            return false;

        Vector2Int left = new(
            currentPos.x - 1,
            currentPos.y + 1
        );

        Vector2Int right = new(
            currentPos.x + 1,
            currentPos.y + 1
        );

        if (gravityMap.IsEmpty(left))
        {
            MoveGem(
                gem,
                currentPos,
                left,
                grid
            );

            return true;
        }

        if (gravityMap.IsEmpty(right))
        {
            MoveGem(
                gem,
                currentPos,
                right,
                grid
            );

            return true;
        }

        return false;
    }

    private void MoveGem(
        GemCtrl gem,
        Vector2Int currentPos,
        Vector2Int targetPos,
        GridModel<GemCtrl> grid)
    {
        grid.Set(currentPos.x, currentPos.y, null);
        grid.Set(targetPos.x, targetPos.y, gem);

        gem.GemData.SetGridPos(
            targetPos.x,
            targetPos.y
        );
    }
}