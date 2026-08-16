using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteHandler : BaseBehaviour
{
    [SerializeField] private TapToSkipUI tapToSkipUI;
    [SerializeField] private NoMoreMoveUI noMoreMoveUI;
    private bool isHandlingCompletion;
    protected override void Start()
    {
        base.Start();

        LevelGoalManager.Instance.OnLevelCompleted +=
            this.HandleLevelCompleted;
        LevelGoalManager.Instance.OnLevelFailed +=
            this.HandleLevelFailed;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();

        this.LoadTapToSkipUI();
        this.LoadNoMoreMoveUI();
    }

    protected void LoadTapToSkipUI()
    {
        if (this.tapToSkipUI != null) return;

        this.tapToSkipUI = FindAnyObjectByType<TapToSkipUI>();

        Debug.Log(transform.name + ": LoadTapToSkipUI", gameObject);
    }

    protected void LoadNoMoreMoveUI()
    {
        if (this.noMoreMoveUI != null) return;

        this.noMoreMoveUI =
            FindAnyObjectByType<NoMoreMoveUI>();

        Debug.Log(transform.name + ": LoadNoMoreMoveUI", gameObject);
    }


    private void HandleLevelCompleted()
    {
        if (this.isHandlingCompletion)
            return;

        this.isHandlingCompletion = true;

        StartCoroutine(this.HandleLevelCompleteRoutine());
    }


    protected override void OnDestroy()
    {
        if (LevelGoalManager.Instance != null)
        {
            LevelGoalManager.Instance.OnLevelCompleted -=
                this.HandleLevelCompleted;
            LevelGoalManager.Instance.OnLevelFailed -=
                this.HandleLevelFailed;
        }

        base.OnDestroy();
    }

    private void HandleLevelFailed()
    {
        if (this.isHandlingCompletion)
            return;

        if (this.noMoreMoveUI == null)
            return;

        this.isHandlingCompletion = true;
        StartCoroutine(this.ShowLoseRoutine());
    }

    private IEnumerator ShowLoseRoutine()
    {
        if (BoardManager.Instance != null &&
            BoardManager.Instance.SwapHandler != null)
        {
            yield return new WaitUntil(() =>
                !BoardManager.Instance.SwapHandler.IsResolving);
        }

        yield return new WaitForSeconds(0.3f);

        this.noMoreMoveUI.ShowThenLose();
    }

    private IEnumerator HandleLevelCompleteRoutine()
    {

        yield return new WaitUntil(() => !BoardManager.Instance.SwapHandler.IsResolving);

        LevelGoalManager.Instance.SetLevelState(LevelState.WaitingForContinue);

        this.tapToSkipUI.Show();
        this.tapToSkipUI.EnableContinue();

        StartCoroutine(
            ConvertMovesToRocketRoutine()
        );
    }

    IEnumerator ConvertMovesToRocketRoutine()
    {
        int remainingMoves =
       LevelGoalManager.Instance.RemainingMoves;

        List<GemCtrl> selectedGems = this.GetRandomNormalGems(remainingMoves);

        foreach (GemCtrl gem in selectedGems)
        {
            if (gem == null) continue;

            LevelGoalManager.Instance.UseMove();

            GemSpecialType rocketType =
                Random.Range(0, 2) == 0
                    ? GemSpecialType.HorizontalRocket
                    : GemSpecialType.VerticalRocket;

            gem.GemData.SetGemSpecialType(rocketType);
            gem.GemModel.RefreshVisual();
            gem.GemModel.PlayTransformToSpecialAnimation();

            AudioManager.Instance.PlaySFX(AudioManager.Instance.AudioDataSO.create);
            VFXSpawner.Instance.SpawnTransformVFX(gem.transform.position);

            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.3f);

        if (BoardManager.Instance != null && BoardManager.Instance.ResolveHandler != null)
        {
            yield return StartCoroutine(BoardManager.Instance.ResolveHandler.ResolveCompletedSpecialGemsRoutine(selectedGems));
        }

        yield return new WaitForSeconds(0.5f);

        LevelGoalManager.Instance.SetLevelState(
            LevelState.Win
        );

        // Chuyển scene sang WinPopupUI.
        SceneLoader.Instance.LoadSceneImmediately(SceneGame.ResultScene);

        // this.tapToSkipUI.DisableContinue();

        // this.tapToSkipUI.Hide(() =>
        // {
        //     this.tapToSkipUI.ShowWinPopup();
        // });
    }

    public List<GemCtrl> GetRandomNormalGems(int count)
    {
        List<GemCtrl> candidates = new();

        GridModel<GemCtrl> grid = BoardManager.Instance.Grid;

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                GemCtrl gem = grid.Get(x, y);

                if (gem == null)
                    continue;

                if (gem.GemData.GemSpecialType != GemSpecialType.None)
                    continue;

                candidates.Add(gem);
            }
        }

        // Không được lấy nhiều hơn số gem hợp lệ hiện có.
        count = Mathf.Min(count, candidates.Count);

        // Trộn danh sách để không chọn trùng.
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (candidates[i], candidates[randomIndex]) =
                (candidates[randomIndex], candidates[i]);
        }

        return candidates.GetRange(0, count);
    }
}