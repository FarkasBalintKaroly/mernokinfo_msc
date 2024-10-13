using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack_project_v1
{
    internal class Backpack_
    {

        class Item
        {
            /// <summary>
            /// An item with a value and a weight.
            /// </summary>
            public int Value { get; set; }

            public int Weight { get; set; }

            public Item(int value, int weight)
            {
                /// <summary>
                /// Init a new item.
                /// </summary>
                /// <param name="value">Value of the item.</param>
                /// <param name="weight">Weight of the item.</param>
                Value = value;
                Weight = weight;
            }

            public override string ToString()
            {
                /// <summary>
                /// Returns a string representation of the item.
                /// </summary>
                /// <returns>A string describing the value and weight of the item.</returns>
                return $"Value = {Value}, Weight = {Weight}";
            }
        }



        class Backpack
        {
            /// <summary>
            /// Backpack wich contains items and max capacity
            /// </summary>

            public List<Item> Items { get; set; }

            public int Capacity { get; set; }

            public Backpack(int capacity)
            {
                /// <summary>
                /// Init a new instance of Backpack.
                /// </summary>
                /// <param name="capacity">Max capacity of the poor slave who will take the backpack.</param>
                Capacity = capacity;
                Items = new List<Item>();
            }

            public int SolveBackpack(out List<Item> selectedItems)
            {
                /// <summary>
                /// Solves Backpack problem.
                /// </summary>
                /// <param name="selectedItems">Outputs the list of selected items.</param>
                /// <return>Max value what can we achieve.</return>
                
                int n = Items.Count;

                // DP table init
                int[,] dp = new int[n + 1, Capacity + 1];

                // Fill the DP table
                for (int i = 0; i <= n; i++)
                {
                    for (int w = 0; w <= Capacity; w++)
                    {
                        if (i == 0 || w == 0)
                        {
                            dp[i, w] = 0;
                        }
                        else if (Items[i - 1].Weight <= w)
                        {
                            dp[i, w] = Math.Max(Items[i - 1].Value + dp[i - 1, w - Items[i - 1].Weight], dp[i - 1, w]);
                        }
                        else
                        {
                            dp[i, w] = dp[i - 1, w];
                        }
                    }
                }

                // Find selected items
                selectedItems = new List<Item>();
                int remainingCapacity = Capacity;

                for (int i = n; i > 0 && remainingCapacity > 0; i--)
                {
                    if (dp[i, remainingCapacity] != dp[i - 1, remainingCapacity])
                    {
                        selectedItems.Add(Items[i - 1]);
                        remainingCapacity -= Items[i - 1].Weight;
                    }
                }

                // Return the maximum value
                return dp[n, Capacity];
            }
        }


        class BackpackConfigReader
        {
            /// <summary>
            /// The file reader to read backpack config files.
            /// </summary>

            public static Backpack ReadFromFile(string filepath)
            {
                /// <summary>
                /// Reads the selected config file.
                /// </summary>
                /// <param name="filepath">The path for the txt file.</param>
                /// <return>A Backpack object containing items and capacity.</return>
                try
                {
                    string[] lines = File.ReadAllLines(filepath);

                    // First line: values
                    string[] valuesString = lines[0].Split(',');
                    int[] values = Array.ConvertAll(valuesString, int.Parse);

                    // Second line: weights
                    string[] weightsString = lines[1].Split(',');
                    int[] weights = Array.ConvertAll(weightsString, int.Parse);

                    // Third line: capacity
                    int capacity = int.Parse(lines[2].Trim());

                    // Create a Backpack object with the specified capacity
                    Backpack backpack = new Backpack(capacity);

                    // Add items to the backpack
                    for (int i = 0; i < values.Length; i++)
                    {
                        backpack.Items.Add(new Item(values[i], weights[i]));
                    }

                    return backpack;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading the file: {ex.Message}");
                    return null;
                }
            }
        }

        class Program
        {
            /// <summary>
            /// The main method where the program starts execution.
            /// </summary>
            
            static void Main(string[] args)
            {
                // Select the config file
                Console.Write("Melyik config file legyen az aktuális:");
                string num_of_config_file = Console.ReadLine();
                string filepath = @"../../backpack_configs/backpack_config_<here>.txt";
                filepath = filepath.Replace("<here>", num_of_config_file);

                // Read the backpack configuration from the file
                Backpack backpack = BackpackConfigReader.ReadFromFile(filepath);

                if (backpack != null)
                {
                    // Solve the knapsack problem and display the result
                    List<Item> selectedItems;
                    int max_value = backpack.SolveBackpack(out selectedItems);

                    // Display the maximum value
                    Console.WriteLine("The maximum value that can be achieved is: " + max_value);

                    // Display the selected items
                    Console.WriteLine("Selected items:");
                    foreach (var item in selectedItems)
                    {
                        Console.WriteLine(item);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to read the file.");
                }
            Console.ReadLine();
            }
            
        }

    }

}

