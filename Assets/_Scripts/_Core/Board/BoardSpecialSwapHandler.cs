using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpecialSwapHandler : BoardAbstract
{
    private SpecialTriggerResolver specialTriggerResolver = new();

    public IEnumerator Resolve(List<MatchResult> originalMatches, GemCtrl gemA, GemCtrl gemB, Action<bool> onCompleted)
    {
        HashSet<Vector2Int> originalCells = new();
        foreach (var match in originalMatches)
        {
            originalCells.UnionWith(match.Cells);
        }

        List<Vector2Int> specialCells = null;

        yield return StartCoroutine(this.specialTriggerResolver
        .Resolve(gemA, gemB, this.boardManager.Grid, result => specialCells = result));

        if (specialCells != null && specialCells.Count > 0)
        {
            AudioManager.Instance.PlaySpecialClearSound(gemA, gemB);

            HashSet<Vector2Int> finalCells = new(specialCells);
            finalCells.UnionWith(originalCells);

            var matchCells = this.boardManager.MatchResolver.ResolveSpecialChains(new List<Vector2Int>(finalCells), this.boardManager.Grid);
            finalCells.UnionWith(matchCells);

            if (gemA.GemData.GemSpecialType == GemSpecialType.Cube || gemB.GemData.GemSpecialType == GemSpecialType.Cube)
            {
                GemCtrl cubeGem = gemA.GemData.GemSpecialType == GemSpecialType.Cube ? gemA : gemB;
                GemCubeModel cubeModel = cubeGem.GemModel as GemCubeModel;

                cubeModel?.PlayAnimateAndEffectCubeGem();

                List<Vector2Int> cubeTargets = new(finalCells);
                cubeTargets.RemoveAll(cell =>
                {
                    GemCtrl targetGem = this.boardManager.Grid.Get(cell.x, cell.y);
                    return targetGem == null
                        || targetGem.GemData.ClearReason != ClearReason.Cube
                        || (cell.x == cubeGem.GemData.GridPos.x && cell.y == cubeGem.GemData.GridPos.y);
                });

                if (cubeTargets.Count > 0)
                {
                    VFXSpawner.Instance.SpawnCubeLightningVFX(cubeGem, cubeTargets, this.boardManager.Grid);
                }

                VFXSpawner.Instance.SpawnGemWasActiveByCubeVFX(cubeGem, new List<Vector2Int>(finalCells));

                yield return new WaitForSeconds(4);
            }

            yield return StartCoroutine(this.HandleSpecialSwapRoutine(gemA, gemB, new List<Vector2Int>(finalCells)));

            onCompleted?.Invoke(true);
            yield break;
        }
        onCompleted?.Invoke(false);
    }

    protected IEnumerator HandleSpecialSwapRoutine(GemCtrl gemA, GemCtrl gemB, List<Vector2Int> cells)
    {
        yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveGravityRoutine(cells));
        yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveBoardRoutine(gemA, gemB));
    }

}