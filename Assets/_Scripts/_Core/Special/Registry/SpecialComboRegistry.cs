using System.Collections.Generic;

public class SpecialComboRegistry
{
    private Dictionary<(GemSpecialType, GemSpecialType), ISpecialComboPattern> patterns = new();

    public SpecialComboRegistry()
    {
        this.Init();
    }

    protected void Init()
    {
        this.patterns.Add(
            (GemSpecialType.HorizontalRocket, GemSpecialType.Cube),
             new CubeSpecialPattern(new RowRocketPattern()));

        this.patterns.Add(
            (GemSpecialType.Cube, GemSpecialType.HorizontalRocket),
             new CubeSpecialPattern(new RowRocketPattern()));

        this.patterns.Add(
            (GemSpecialType.VerticalRocket, GemSpecialType.Cube),
             new CubeSpecialPattern(new ColumnRocketPattern()));

        this.patterns.Add(
            (GemSpecialType.Cube, GemSpecialType.VerticalRocket),
             new CubeSpecialPattern(new ColumnRocketPattern()));

        this.patterns.Add(
       (GemSpecialType.Bomb, GemSpecialType.Cube),
        new CubeSpecialPattern(new BombPattern()));

        this.patterns.Add(
            (GemSpecialType.Cube, GemSpecialType.Bomb),
             new CubeSpecialPattern(new BombPattern()));
    }

    public ISpecialComboPattern GetPattern(GemSpecialType typeA, GemSpecialType typeB)
    {
        if (!this.patterns.ContainsKey((typeA, typeB)))
            return null;

        return this.patterns[(typeA, typeB)];
    }
}