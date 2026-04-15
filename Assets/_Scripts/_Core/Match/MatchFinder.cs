using System.Collections.Generic;

public class MatchFinder
{
    public List<MatchResult> FindMatches(GridModel grid)
    {
        var results = new List<MatchResult>();

        // 🔹 Check ngang
        for (int y = 0; y < grid.Height; y++)
        {
            int count = 1;

            for (int x = 1; x < grid.Width; x++)
            {
                var current = grid.Get(x, y).pieceType;
                var prev = grid.Get(x - 1, y).pieceType;

                if (current == prev && current != PieceType.None)
                {
                    count++;

                    if (x == grid.Width - 1 && count >= 3)
                        results.Add(CreateHorizontal(x, y, count));
                }
                else
                {
                    if (count >= 3)
                        results.Add(CreateHorizontal(x - 1, y, count));

                    count = 1;
                }
            }
        }

        // 🔹 Check dọc
        for (int x = 0; x < grid.Width; x++)
        {
            int count = 1;

            for (int y = 1; y < grid.Height; y++)
            {
                var current = grid.Get(x, y).pieceType;
                var prev = grid.Get(x, y - 1).pieceType;

                if (current == prev && current != PieceType.None)
                {
                    count++;

                    if (y == grid.Height - 1 && count >= 3)
                        results.Add(CreateVertical(x, y, count));
                }
                else
                {
                    if (count >= 3)
                        results.Add(CreateVertical(x, y - 1, count));

                    count = 1;
                }
            }
        }

        return results;
    }

    private MatchResult CreateHorizontal(int endX, int y, int count)
    {
        var match = new MatchResult();

        for (int i = 0; i < count; i++)
            match.Cells.Add((endX - i, y));

        return match;
    }

    private MatchResult CreateVertical(int x, int endY, int count)
    {
        var match = new MatchResult();

        for (int i = 0; i < count; i++)
            match.Cells.Add((x, endY - i));

        return match;
    }
}