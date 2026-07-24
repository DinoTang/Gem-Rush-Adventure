using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GemMove : BaseBehaviour
{
    [Header("Landing Animation")]
    [SerializeField] private Transform gemVisual;
    [SerializeField] private float landingPunchX = 0.45f;
    [SerializeField] private float landingPunchY = -0.35f;
    [SerializeField] private float landingDuration = 0.3f;
    [SerializeField] private int landingVibrato = 8;
    [SerializeField] private float landingElasticity = 1f;

    public float LandingDuration => this.landingDuration;

    private Coroutine moveCoroutine;
    private Vector3 originalVisualScale = Vector3.one;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemVisual();
    }

    protected override void Start()
    {
        base.Start();

        if (this.gemVisual != null)
            this.originalVisualScale = this.gemVisual.localScale;
    }

    protected void LoadGemVisual()
    {
        if (this.gemVisual != null)
            return;

        this.gemVisual = transform;
        Debug.Log(transform.name + ": LoadGemVisual", gameObject);
    }

    public void MoveTo(Vector3 target, float duration)
    {
        this.StopMoveCoroutine();

        if (duration <= 0f)
        {
            transform.parent.position = target;
            this.PlayLandingAnimation();
            return;
        }

        this.moveCoroutine = StartCoroutine(this.MoveToRoutine(target, duration));
    }

    private IEnumerator MoveToRoutine(Vector3 target, float duration)
    {
        Vector3 start = transform.parent.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / duration);
            transform.parent.position = Vector3.Lerp(start, target, progress);

            yield return null;
        }

        transform.parent.position = target;
        this.moveCoroutine = null;

        this.PlayLandingAnimation();
    }

    private void PlayLandingAnimation()
    {
        if (this.gemVisual == null)
            return;

        this.gemVisual.DOKill();
        this.gemVisual.localScale = this.originalVisualScale;

        Vector3 punchScale = new(
            this.landingPunchX,
            this.landingPunchY,
            0f
        );

        this.gemVisual
            .DOPunchScale(
                punchScale,
                this.landingDuration,
                this.landingVibrato,
                this.landingElasticity
            )
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (this.gemVisual != null)
                    this.gemVisual.localScale = this.originalVisualScale;
            });
    }

    private void StopMoveCoroutine()
    {
        if (this.moveCoroutine == null)
            return;

        StopCoroutine(this.moveCoroutine);
        this.moveCoroutine = null;
    }

    private void KillLandingAnimation()
    {
        if (this.gemVisual == null)
            return;

        this.gemVisual.DOKill();
        this.gemVisual.localScale = this.originalVisualScale;
    }

    protected override void OnDisable()
    {
        this.StopMoveCoroutine();
        this.KillLandingAnimation();

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        this.StopMoveCoroutine();
        this.KillLandingAnimation();

        base.OnDestroy();
    }
}