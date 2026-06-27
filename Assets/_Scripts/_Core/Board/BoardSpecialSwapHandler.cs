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
        List<Vector2Int> specialCells = null;

        yield return StartCoroutine(this.ResolveSpecialCells(
            gemA,
            gemB,
            result => specialCells = result
        ));

        if (!HasSpecialCells(specialCells))
        {
            onCompleted?.Invoke(false);
            yield break;
        }

        List<CellClearInfo> finalCells = this.BuildFinalClearCells(
            originalMatches,
            specialCells
        );

        yield return StartCoroutine(this.PlayCubeEffectIfNeeded(gemA, gemB, finalCells));

        yield return StartCoroutine(this.HandleSpecialSwapRoutine(gemA, gemB, finalCells));

        onCompleted?.Invoke(true);
    }

    private IEnumerator ResolveSpecialCells(
    GemCtrl gemA,
    GemCtrl gemB,
    Action<List<Vector2Int>> onResolved)
    {
        yield return StartCoroutine(this.specialTriggerResolver.Resolve(
            gemA,
            gemB,
            this.boardManager.Grid,
            onResolved
        ));
    }

    private bool HasSpecialCells(List<Vector2Int> specialCells)
    {
        return specialCells != null && specialCells.Count > 0;
    }

    private List<CellClearInfo> BuildFinalClearCells(
        List<MatchResult> originalMatches,
        List<Vector2Int> specialCells)
    {
        HashSet<Vector2Int> addedCells = new();
        List<CellClearInfo> finalCells = new();

        this.AddSpecialCells(specialCells, addedCells, finalCells);
        this.AddResolvedMatchCells(originalMatches, addedCells, finalCells);
        this.AddOriginalMatchCells(originalMatches, addedCells, finalCells);

        return finalCells;
    }

    private void AddSpecialCells(
        List<Vector2Int> specialCells,
        HashSet<Vector2Int> addedCells,
        List<CellClearInfo> finalCells)
    {
        foreach (var cell in specialCells)
        {
            GemCtrl gem = this.boardManager.Grid.Get(cell.x, cell.y);
            if (gem == null) continue;

            ClearReason reason =
                this.boardManager.MatchResolver.ConvertReason(gem.GemData.GemSpecialType);

            this.AddCell(addedCells, finalCells, cell, reason);
        }
    }

    private void AddResolvedMatchCells(
        List<MatchResult> originalMatches,
        HashSet<Vector2Int> addedCells,
        List<CellClearInfo> finalCells)
    {
        List<CellClearInfo> resolvedCells =
            this.boardManager.MatchResolver.ResolveMatches(
                originalMatches,
                this.boardManager.Grid
            );

        foreach (var cell in resolvedCells)
        {
            this.AddCell(
                addedCells,
                finalCells,
                cell.GridPos,
                cell.ClearReason
            );
        }
    }

    private void AddOriginalMatchCells(
        List<MatchResult> originalMatches,
        HashSet<Vector2Int> addedCells,
        List<CellClearInfo> finalCells)
    {
        foreach (var match in originalMatches)
        {
            foreach (var cell in match.Cells)
            {
                this.AddCell(
                    addedCells,
                    finalCells,
                    cell,
                    ClearReason.Match
                );
            }
        }
    }

    private void AddCell(
        HashSet<Vector2Int> addedCells,
        List<CellClearInfo> finalCells,
        Vector2Int pos,
        ClearReason reason)
    {
        if (!addedCells.Add(pos))
            return;

        finalCells.Add(new CellClearInfo
        {
            GridPos = pos,
            ClearReason = reason
        });
    }

    private void AddOrUpgradeCell(
    Dictionary<Vector2Int, CellClearInfo> map,
    Vector2Int pos,
    ClearReason reason)
    {
        if (!map.ContainsKey(pos))
        {
            map[pos] = new CellClearInfo
            {
                GridPos = pos,
                ClearReason = reason
            };
            return;
        }

        if (map[pos].ClearReason == ClearReason.Match && reason != ClearReason.Match)
        {
            map[pos] = new CellClearInfo
            {
                GridPos = pos,
                ClearReason = reason
            };
        }
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

        VFXSpawner.Instance.SpawnGemWasActiveByCubeVFX(
            cubeGem,
            ExtractCells(finalCells)
        );

        yield return new WaitForSeconds(4);
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


    private List<Vector2Int> ExtractCells(List<CellClearInfo> list)
    {
        List<Vector2Int> result = new();
        foreach (var i in list)
            result.Add(i.GridPos);
        return result;
    }

    protected IEnumerator HandleSpecialSwapRoutine(GemCtrl gemA, GemCtrl gemB, List<CellClearInfo> cells)
    {
        yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveGravityRoutine(cells));
        yield return StartCoroutine(this.boardManager.ResolveHandler.ResolveBoardRoutine(gemA, gemB));
    }
}