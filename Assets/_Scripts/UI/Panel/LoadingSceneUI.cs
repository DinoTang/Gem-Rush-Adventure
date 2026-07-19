using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneUI : BaseBehaviour
{
    protected static LoadingSceneUI instance;
    public static LoadingSceneUI Instance => instance;
    [Header("UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private LoadingBarUI loadingBarUI;
    [SerializeField] private TapToStartUI tapToStartUI;

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "MainMenu";

    [Header("Animation")]
    [SerializeField] private float minimumLoadingTime = 1.5f;
    [SerializeField] private float fillSpeed = 2f;

    private AsyncOperation loadOperation;
    private bool isReadyToStart;

    protected override void Awake()
    {
        if (instance != null) return;
        instance = this;
    }
    protected override void Start()
    {
        base.Start();
        StartCoroutine(this.LoadingRoutine());
    }
    private IEnumerator LoadingRoutine()
    {
        this.fillImage.fillAmount = 0f;

        // Giữ object TapToStart active,
        // chỉ ẩn bằng CanvasGroup.
        this.tapToStartUI.HideImmediately();

        this.loadOperation =
            SceneManager.LoadSceneAsync(
                this.nextSceneName
            );

        if (this.loadOperation == null)
        {
            Debug.LogError(
                $"Cannot load scene: {this.nextSceneName}",
                gameObject
            );

            yield break;
        }

        this.loadOperation.allowSceneActivation = false;

        float elapsedTime = 0f;
        float displayedProgress = 0f;

        while (!this.isReadyToStart)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float realProgress =
                Mathf.Clamp01(
                    this.loadOperation.progress / 0.9f
                );

            float timeProgress =
                Mathf.Clamp01(
                    elapsedTime /
                    this.minimumLoadingTime
                );

            float targetProgress =
                Mathf.Min(
                    realProgress,
                    timeProgress
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

            bool sceneLoaded =
                this.loadOperation.progress >= 0.9f;

            bool minimumTimeReached =
                elapsedTime >=
                this.minimumLoadingTime;

            bool barFilled =
                displayedProgress >= 0.999f;

            if (sceneLoaded &&
                minimumTimeReached &&
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

        if (this.loadOperation == null)
            return;

        this.loadOperation.allowSceneActivation = true;
    }
}