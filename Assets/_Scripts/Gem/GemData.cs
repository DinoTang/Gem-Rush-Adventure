using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemData : GemAbstract
{
    [SerializeField] private Vector2Int gridPos;
    public Vector2Int GridPos => gridPos;
    [SerializeField] protected GemType gemType;
    public GemType GemType => gemType;
    [SerializeField] protected GemSpecialType gemSpecialType;
    public GemSpecialType GemSpecialType => gemSpecialType;
    [SerializeField] protected bool isSelected = false;
    public bool IsSelected => isSelected;
    public void SetGridPos(int x, int y)
    {
        gridPos = new Vector2Int(x, y);
    }
    public void SetGemType(GemType gemType)
    {
        this.gemType = gemType;
    }
    public virtual void SetGemSpecialType(GemSpecialType gemSpecialType)
    {
        this.gemSpecialType = gemSpecialType;
    }
    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public void SetData(GemType type, int x, int y, GemSpecialType specialType = GemSpecialType.None)
    {
        this.SetGemType(type);
        this.SetGemSpecialType(specialType);
        this.SetGridPos(x, y);
    }
    public void ResetData()
    {
        this.SetData(GemType.None, -1, -1);
        this.isSelected = false;
    }
}
