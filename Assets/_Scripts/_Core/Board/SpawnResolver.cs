using System;
using System.Collections.Generic;

public class SpawnResolver
{
    static Random random = new Random();
    static PieceType[] types =
    {
        PieceType.Red,
        PieceType.Blue,
        PieceType.Green,
        PieceType.Yellow,
        PieceType.Purple
    };
    static PieceType GetRandomPieceType()
    {
        return types[new Random().Next(types.Length)];
    }

    public void FillEmptyCells(GridModel grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Get(x, y).pieceType == PieceType.None)
                    grid.Set(x, y, new Piece(GetRandomPieceType()));
            }
        }
    }
}