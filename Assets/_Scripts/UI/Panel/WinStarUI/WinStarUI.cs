using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class WinStarUI : BaseBehaviour
{
    [Header("Components")]
    [SerializeField] protected Image starImage;
    [SerializeField] protected RectTransform starRect;

    [Tooltip("Ngôi sao hoặc slot có sẵn dùng làm vị trí đích.")]
    [SerializeField] protected RectTransform targetStarSlot;

    [Header("Sprites")]
    [SerializeField] protected StarSpriteSO starSpriteSO;

    [Header("Intro Animation")]
    [SerializeField] protected float startScale = 2.8f;
    [SerializeField] protected float startRotation = -18f;
    [SerializeField] protected float introDelay = 0.12f;
    [SerializeField] protected float shrinkDuration = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] protected float sprite4Time = 0.38f;

    [Range(0f, 1f)]
    [SerializeField] protected float sprite1Time = 0.72f;

    [Header("Fly To Slot")]
    [SerializeField] protected float flyDuration = 0.55f;
    [SerializeField] protected Ease flyEase = Ease.InOutCubic;

    [Header("Finish Animation")]
    [SerializeField] protected float punchDuration = 0.28f;
    [SerializeField] protected float punchScale = 0.12f;
    [SerializeField] protected Vector2 targetOffset = new(0f, 0.05f);
    private Sequence animationSequence;

    protected override void LoadComponent()
    {
        base.LoadComponent();

        this.LoadImage();
        this.LoadRectTransform();
        this.LoadTargetStarSlot();
        this.LoadStarSpriteSO();
    }

    protected void LoadImage()
    {
        if (this.starImage == null)
            this.starImage = GetComponent<Image>();
    }

    protected void LoadRectTransform()
    {
        if (this.starRect == null)
            this.starRect = GetComponent<RectTransform>();
    }
    protected abstract void LoadTargetStarSlot();
    protected void LoadStarSpriteSO()
    {
        if (this.starSpriteSO == null)
            this.starSpriteSO = Resources.Load<StarSpriteSO>("StarSpriteSO");
    }
    protected override void Start()
    {
        base.Start();

        this.ResetStar();

        // Chỉ để test trực tiếp.
        // Khi nối với score milestone thì nên xóa dòng này.
        // this.PlayUnlockAnimation();
    }

    public void PlayUnlockAnimation()
    {
        if (this.targetStarSlot == null)
        {
            Debug.LogError(
                $"{transform.name}: Target Star Slot is missing",
                this
            );

            return;
        }

        this.KillAnimation();

        this.gameObject.SetActive(true);

        // Trạng thái bắt đầu.
        this.starImage.sprite = this.starSpriteSO.Star3Sprite;

        this.starRect.localScale =
            Vector3.one * this.startScale;

        this.starRect.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                this.startRotation
            );

        Vector3 targetWorldPosition =
            this.targetStarSlot.position +
            (Vector3)targetOffset;

        Quaternion targetWorldRotation =
            this.targetStarSlot.rotation;

        Vector3 targetLocalScale =
            this.GetTargetScaleRelativeToCurrentParent();

        this.animationSequence = DOTween.Sequence();

        this.animationSequence
            .SetUpdate(true)
            .SetAutoKill(true);

        // Giữ ngôi sao lớn một nhịp.
        this.animationSequence.AppendInterval(
            this.introDelay
        );

        /*
         * Phase 1:
         * Ngôi sao thu nhỏ nhưng vẫn còn lớn hơn slot một chút.
         */
        // AudioManager.Instance.PlaySFX(AudioManager.Instance.AudioDataSO.progressStar);

        this.animationSequence.Append(
            this.starRect
                .DOScale(
                    targetLocalScale * 1.45f,
                    this.shrinkDuration
                )
                .SetEase(Ease.OutCubic)
        );

        this.animationSequence.Join(
            this.starRect
                .DOLocalRotate(
                    Vector3.zero,
                    this.shrinkDuration
                )
                .SetEase(Ease.OutCubic)
        );

        // Đổi sprite trong lúc thu nhỏ.
        this.animationSequence.InsertCallback(
            this.introDelay +
            this.shrinkDuration *
            this.sprite4Time,
            () =>
            {
                if (this.starImage != null)
                    this.starImage.sprite =
                        this.starSpriteSO.Star4Sprite;
            }
        );

        this.animationSequence.InsertCallback(
            this.introDelay +
            this.shrinkDuration *
            this.sprite1Time,
            () =>
            {
                if (this.starImage != null)
                    this.starImage.sprite =
                        this.starSpriteSO.Star1Sprite;
            }
        );

        /*
         * Phase 2:
         * Bay vào đúng vị trí, rotation và scale của slot.
         */
        this.animationSequence.Append(
            this.starRect
                .DOMove(
                    targetWorldPosition,
                    this.flyDuration
                )
                .SetEase(this.flyEase)
        );

        this.animationSequence.Join(
            this.starRect
                .DORotateQuaternion(
                    targetWorldRotation,
                    this.flyDuration
                )
                .SetEase(this.flyEase)
        );

        this.animationSequence.Join(
            this.starRect
                .DOScale(
                    targetLocalScale,
                    this.flyDuration
                )
                .SetEase(Ease.OutCubic)
        );

        /*
         * Phase 3:
         * Nhấn nhẹ khi ngôi sao chạm vào slot.
         */
        this.animationSequence.Append(
            this.starRect
                .DOPunchScale(
                    targetLocalScale *
                    this.punchScale,
                    this.punchDuration,
                    4,
                    0.55f
                )
        );

        this.animationSequence.OnComplete(() =>
        {
            this.SnapToTarget();

            this.starImage.sprite =
                this.starSpriteSO.Star1Sprite;

            this.animationSequence = null;
        });
    }

    private void SnapToTarget()
    {
        if (this.targetStarSlot == null)
            return;

        this.starRect.position =
            this.targetStarSlot.position +
            (Vector3)targetOffset;

        this.starRect.rotation =
            this.targetStarSlot.rotation;

        this.starRect.localScale =
            this.GetTargetScaleRelativeToCurrentParent();
    }

    private Vector3 GetTargetScaleRelativeToCurrentParent()
    {
        if (this.targetStarSlot == null)
            return Vector3.one;

        Transform currentParent =
            this.starRect.parent;

        if (currentParent == null)
            return this.targetStarSlot.lossyScale;

        Vector3 parentWorldScale =
            currentParent.lossyScale;

        Vector3 targetWorldScale =
            this.targetStarSlot.lossyScale;

        return new Vector3(
            this.SafeDivide(
                targetWorldScale.x,
                parentWorldScale.x
            ),
            this.SafeDivide(
                targetWorldScale.y,
                parentWorldScale.y
            ),
            this.SafeDivide(
                targetWorldScale.z,
                parentWorldScale.z
            )
        );
    }

    private float SafeDivide(float value, float divisor)
    {
        if (Mathf.Approximately(divisor, 0f))
            return value;

        return value / divisor;
    }

    public void ResetStar()
    {
        this.KillAnimation();

        if (this.starImage != null)
        {
            this.starImage.sprite =
                this.starSpriteSO.Star3Sprite;
        }

        if (this.starRect != null)
        {
            this.starRect.localScale =
                Vector3.zero;

            this.starRect.localRotation =
                Quaternion.identity;
        }
    }

    private void KillAnimation()
    {
        this.animationSequence?.Kill();
        this.animationSequence = null;

        this.starRect?.DOKill();
        this.starImage?.DOKill();
    }

    protected override void OnDisable()
    {
        this.KillAnimation();

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        this.KillAnimation();

        base.OnDestroy();
    }
}