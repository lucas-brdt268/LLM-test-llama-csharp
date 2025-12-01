using System;

namespace LLM_Test
{
    class Program
    {
        private static LlamaLLMService? _demoService;
        private static LlamaSharpLLMService? _productionService;
        private static bool _useProductionMode = false;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Llama LLM Text Generator ===");
            Console.WriteLine("Welcome to the Llama LLM Application!");
            
            // Check command line arguments for production mode
            if (args.Length > 0 && args[0].ToLower() == "--production")
            {
                _useProductionMode = true;
                Console.WriteLine("Starting in PRODUCTION mode with LLamaSharp...");
            }
            else
            {
                Console.WriteLine("Starting in DEMO mode...");
                Console.WriteLine("To use production mode, run: dotnet run -- --production");
            }
            
            Console.WriteLine("Type your questions or prompts, and I'll generate responses.");
            Console.WriteLine("Type 'exit' or 'quit' to close the application.");
            Console.WriteLine("Press Ctrl+C to exit...\n");

            try
            {
                bool initialized = false;
                
                if (_useProductionMode)
                {
                    // Initialize production LLM service
                    _productionService = new LlamaSharpLLMService();
                    initialized = await _productionService.InitializeAsync();
                    
                    if (initialized)
                    {
                        Console.WriteLine("Production LLM service ready! Start typing your questions:\n");
                    }
                }
                else
                {
                    // Initialize demo LLM service
                    _demoService = new LlamaLLMService();
                    initialized = await _demoService.InitializeAsync();
                    
                    if (initialized)
                    {
                        Console.WriteLine("Demo LLM service ready! Start typing your questions:\n");
                    }
                }
                
                if (!initialized)
                {
                    Console.WriteLine("Failed to initialize LLM service. Exiting...");
                    return;
                }

                // Main interaction loop
                while (true)
                {
                    Console.Write("You: ");
                    string? input = Console.ReadLine();
                    
                    if (input == null || input.ToLower() == "exit" || input.ToLower() == "quit")
                    {
                        Console.WriteLine("Goodbye!");
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Please enter a valid question or prompt.\n");
                        continue;
                    }

                    // Generate response using appropriate service
                    Console.Write("LLM: ");
                    string response;
                    
                    if (_useProductionMode && _productionService != null)
                    {
                        response = await _productionService.GenerateTextSync(input); // Use sync method
                    }
                    else if (_demoService != null)
                    {
                        response = await _demoService.GenerateTextAsync(input);
                    }
                    else
                    {
                        response = "Error: No LLM service available.";
                    }
                    
                    Console.WriteLine(response);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            finally
            {
                // Clean up resources
                _demoService?.Dispose();
                _productionService?.Dispose();
            }
        }
    }
}
