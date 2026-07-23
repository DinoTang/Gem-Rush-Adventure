using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneGame
{
    BootScene,
    LoadingScene,
    HomeScene,
    GamePlayScene
}

public class SceneLoader : BaseBehaviour
{
    private static SceneLoader instance;
    public static SceneLoader Instance => instance;

    [Header("Default Loading")]
    [SerializeField] private float defaultLoadingTime = 1.5f;
    [SerializeField] private float timer = 0;
    public SceneGame NextScene { get; private set; } = SceneGame.HomeScene;
    public float LoadingTime { get; private set; }

    [Header("Home Scene")]
    [SerializeField] private LevelSO levelSO;
    public LevelSO LevelSO => levelSO;
    protected override void Awake()
    {
        base.Awake();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        this.LoadingTime = this.defaultLoadingTime;

        DontDestroyOnLoad(gameObject);
    }
    public void SetLevelSO(LevelSO levelSO)
    {
        this.levelSO = levelSO;
    }

    public void Update()
    {
        this.GoToNextScene();
    }

    public void GoToScene(SceneGame targetScene)
    {
        this.GoToScene(targetScene, this.defaultLoadingTime);
    }

    public void GoToScene(SceneGame targetScene, float loadingTime)
    {
        if (targetScene == SceneGame.LoadingScene)
        {
            Debug.LogWarning("Target scene cannot be LoadingScene_02.", gameObject);
            return;
        }

        this.NextScene = targetScene;
        this.LoadingTime = Mathf.Max(0f, loadingTime);

        SceneManager.LoadScene(SceneGame.LoadingScene.ToString());
    }

    // public AsyncOperation LoadNextSceneAsync()
    // {
    //     return SceneManager.LoadSceneAsync(this.NextScene.ToString());
    // }

    protected void GoToNextScene()
    {
        if (this.IsCurrentScene(SceneGame.LoadingScene))
        {
            this.timer += Time.deltaTime;
            if (this.timer < this.LoadingTime) return;
            this.timer = this.LoadingTime;
            this.timer = 0;
            this.LoadSceneImmediately(this.NextScene);
        }
    }

    public void LoadSceneImmediately(SceneGame targetScene)
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    public bool IsCurrentScene(SceneGame scene)
    {
        return SceneManager.GetActiveScene().name == scene.ToString();
    }

    protected override void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        base.OnDestroy();
    }
}