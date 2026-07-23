using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ComingSoonBtn : BaseBtn
{
    protected override void LoadBtnSpriteSO()
    {

    }

    protected override void LoadButtonImage()
    {
        if (this.buttonImage == null)
            this.buttonImage = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": LoadButtonImage", gameObject);
    }

    protected override void OnButtonClicked()
    {
        if (ComingSoonUI.Instance == null)
        {
            Debug.LogWarning("ComingSoonUI.Instance is null", gameObject);
            return;
        }

        ComingSoonUI.Instance.Show();
    }
}