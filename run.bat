@echo off
echo Building and running Llama LLM Text Generator...
echo.

dotnet build
if %errorlevel% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Choose mode:
echo 1. Demo mode (no model required)
echo 2. Production mode (requires GGUF model)
echo.
set /p choice="Enter your choice (1 or 2): "

if "%choice%"=="1" (
    echo Starting in DEMO mode...
    dotnet run
) else if "%choice%"=="2" (
    echo Starting in PRODUCTION mode...
    dotnet run -- --production
) else (
    echo Invalid choice. Starting in demo mode...
    dotnet run
)

pause
