using System;
using System.Collections.Generic;
using System.IO;

class Domino
{
    public int Index1 { get; set; }
    public int Index2 { get; set; }
    public bool IsVertical { get; set; }

    public Domino(int index1, int index2, bool isVertical)
    {
        Index1 = index1;
        Index2 = index2;
        IsVertical = isVertical;
    }

    public override string ToString()
    {
        return IsVertical ? $"Dominó ({Index1}, {Index2}) Függőlegesen" : $"Dominó ({Index1}, {Index2}) Vízszintesen";
    }
}

class Grid
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public bool[] Cells { get; private set; } // Pálya cellái
    public List<(int, int)> Lines { get; private set; } // Vonalak listája
    public List<Domino> PlacedDominoes { get; private set; } // Elhelyezett dominók

    public Grid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new bool[rows * columns];
        Lines = new List<(int, int)>();
        PlacedDominoes = new List<Domino>();
    }

    public void AddLine(int index1, int index2)
    {
        Lines.Add((index1, index2));
    }

    public bool HasLine(int index1, int index2)
    {
        return Lines.Contains((index1, index2)) || Lines.Contains((index2, index1));
    }

    public bool CanPlaceDomino(int index1, int index2)
    {
        return !Cells[index1] && !Cells[index2] && !HasLine(index1, index2);
    }

    public void PlaceDomino(int index1, int index2, bool isVertical)
    {
        if (CanPlaceDomino(index1, index2))
        {
            Cells[index1] = true;
            Cells[index2] = true;
            PlacedDominoes.Add(new Domino(index1, index2, isVertical));
        }
    }

    public void ResetGrid()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = false;
        }
        PlacedDominoes.Clear();
    }

    public void DisplayGrid()
    {
        Console.WriteLine("  " + new string('-', Columns * 2));

        for (int i = 0; i < Rows; i++)
        {
            Console.Write("| ");
            for (int j = 0; j < Columns; j++)
            {
                int index = i * Columns + j;
                Console.Write(Cells[index] ? "X " : ". ");

                if (j + 1 < Columns && HasLine(index, index + 1))
                {
                    Console.Write("| ");
                }
                else
                {
                    Console.Write("  ");
                }
            }
            Console.WriteLine("|");

            if (i + 1 < Rows)
            {
                Console.Write("  ");
                for (int j = 0; j < Columns; j++)
                {
                    int index = i * Columns + j;
                    if (HasLine(index, index + Columns))
                    {
                        Console.Write("--");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    Console.Write("  ");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine("  " + new string('-', Columns * 2));
    }
}

class Game
{
    public Grid Grid { get; set; }

    public Game(string filename)
    {
        ReadFromFile(filename);
    }

    public void ReadFromFile(string filename)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string[] size = reader.ReadLine().Split(',');
                int rows = int.Parse(size[0]);
                int columns = int.Parse(size[1]);

                Grid = new Grid(rows, columns);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineParts = line.Split(',');
                    int index1 = int.Parse(lineParts[0].Trim()) - 1;
                    int index2 = int.Parse(lineParts[1].Trim()) - 1;
                    Grid.AddLine(index1, index2);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba a fájl beolvasásakor: {ex.Message}");
        }
    }

    public bool TryPlaceDominoes()
    {
        Grid.ResetGrid();
        return TryPlaceDominoRecursively(0);
    }

    private bool TryPlaceDominoRecursively(int currentIndex)
    {
        if (currentIndex >= Grid.Rows * Grid.Columns)
        {
            return true; // Success
        }

        int row = currentIndex / Grid.Columns;
        int col = currentIndex % Grid.Columns;

        // Horizontal domino placing
        if (col + 1 < Grid.Columns && Grid.CanPlaceDomino(currentIndex, currentIndex + 1))
        {
            Grid.PlaceDomino(currentIndex, currentIndex + 1, false);
            if (TryPlaceDominoRecursively(currentIndex + 2)) return true;
            Grid.Cells[currentIndex] = false;
            Grid.Cells[currentIndex + 1] = false;
        }

        // Vertical domino placing
        if (row + 1 < Grid.Rows && Grid.CanPlaceDomino(currentIndex, currentIndex + Grid.Columns))
        {
            Grid.PlaceDomino(currentIndex, currentIndex + Grid.Columns, true);
            if (TryPlaceDominoRecursively(currentIndex + 1)) return true;
            Grid.Cells[currentIndex] = false;
            Grid.Cells[currentIndex + Grid.Columns] = false;
        }

        
        return false;
    }

    public void DisplayResult()
    {
        if (TryPlaceDominoes())
        {
            Console.WriteLine("Sikeres dominó lefedés:");
            Grid.DisplayGrid();
            Console.WriteLine("Elhelyezett dominók:");
            foreach (var domino in Grid.PlacedDominoes)
            {
                Console.WriteLine(domino);
            }
        }
        else
        {
            Console.WriteLine("Nem sikerült dominókat elhelyezni.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game(@"/../map-configs/map-config-1.txt");
        game.DisplayResult();
    }
}
