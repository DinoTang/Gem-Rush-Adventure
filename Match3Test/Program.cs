using System;
using System.Collections.Generic;

class Program
{
    static GridModel grid;
    static MatchFinder matchFinder = new MatchFinder();
    static ResolveMatchResult matchResolver = new ResolveMatchResult();
    static Random rand = new Random();

    static PieceType[] types =
    {
        PieceType.Red,
        PieceType.Blue,
        PieceType.Green,
        PieceType.Yellow,
        PieceType.Purple
    };

    static void Main()
    {
        grid = new GridModel(8, 8);

        InitGrid();

        while (true)
        {
            PrintGrid();

            Console.WriteLine("Nhap swap: x1 y1 x2 y2 (hoac q de thoat)");
            string input = Console.ReadLine();

            if (input == "q")
                break;

            var parts = input.Split(' ');

            if (parts.Length != 4)
            {
                Console.WriteLine("Sai format!");
                continue;
            }

            int x1 = int.Parse(parts[0]);
            int y1 = int.Parse(parts[1]);
            int x2 = int.Parse(parts[2]);
            int y2 = int.Parse(parts[3]);

            bool valid;

            try
            {
                valid = TrySwap((x1, y1), (x2, y2));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if (!valid)
            {
                Console.WriteLine("Swap khong tao match -> revert!");
                continue;
            }

            var matches = matchFinder.FindMatches(grid);

            // Console.WriteLine($"Matches found: {matches.Count}");

            // foreach (var match in matches)
            // {
            //     Console.WriteLine("Match:");

            //     foreach (var cell in match.Cells)
            //     {
            //         Console.Write($"({cell.x},{cell.y}) ");
            //     }

            //     Console.WriteLine();
            // }
            matchResolver.ClearMatches(matches, grid);
        }
    }

    // ==========================
    // Spawn board không có match sẵn
    // ==========================
    static void InitGrid()
    {
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                PieceType type = GetSafeRandomPieceType(x, y);
                grid.Set(x, y, new Piece(type));
            }
        }
    }

    static PieceType GetSafeRandomPieceType(int x, int y)
    {
        List<PieceType> availableTypes = new List<PieceType>(types);

        RemoveHorizontalMatchCandidate(x, y, availableTypes);
        RemoveVerticalMatchCandidate(x, y, availableTypes);

        return availableTypes[rand.Next(availableTypes.Count)];
    }

    static void RemoveHorizontalMatchCandidate(
        int x,
        int y,
        List<PieceType> availableTypes)
    {
        if (x < 2)
            return;

        PieceType left1 = grid.Get(x - 1, y).pieceType;
        PieceType left2 = grid.Get(x - 2, y).pieceType;

        if (left1 == left2 && left1 != PieceType.None)
            availableTypes.Remove(left1);
    }

    static void RemoveVerticalMatchCandidate(
        int x,
        int y,
        List<PieceType> availableTypes)
    {
        if (y < 2)
            return;

        PieceType up1 = grid.Get(x, y - 1).pieceType;
        PieceType up2 = grid.Get(x, y - 2).pieceType;

        if (up1 == up2 && up1 != PieceType.None)
            availableTypes.Remove(up1);
    }

    static PieceType RandomType()
    {
        return types[rand.Next(types.Length)];
    }

    // ==========================
    // Swap hợp lệ nếu tạo match
    // ==========================
    static bool TrySwap((int x, int y) a, (int x, int y) b)
    {
        grid.Swap(a, b);

        var matches = matchFinder.FindMatches(grid);

        if (matches.Count == 0)
        {
            grid.Swap(a, b);
            return false;
        }

        return true;
    }

    // ==========================
    // In board debug
    // ==========================
    static void PrintGrid()
    {
        Console.WriteLine("\nGRID:\n");

        Console.Write("    ");

        for (int x = 0; x < grid.Width; x++)
        {
            Console.Write(x + " ");
        }

        Console.WriteLine();
        Console.WriteLine("   " + new string('-', grid.Width * 2));

        for (int y = 0; y < grid.Height; y++)
        {
            Console.Write(y + " | ");

            for (int x = 0; x < grid.Width; x++)
            {
                PieceType type = grid.Get(x, y).pieceType;

                SetColor(type);
                Console.Write(ToChar(type) + " ");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    static void SetColor(PieceType type)
    {
        switch (type)
        {
            case PieceType.Red:
                Console.ForegroundColor = ConsoleColor.Red;
                break;

            case PieceType.Blue:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;

            case PieceType.Green:
                Console.ForegroundColor = ConsoleColor.Green;
                break;

            case PieceType.Yellow:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;

            case PieceType.Purple:
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;

            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }
    }

    static char ToChar(PieceType type)
    {
        return type switch
        {
            PieceType.Red => 'R',
            PieceType.Blue => 'B',
            PieceType.Green => 'G',
            PieceType.Yellow => 'Y',
            PieceType.Purple => 'P',
            PieceType.None => '_'
        };
    }
}