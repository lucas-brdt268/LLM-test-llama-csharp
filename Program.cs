using System;

namespace LLM_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Welcome to your C# project!");
            Console.WriteLine("Press Ctrl+C to exit...");
            try{
                while (true)
                {
                    string input = Console.ReadLine();
                    // INSERT_YOUR_CODE
                    if (input == null)
                    {
                        Console.WriteLine("Ctrl+C detected. Exiting...");
                        break;
                    }
                    Console.WriteLine("You entered: " + input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
