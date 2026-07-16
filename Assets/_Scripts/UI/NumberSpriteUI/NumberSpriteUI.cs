using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class NumberSpriteUI : BaseBehaviour
{
    [SerializeField] protected Image digitPrefab;
    [SerializeField] protected NumberSpriteSO numberSpriteSO;

    [SerializeField] protected readonly List<Image> digitImages = new();
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadDigitPrefab();
        this.LoadNumberSprites();
        this.digitPrefab.gameObject.SetActive(false);
    }
    protected virtual void LoadDigitPrefab()
    {
        if (this.digitPrefab != null) return;

        this.digitPrefab = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": Load Digit Prefab", gameObject);
    }

    protected abstract void LoadNumberSprites();

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
            this.digitImages[i].sprite = this.numberSpriteSO.digitSprites[digitValue];
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
        if (this.numberSpriteSO == null || this.numberSpriteSO.digitSprites.Length != 10)
        {
            Debug.LogWarning(
                $"{name}: Number Sprites phải có đúng 10 sprite từ 0 đến 9.",
                this
            );
        }
    }
}