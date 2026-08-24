using System.Collections.Generic;
using UnityEngine;

public class MatchResolver
{
    private SpecialPatternRegistry specialPatternRegistry = new();

    // =========================================================
    // NORMAL MATCH
    // =========================================================

    public List<Vector2Int> ResolveMatches(
    List<MatchResult> matches,
    GridModel<GemCtrl> grid,
    HashSet<Vector2Int> excluded = null)
    {
        HashSet<Vector2Int> cellsToClear = new();

        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                if (excluded != null &&
                    excluded.Contains(cell))
                {
                    continue;
                }

                GemCtrl gem =
                    grid.Get(cell.x, cell.y);

                if (gem != null)
                {
                    gem.GemData.SetClearReason(
                        ClearReason.Match
                    );
                }

                cellsToClear.Add(cell);
            }
        }

        // =====================================================
        // Resolve Special Chain
        // =====================================================

        SpecialChainResult result =
            ResolveSpecialChains(
                new List<Vector2Int>(cellsToClear),
                grid
            );

        // =====================================================
        // ⭐ Trigger VFX cho Rocket / Bomb / Special
        // =====================================================

        PlaySpecialChainEffects(
            result.SpecialCells,
            grid
        );

        return result.ClearCells;
    }

    // =========================================================
    // RESOLVE SPECIAL CHAIN
    //
    // Chỉ tính toán.
    //
    // KHÔNG:
    // - Spawn VFX
    // - Play sound
    // =========================================================

    public SpecialChainResult ResolveSpecialChains(
      List<Vector2Int> inputCells,
      GridModel<GemCtrl> grid)
    {
        SpecialChainResult result = new();

        HashSet<Vector2Int> cellsToClear =
            new(inputCells);

        Queue<Vector2Int> specialQueue =
            new();

        HashSet<Vector2Int> processedSpecials =
            new();

        foreach (var cell in inputCells)
        {
            GemCtrl gem =
                grid.Get(cell.x, cell.y);

            if (gem == null)
                continue;

            if (gem.GemData.GemSpecialType !=
                GemSpecialType.None)
            {
                specialQueue.Enqueue(cell);
            }
        }

        while (specialQueue.Count > 0)
        {
            Vector2Int specialCell =
                specialQueue.Dequeue();

            if (!processedSpecials.Add(specialCell))
                continue;

            GemCtrl specialGem =
                grid.Get(
                    specialCell.x,
                    specialCell.y
                );

            if (specialGem == null)
                continue;

            // =====================================================
            // GHI NHỚ SPECIAL NÀY CẦN TRIGGER
            // =====================================================

            result.SpecialCells.Add(
                specialCell
            );

            var pattern =
                specialPatternRegistry.GetPattern(
                    specialGem.GemData.GemSpecialType
                );

            if (pattern == null)
                continue;

            var extraCells =
                pattern.GetCells(
                    specialGem,
                    grid
                );

            foreach (var cell in extraCells)
            {
                if (!cellsToClear.Add(cell))
                    continue;

                GemCtrl targetGem =
                    grid.Get(
                        cell.x,
                        cell.y
                    );

                if (targetGem == null)
                    continue;

                // Cube bị clear nhưng không chain sang Cube khác
                if (targetGem.GemData.GemSpecialType ==
                    GemSpecialType.Cube)
                {
                    continue;
                }

                if (targetGem.GemData.GemSpecialType !=
                    GemSpecialType.None)
                {
                    specialQueue.Enqueue(cell);
                }
            }
        }

        result.ClearCells =
            new List<Vector2Int>(cellsToClear);

        return result;
    }

    // =========================================================
    // GET PATTERN
    // =========================================================

    public ISpecialPattern GetSpecialPattern(
        GemSpecialType type)
    {
        return specialPatternRegistry.GetPattern(type);
    }

    // =========================================================
    // PLAY SPECIAL VFX
    // =========================================================

    public void PlaySpecialEffects(
        List<Vector2Int> specialCells,
        GridModel<GemCtrl> grid,
        HashSet<Vector2Int> blockedCubeCells = null)
    {
        if (specialCells == null)
            return;

        foreach (var cell in specialCells)
        {
            GemCtrl gem =
                grid.Get(
                    cell.x,
                    cell.y
                );

            if (gem == null)
                continue;

            GemSpecialType type =
                gem.GemData.GemSpecialType;

            // -----------------------------------------------------
            // Cube bị Cube khác tác động
            // => KHÔNG trigger
            // -----------------------------------------------------

            if (type == GemSpecialType.Cube &&
                blockedCubeCells != null &&
                blockedCubeCells.Contains(cell))
            {
                continue;
            }

            // -----------------------------------------------------
            // Spawn Rocket / Bomb / Cube VFX
            // -----------------------------------------------------

            VFXSpawner.Instance.SpawnSpecialVFX(
                gem
            );

            AudioManager.Instance?.PlaySpecialClearSound(
                type
            );

            // -----------------------------------------------------
            // Cube
            // -----------------------------------------------------

            if (type == GemSpecialType.Cube)
            {
                var pattern =
                    specialPatternRegistry.GetPattern(
                        GemSpecialType.Cube
                    );

                if (pattern == null)
                    continue;

                var extraCells =
                    pattern.GetCells(
                        gem,
                        grid
                    );

                VFXSpawner.Instance.SpawnCubeLightningVFX(
                    gem,
                    extraCells,
                    grid
                );
            }
        }
    }
    public void PlaySpecialChainEffects(
    List<Vector2Int> specialCells,
    GridModel<GemCtrl> grid)
    {
        if (specialCells == null ||
            specialCells.Count == 0)
        {
            return;
        }

        foreach (var cell in specialCells)
        {
            GemCtrl specialGem =
                grid.Get(cell.x, cell.y);

            if (specialGem == null)
                continue;

            GemSpecialType type =
                specialGem.GemData.GemSpecialType;

            if (type == GemSpecialType.None)
                continue;

            // =====================================================
            // SPECIAL VFX
            // =====================================================

            VFXSpawner.Instance.SpawnSpecialVFX(
                specialGem
            );

            // =====================================================
            // SOUND
            // =====================================================

            AudioManager.Instance?.PlaySpecialClearSound(
                type
            );

            // =====================================================
            // CUBE
            //
            // Không cho Cube chain sang Cube khác.
            // =====================================================

            if (type == GemSpecialType.Cube)
                continue;

            // Special đã trigger ở đây.
        }
    }
}