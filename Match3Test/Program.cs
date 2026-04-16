using System;
using System.Collections.Generic;
using System.Diagnostics;
class Program
{
    static GridModel grid;
    static MatchFinder matchFinder = new MatchFinder();
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

            if (input == "q") break;

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
                Console.WriteLine("Swap khong tao match → revert!");
                continue;
            }
            var matches = matchFinder.FindMatches(grid);

            Console.WriteLine($"Matches found: {matches.Count}");
            foreach (var m in matches)
            {
                Console.WriteLine("Match:");
                foreach (var c in m.Cells)
                    Console.Write($"({c.x},{c.y}) ");
                Console.WriteLine();
            }
        }
    }

    static void InitGrid()
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                grid.Set(x, y, new Piece(RandomType()));
            }
        }
    }

    static PieceType RandomType()
    {
        return types[rand.Next(types.Length)];
    }

    // =======================
    // 🔥 PRINT GRID (CÓ MÀU)
    // =======================
    static void PrintGrid()
    {
        Console.WriteLine("\nGRID:\n");

        // ======================
        // In header cột (x-axis)
        // ======================
        Console.Write("    "); // offset cho cột số

        for (int x = 0; x < grid.Width; x++)
        {
            Console.Write(x + " ");
        }

        Console.WriteLine();

        Console.WriteLine("   " + new string('-', grid.Width * 2));

        // ======================
        // In từng hàng + số hàng (y-axis)
        // ======================
        for (int y = 0; y < grid.Height; y++)
        {
            Console.Write(y + " | ");

            for (int x = 0; x < grid.Width; x++)
            {
                var p = grid.Get(x, y).pieceType;

                SetColor(p);
                Console.Write(ToChar(p) + " ");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }


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

    static void SetColor(PieceType t)
    {
        switch (t)
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

    static char ToChar(PieceType t)
    {
        return t switch
        {
            PieceType.Red => 'R',
            PieceType.Blue => 'B',
            PieceType.Green => 'G',
            PieceType.Yellow => 'Y',
            PieceType.Purple => 'P',
            _ => '.'
        };
    }
}