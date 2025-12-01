# Llama LLM Text Generator

A C# application that demonstrates LLM text generation using Llama models. This project provides both a demo implementation and the foundation for integrating with real Llama ONNX models.

## Features

- **Interactive Text Generation**: Chat-like interface for generating responses to user prompts
- **Demo Mode**: Works out of the box with simulated responses for testing
- **Production Ready**: Includes commented code for integrating with real Llama ONNX models
- **Async Operations**: Non-blocking text generation for better user experience
- **Error Handling**: Comprehensive error handling and user feedback

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Windows, macOS, or Linux

### Installation

1. Clone or download this project
2. Navigate to the project directory
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

### Running the Application

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Start typing your questions or prompts and see the LLM responses!

## Using with Real Llama Models

To use this application with actual Llama models, you'll need to:

### 1. Download a Llama ONNX Model

You can find ONNX-compatible Llama models from:
- [Hugging Face Model Hub](https://huggingface.co/models?pipeline_tag=text-generation&library=transformers&sort=downloads)
- [ONNX Model Zoo](https://github.com/onnx/models)

Popular small Llama models for testing:
- `microsoft/DialoGPT-small`
- `distilbert-base-uncased`

### 2. Update the Model Loading

1. Place your ONNX model file in a `models` folder
2. Uncomment the production code in `LlamaLLMService.cs`:
   - Uncomment the `LoadModelAsync()` method
   - Uncomment the `GenerateWithModelAsync()` method
   - Uncomment the helper methods at the bottom

3. Update the `InitializeAsync()` method to call `LoadModelAsync()`

4. Update the `GenerateTextAsync()` method to call `GenerateWithModelAsync()`

### 3. Model Configuration

You can adjust model parameters in the `LlamaLLMService` constructor:

```csharp
var llmService = new LlamaLLMService(
    modelPath: "path/to/your/models",
    maxTokens: 150,        // Maximum tokens to generate
    temperature: 0.8f      // Creativity level (0.0 = deterministic, 1.0 = very creative)
);
```

## Project Structure

```
LLM-Test/
├── Program.cs              # Main application entry point
├── LlamaLLMService.cs      # LLM service implementation
├── LLM-Test.csproj         # Project file with dependencies
└── README.md               # This file
```

## Dependencies

- **Microsoft.ML.OnnxRuntime**: For running ONNX models
- **Microsoft.ML.Tokenizers**: For text tokenization
- **System.Numerics.Tensors**: For tensor operations

## Example Usage

```
=== Llama LLM Text Generator ===
Welcome to the Llama LLM Demo!
Type your questions or prompts, and I'll generate responses.
Type 'exit' or 'quit' to close the application.
Press Ctrl+C to exit...

LLM service ready! Start typing your questions:

You: Hello, how are you?
LLM: Hello! How can I assist you today?

You: What is artificial intelligence?
LLM: I understand you're asking about 'What is artificial intelligence?'. While I'm currently running in demo mode, a real Llama model would provide a detailed response to your question. To use an actual Llama model, please download an ONNX-compatible Llama model and update the model loading logic in the LlamaLLMService class.

You: exit
Goodbye!
```

## Performance Considerations

- **Memory Usage**: Larger models require more RAM
- **CPU vs GPU**: ONNX Runtime supports both CPU and GPU inference
- **Model Size**: Smaller models (7B parameters) are recommended for local deployment
- **Batch Processing**: Consider implementing batch processing for multiple requests

## Troubleshooting

### Common Issues

1. **Model Not Found**: Ensure the model file is in the correct directory
2. **Out of Memory**: Try using a smaller model or reducing maxTokens
3. **Slow Performance**: Consider using GPU acceleration or a smaller model

### Getting Help

- Check the console output for error messages
- Verify your model is ONNX-compatible
- Ensure all dependencies are properly installed

## License

This project is provided as-is for educational and demonstration purposes.

## Contributing

Feel free to submit issues and enhancement requests!
