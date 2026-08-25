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
        // =========================================================
        // 1. LẤY CÁC CELL MATCH BAN ĐẦU
        // =========================================================

        HashSet<Vector2Int> originalCells = new();

        foreach (var match in originalMatches)
        {
            originalCells.UnionWith(match.Cells);
        }

        // =========================================================
        // 2. RESOLVE SWAP SPECIAL
        //
        // SpecialTriggerResolver chỉ xác định những cell
        // bị tác động trực tiếp bởi swap.
        // =========================================================

        List<Vector2Int> specialCells = null;

        yield return StartCoroutine(
            specialTriggerResolver.Resolve(
                gemA,
                gemB,
                boardManager.Grid,
                result => specialCells = result
            )
        );

        if (specialCells == null || specialCells.Count == 0)
        {
            onCompleted?.Invoke(false);
            yield break;
        }

        AudioManager.Instance.PlaySpecialClearSound(
            gemA,
            gemB
        );

        // =========================================================
        // 3. KIỂM TRA CÓ CUBE KHÔNG
        // =========================================================

        bool hasCube =
            gemA.GemData.GemSpecialType == GemSpecialType.Cube ||
            gemB.GemData.GemSpecialType == GemSpecialType.Cube;

        GemCtrl cubeGem = null;

        if (hasCube)
        {
            cubeGem =
                gemA.GemData.GemSpecialType == GemSpecialType.Cube
                    ? gemA
                    : gemB;
        }

        // =========================================================
        // CASE 1 + CASE 2
        //
        // Có Cube trong swap
        // =========================================================

        if (hasCube)
        {
            GemCtrl otherGem =
                cubeGem == gemA
                    ? gemB
                    : gemA;

            bool otherIsSpecial =
                otherGem.GemData.GemSpecialType !=
                GemSpecialType.None;

            // =====================================================
            // CASE 2:
            //
            // CUBE + SPECIAL
            //
            // Special trigger NGAY.
            // Không wait 4 giây.
            // =====================================================

            if (otherIsSpecial)
            {
                HashSet<Vector2Int> finalCells =
                    new(specialCells);

                finalCells.UnionWith(originalCells);

                // -------------------------------------------------
                // Trigger Special ngay.
                //
                // Không cho Cube chain sang Cube khác.
                // -------------------------------------------------

                foreach (var cell in specialCells)
                {
                    GemCtrl specialGem =
                        boardManager.Grid.Get(
                            cell.x,
                            cell.y
                        );

                    if (specialGem == null)
                        continue;

                    GemSpecialType type =
                        specialGem.GemData.GemSpecialType;

                    // Không trigger Cube.
                    if (type == GemSpecialType.Cube)
                        continue;

                    TriggerSpecialWithoutCubeChain(
                        specialGem,
                        finalCells
                    );
                }

                // -------------------------------------------------
                // Clear
                // -------------------------------------------------

                yield return StartCoroutine(
                    HandleSpecialSwapRoutine(
                        gemA,
                        gemB,
                        new List<Vector2Int>(finalCells)
                    )
                );

                onCompleted?.Invoke(true);
                yield break;
            }

            // =====================================================
            // CASE 1:
            //
            // CUBE + GEM THƯỜNG
            // =====================================================

            HashSet<Vector2Int> finalCubeCells =
                new(specialCells);

            finalCubeCells.UnionWith(originalCells);

            // =====================================================
            // 4. XÁC ĐỊNH TARGET TRỰC TIẾP CỦA CUBE
            //
            // specialCells ở đây là vùng tác động trực tiếp
            // của Cube.
            //
            // Không dùng ResolveSpecialChains().
            // =====================================================

            List<Vector2Int> cubeTargets =
                new(specialCells);

            // Không tính chính Cube.
            cubeTargets.RemoveAll(
                cell => cell == cubeGem.GemData.GridPos
            );

            // =====================================================
            // 5. TÁCH SPECIAL BỊ CUBE ĐÁNH TRỰC TIẾP
            //
            // Rocket / Bomb / Horizontal / Vertical:
            //     -> đợi 4s rồi trigger
            //
            // Cube:
            //     -> KHÔNG trigger
            //     -> chỉ bị clear
            // =====================================================

            HashSet<Vector2Int> delayedSpecials =
                new();

            foreach (var cell in cubeTargets)
            {
                GemCtrl targetGem =
                    boardManager.Grid.Get(
                        cell.x,
                        cell.y
                    );

                if (targetGem == null)
                    continue;

                GemSpecialType type =
                    targetGem.GemData.GemSpecialType;

                // Gem thường -> không phải delayed special.
                if (type == GemSpecialType.None)
                    continue;

                // =================================================
                // QUAN TRỌNG:
                //
                // Cube 2 tuyệt đối không được trigger.
                // =================================================

                if (type == GemSpecialType.Cube)
                    continue;

                delayedSpecials.Add(cell);
            }

            // =====================================================
            // 6. CUBE ANIMATION
            // =====================================================

            GemCubeModel cubeModel =
                cubeGem.GemModel as GemCubeModel;

            if (cubeModel != null)
            {
                cubeModel.PlayAnimateAndEffectCubeGem();
            }

            // =====================================================
            // 7. XÁC ĐỊNH CÁC GEM ĐƯỢC NHẬN CUBE VFX
            //
            // Gem thường:
            //     -> nhận VFX
            //
            // Rocket/Bomb:
            //     -> nhận VFX
            //
            // Cube 2:
            //     -> KHÔNG nhận VFX
            //     -> KHÔNG trigger
            // =====================================================

            List<Vector2Int> cubeVFXTargets =
                new();

            foreach (var cell in cubeTargets)
            {
                GemCtrl targetGem =
                    boardManager.Grid.Get(
                        cell.x,
                        cell.y
                    );

                if (targetGem == null)
                    continue;

                // =================================================
                // Cube khác không được kích hoạt.
                // =================================================

                if (targetGem.GemData.GemSpecialType ==
                    GemSpecialType.Cube)
                {
                    continue;
                }

                cubeVFXTargets.Add(cell);
            }

            // =====================================================
            // 8. SPAWN CUBE VFX NGAY SAU KHI SWAP
            //
            // Spawn đúng 1 lần.
            // =====================================================

            if (cubeVFXTargets.Count > 0)
            {
                // -------------------------------------------------
                // Cube Lightning
                // -------------------------------------------------

                VFXSpawner.Instance.SpawnCubeLightningVFX(
                    cubeGem,
                    cubeVFXTargets,
                    boardManager.Grid
                );

                // -------------------------------------------------
                // Gem Was Activated By Cube
                // -------------------------------------------------

                VFXSpawner.Instance.SpawnGemWasActiveByCubeVFX(
                    cubeGem,
                    cubeVFXTargets
                );
            }

            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.AudioDataSO.electronic
            );

            // =====================================================
            // 9. WAIT 4 GIÂY
            // =====================================================

            yield return new WaitForSeconds(4f);

            // =====================================================
            // 10. SAU 4 GIÂY TRIGGER SPECIAL
            //
            // Rocket / Bomb / HorizontalRocket /
            // VerticalRocket...
            //
            // Cube 2 không nằm trong delayedSpecials
            // nên tuyệt đối không trigger.
            // =====================================================

            foreach (var cell in delayedSpecials)
            {
                GemCtrl specialGem =
                    boardManager.Grid.Get(
                        cell.x,
                        cell.y
                    );

                if (specialGem == null)
                    continue;

                TriggerSpecialWithoutCubeChain(
                    specialGem,
                    finalCubeCells
                );
            }

            // =====================================================
            // 11. CLEAR
            // =====================================================

            yield return StartCoroutine(
                HandleSpecialSwapRoutine(
                    gemA,
                    gemB,
                    new List<Vector2Int>(finalCubeCells)
                )
            );

            onCompleted?.Invoke(true);
            yield break;
        }

        // =========================================================
        // CASE 3:
        //
        // KHÔNG CÓ CUBE
        //
        // Giữ logic Special bình thường.
        // =========================================================

        HashSet<Vector2Int> normalFinalCells =
            new(specialCells);

        normalFinalCells.UnionWith(originalCells);

        foreach (var cell in specialCells)
        {
            GemCtrl specialGem =
                boardManager.Grid.Get(
                    cell.x,
                    cell.y
                );

            if (specialGem == null)
                continue;

            if (specialGem.GemData.GemSpecialType ==
                GemSpecialType.None)
            {
                continue;
            }

            TriggerSpecialWithoutCubeChain(
                specialGem,
                normalFinalCells
            );
        }

        // =========================================================
        // CLEAR
        // =========================================================

        yield return StartCoroutine(
            HandleSpecialSwapRoutine(
                gemA,
                gemB,
                new List<Vector2Int>(normalFinalCells)
            )
        );

        onCompleted?.Invoke(true);
    }

    // =============================================================
    // TRIGGER SPECIAL
    //
    // Dùng cho:
    //
    // - Cube + Special
    // - Cube + Gem thường sau 4s
    // - Special thường
    //
    // QUAN TRỌNG:
    //
    // Special này có thể clear Cube khác,
    // nhưng KHÔNG trigger Cube khác.
    // =============================================================

    private void TriggerSpecialWithoutCubeChain(
        GemCtrl specialGem,
        HashSet<Vector2Int> finalCells)
    {
        if (specialGem == null)
            return;

        GemSpecialType type =
            specialGem.GemData.GemSpecialType;

        // Không phải Special.
        if (type == GemSpecialType.None)
            return;

        // =========================================================
        // CUBE KHÔNG ĐƯỢC TRIGGER BỞI CUBE KHÁC
        // =========================================================

        if (type == GemSpecialType.Cube)
            return;


        AudioManager.Instance?.PlaySpecialClearSound(
            type
        );


        VFXSpawner.Instance.SpawnSpecialVFX(
            specialGem
        );

        // =========================================================
        // LẤY PATTERN
        // =========================================================

        var pattern =
            boardManager.MatchResolver
                .GetSpecialPattern(type);

        if (pattern == null)
            return;

        List<Vector2Int> extraCells =
            pattern.GetCells(
                specialGem,
                boardManager.Grid
            );

        // =========================================================
        // ADD CÁC CELL BỊ SPECIAL CLEAR
        //
        // Nếu gặp Cube:
        //
        // -> Cube bị clear
        // -> KHÔNG trigger Cube
        // =========================================================

        foreach (var cell in extraCells)
        {
            GemCtrl targetGem =
                boardManager.Grid.Get(
                    cell.x,
                    cell.y
                );

            if (targetGem == null)
                continue;

            finalCells.Add(cell);

            if (targetGem.GemData.GemSpecialType ==
                GemSpecialType.Cube)
            {
                // Cube chỉ bị clear.
                // Không gọi TriggerSpecialWithoutCubeChain().
                continue;
            }
        }
    }

    // =============================================================
    // CLEAR + GRAVITY + RESOLVE
    // =============================================================

    protected IEnumerator HandleSpecialSwapRoutine(
        GemCtrl gemA,
        GemCtrl gemB,
        List<Vector2Int> cells)
    {
        yield return StartCoroutine(
            boardManager.ResolveHandler
                .ResolveGravityRoutine(cells)
        );

        yield return StartCoroutine(
            boardManager.ResolveHandler
                .ResolveBoardRoutine(
                    gemA,
                    gemB
                )
        );
    }
}