using DG.Tweening;
using UnityEngine;

public class LoadingPanelSlideScaleAnim : PanelSlideScaleAnimation
{
    [Header("LoadingSprite")]
    [SerializeField] protected LoadingSpriteSO loadingSpriteSO;
    [SerializeField] protected HintLoadingImage hintLoadingImage;
    [SerializeField] protected HintLoadingText hintLoadingText;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadLoadingSpriteSO();
        this.LoadHintLoadingImage();
        this.LoadHintLoadingText();

        this.LoadingSprite();
    }

    protected void LoadLoadingSpriteSO()
    {
        if (this.loadingSpriteSO != null) return;
        this.loadingSpriteSO = Resources.Load<LoadingSpriteSO>("UI/LoadingSpriteSO");
    }

    protected void LoadHintLoadingImage()
    {
        if (this.hintLoadingImage != null) return;
        this.hintLoadingImage = GetComponentInChildren<HintLoadingImage>();
        Debug.Log(transform.name + ": LoadHintLoadingImage");
    }

    protected void LoadHintLoadingText()
    {
        if (this.hintLoadingText != null) return;
        this.hintLoadingText = GetComponentInChildren<HintLoadingText>();
        Debug.Log(transform.name + ": LoadHintLoadingText");
    }

    public void LoadingSprite()
    {
        LoadingSpriteData loadingSpriteData = loadingSpriteSO.loadingSprites[
       Random.Range(
           0,
           loadingSpriteSO.loadingSprites.Count
       )
   ];
        this.hintLoadingImage.Image.sprite = loadingSpriteData.LoadingImage;
        this.hintLoadingText.Image.sprite = loadingSpriteData.LoadingText;
    }
}