using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicBtn : ToggleBtn
{
    protected override void Start()
    {
        base.Start();

        this.SetState(SaveManager.Instance.SaveData.musicEnabled);
    }
    
    protected override void LoadBtnSpriteSO()
    {
        if (this.btnSpriteSO != null) return;
        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/ToggleBtnSpriteSO/MusicBtnSpriteSO");
        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    protected override void OnToggleChanged()
    {
        // throw new System.NotImplementedException();
        AudioManager.Instance.ToggleMusic(this.isOn);
    }
}