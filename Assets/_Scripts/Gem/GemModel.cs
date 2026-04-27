using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemModel : BaseBehaviour
{
    [SerializeField] protected GemCtrl gemCtrl;
    [SerializeField] protected SpriteRenderer sprtRdr;
    [SerializeField] protected GemType gemType;
    public GemType GemType => gemType;
    [SerializeField] private float hintPulseSpeed = 3f;
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

    public void SetVisual()
    {
        this.sprtRdr.sprite = this.gemCtrl.GemDespawn.GemSpawner.GetGemVisual(this.gemType);
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
}
