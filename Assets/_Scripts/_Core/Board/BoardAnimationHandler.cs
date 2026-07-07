using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardAnimationHandler : BoardAbstract
{
    [SerializeField] protected float swapGemMoveTime = 0.18f;
    [SerializeField] protected float gravityGemMoveTime = 0.18f;
    public float SwapGemMoveTime => swapGemMoveTime;
    public float GravityGemMoveTime => gravityGemMoveTime;

    public IEnumerator AnimateGravity(List<FallMove> fallMoves)
    {
        float maxDuration = this.gravityGemMoveTime;

        foreach (var fallMove in fallMoves)
        {
            float distance = Mathf.Abs(fallMove.TargetPos.y - fallMove.CurrentPos.y);
            float moveDuration = Mathf.Max(this.gravityGemMoveTime, distance * this.gravityGemMoveTime);

            maxDuration = Mathf.Max(maxDuration, moveDuration);

            Vector3 start = this.boardManager.GetWorldPos((int)fallMove.CurrentPos.x, (int)fallMove.CurrentPos.y);
            Vector3 target = this.boardManager.GetWorldPos((int)fallMove.TargetPos.x, (int)fallMove.TargetPos.y);

            fallMove.Gem.transform.position = start;
            fallMove.Gem.GemMove.MoveTo(target, moveDuration);
        }

        yield return new WaitForSeconds(maxDuration);
    }

    public IEnumerator AnimateMerge(List<SpecialMergeInfo> mergeInfos)
    {
        if (mergeInfos == null || mergeInfos.Count == 0)
        {
            yield break;
        }

        foreach (var info in mergeInfos)
        {
            Vector3 targetPos = this.boardManager.GetWorldPos(info.SpecialCell.x, info.SpecialCell.y);

            foreach (var sourceCell in info.SourceCells)
            {
                var gem = this.boardManager.Grid.Get(sourceCell.x, sourceCell.y);
                if (gem == null) continue;
                gem.GemMove.MoveTo(targetPos, this.swapGemMoveTime);
            }
        }

        yield return new WaitForSeconds(this.swapGemMoveTime);
    }
}
