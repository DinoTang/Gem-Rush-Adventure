using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnimationHandler : BoardAbstract
{
    [SerializeField] protected float swapGemMoveTime = 0.18f;
    [SerializeField] protected float gravityGemMoveTime = 0.18f;

    public float SwapGemMoveTime => this.swapGemMoveTime;
    public float GravityGemMoveTime => this.gravityGemMoveTime;

    public IEnumerator AnimateGravity(List<FallMove> fallMoves)
    {
        if (fallMoves == null || fallMoves.Count == 0)
            yield break;

        float maxDuration = 0f;

        foreach (FallMove fallMove in fallMoves)
        {
            if (fallMove == null || fallMove.Gem == null)
                continue;

            float distance = Mathf.Abs(
                fallMove.TargetPos.y -
                fallMove.CurrentPos.y
            );

            float moveDuration = Mathf.Max(
                this.gravityGemMoveTime,
                distance * this.gravityGemMoveTime
            );

            float totalDuration = moveDuration;

            if (fallMove.Gem.GemMove != null)
                totalDuration += fallMove.Gem.GemMove.LandingDuration;

            maxDuration = Mathf.Max(maxDuration, totalDuration);

            Vector3 start = this.boardManager.GetWorldPos(
                (int)fallMove.CurrentPos.x,
                (int)fallMove.CurrentPos.y
            );

            Vector3 target = this.boardManager.GetWorldPos(
                (int)fallMove.TargetPos.x,
                (int)fallMove.TargetPos.y
            );

            fallMove.Gem.transform.position = start;
            fallMove.Gem.GemMove.MoveTo(target, moveDuration);
        }

        if (maxDuration > 0f)
            yield return new WaitForSeconds(maxDuration);
    }

    public IEnumerator AnimateMerge(List<SpecialMergeInfo> mergeInfos)
    {
        if (mergeInfos == null || mergeInfos.Count == 0)
            yield break;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.AudioDataSO.create);

        foreach (SpecialMergeInfo info in mergeInfos)
        {
            Vector3 targetPos = this.boardManager.GetWorldPos(
                info.SpecialCell.x,
                info.SpecialCell.y
            );

            foreach (Vector2Int sourceCell in info.SourceCells)
            {
                GemCtrl gem = this.boardManager.Grid.Get(
                    sourceCell.x,
                    sourceCell.y
                );

                if (gem == null || gem.GemMove == null)
                    continue;

                gem.GemMove.MoveTo(targetPos, this.swapGemMoveTime);
            }
        }

        yield return new WaitForSeconds(this.swapGemMoveTime);
    }
}