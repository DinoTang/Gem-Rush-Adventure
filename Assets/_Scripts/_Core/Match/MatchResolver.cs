// using System.Collections.Generic;

// public class ResolveMatchResult
// {
//     public List<(int x, int y)> GetCellsToClear(List<MatchResult> matches)
//     {
//         HashSet<(int x, int y)> cellsToClear = new();

//         foreach (var match in matches)
//         {
//             foreach (var cell in match.Cells)
//             {
//                 cellsToClear.Add((cell.x, cell.y));
//             }
//         }

//         List<(int x, int y)> result = new List<(int x, int y)>(cellsToClear);

//         return result;
//     }

//     public void ClearMatches(List<MatchResult> matches, GridModel grid)
//     {
//         List<(int x, int y)> result = GetCellsToClear(matches);
//         result.Sort();

//         foreach (var cell in result)
//         {
//             grid.Set(cell.x, cell.y, new Piece(PieceType.None));
//         }
//     }
// }