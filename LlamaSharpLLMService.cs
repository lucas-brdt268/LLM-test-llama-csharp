using LLama;
using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace LLM_Test
{
    public class LlamaSharpLLMService : IDisposable
    {
        private LLamaWeights? _weights;
        private LLamaContext? _context;
        private InteractiveExecutor? _executor;
        private ChatSession? _session;
        private readonly string _modelPath;
        private readonly int _maxTokens;
        private readonly float _temperature;
        private readonly int _contextSize;
        private readonly int _gpuLayerCount;
        private readonly int _threadCount;
        private static bool _nativeLibraryInitialized = false;

        public LlamaSharpLLMService(
            string modelPath = "models", 
            int maxTokens = 256, 
            float temperature = 0.7f,
            int contextSize = 2048,
            int gpuLayerCount = 0,
            int threadCount = 0) // 0 = auto-detect
        {
            _modelPath = modelPath;
            _maxTokens = maxTokens;
            _temperature = temperature;
            _contextSize = contextSize;
            _gpuLayerCount = gpuLayerCount;
            _threadCount = threadCount == 0 ? Environment.ProcessorCount : threadCount;
        }

        public Task<bool> InitializeAsync()
        {
            try
            {
                Console.WriteLine("Initializing LLamaSharp LLM service...");
                
                // Set high priority for the process
                try
                {
                    using (var process = System.Diagnostics.Process.GetCurrentProcess())
                    {
                        process.PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
                        Console.WriteLine($"Process priority set to: {process.PriorityClass}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not set process priority: {ex.Message}");
                }

                // Configure threading for maximum CPU usage
                ThreadPool.SetMinThreads(_threadCount, _threadCount);
                ThreadPool.SetMaxThreads(_threadCount * 2, _threadCount * 2);
                Console.WriteLine($"Thread pool configured for {_threadCount} threads");

                // Check if model directory exists
                if (!Directory.Exists(_modelPath))
                {
                    Directory.CreateDirectory(_modelPath);
                    Console.WriteLine($"Created models directory: {_modelPath}");
                }

                // Look for GGUF model files
                var modelFiles = Directory.GetFiles(_modelPath, "*.gguf", SearchOption.AllDirectories);
                if (modelFiles.Length == 0)
                {
                    Console.WriteLine("No GGUF model files found in the models directory.");
                    Console.WriteLine("To use production mode:");
                    Console.WriteLine("1. Download a Llama GGUF model (e.g., from Hugging Face)");
                    Console.WriteLine("2. Place it in the 'models' folder");
                    Console.WriteLine("3. Restart the application");
                    Console.WriteLine("");
                    Console.WriteLine("Recommended models for testing:");
                    Console.WriteLine("- llama-2-7b-chat.Q4_K_M.gguf (smaller, faster)");
                    Console.WriteLine("- llama-2-7b-chat.Q8_0.gguf (better quality)");
                    Console.WriteLine("- phi-3-mini-4k-instruct.Q4_K_M.gguf (very small, good quality)");
                    return Task.FromResult(false);
                }

                // Initialize native library explicitly with CPU optimization
                if (!_nativeLibraryInitialized)
                {
                    NativeLibraryConfig.All.WithCuda(false);
                    _nativeLibraryInitialized = true;
                    Console.WriteLine("Native library initialized with CPU backend");
                }

                var modelFile = modelFiles[0];
                Console.WriteLine($"Loading model: {Path.GetFileName(modelFile)}");
                
                // Configure model parameters for maximum CPU usage
                var parameters = new ModelParams(modelFile)
                {
                    ContextSize = (uint)_contextSize,
                    GpuLayerCount = _gpuLayerCount,
                    UseMemorymap = true, // Use memory mapping for better performance
                    UseMemoryLock = true, // Lock memory to prevent swapping
                    Threads = _threadCount, // Use all available threads
                    BatchSize = (uint)512, // Larger batch size for better CPU utilization
                };

                // Load the model
                _weights = LLamaWeights.LoadFromFile(parameters);
                _context = _weights.CreateContext(parameters);
                _executor = new InteractiveExecutor(_context);

                // Initialize chat session
                var chatHistory = new ChatHistory();
                chatHistory.AddMessage(AuthorRole.System, "You are a helpful AI assistant. Provide clear, accurate, and helpful responses to user questions.");
                
                _session = new ChatSession(_executor, chatHistory);

                Console.WriteLine("LLamaSharp model loaded successfully!");
                Console.WriteLine($"Model: {Path.GetFileName(modelFile)}");
                Console.WriteLine($"Context size: {_contextSize}");
                Console.WriteLine($"GPU layers: {_gpuLayerCount}");
                Console.WriteLine($"Threads: {_threadCount}");
                Console.WriteLine($"Max tokens: {_maxTokens}");
                Console.WriteLine($"Temperature: {_temperature}");
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing LLamaSharp service: {ex.Message}");
                Console.WriteLine("Make sure you have:");
                Console.WriteLine("1. Downloaded a GGUF model file");
                Console.WriteLine("2. Placed it in the 'models' folder");
                Console.WriteLine("3. Have sufficient RAM (at least 4GB for small models)");
                return Task.FromResult(false);
            }
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            try
            {
                if (_session == null)
                {
                    return "Model not loaded. Please initialize the service first.";
                }

                if (string.IsNullOrWhiteSpace(prompt))
                {
                    return "Please provide a valid prompt.";
                }

                Console.WriteLine($"Processing prompt: {prompt}");
                Console.WriteLine("Generating response...");

                // Configure inference parameters for maximum CPU usage
                var inferenceParams = new InferenceParams()
                {
                    MaxTokens = _maxTokens,
                    AntiPrompts = new List<string> { "User:", "Human:", "\n\n" },
                };

                // Generate response with proper async handling
                var response = new System.Text.StringBuilder();
                
                // Use ConfigureAwait(false) to avoid deadlocks in console apps
                await foreach (var text in _session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, prompt),
                    inferenceParams).ConfigureAwait(false))
                {
                    response.Append(text);
                    // Force immediate output to see progress
                    Console.Write(text);
                }

                Console.WriteLine(); // New line after response
                return response.ToString().Trim();
            }
            catch (Exception ex)
            {
                return $"Error generating text: {ex.Message}";
            }
        }

        // Alternative synchronous method for maximum CPU usage
        public async Task<string> GenerateTextSync(string prompt)
        {
            try
            {
                if (_session == null)
                {
                    return "Model not loaded. Please initialize the service first.";
                }

                if (string.IsNullOrWhiteSpace(prompt))
                {
                    return "Please provide a valid prompt.";
                }

                Console.WriteLine($"Processing prompt: {prompt}");
                Console.WriteLine("Generating response...");

                var inferenceParams = new InferenceParams()
                {
                    MaxTokens = _maxTokens,
                    AntiPrompts = new List<string> { "User:", "Human:", "\n\n" },
                };

                var response = new System.Text.StringBuilder();
                
                // Use async enumeration for maximum CPU usage
                await foreach (var text in _session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, prompt),
                    inferenceParams).ConfigureAwait(false))
                {
                    response.Append(text);
                    Console.Write(text);
                }

                Console.WriteLine();
                return response.ToString().Trim();
            }
            catch (Exception ex)
            {
                return $"Error generating text: {ex.Message}";
            }
        }

        public void Dispose()
        {
            _session = null;
            _executor = null;
            _context?.Dispose();
            _weights?.Dispose();
        }
    }
}
