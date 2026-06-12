using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardAnimationHandler : BaseBehaviour
{
    [SerializeField] protected BoardManager boardManager;

    [SerializeField] protected float animGemMoveTime = 0.18f;
    public float AnimGemMoveTime => animGemMoveTime;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadBoardManager();
    }
    protected void LoadBoardManager()
    {
        if (this.boardManager != null) return;
        this.boardManager = transform.parent.GetComponent<BoardManager>();
        Debug.Log(transform.name + ": LoadBoardManager");
    }

    public IEnumerator AnimateGravity(List<FallMove> fallMoves)
    {
        float time = 0f;
        float duration = this.animGemMoveTime;
        // float duration = 0f;

        // foreach (var fallMove in fallMoves)
        // {
        //     float distance =
        //         Mathf.Abs(fallMove.targetPos.y - fallMove.currentPos.y);

        //     duration = Mathf.Max(
        //         duration,
        //         distance * animGemMoveTime
        //     );
        // }
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            foreach (var fallMove in fallMoves)
            {
                Vector3 start = this.boardManager.GetWorldPos((int)fallMove.currentPos.x, (int)fallMove.currentPos.y);
                Vector3 target = this.boardManager.GetWorldPos((int)fallMove.targetPos.x, (int)fallMove.targetPos.y);
                fallMove.gem.transform.position = Vector3.Lerp(start, target, t);
            }
            yield return null;
        }

        foreach (var fallMove in fallMoves)
        {
            fallMove.gem.transform.position = this.boardManager.GetWorldPos((int)fallMove.targetPos.x, (int)fallMove.targetPos.y);
        }
    }

    public IEnumerator AnimateMerge(List<SpecialMergeInfo> mergeInfos)
    {
        foreach (var info in mergeInfos)
        {
            Vector3 targetPos = this.boardManager.GetWorldPos(info.SpecialCell.x, info.SpecialCell.y);

            foreach (var source in info.SourceCells)
            {
                var gem = this.boardManager.Grid.Get(source.x, source.y);
                if (gem == null) continue;
                StartCoroutine(gem.GemMove.MoveTo(targetPos, this.animGemMoveTime));
            }
        }

        yield return new WaitForSeconds(this.animGemMoveTime);
    }
}
