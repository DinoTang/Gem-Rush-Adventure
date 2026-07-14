using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GemSpriteSO", menuName = "SO/GemSpriteSO")]
public class GemSpriteSO : ScriptableObject
{
    [SerializeField] private List<GemSpriteData> gemSprites = new();

    public Sprite GetSprite(GemType gemType, GemSpecialType gemSpecialType)
    {
        foreach (GemSpriteData data in gemSprites)
        {
            if (data.GemType != gemType) continue;
            if (data.GemSpecialType != gemSpecialType) continue;
            return data.Icon;
        }

        return null;
    }
}