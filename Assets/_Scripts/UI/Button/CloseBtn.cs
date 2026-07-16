using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseBtn : BaseBtn
{
    [SerializeField] private PausePopupUI popup;
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/CloseBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        this.popup.Hide();
    }
}