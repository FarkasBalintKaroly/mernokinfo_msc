using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Domino_Project_v3
{

    class DominoBruteForce
    {
        // map sizes
        static int n; // map height
        static int m; // map width

        // map in 1D
        static bool[] map;

        // Domino binary array
        static bool[] dominos;

        // Lines
        static List<(int, int)> lines = new List<(int, int)>();

        static void Main(string[] args)
        {
            /// <summary>
            /// Main function for solving domino task with brute force.
            /// </summary>
            
            // Reading input file -- map and lines
            ReadFromFile(@"../../map-configs/map-config-2.txt");
            
            map = new bool[n * m];
            dominos = new bool[(n * m) / 2];

            // Trying combinations
            while (true)
            {
                // Initialize map
                Init_map();

                // Try place dominos
                bool success_placing = Place_dominos();

                if (success_placing)
                {
                    Console.WriteLine("Sikeres lefedés:");
                    Draw_map();
                    Write_domino_status();
                    break;
                }

                // Increment domino array
                if (!Increment())
                {
                    Console.WriteLine("Nincs több kombináció, nem találtunk megoldást.");
                    break;
                }
            }
            Console.ReadLine();
        }

        
        static void ReadFromFile(string filename)
        {
            /// <summary>
            /// Reading map and lines from file.
            /// </summary>
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    // n and m
                    string[] size = reader.ReadLine().Split(',');
                    n = int.Parse(size[0]);
                    m = int.Parse(size[1]);

                    // Vonalak beolvasása
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] vonal = line.Split(',');
                        int index1 = int.Parse(vonal[0].Trim()) - 1; // -1, mert 1 alapú számozást feltételezünk
                        int index2 = int.Parse(vonal[1].Trim()) - 1; // -1, mert 1 alapú számozást feltételezünk
                        lines.Add((index1, index2));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a fájl beolvasása során: {ex.Message}");
            }
        }

        
        static void Init_map()
        {
            /// <summary>
            /// Initialize map in every round.
            /// </summary>
            
            for (int i = 0; i < n * m; i++)
            {
                map[i] = false;
            }
        }

    
        static bool Place_dominos()
        {
            /// <summary>
            /// Try placing dominos.
            /// </summary>
            /// <return>True if success or False if not success.</return>
            
            int domino_index = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int index = i * m + j; // 2D to 1D

                    if (!map[index])
                    {
                        if (domino_index >= dominos.Length)
                            return false;

                        if (dominos[domino_index] == false)
                        {
                            // Horizontal domino placing
                            if (j + 1 < m && !map[index + 1] && !There_is_line(index, index + 1))
                            {
                                map[index] = true;
                                map[index + 1] = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // Vertical domino placing
                            if (i + 1 < n && !map[index + m] && !There_is_line(index, index + m))
                            {
                                map[index] = true;
                                map[index + m] = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        domino_index++;
                    }
                }
            }
            return true;
        }

        
        static bool Increment()
        {
            /// <summary>
            /// Increment binary counter.
            /// </summary>
            for (int i = dominos.Length - 1; i >= 0; i--)
            {
                if (dominos[i] == false)
                {
                    dominos[i] = true;
                    return true;
                }
                else
                {
                    dominos[i] = false;
                }
            }
            return false;
        }

        
        static bool There_is_line(int index1, int index2)
        {
            /// <summary>
            /// Check if there is line.
            /// </summary>
            /// <return></return>
            return lines.Contains((index1, index2)) || lines.Contains((index2, index1));
        }

        
        static void Draw_map()
        {
            /// <summary>
            /// Drawing map with ascii characters.
            /// </summary>
            
            // Top
            Console.Write("  ");
            for (int j = 0; j < m; j++)
            {
                Console.Write("--");
            }
            Console.WriteLine();

            // map
            for (int i = 0; i < n; i++)
            {
                Console.Write("| ");
                for (int j = 0; j < m; j++)
                {
                    int index = i * m + j;
                    Console.Write(map[index] ? "X " : ". ");

                    // lines
                    if (j + 1 < m && There_is_line(index, index + 1))
                    {
                        Console.Write("| ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine("|");

                // vertical lines
                if (i + 1 < n)
                {
                    Console.Write("  ");
                    for (int j = 0; j < m; j++)
                    {
                        int index = i * m + j;
                        
                        if (There_is_line(index, index + m))
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

            // Bottom
            Console.Write("  ");
            for (int j = 0; j < m; j++)
            {
                Console.Write("--");
            }
            Console.WriteLine();
        }

        
        static void Write_domino_status()
        {
            /// <summary>
            /// Print out domino status. (Horizontal or Vertical)
            /// </summary>
            Console.WriteLine("Dominók elhelyezése:");
            for (int i = 0; i < dominos.Length; i++)
            {
                string status = dominos[i] ? "Függőleges" : "Vízszintes";
                Console.WriteLine($"{i + 1}.: {status}");
            }
        }
    }
}