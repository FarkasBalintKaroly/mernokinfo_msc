using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino_project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Creating map for dominos
            Console.WriteLine("Adja meg a pálya méretét:");
            int map_size;
            while (!Int32.TryParse(Console.ReadLine(), out map_size));
            int[] map = new int[map_size];
            zeroing_map(map);
            write_map(map);

            // Algorithm for trying place dominos
            while (true)
            {
                int can_place = place_dominos(map);
                if (can_place == -1) break;
            }

            write_map(map);

            // Eliminate success
            if (map.Sum()==map_size)
            {
                Console.WriteLine("Siker");
            }
            else
            {
                Console.WriteLine("Nem sikerült");
            }
            Console.ReadLine();
            
        }

        private static int place_dominos(int[] map)
        {
            // function to place dominos
            for (int i = 0; i < map.Length-1; i++)
            {
                // Check if we can place
                if (map[i] == 0 && map[i+1] == 0)
                {
                    map[i] = 1; map[i+1] = 1;
                    return i;
                }
                write_map(map);
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

        private static void zeroing_map(int[] map)
        {
            // function to initialize map
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = 0;
            }
        }
    }
}
