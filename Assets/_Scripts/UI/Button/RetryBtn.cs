using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RetryBtn : PausePopupBtn
{
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/RetryBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnButtonClicked()
    {
        if (pausePopup.PausePopupState == PausePopupState.Show)
        {
            pausePopup.ShowAreYouSure();
            return;
        }

        if (pausePopup.PausePopupState == PausePopupState.AreYouSure)
        {
            SceneLoader.Instance.GoToScene(SceneGame.GamePlayScene);
        }
    }
}