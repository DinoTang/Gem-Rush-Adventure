using UnityEngine;

public class MusicAndSoundGroupUI : AnimatedPanelUI
{
    protected override Vector2 MoveOffset => new Vector2(-moveDistance, 0f);

    public override void Show()
    {
        base.Show();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public override void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        base.Hide();
    }

    public override void ShowAnimated()
    {
        base.ShowAnimated();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public override void HideAnimated()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        base.HideAnimated();
    }
}