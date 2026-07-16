using DG.Tweening;
using UnityEngine;

public class PausePopupUI : BaseBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    public void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.zero;

        canvasGroup.DOFade(1f, 0.2f);
        panel.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f);
        panel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}