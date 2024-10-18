using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lengyel_forma
{
    class EquationReader
    {
        /// <summary>
        /// Reading equations from file.
        /// </summary>
        /// <param name="filepath">The path to the file.</param>
        /// <returns>A string array of equations.</returns>
        public static string[] ReadEquationsFromFile(string filepath)
        {
            try
            {
                return File.ReadAllLines(filepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the file: {ex.Message}");
                return null;
            }
        }
    }

    class InfixToPostfixConverter
    {
        /// <summary>
        /// Converts an infix expression to postfix.
        /// </summary>
        /// <param name="infix">The infix expression.</param>
        /// <returns>The postfix expression.</returns>
        public static string Convert(string infix)
        {
            Stack<string> operators = new Stack<string>();
            string postfix = "";

            string[] chars = infix.Split(' ');

            foreach (string character in chars)
            {
                // Ha nem szám
                if (int.TryParse(character, out int number))
                {
                    postfix += character + " ";
                }
                
                // Ha nyitó zárójel
                else if (character == "(")
                {
                    operators.Push(character);
                }

                // Ha csukó zárójel
                else if (character == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        postfix += operators.Pop() + " ";
                    }
                    operators.Pop();
                }

                // Sorrend figyelése
                else if (IsOperator(character))
                {
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(character))
                    {
                        postfix += operators.Pop() + " ";
                    }
                    operators.Push(character);
                }
            }

            // Minden megmaradt operátort ki
            while (operators.Count > 0)
            {
                postfix += operators.Pop() + " ";
            }

            return postfix.Trim();
        }

        /// <summary>
        /// If string is an operator.
        /// </summary>
        /// <param name="character">The string to check.</param>
        /// <returns>True if the string is an operator; otherwise, false.</returns>
        private static bool IsOperator(string character)
        {
            return character == "+" || character == "-" || character == "*" || character == "/";
        }

        /// <summary>
        /// Determines the precedence of an operator.
        /// </summary>
        /// <param name="op">The operator as a string.</param>
        /// <returns>The precedence level as an integer.</returns>
        private static int Precedence(string op)
        {
            if (op == "+" || op == "-")
                return 1;
            if (op == "*" || op == "/")
                return 2;
            return 0;
        }
    }

    class PostfixEvaluator
    {
        /// <summary>
        /// Evaluates a postfix expression.
        /// </summary>
        /// <param name="postfix">The postfix expression as a string.</param>
        /// <returns>The result of the evaluation.</returns>
        public static int Evaluate(string postfix)
        {
            Stack<int> stack = new Stack<int>();

            string[] characters = postfix.Split(' ');

            foreach (string character in characters)
            {
                if (int.TryParse(character, out int number))
                {
                    stack.Push(number);
                }
                else if (IsOperator(character))
                {
                    int operand2 = stack.Pop();
                    int operand1 = stack.Pop();
                    int result = ApplyOperator(character, operand1, operand2);
                    stack.Push(result);
                }
            }

            return stack.Pop();
        }

        /// <summary>
        /// Applies the operator to two operands.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <returns>The result of the operation.</returns>
        private static int ApplyOperator(string op, int operand1, int operand2)
        {
            switch (op)
            {
                case "+": return operand1 + operand2;
                case "-": return operand1 - operand2;
                case "*": return operand1 * operand2;
                case "/": return operand1 / operand2;
                default: throw new ArgumentException("Invalid operator");
            }
        }

        /// <summary>
        /// Determines if a string is an operator.
        /// </summary>
        /// <param name="token">The string to check.</param>
        /// <returns>True if the string is an operator; otherwise, false.</returns>
        private static bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // File path
            string filepath = @"../../equations.txt";

            string[] equations = EquationReader.ReadEquationsFromFile(filepath);

            if (equations != null && equations.Length > 0)
            {
                Console.WriteLine($"{equations.Length} egyenlet van a file-ban.");

                Console.Write("Melyik egyenletet oldjuk meg (1 to {0}):", equations.Length);
                if (int.TryParse(Console.ReadLine(), out int equationNumber) && equationNumber >= 1 && equationNumber <= equations.Length)
                {
                    string selectedEquation = equations[equationNumber - 1];

                    Console.WriteLine($"Eredeti egyenlet: {selectedEquation}");

                    // Lengyel formára alakítás
                    string postfix = InfixToPostfixConverter.Convert(selectedEquation);
                    Console.WriteLine($"Postfix forma (Lengyel forma): {postfix}");

                    // Postfix kifejezés kiértékelése
                    int result = PostfixEvaluator.Evaluate(postfix);
                    Console.WriteLine($"Végeredmény: {result}");
                }
                else
                {
                    Console.WriteLine("Nem létezik ilyen számú egyenlet.");
                }
            }
            else
            {
                Console.WriteLine("Nem tudtam beolvasni a file-ból az egyenletet.");
            }
            Console.ReadLine();
        }
    }
}