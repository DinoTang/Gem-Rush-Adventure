using System.Collections.Generic;

public class GravityResolver
{
    // public void ApplyGravity(GridModel grid)
    // {
    //     for (int column = 0; column < grid.Width; column++)
    //     {
    //         for (int emptyRow = grid.Height - 1; emptyRow >= 0; emptyRow--)
    //         {
    //             if (grid.Get(column, emptyRow).pieceType != PieceType.None) continue;

    //             for (int sourceRow = emptyRow - 1; sourceRow >= 0; sourceRow--)
    //             {
    //                 if (grid.Get(column, sourceRow).pieceType == PieceType.None) continue;

    //                 PieceType fallingType = grid.Get(column, sourceRow).pieceType;

    //                 grid.Set(column, emptyRow, new Piece(fallingType));
    //                 grid.Set(column, sourceRow, new Piece(PieceType.None));

    //                 break;
    //             }
    //         }
    //     }
    // }
}