using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemModel : BaseBehaviour
{
    [Header("GemModel")]
    [SerializeField] protected GemCtrl gemCtrl;
    [SerializeField] protected SpriteRenderer sprtRdr;
    [SerializeField] protected GemType gemType;
    public GemType GemType => gemType;
    [SerializeField] protected GemSpecialType gemSpecialType;
    public GemSpecialType GemSpecialType => gemSpecialType;
    [SerializeField] private float hintPulseSpeed = 3f;
    [SerializeField] protected bool isSelected = false;
    protected Vector3 defaultLocalScale;
    private Coroutine hintRoutine;
    protected override void Start()
    {
        base.Start();
        this.defaultLocalScale = this.transform.localScale;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemCtrl();
        this.LoadSpriteRenderer();
    }

    protected void LoadGemCtrl()
    {
        if (this.gemCtrl != null) return;
        this.gemCtrl = transform.parent.GetComponent<GemCtrl>();
        Debug.Log(transform.name + ": LoadGemCtrl");
    }
    protected void LoadSpriteRenderer()
    {
        if (this.sprtRdr != null) return;
        this.sprtRdr = transform.GetComponent<SpriteRenderer>();
        Debug.Log(transform.name + ": LoadSpriteRenderer");
    }

    public void SetGemType(GemType gemType)
    {
        this.gemType = gemType;
    }
    public virtual void SetGemSpecialType(GemSpecialType gemSpecialType)
    {
        if (gemSpecialType != GemSpecialType.None)
            this.sprtRdr.sortingOrder = 1;
        else this.sprtRdr.sortingOrder = 0;

        this.gemSpecialType = gemSpecialType;
    }
    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }
    public void SetVisual()
    {
        this.sprtRdr.sprite = this.isSelected
        ? this.GetGemVisualSelected(this.gemType, this.gemSpecialType)
        : this.GetGemVisualIdle(this.gemType, this.gemSpecialType);
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
        var visuals = this.gemCtrl?.GemDespawn?.GemSpawner?.gemVisuals;

        if (visuals == null || visuals.Count == 0)
        {
            var spawner = FindAnyObjectByType<GemSpawner>();
            visuals = spawner?.gemVisuals;
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
}
