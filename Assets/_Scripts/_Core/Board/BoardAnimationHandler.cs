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
}
