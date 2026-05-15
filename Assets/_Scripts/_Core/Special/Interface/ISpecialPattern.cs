using System.Collections.Generic;

public interface ISpecialPattern
{
    public List<(int x, int y)> GetCells(
        GemCtrl gem,
        GridModel<GemCtrl> grid
    );
}