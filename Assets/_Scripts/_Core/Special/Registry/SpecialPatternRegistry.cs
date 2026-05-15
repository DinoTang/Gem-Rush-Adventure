using System.Collections.Generic;

public class SpecialPatternRegistry
{
    private Dictionary<GemSpecialType, ISpecialPattern> patterns = new();

    public SpecialPatternRegistry()
    {
        this.Init();
    }

    protected void Init()
    {
        this.patterns.Add(GemSpecialType.HorizontalRocket, new RowRocketPattern());
        this.patterns.Add(GemSpecialType.VerticalRocket, new ColumnRocketPattern());
        this.patterns.Add(GemSpecialType.Bomb, new BombPattern());
        this.patterns.Add(GemSpecialType.Cube, new CubePattern());
    }

    public ISpecialPattern GetPattern(GemSpecialType type)
    {
        if (!this.patterns.ContainsKey(type))
            return null;

        return this.patterns[type];
    }
}