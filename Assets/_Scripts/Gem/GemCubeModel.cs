using DG.Tweening;
using UnityEngine;

public class GemCubeModel : GemModel
{
    [Header("GemCubeModel")]
    [SerializeField] protected Animator anim;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        LoadAnimator();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (anim != null)
            anim.enabled = false;
    }

    protected void LoadAnimator()
    {
        if (anim != null) return;

        anim = GetComponent<Animator>();

        Debug.Log(transform.name + ": LoadAnimator");
    }

    public override void RefreshVisual()
    {
        base.RefreshVisual();

        if (this.gemCtrl == null)
            return;

        if (this.gemCtrl.GemData.GemSpecialType == GemSpecialType.Cube)
            PlayCubeIdleAnimation();
        else
            StopCubeIdleAnimation();
    }

    protected void PlayCubeIdleAnimation()
    {
        Transform target = transform.parent;

        target.DOKill();

        target.DOScale(1.08f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        target.DOLocalRotate(
                new Vector3(0, 0, 4f),
                1.2f)
            .From(new Vector3(0, 0, -4f))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    protected void StopCubeIdleAnimation()
    {
        Transform target = transform.parent;

        target.DOKill();

        target.localScale = Vector3.one;
        target.localRotation = Quaternion.identity;
    }

    public VFXCtrl PlayAnimateAndEffectCubeGem()
    {
        anim.enabled = true;

        return VFXSpawner.Instance
            .SpawnSpecialVFXCubeGem(this.gemCtrl);
    }
}