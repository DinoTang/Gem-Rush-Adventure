using System.Collections.Generic;
using UnityEngine;

public class GoalContainerUI : BaseBehaviour
{
    public static GoalContainerUI Instance { get; private set; }

    [SerializeField] private GoalItemUI goalItemPrefab;

    private readonly List<GoalItemUI> goalItems = new();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null)
            Debug.LogWarning("Only 1 GoalContainerUI allows to exist");

        Instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LevelGoalManager.Instance.OnGoalProgressChanged += this.OnGoalProgressChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelGoalManager.Instance.OnGoalProgressChanged -= this.OnGoalProgressChanged;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGoalItemPrefab();
    }
    protected void LoadGoalItemPrefab()
    {
        if (this.goalItemPrefab != null) return;

        this.goalItemPrefab = GetComponentInChildren<GoalItemUI>();
        Debug.Log(transform.name + ": LoadGoalItemPrefab", gameObject);
    }
    protected override void Start()
    {
        base.Start();
        this.SpawnGoalItems();
        this.goalItemPrefab.gameObject.SetActive(false);
    }

    private void SpawnGoalItems()
    {
        this.goalItems.Clear();

        IReadOnlyList<LevelGoalProgress> progresses =
            LevelGoalManager.Instance.GoalProgresses;

        foreach (LevelGoalProgress progress in progresses)
        {
            GoalItemUI item = Instantiate(
                goalItemPrefab,
                transform
            );

            item.transform.name = this.MakeGoalItemName(progress);
            item.Init(progress);
            this.goalItems.Add(item);
        }
    }

    protected string MakeGoalItemName(LevelGoalProgress progress)
    {
        return progress.Data.gemType + "_" + progress.Data.gemSpecialType + "_" + progress.Data.targetAmount;
    }

    private void OnGoalProgressChanged(LevelGoalProgress progress)
    {
        this.RefreshGoalItem(progress);
    }

    private void RefreshGoalItem(LevelGoalProgress progress)
    {
        if (progress == null)
            return;

        GoalItemUI item = this.goalItems.Find(item => item.Progress == progress);
        if (item == null)
            return;

        item.Refresh();
    }
}