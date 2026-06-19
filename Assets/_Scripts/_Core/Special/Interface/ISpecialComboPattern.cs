using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecialComboPattern
{
    IEnumerator Execute(
    GemCtrl gemA,
    GemCtrl gemB,
    GridModel<GemCtrl> grid,
    Action<List<Vector2Int>> onCompleted
);
}