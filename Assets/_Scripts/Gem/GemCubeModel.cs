using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GemCubeModel : GemModel
{
    [Header("GemCubeModel")]
    [SerializeField] protected Animator anim;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadAnimator();
    }
    protected void LoadAnimator()
    {
        if (this.anim != null) return;
        this.anim = transform.GetComponent<Animator>();
        Debug.Log(transform.name + ": LoadAnimator");
    }

    public override void SetGemSpecialType(GemSpecialType gemSpecialType)
    {
        base.SetGemSpecialType(gemSpecialType);
        if (gemSpecialType == GemSpecialType.Cube)
            PlayCubeIdleAnimation();
        else
            StopCubeIdleAnimation();
    }
    protected void PlayCubeIdleAnimation()
    {
        if (this.gemSpecialType != GemSpecialType.Cube) return;

        transform.parent.DOScale(1.08f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        transform.parent.DOLocalRotate(
                new Vector3(0, 0, 4f),
                1.2f)
            .From(new Vector3(0, 0, -4f))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    protected void StopCubeIdleAnimation()
    {
        transform.parent.DOKill();

        transform.parent.localScale = Vector3.one;
        transform.parent.localRotation = Quaternion.identity;
    }
}
