using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GemModel : GemAbstract
{
    [Header("GemModel")]
    [SerializeField] protected SpriteRenderer sprtRdr;
    [SerializeField] private float hintPulseSpeed = 3f;
    protected Vector3 defaultLocalScale;
    private Coroutine hintRoutine;

    [Header("Transform Special Animation")]
    [SerializeField] private float transformScale = 1.18f;
    [SerializeField] private float transformScaleDuration = 0.15f;
    [SerializeField] private float transformRotateAngle = 10f;
    [SerializeField] private float transformRotateDuration = 0.08f;

    private Sequence transformSpecialSequence;
    protected override void Start()
    {
        base.Start();
        this.defaultLocalScale = this.transform.localScale;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadSpriteRenderer();
    }

    protected void LoadSpriteRenderer()
    {
        if (this.sprtRdr != null) return;
        this.sprtRdr = transform.GetComponent<SpriteRenderer>();
        Debug.Log(transform.name + ": LoadSpriteRenderer");
    }

    public virtual void SetSortingOrder(GemSpecialType gemSpecialType)
    {
        if (gemSpecialType != GemSpecialType.None)
            this.sprtRdr.sortingOrder = 1;
        else this.sprtRdr.sortingOrder = 0;

    }

    public virtual void RefreshVisual()
    {
        var data = this.gemCtrl.GemData;

        Sprite sprite =
            data.IsSelected
            ? GetGemVisualSelected(
                data.GemType,
                data.GemSpecialType)
            : GetGemVisualIdle(
                data.GemType,
                data.GemSpecialType);

        this.sprtRdr.sprite = sprite;

        this.SetSortingOrder(data.GemSpecialType);
    }

    public void ShowHintRoutine()
    {
        this.HideHintRoutine();
        this.hintRoutine = StartCoroutine(this.HintPulseRoutine());
    }

    public void HideHintRoutine()
    {
        if (this.hintRoutine != null)
        {
            StopCoroutine(this.hintRoutine);
            this.hintRoutine = null;
        }

        this.transform.localScale = this.defaultLocalScale;
    }

    IEnumerator HintPulseRoutine()
    {
        while (true)
        {
            float t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime * this.hintPulseSpeed;
                float scale = Mathf.Lerp(1.08f, 1, t);
                this.transform.localScale = this.defaultLocalScale * scale;

                yield return null;
            }

            t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime * this.hintPulseSpeed;
                float scale = Mathf.Lerp(1, 1.08f, t);
                this.transform.localScale = this.defaultLocalScale * scale;

                yield return null;
            }
        }
    }

    public Sprite GetGemVisualIdle(GemType gemType, GemSpecialType gemSpecialType)
    {
        var visuals = this.gemCtrl.GemDespawn.GemSpawner.gemVisuals;

        if (visuals == null || visuals.Count == 0)
        {
            var spawner = FindAnyObjectByType<GemSpawner>();
            visuals = spawner.gemVisuals;
        }

        var found = visuals?.Find(
            x =>
            x.GemType == gemType &&
            x.GemSpecialType == gemSpecialType
        );

        if (found == null) return null;
        return found.Sprite_Idle;
    }

    public Sprite GetGemVisualSelected(GemType gemType, GemSpecialType gemSpecialType)
    {
        // TODO: Refactor to avoid duplicate code with GetGemVisualIdle
        var visuals = this.gemCtrl?.GemDespawn?.GemSpawner?.gemVisuals;

        if (visuals == null || visuals.Count == 0)
        {
            var spawner = FindAnyObjectByType<GemSpawner>();
            visuals = spawner?.gemVisuals;
        }

        GemVisual found = null;
        if (gemSpecialType != GemSpecialType.Bomb)
        {
            found = visuals?.Find(x => x.GemType == gemType);
        }
        else
        {
            found = visuals?.Find(x => x.GemType == gemType && x.GemSpecialType == gemSpecialType);
        }

        if (found == null) return null;
        return found.Sprite_Selected;
    }

    public void PlayTransformToSpecialAnimation()
    {
        transformSpecialSequence?.Kill();
        transform.DOKill();

        this.HideHintRoutine();

        Vector3 startScale = this.defaultLocalScale;
        Vector3 startRotation = Vector3.zero;

        transform.localScale = startScale;
        transform.localEulerAngles = startRotation;

        transformSpecialSequence = DOTween.Sequence();

        transformSpecialSequence.Append(
            transform
                .DOScale(startScale * transformScale, transformScaleDuration)
                .SetEase(Ease.OutQuad)
        );

        transformSpecialSequence.Append(
            transform
                .DOLocalRotate(
                    new Vector3(0f, 0f, -transformRotateAngle),
                    transformRotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        transformSpecialSequence.Append(
            transform
                .DOLocalRotate(
                    new Vector3(0f, 0f, transformRotateAngle),
                    transformRotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        transformSpecialSequence.Append(
            transform
                .DOLocalRotate(
                    new Vector3(0f, 0f, -transformRotateAngle * 0.5f),
                    transformRotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        transformSpecialSequence.Append(
            transform
                .DOLocalRotate(startRotation, transformRotateDuration)
                .SetEase(Ease.InOutSine)
        );

        transformSpecialSequence.Join(
            transform
                .DOScale(startScale, transformScaleDuration)
                .SetEase(Ease.InQuad)
        );

        transformSpecialSequence.OnComplete(() =>
        {
            transform.localScale = startScale;
            transform.localEulerAngles = startRotation;
        });
    }
}
