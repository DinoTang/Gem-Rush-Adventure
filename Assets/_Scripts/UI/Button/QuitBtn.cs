using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitBtn : PausePopupBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/QuitBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        if (pausePopup.PausePopupState == PausePopupState.Show)
        {
            pausePopup.ShowAreYouSure();
            return;
        }

        if (pausePopup.PausePopupState == PausePopupState.AreYouSure)
        {
            SceneLoader.Instance.GoToScene(SceneGame.HomeScene);
        }
    }
}