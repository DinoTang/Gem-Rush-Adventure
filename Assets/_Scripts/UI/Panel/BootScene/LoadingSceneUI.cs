using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUI : BaseBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private LoadingBarUI loadingBarUI;
    [SerializeField] private TapToStartUI tapToStartUI;

    [Header("Animation")]
    [SerializeField] private float fillSpeed = 2f;

    private bool isReadyToStart;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(this.LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "SceneLoader.Instance is null",
                this
            );

            yield break;
        }

        if (this.fillImage == null ||
            this.loadingBarUI == null ||
            this.tapToStartUI == null)
        {
            Debug.LogError(
                "LoadingSceneUI is missing references",
                this
            );

            yield break;
        }

        this.isReadyToStart = false;

        this.fillImage.fillAmount = 0f;
        this.tapToStartUI.HideImmediately();

        float minimumLoadingTime =
            SceneLoader.Instance.LoadingTime;

        float elapsedTime = 0f;
        float displayedProgress = 0f;

        while (!this.isReadyToStart)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float targetProgress =
                Mathf.Clamp01(
                    elapsedTime /
                    minimumLoadingTime
                );

            displayedProgress =
                Mathf.MoveTowards(
                    displayedProgress,
                    targetProgress,
                    this.fillSpeed *
                    Time.unscaledDeltaTime
                );

            this.fillImage.fillAmount =
                displayedProgress;

            bool minimumTimeReached =
                elapsedTime >=
                minimumLoadingTime;

            bool barFilled =
                displayedProgress >= 0.999f;

            if (minimumTimeReached &&
                barFilled)
            {
                this.isReadyToStart = true;

                this.fillImage.fillAmount = 1f;

                this.loadingBarUI.Hide();
                this.tapToStartUI.Show();
            }

            yield return null;
        }
    }

    public void ActivateLoadedScene()
    {
        if (!this.isReadyToStart)
            return;

        SceneLoader.Instance.GoToScene(SceneGame.HomeScene);
    }
}