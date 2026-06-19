using System.Collections.Generic;
using UnityEngine;

public interface ISpecialPattern
{
    public List<Vector2Int> GetCells(
        GemCtrl gem,
        GridModel<GemCtrl> grid
    );
}