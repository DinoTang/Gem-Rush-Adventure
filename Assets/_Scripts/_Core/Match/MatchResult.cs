using System.Collections.Generic;
public enum MatchDirection
{
    Horizontal,
    Vertical
}
public class MatchResult
{
    public List<(int x, int y)> Cells = new();
    public MatchDirection MatchDirection;
}
