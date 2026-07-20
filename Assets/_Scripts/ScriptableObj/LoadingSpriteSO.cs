using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadingSpriteSO", menuName = "SO/LoadingSpriteSO")]
public class LoadingSpriteSO : ScriptableObject
{
    public List<LoadingSpriteData> loadingSprites = new();
}