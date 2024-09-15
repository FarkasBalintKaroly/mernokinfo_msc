using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino_Project_v2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create map from file
            string file_path = @"../../map-configs/map-config-2.txt";

            (int[] map, int[][] lines) = ProcessFile(file_path);

            Console.WriteLine("Kiinduló állapot:");
            draw_map(map);
            write_map(map);


            while (true)
            {
                int can_place = place_dominos(map, lines);
                if (can_place == -1) break;
            }

            eliminate(map);


            Console.ReadLine();
        }

        private static void eliminate(int[] map)
        {
            if (map.Sum() == map.Length)
            {
                Console.WriteLine("Siker");
            }
            else
            {
                Console.WriteLine("Nem sikerült");
            }

            Console.WriteLine("A játéktér:");
            draw_map(map);
            write_map(map);
        }

        private static void draw_map(int[] map)
        {
            // Drawing map with ascii characters
            for (int i = 0; i < map.Length; i++)
            {
                Console.Write("+---+");
            }
            Console.WriteLine("");

            for (int i = 0; i < map.Length; i++)
            {
                if (map[i] == 1)
                {
                    Console.Write("| * |");
                }
                else
                {
                    Console.Write("|   |");
                }
            }
            Console.WriteLine("");

            for (int i = 0; i < map.Length; i++)
            {
                Console.Write("+---+");
            }
            Console.WriteLine("");
        }

        private static int place_dominos(int[] map, int[][] lines)
        {
            int border;
            for (int i  = 0; i < lines.Length+1; i++)
            {
                if (i < lines.Length)
                {
                    border = lines[i][0];
                }
                else
                {
                    border = map.Length;
                }
                

                for (int j = 0; j < map.Length-1; j++)
                {
                    if (map[j] == 0 && map[j + 1] == 0 && j != border-1)
                    {
                        map[j] = 1; map[j + 1] = 1;
                        if (j > border || j == border)
                        {
                            break;
                        }
                    }
                }
            }
            return -1;
        }

        private static void write_map(int[] map)
        {
            // function for write down the actual state of the map
            for (int i = 0; i < map.Length; i++)
            {
                Console.Write(map[i]);
            }
            Console.WriteLine("");
        }

        private static (int[] map, int[][] lines) ProcessFile(string file_path)
        {
            // Function to process file and create arrays
            string[] read_lines = File.ReadAllLines(file_path);

            // Extract map size and create an array
            int map_size = int.Parse(read_lines[0]);
            int[] map = new int[map_size];
            for (int i = 0; i < map_size; i++)
            {
                map[i] = 0;
            }

            // Extract line positions and create an array
            List<int[]> lines = new List<int[]>();

            for (int i = 1; i <= read_lines.Length-1; i++)
            {
                string[] parts = read_lines[i].Split(',');

                int firstNumber = int.Parse(parts[0].Trim());
                int secondNumber = int.Parse(parts[1].Trim());

                lines.Add(new int[] { firstNumber, secondNumber });
            }

            return (map.ToArray(), lines.ToArray());
        }
    }
}
