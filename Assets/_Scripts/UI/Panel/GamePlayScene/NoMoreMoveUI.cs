using System.Collections;
using DG.Tweening;
using UnityEngine;

public class NoMoreMoveUI : EndGameMessageUI
{
    [Header("Popup")]
    [SerializeField] private LosePopupUI losePopupUI;

    [Header("Move Animation")]
    [SerializeField] private float startOffsetY = 250f;
    [SerializeField] private float moveDuration = 0.45f;
    [SerializeField] private Ease moveEase = Ease.OutBack;
    [SerializeField] private float stayDuration = 1f;

    private Vector2 originalTitlePosition;
    private Coroutine showRoutine;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadLosePopupUI();
    }

    protected override void Start()
    {
        base.Start();

        this.originalTitlePosition = this.titleRect.anchoredPosition;
        this.InitializeNoMoreMoveHidden();
    }

    protected override void LoadTitleRect()
    {
        if (this.titleRect != null) return;

        NoMoreMoveTitleUI titleUI =
            GetComponentInChildren<NoMoreMoveTitleUI>(true);

        if (titleUI == null)
        {
            Debug.LogError(
                transform.name + ": NoMoreMoveTitleUI not found",
                gameObject
            );

            return;
        }

        this.titleRect = titleUI.GetComponent<RectTransform>();

        Debug.Log(
            transform.name + ": LoadNoMoreMoveTitleUI",
            gameObject
        );
    }

    private void LoadLosePopupUI()
    {
        if (this.losePopupUI != null) return;

        this.losePopupUI =
            FindAnyObjectByType<LosePopupUI>(
                FindObjectsInactive.Include
            );

        if (this.losePopupUI == null)
        {
            Debug.LogError(
                transform.name + ": LosePopupUI not found",
                gameObject
            );

            return;
        }

        Debug.Log(
            transform.name + ": LoadLosePopupUI",
            gameObject
        );
    }

    public override void Show()
    {
        base.Show();

        this.KillTweens();

        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        this.titleRect.anchoredPosition =
            this.originalTitlePosition -
            Vector2.up * this.startOffsetY;

        this.titleRect.localScale = Vector3.one;

        this.canvasGroup
            .DOFade(1f, this.fadeDuration)
            .SetUpdate(true);

        this.titleRect
            .DOAnchorPos(
                this.originalTitlePosition,
                this.moveDuration
            )
            .SetEase(this.moveEase)
            .SetUpdate(true);
    }

    public void ShowThenLose()
    {
        if (this.showRoutine != null)
        {
            StopCoroutine(this.showRoutine);
            this.showRoutine = null;
        }

        this.showRoutine =
            StartCoroutine(this.ShowThenLoseRoutine());
    }

    private IEnumerator ShowThenLoseRoutine()
    {
        this.Show();

        yield return new WaitForSecondsRealtime(
            this.moveDuration + this.stayDuration + 1.5f
        );

        this.showRoutine = null;

        this.Hide(() =>
        {
            Debug.LogWarning(
                "NoMoreMove hidden. Showing LosePopup.",
                gameObject
            );

            GamePlayUI.Instance.Hide();

            if (this.losePopupUI == null)
            {
                Debug.LogError(
                    "Cannot show LosePopupUI because reference is null.",
                    gameObject
                );

                return;
            }
            // Lúc trước dùng chung scene nên dùng lệnh này
            // this.losePopupUI.Show();

            // Lúc sau tách scene nên dùng lệnh này
            SceneLoader.Instance.LoadSceneImmediately(SceneGame.ResultScene);
        });
    }

    private void InitializeNoMoreMoveHidden()
    {
        this.KillTweens();

        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0f;
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
        }

        if (this.titleRect != null)
        {
            this.titleRect.anchoredPosition =
                this.originalTitlePosition;

            this.titleRect.localScale = Vector3.one;
        }
    }

    protected override void OnDisable()
    {
        if (this.showRoutine != null)
        {
            StopCoroutine(this.showRoutine);
            this.showRoutine = null;
        }

        base.OnDisable();
    }
}