using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpecialSwapHandler : BoardAbstract
{
    private SpecialTriggerResolver specialTriggerResolver = new();

    public IEnumerator Resolve(
        List<MatchResult> originalMatches,
        GemCtrl gemA,
        GemCtrl gemB,
        Action<bool> onCompleted)
    {
        List<Vector2Int> startCells = null;

        yield return StartCoroutine(this.specialTriggerResolver.Resolve(
            gemA,
            gemB,
            this.boardManager.Grid,
            result => startCells = result
        ));

        if (startCells == null || startCells.Count == 0)
        {
            onCompleted?.Invoke(false);
            yield break;
        }

        List<CellClearInfo> finalCells =
            this.boardManager.MatchResolver.ResolveSpecialSwapCells(
                startCells,
                this.boardManager.Grid,
                gemA,
                gemB
            );

        yield return StartCoroutine(this.PlayCubeEffectIfNeeded(gemA, gemB, finalCells));

        yield return StartCoroutine(this.HandleSpecialSwapRoutine(gemA, gemB, finalCells));

        onCompleted?.Invoke(true);
    }

    private IEnumerator PlayCubeEffectIfNeeded(
        GemCtrl gemA,
        GemCtrl gemB,
        List<CellClearInfo> finalCells)
    {
        if (!IsCubeSwap(gemA, gemB))
            yield break;

        GemCtrl cubeGem = GetCubeGem(gemA, gemB);

        GemCubeModel cubeModel = cubeGem.GemModel as GemCubeModel;
        cubeModel?.PlayAnimateAndEffectCubeGem();

        VFXSpawner.Instance.SpawnSpecialVFXCubeGem(cubeGem);

        VFXSpawner.Instance.SpawnGemWasActiveByCubeVFX(
            cubeGem,
            ExtractCubeAffectedCells(cubeGem, finalCells)
        );

        VFXSpawner.Instance.SpawnCubeLightningVFX(
            cubeGem.transform.position,
             GetCubeTargetWorldPositions(cubeGem, finalCells)
        );

        yield return new WaitForSeconds(4f);
    }
    private List<Vector2Int> ExtractCubeAffectedCells(
        GemCtrl cubeGem,
        List<CellClearInfo> finalCells
        )
    {
        List<Vector2Int> result = new();

        foreach (var cell in finalCells)
        {
            if (cell.GridPos == cubeGem.GemData.GridPos)
                continue;

            if (cell.ClearReason != ClearReason.Cube)
                continue;

            result.Add(cell.GridPos);
        }

        return result;
    }
    private List<Vector3> GetCubeTargetWorldPositions(
            GemCtrl cubeGem,
            List<CellClearInfo> finalCells)
    {
        List<Vector3> targets = new();

        Vector2Int cubePos = cubeGem.GemData.GridPos;

        foreach (var cell in finalCells)
        {
            if (cell.GridPos == cubePos)
                continue;

            if (cell.ClearReason != ClearReason.Cube)
                continue;

            Vector3 worldPos = this.boardManager.GetWorldPos(
                cell.GridPos.x,
                cell.GridPos.y
            );

            targets.Add(worldPos);
        }

        return targets;
    }

    private bool IsCubeSwap(GemCtrl gemA, GemCtrl gemB)
    {
        return gemA.GemData.GemSpecialType == GemSpecialType.Cube ||
               gemB.GemData.GemSpecialType == GemSpecialType.Cube;
    }

    private GemCtrl GetCubeGem(GemCtrl gemA, GemCtrl gemB)
    {
        return gemA.GemData.GemSpecialType == GemSpecialType.Cube ? gemA : gemB;
    }

    protected IEnumerator HandleSpecialSwapRoutine(
        GemCtrl gemA,
        GemCtrl gemB,
        List<CellClearInfo> cells)
    {
        yield return StartCoroutine(
            this.boardManager.ResolveHandler.ResolveGravityRoutine(cells)
        );

        yield return StartCoroutine(
            this.boardManager.ResolveHandler.ResolveBoardRoutine(gemA, gemB)
        );
    }
}