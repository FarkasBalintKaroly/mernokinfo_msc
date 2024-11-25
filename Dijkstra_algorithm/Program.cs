using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dijkstra_alg;

namespace Dijkstra_algorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph();

            string filePath = @"../../datasets/indian-cities-dataset.csv";
            graph.LoadFromCSV(filePath);

            Console.WriteLine("Adja meg a kiindulási várost:");
            string startingCity = Console.ReadLine();

            Console.WriteLine("Adja meg az úticélt:");
            string endCity = Console.ReadLine();

            var (shortestPath, shortestDistance) = graph.FindShortestPath(startingCity, endCity);

            if (shortestPath != null)
            {
                Console.WriteLine($"Legrövidebb út: {string.Join(" -> ", shortestPath)}");
                Console.WriteLine($"Legrövidebb út hossza: {shortestDistance} km");
            }
            else
            {
                Console.WriteLine("Nincs ilyen út");
            }
            Console.ReadLine();
        }
    }
}
