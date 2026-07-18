using UnityEngine;

public class GamePlayUI : BaseUI
{
    protected static GamePlayUI instance;
    public static GamePlayUI Instance => instance;
    [SerializeField] protected GemSpawner gemSpawner;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemSpawner();
    }
    protected void LoadGemSpawner()
    {
        if (this.gemSpawner != null) return;
        this.gemSpawner = FindAnyObjectByType<GemSpawner>();
        Debug.Log(transform.name + ": LoadGemSpawner");
    }
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 GamePlayUI allows to exist");
        instance = this;
    }

    public override void Hide()
    {
        base.Hide();
        BoardManager.Instance.gameObject.SetActive(false);
        this.gemSpawner.gameObject.SetActive(false);
    }
}
