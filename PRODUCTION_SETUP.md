# Production Mode Setup Guide

This guide will help you set up and use the Llama LLM application in production mode with real Llama models.

## 🚀 Quick Start

### 1. Install Dependencies

First, restore the new packages:

```bash
dotnet restore
```

### 2. Download a Llama Model

Download a GGUF format Llama model. Here are some recommended options:

#### **Small Models (Recommended for Testing)**
- **Phi-3 Mini** (3.8GB): `microsoft/Phi-3-mini-4k-instruct-gguf`
- **Llama-2 7B Q4_K_M** (3.8GB): `TheBloke/Llama-2-7B-Chat-GGUF`
- **Llama-2 7B Q8_0** (6.7GB): `TheBloke/Llama-2-7B-Chat-GGUF`

#### **Medium Models (Better Quality)**
- **Llama-2 13B Q4_K_M** (7.3GB): `TheBloke/Llama-2-13B-Chat-GGUF`
- **Llama-3 8B Q4_K_M** (4.7GB): `bartowski/Llama-3-8B-Instruct-GGUF`

### 3. Download Instructions

#### Option A: Using Hugging Face CLI
```bash
# Install huggingface-hub if not already installed
pip install huggingface-hub

# Download Phi-3 Mini (recommended for testing)
huggingface-cli download microsoft/Phi-3-mini-4k-instruct-gguf Phi-3-mini-4k-instruct.Q4_K_M.gguf --local-dir ./models

# Or download Llama-2 7B
huggingface-cli download TheBloke/Llama-2-7B-Chat-GGUF llama-2-7b-chat.Q4_K_M.gguf --local-dir ./models
```

#### Option B: Manual Download
1. Go to [Hugging Face Models](https://huggingface.co/models?pipeline_tag=text-generation&library=gguf)
2. Find a model you want (e.g., `microsoft/Phi-3-mini-4k-instruct-gguf`)
3. Download the `.gguf` file (usually the largest file)
4. Place it in the `models` folder in your project directory

### 4. Run in Production Mode

```bash
# Run with production mode
dotnet run -- --production
```

## 🔧 Configuration Options

You can customize the LLM service by modifying the parameters in `Program.cs`:

```csharp
_productionService = new LlamaSharpLLMService(
    modelPath: "models",           // Path to model files
    maxTokens: 256,               // Maximum tokens to generate
    temperature: 0.7f,            // Creativity (0.0 = deterministic, 1.0 = very creative)
    contextSize: 2048,            // Context window size
    gpuLayerCount: 0              // GPU layers (0 = CPU only, >0 = GPU acceleration)
);
```

### GPU Acceleration

To use GPU acceleration, uncomment the appropriate backend in `LLM-Test.csproj`:

```xml
<!-- For NVIDIA GPU -->
<PackageReference Include="LLamaSharp.Backend.Cuda11" Version="0.12.0" />

<!-- For AMD GPU -->
<PackageReference Include="LLamaSharp.Backend.OpenCL" Version="0.12.0" />

<!-- For Apple Silicon -->
<PackageReference Include="LLamaSharp.Backend.Metal" Version="0.12.0" />
```

Then set `gpuLayerCount` to a value > 0 (e.g., 20-40 depending on your GPU memory).

## 📊 Performance Recommendations

### **System Requirements**

| Model Size | RAM Required | VRAM (GPU) | Recommended Use |
|------------|--------------|------------|-----------------|
| 3-4GB      | 8GB+        | 4GB+       | Testing, Development |
| 6-8GB      | 16GB+       | 8GB+       | Production, Better Quality |
| 13GB+      | 32GB+       | 16GB+      | High-Quality Production |

### **Model Quantization Levels**

| Quantization | Size | Quality | Speed | Use Case |
|--------------|------|---------|-------|----------|
| Q4_K_M       | Small | Good    | Fast  | Balanced |
| Q8_0         | Medium| Better  | Medium| High Quality |
| Q6_K         | Medium| Good    | Fast  | Balanced |
| Q5_K_M       | Small | Good    | Fast  | Balanced |

## 🎯 Usage Examples

### Basic Usage
```bash
# Demo mode (no model required)
dotnet run

# Production mode (requires GGUF model)
dotnet run -- --production
```

### Example Conversation
```
=== Llama LLM Text Generator ===
Welcome to the Llama LLM Application!
Starting in PRODUCTION mode with LLamaSharp...
Loading model: Phi-3-mini-4k-instruct.Q4_K_M.gguf
LLamaSharp model loaded successfully!
Model: Phi-3-mini-4k-instruct.Q4_K_M.gguf
Context size: 2048
GPU layers: 0
Max tokens: 256
Temperature: 0.7
Production LLM service ready! Start typing your questions:

You: Explain quantum computing in simple terms
LLM: Quantum computing is like having a computer that can be in multiple states at once, rather than just 0 or 1 like regular computers. Think of it like a spinning coin - while it's spinning, it's both heads and tails simultaneously until it lands. Quantum computers use this principle to solve certain problems much faster than regular computers, especially in areas like cryptography, drug discovery, and optimization problems.

You: Write a short poem about AI
LLM: In circuits deep and data streams,
Where silicon dreams take flight,
AI learns from human schemes,
And finds patterns in the night.

With neural nets and algorithms fine,
It processes thought and word,
Creating something almost divine,
From the data it has heard.

Though built by human hands and mind,
It grows beyond our sight,
A digital companion kind,
In the realm of artificial light.

You: exit
Goodbye!
```

## 🔍 Troubleshooting

### Common Issues

1. **"No GGUF model files found"**
   - Ensure you've downloaded a `.gguf` model file
   - Place it in the `models` folder
   - Check the file extension is `.gguf`

2. **"Out of Memory" errors**
   - Try a smaller model (3-4GB instead of 7GB+)
   - Reduce `contextSize` parameter
   - Close other applications to free RAM

3. **Slow performance**
   - Use a quantized model (Q4_K_M, Q5_K_M)
   - Enable GPU acceleration if available
   - Reduce `maxTokens` parameter

4. **Model loading fails**
   - Ensure the model file is not corrupted
   - Try downloading a different model
   - Check available disk space

### Performance Tips

1. **For faster responses:**
   - Use smaller models (3-4GB)
   - Lower `maxTokens` (128-256)
   - Use quantized models (Q4_K_M)

2. **For better quality:**
   - Use larger models (7GB+)
   - Higher quantization (Q8_0, Q6_K)
   - Increase `contextSize` (4096+)

3. **For GPU acceleration:**
   - Install appropriate CUDA/OpenCL/Metal backend
   - Set `gpuLayerCount` to 20-40
   - Ensure sufficient VRAM

## 📚 Additional Resources

- [LLamaSharp Documentation](https://github.com/SciSharp/LLamaSharp)
- [GGUF Model Format](https://github.com/ggerganov/ggml/blob/master/docs/gguf.md)
- [Hugging Face GGUF Models](https://huggingface.co/models?pipeline_tag=text-generation&library=gguf)
- [Llama Model Comparison](https://huggingface.co/spaces/huggingface-projects/llama-2-7b-chat)

## 🆘 Support

If you encounter issues:

1. Check the console output for error messages
2. Verify your model file is in the correct location
3. Ensure you have sufficient RAM/VRAM
4. Try with a smaller model first
5. Check the LLamaSharp GitHub issues for known problems

Happy coding! 🚀
