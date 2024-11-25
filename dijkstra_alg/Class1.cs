using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace dijkstra_alg
{
    /// <summary>
    /// Graph with nodes and weighted edges.
    /// Provides: load data from csv + find shortest path.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Edge in the graph with a weight.
        /// </summary>
        public class Edge
        {
            /// <summary>
            /// Destination node of the edge.
            /// </summary>
            public string Destination {  get; set; }

            /// <summary>
            /// Weight of the edge. (Distance)
            /// </summary>
            public int Distance { get; set; }

            /// <summary>
            /// Init a new edhe.
            /// </summary>
            /// <param name="destination">Destination node of the edge.</param>
            /// <param name="distance">Weight of the edge.</param>
            public Edge(string destination, int distance)
            {
                Destination = destination;
                Distance = distance;
            }
        }

        /// <summary>
        /// Representation of the graph.
        /// </summary>
        private Dictionary<string, List<Edge>> _graph;

        /// <summary>
        /// Init a new instance of the Graph class.
        /// </summary>
        public Graph()
        {
            _graph = new Dictionary<string, List<Edge>>();
        }

        /// <summary>
        /// Loads a graph from a CSV file containing edges with weights.
        /// The CSV should have the following format:
        /// Origin,Destination,Distance
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <exception cref="FormatException">Thrown if the file contains invalid data.</exception>
        public void LoadFromCSV(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                // Header
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var parts = line.Split(',');
                
                    string origin = parts[0].Trim();
                    string destination = parts[1].Trim();
                    int distance = int.Parse(parts[2].Trim());

                    // Add edge to graph
                    if (!_graph.ContainsKey(origin))
                    {
                        _graph[origin] = new List<Edge>();
                    }
                    _graph[origin].Add(new Edge(destination, distance));
                }
            }
        }

        /// <summary>
        /// Finds the shortest path between two nodes using Dijkstra's algorithm.
        /// </summary>
        /// <param name="start">Starting node.</param>
        /// <param name="end">Destination node.</param>
        /// <returns>Tuple: List: cities (shortest path from start to end) and int (Total distance).</returns>
        public (List<string>, int) FindShortestPath(string start, string end)
        {
            var distances = new Dictionary<string, int>();
            var previousNodes = new Dictionary<string, string>();
            var priorityQueue = new SortedSet<(int Distance, string Node)>();

            // init distances and priority queue
            foreach (var node in _graph.Keys)
            {
                distances[node] = int.MaxValue;
                previousNodes[node] = null;
            }

            distances[start] = 0;
            priorityQueue.Add((0, start));

            while (priorityQueue.Count > 0)
            {
                // get the node with the smallest distance
                var current = priorityQueue.Min;
                priorityQueue.Remove(current);

                string currentNode = current.Node;
                
                // stop, if current node is the destination
                if (currentNode == end)
                {
                    break;
                }

                // process neighbors
                if (_graph.ContainsKey(currentNode))
                {
                    foreach (var edge in _graph[currentNode])
                    {
                        int newDistance = distances[currentNode] + edge.Distance;

                        if (newDistance < distances[edge.Destination])
                        {
                            // update priority queue
                            priorityQueue.Remove((distances[edge.Destination], edge.Destination));
                            distances[edge.Destination] = newDistance;
                            previousNodes[edge.Destination] = currentNode;
                            priorityQueue.Add((newDistance, edge.Destination));
                        }
                    }
                }
            }

            // create path
            var path = new List<string>();
            string currentPathNode = end;

            while (currentPathNode != null)
            {
                path.Insert(0, currentPathNode);
                currentPathNode = previousNodes[currentPathNode];
            }

            if (distances[end] == int.MaxValue)
                return (null, int.MaxValue);

            return (path, distances[end]);
        }
    }
}
