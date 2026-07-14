using System;
using UnityEngine;

[Serializable]
public class GemSpriteData
{
    [SerializeField] private GemType gemType;
    [SerializeField] private GemSpecialType gemSpecialType;
    [SerializeField] private Sprite icon;

    public GemType GemType => gemType;
    public GemSpecialType GemSpecialType => gemSpecialType;
    public Sprite Icon => icon;
}