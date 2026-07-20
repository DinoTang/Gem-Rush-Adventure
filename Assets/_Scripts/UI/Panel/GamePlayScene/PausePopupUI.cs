using DG.Tweening;
using UnityEngine;
public enum PausePopupState
{
    Hide,
    Show,
    AreYouSure,
}
public class PausePopupUI : BaseUI
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;
    [SerializeField] protected MusicAndSoundGroupUI musicAndSoundGroupUI;
    [SerializeField] protected AYSTitleGroupUI aYSTitleGroupUI;
    public PausePopupState PausePopupState = PausePopupState.Hide;
    public MusicAndSoundGroupUI MusicAndSoundGroupUI => this.musicAndSoundGroupUI;
    public AYSTitleGroupUI AYSTitleGroupUI => this.aYSTitleGroupUI;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadCanvasGroup();
        this.LoadPanel();
        this.LoadMusicAndSoundGroupUI();
        this.LoadAYSTitleGroupUI();
    }
    protected void LoadCanvasGroup()
    {
        if (this.canvasGroup != null) return;
        this.canvasGroup = GetComponent<CanvasGroup>();
        Debug.Log(transform.name + ": LoadCanvasGroup", gameObject);
    }
    protected void LoadPanel()
    {
        if (this.panel != null) return;
        this.panel = FindAnyObjectByType<PausePanelGroupUI>().GetComponent<RectTransform>();
        Debug.Log(transform.name + ": LoadPanel", gameObject);
    }

    protected void LoadMusicAndSoundGroupUI()
    {
        if (this.musicAndSoundGroupUI != null) return;
        this.musicAndSoundGroupUI = FindAnyObjectByType<MusicAndSoundGroupUI>();
        Debug.Log(transform.name + ": LoadMusicAndSoundGroupUI", gameObject);
    }

    protected void LoadAYSTitleGroupUI()
    {
        if (this.aYSTitleGroupUI != null) return;
        this.aYSTitleGroupUI = FindAnyObjectByType<AYSTitleGroupUI>();
        Debug.Log(transform.name + ": LoadAYSTitleGroupUI", gameObject);
    }
    protected override void Start()
    {
        base.Start();
        // this.Hide();
    }
    public override void Show()
    {
        base.Show();

        this.PausePopupState = PausePopupState.Show;

        this.aYSTitleGroupUI.Hide();
        this.musicAndSoundGroupUI.Show();

        this.canvasGroup.DOKill();
        this.panel.DOKill();

        this.canvasGroup.alpha = 0f;
        this.panel.localScale = Vector3.zero;

        this.canvasGroup
            .DOFade(1f, 0.2f)
            .SetUpdate(true);

        this.panel
            .DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public override void Hide()
    {
        this.PausePopupState = PausePopupState.Hide;
        this.canvasGroup.DOFade(0f, 0.2f);
        this.panel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            base.Hide();
        });
    }

    public void ShowAreYouSure()
    {
        if (this.PausePopupState != PausePopupState.Show)
            return;

        this.PausePopupState = PausePopupState.AreYouSure;

        this.musicAndSoundGroupUI.HideAnimated();
        this.aYSTitleGroupUI.ShowAnimated();

        this.panel.DOKill();

        this.panel
            .DOPunchScale(
                new Vector3(0.04f, 0.04f, 0f),
                0.25f,
                6,
                0.5f
            )
            .SetUpdate(true);
    }

    public void HideAreYouSure()
    {
        if (this.PausePopupState != PausePopupState.AreYouSure)
            return;

        this.PausePopupState = PausePopupState.Show;

        this.aYSTitleGroupUI.HideAnimated();
        this.musicAndSoundGroupUI.ShowAnimated();

        this.panel.DOKill();

        this.panel
            .DOPunchScale(
                new Vector3(0.025f, 0.025f, 0f),
                0.2f,
                5,
                0.5f
            )
            .SetUpdate(true);
    }
}