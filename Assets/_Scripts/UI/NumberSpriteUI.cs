using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberSpriteUI : BaseBehaviour
{
    [SerializeField] private Image digitPrefab;
    [SerializeField] private NumberSpriteSO targetNumberSprites;

    [SerializeField] private readonly List<Image> digitImages = new();
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadDigitPrefab();
        this.LoadTargetNumberSprites();
        this.digitPrefab.gameObject.SetActive(false);
    }
    protected virtual void LoadDigitPrefab()
    {
        if (this.digitPrefab != null) return;

        this.digitPrefab = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": Load Digit Prefab", gameObject);
    }

    protected virtual void LoadTargetNumberSprites()
    {
        if (this.targetNumberSprites != null) return;

        this.targetNumberSprites = Resources.Load<NumberSpriteSO>("NumberSpriteSO/TargetNumberSprites");
        Debug.Log(transform.name + ": Load Target Number Sprites", gameObject);
    }

    public void SetNumber(int value)
    {
        value = Mathf.Max(0, value);

        string numberText = value.ToString();

        this.EnsureDigitCount(numberText.Length);

        for (int i = 0; i < this.digitImages.Count; i++)
        {
            bool shouldShow = i < numberText.Length;
            this.digitImages[i].gameObject.SetActive(shouldShow);

            if (!shouldShow)
                continue;

            int digitValue = numberText[i] - '0';
            this.digitImages[i].sprite = this.targetNumberSprites.digitSprites[digitValue];
        }
    }

    private void EnsureDigitCount(int requiredCount)
    {
        while (this.digitImages.Count < requiredCount)
        {
            Image newDigit = Instantiate(this.digitPrefab, transform);
            newDigit.gameObject.SetActive(true);

            this.digitImages.Add(newDigit);
        }
    }

    private void OnValidate()
    {
        if (this.targetNumberSprites == null || this.targetNumberSprites.digitSprites.Length != 10)
        {
            Debug.LogWarning(
                $"{name}: Target Number Sprites phải có đúng 10 sprite từ 0 đến 9.",
                this
            );
        }
    }
}