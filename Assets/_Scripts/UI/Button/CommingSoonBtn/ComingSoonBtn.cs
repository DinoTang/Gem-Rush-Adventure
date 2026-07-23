using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ComingSoonBtn : BaseBtn
{
    [SerializeField] protected ComingSoonUI comingSoonUI;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadComingSoonUI();
    }
    protected override void LoadBtnSpriteSO()
    {

    }

    protected void LoadComingSoonUI()
    {
        if (this.comingSoonUI == null)
            this.comingSoonUI = FindAnyObjectByType<ComingSoonUI>();
        Debug.Log(transform.name + ": LoadComingSoonUI", gameObject);
    }

    protected override void LoadButtonImage()
    {
        if (this.buttonImage == null)
            this.buttonImage = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": LoadButtonImage", gameObject);
    }

    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        this.comingSoonUI.Show();
    }
}