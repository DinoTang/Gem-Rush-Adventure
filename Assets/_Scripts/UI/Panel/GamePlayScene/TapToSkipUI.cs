using UnityEngine;
using UnityEngine.EventSystems;

public class TapToSkipUI : EndGameMessageUI, IPointerClickHandler
{
    [SerializeField] protected WinPopupUI winPopupUI;
    [SerializeField] private bool canContinue;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadWinPopupUI();
    }

    protected override void LoadTitleRect()
    {
        if (this.titleRect != null) return;

        TapToSkipTitleUI titleUI = GetComponentInChildren<TapToSkipTitleUI>(true);

        if (titleUI == null)
        {
            Debug.LogError(transform.name + ": TapToSkipTitleUI not found", gameObject);
            return;
        }

        this.titleRect = titleUI.GetComponent<RectTransform>();
        Debug.Log(transform.name + ": LoadTapToSkipTitleUI", gameObject);
    }

    protected void LoadWinPopupUI()
    {
        if (this.winPopupUI != null) return;

        this.winPopupUI = FindAnyObjectByType<WinPopupUI>(FindObjectsInactive.Include);
        Debug.Log(transform.name + ": LoadWinPopupUI", gameObject);
    }

    public override void Show()
    {
        base.Show();

        this.canContinue = false;
        this.SetRaycastState(true);
    }

    public void EnableContinue()
    {
        this.canContinue = true;
    }

    public void DisableContinue()
    {
        this.canContinue = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!this.canContinue) return;

        this.canContinue = false;
        this.SetRaycastState(false);

        this.Hide(() =>
        {
            this.ShowWinPopup();
        });

        Debug.LogWarning("Tap To Skip clicked", gameObject);
    }

    public void ShowWinPopup()
    {
        GamePlayUI.Instance?.Hide();
        this.winPopupUI?.Show();
    }
}