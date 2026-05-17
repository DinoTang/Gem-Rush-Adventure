using System;
using System.Collections;
using System.Collections.Generic;

public interface ISpecialComboPattern
{
    IEnumerator Execute(
    GemCtrl gemA,
    GemCtrl gemB,
    GridModel<GemCtrl> grid,
    Action<List<(int x, int y)>> onCompleted
);
}